//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

#if UNITY_5_3 && !UNITY_WEBPLAYER && (!UNITY_IOS || ENABLE_IL2CPP)
#define USE_WEB_REQUEST //Comment out to force use of old WWW class on Unity 5.3+
#endif

using System;
using System.IO;

#if !DOT_NET
using UnityEngine;
using JsonFx.Json;
#if USE_WEB_REQUEST
using UnityEngine.Experimental.Networking;
#endif
#else
using System.Net;
using System.Collections.Generic;
using System.Text;
#endif

namespace BrainCloud.Internal
{
    internal class FileUploader
    {
        public enum FileUploaderStatus
        {
            None,
            Pending,
            Uploading,
            CompleteFailed,
            CompleteSuccess
        };

        #region Properties
        public string UploadId { get; private set; }

        public double Progress { get; private set; }

        public long BytesTransferred { get { return (long)(TotalBytesToTransfer * Progress); } }

        public long TotalBytesToTransfer { get; private set; }

        public FileUploaderStatus Status { get; private set; }

        public string Response { get; private set; }

        public int StatusCode { get; private set; }

        public int ReasonCode { get; private set; }
        #endregion

        private string _sessionId;
        private string _localPath;
        private string _serverUrl;
        private string _fileName;
        private long _timeoutThreshold = 50;
        private int _timeout = 120;

        //transfer rate
        private const double TIME_INTERVAL = 0.25f;
        private double _transferElapsedTime;
        private long _transferRatesTotal;
        private long _lastTransferTotal;
        private long _transferRatePerSecond;

        //delta time
        private DateTime _lastTime;
        private double _deltaTime;

        //timeout
        private double _elapsedTime;
        private double _timeUnderMinRate;

#if USE_WEB_REQUEST
        private UnityWebRequest _request;
#elif !DOT_NET
        private WWW _request;
#else
        private WebClient _request;
        private UploadDataCompletedEventArgs _result;
        private Object _lock = new Object();
#endif

        public FileUploader(string uploadId, string localPath, string serverUrl, string sessionId, int timeout, int timeoutThreshold)
        {
#if UNITY_WEBPLAYER || UNITY_WEBGL
            throw new Exception("File upload API is not supported on Web builds");
#else
            UploadId = uploadId;
            _localPath = localPath;
            _serverUrl = serverUrl;
            _sessionId = sessionId;

            _timeout = timeout;
            _timeoutThreshold = timeoutThreshold;

            if (!File.Exists(localPath))
            {
                ThrowError(ReasonCodes.CLIENT_UPLOAD_FILE_UNKNOWN, "File at" + localPath + " does not exist");
                return;
            }

            FileInfo info = new FileInfo(localPath);
            _fileName = info.Name;
            TotalBytesToTransfer = info.Length;

            Status = FileUploaderStatus.Pending;

            Start();
#endif
        }

        public void Start()
        {
#if UNITY_WEBPLAYER || UNITY_WEBGL
            throw new Exception("File upload API is not supported on Web builds");
#else
            byte[] file = File.ReadAllBytes(_localPath);

#if !DOT_NET
            WWWForm postForm = new WWWForm();
            postForm.AddField("sessionId", _sessionId);
            postForm.AddField("uploadId", UploadId);
            postForm.AddField("fileSize", TotalBytesToTransfer.ToString());
            postForm.AddBinaryData("uploadFile", file, _fileName);

#if USE_WEB_REQUEST
            _request = UnityWebRequest.Post(_serverUrl, postForm);
            _request.Send();
#else
            _request = new WWW(_serverUrl, postForm);
#endif
#else
            _request = new WebClient();

            // Generate post objects
            Dictionary<string, object> postParameters = new Dictionary<string, object>();
            postParameters.Add("sessionId", _sessionId);
            postParameters.Add("uploadId", UploadId);
            postParameters.Add("fileSize", TotalBytesToTransfer.ToString());
            postParameters.Add("uploadFile", new FormUpload.FileParameter(file, _fileName, "application/octet-stream"));

            string boundary = Guid.NewGuid().ToString();
            _request.Headers.Set("Content-type", "multipart/form-data; boundary=" + boundary);

            _request.UploadProgressChanged += new UploadProgressChangedEventHandler(UploadProgress);
            _request.UploadDataCompleted += new UploadDataCompletedEventHandler(UploadComplete);
            _request.UploadDataAsync(new Uri(_serverUrl), FormUpload.GetMultipartFormData(postParameters, boundary));
#endif

            Status = FileUploaderStatus.Uploading;
            BrainCloudClient.Get().Log("Started upload of " + _fileName);
            _lastTime = DateTime.Now;
#endif //!Web build
        }

