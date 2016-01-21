//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

#if UNITY_5_3 && (!UNITY_IOS || ENABLE_IL2CPP)
#define USE_WEB_REQUEST //Comment out to force use of old WWW class on Unity 5.3+
#endif

using JsonFx.Json;
using System;
using System.IO;

#if !DOT_NET
using UnityEngine;
#if USE_WEB_REQUEST
using UnityEngine.Experimental.Networking;
#endif
#else
using System.Net;
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
        private WebRequest _request;
#endif

        public FileUploader(string uploadId, string localPath, string serverUrl, string sessionId, int timeout, int timeoutThreshold)
        {
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
        }

        public void Start()
        {
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
            _request = WebRequest.Create(_serverUrl);
            _request.Method = "POST";
            _request.ContentLength = TotalBytesToTransfer;
            _request.ContentType = "multipart/form-data";

            Stream dataStream = _request.GetRequestStream();
            dataStream.Write(file, 0, file.Length);
            dataStream.Close();

            _request.BeginGetResponse(FinishWebRequest, null);
#endif
            Status = FileUploaderStatus.Uploading;
            BrainCloudClient.Get().Log("Started upload of " + _fileName);
            _lastTime = DateTime.Now;
        }

        public void CancelUpload()
        {
#if USE_WEB_REQUEST
            _request.Abort();
#elif !DOT_NET
            _request = null;
#else
            //.NET
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
#if USE_WEB_REQUEST
                CleanupRequest();
#endif
                return;
            }

#if !DOT_NET
            Progress = _request.uploadProgress;

            if (_request.isDone) HandleResponse();
#else
            //.NET
#endif
        }

        private void HandleResponse()
        {
#if !DOT_NET
            _transferRatePerSecond = 0;
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

            CleanupRequest();
#else
            //.NET
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

#if DOT_NET
        void FinishWebRequest(IAsyncResult result)
        {
            _request.EndGetResponse(result);
        }
#endif
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

#if USE_WEB_REQUEST
        private void CleanupRequest()
        {
            if (_request == null) return;
            _request.Dispose();
            _request = null;
        }
#endif
    }
}