        public void CancelUpload()
        {
#if USE_WEB_REQUEST
            _request.Abort();
#elif !DOT_NET
            _request = null;
#else
            _request.CancelAsync();
#endif
            Status = FileUploaderStatus.CompleteFailed;
            StatusCode = StatusCodes.CLIENT_NETWORK_ERROR;
            ReasonCode = ReasonCodes.CLIENT_UPLOAD_FILE_CANCELLED;
            Response = CreateErrorString(StatusCode, ReasonCode, "Upload of " + _fileName + " cancelled by user");
            BrainCloudClient.Get().Log("Upload of " + _fileName + " cancelled by user");
        }

        public void Update()
        {
            UpdateDeltaTime();
            _elapsedTime += _deltaTime;

            UpdateTransferRate();
#if !DOT_NET && (UNITY_IOS || UNITY_ANDROID)
            CheckTimeout();
#endif
            if (Status == FileUploaderStatus.CompleteFailed || Status == FileUploaderStatus.CompleteSuccess)
            {
#if DOT_NET || USE_WEB_REQUEST
                CleanupRequest();
#endif
                return;
            }

#if !DOT_NET
            Progress = _request.uploadProgress;

            if (_request.isDone) HandleResponse();
#else
            if (_result != null) HandleResponse();
#endif
        }

#if DOT_NET
        private void UploadProgress(object sender, UploadProgressChangedEventArgs e)
        {
            Progress = (double)e.BytesSent / e.TotalBytesToSend;
        }

        private void UploadComplete(object sender, UploadDataCompletedEventArgs e)
        {
            lock (_lock)
            {
                _result = e;
            }
        }
#endif

        private void HandleResponse()
        {
            _transferRatePerSecond = 0;
#if !DOT_NET
#if USE_WEB_REQUEST
            StatusCode = (int)_request.responseCode;
#else
            if (_request.responseHeaders.ContainsKey("STATUS"))
            {
                string code = _request.responseHeaders["STATUS"].Split(' ')[1];
                StatusCode = int.Parse(code);
            }
            else StatusCode = StatusCodes.CLIENT_NETWORK_ERROR;
#endif
            if (StatusCode != StatusCodes.OK)
            {
                Status = FileUploaderStatus.CompleteFailed;

                if (_request.error != null)
                {
                    ReasonCode = ReasonCodes.CLIENT_UPLOAD_FILE_UNKNOWN;
                    Response = CreateErrorString(StatusCode, ReasonCode, _request.error);
                }
                else
#if USE_WEB_REQUEST
                    Response = _request.downloadHandler.text;
#else
                Response = _request.text;
#endif
                JsonErrorMessage resp = null;

                try { resp = JsonReader.Deserialize<JsonErrorMessage>(Response); }
                catch (JsonDeserializationException e) { BrainCloudClient.Get().Log(e.Message); }

                if (resp != null)
                    ReasonCode = resp.reason_code;
                else
                {
                    ReasonCode = ReasonCodes.CLIENT_UPLOAD_FILE_UNKNOWN;
                    Response = CreateErrorString(StatusCode, ReasonCode, Response);
                }
            }
            else
            {
                Status = FileUploaderStatus.CompleteSuccess;

#if USE_WEB_REQUEST
                Response = _request.downloadHandler.text;
#else
                Response = _request.text;
#endif            
                BrainCloudClient.Get().Log("Uploaded " + _fileName + " in " + _elapsedTime.ToString("0.0##") + " seconds");
            }

#if USE_WEB_REQUEST
            CleanupRequest();
#endif
#else
            if (_result.Error != null)
            {
                Status = FileUploaderStatus.CompleteFailed;

                if (_result.Error is WebException)
                {
                    StatusCode = (int)((WebException)_result.Error).Status;
                    Response = CreateErrorString(StatusCode, ReasonCode, ((WebException)_result.Error).Message);
                }
                else
                {
                    StatusCode = StatusCodes.CLIENT_NETWORK_ERROR;
                }
                ReasonCode = ReasonCodes.CLIENT_UPLOAD_FILE_UNKNOWN;
            }
            else
            {
                Status = FileUploaderStatus.CompleteSuccess;
                StatusCode = StatusCodes.OK;
                Response = Encoding.UTF8.GetString(_result.Result);
                BrainCloudClient.Get().Log("Uploaded " + _fileName + " in " + _elapsedTime.ToString("0.0##") + " seconds");
            }

            CleanupRequest();
#endif
#if DOT_NET || USE_WEB_REQUEST

#endif
        }

        private void UpdateTransferRate()
        {
            _transferElapsedTime += _deltaTime;

            if (_transferElapsedTime > TIME_INTERVAL)
            {
                _transferRatePerSecond = (long)(_transferRatesTotal / _transferElapsedTime);
                _transferRatesTotal = 0;
                _transferElapsedTime = 0;
            }
            else
            {
                _transferRatesTotal += BytesTransferred - _lastTransferTotal;
                _lastTransferTotal = BytesTransferred;
            }
        }

        private void CheckTimeout()
        {
            if (_transferRatePerSecond < _timeoutThreshold)
                _timeUnderMinRate += _deltaTime;
            else
                _timeUnderMinRate = 0.0;

            if (_timeUnderMinRate > _timeout)
                ThrowError(ReasonCodes.CLIENT_UPLOAD_FILE_TIMED_OUT, "Upload of " + _fileName + " failed due to timeout.");
        }

        private void UpdateDeltaTime()
        {
            _deltaTime = DateTime.Now.Subtract(_lastTime).TotalSeconds;
            _lastTime = DateTime.Now;
        }

        private void ThrowError(int reasonCode, string message)
        {
            Status = FileUploaderStatus.CompleteFailed;
            StatusCode = StatusCodes.CLIENT_NETWORK_ERROR;
            ReasonCode = reasonCode;
            Response = CreateErrorString(StatusCode, ReasonCode, message);
        }

        private string CreateErrorString(int statusCode, int reasonCode, string message)
        {
            return new JsonErrorMessage(statusCode, reasonCode, message).GetJsonString();
        }

#if DOT_NET || USE_WEB_REQUEST
        private void CleanupRequest()
        {
            if (_request == null) return;
            _request.Dispose();
            _request = null;
        }
#endif
    }

#if DOT_NET
    // Implements multipart/form-data POST in C# http://www.ietf.org/rfc/rfc2388.txt
    // http://www.briangrinstead.com/blog/multipart-form-post-in-c
    public static class FormUpload
    {
        private static readonly Encoding encoding = Encoding.UTF8;

        public static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                // Thanks to feedback from commenters, add a CRLF to allow multiple parameters to be added.
                // Skip it on the first parameter, add it to subsequent parameters.
                if (needsCLRF)
                    formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                if (param.Value is FileParameter)
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;

                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        param.Key,
                        param.Value);
                    formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                }
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }

        public class FileParameter
        {
            public byte[] File { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
            public FileParameter(byte[] file) : this(file, null) { }
            public FileParameter(byte[] file, string filename) : this(file, filename, null) { }
            public FileParameter(byte[] file, string filename, string contenttype)
            {
                File = file;
                FileName = filename;
                ContentType = contenttype;
            }
        }
    }
#endif
}
