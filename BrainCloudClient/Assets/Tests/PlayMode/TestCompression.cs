using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using BrainCloud.Common;
using UnityEngine;
using BrainCloud;
using BrainCloud.JsonFx.Json;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class TestCompression : TestFixtureBase
    {
        private readonly string _defaultEntityType = "TestCompressedEntity";

        private readonly string _defaultEntityValue =
            @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce malesuada aliquet orci, vel ornare eros mollis non. Sed elementum dictum sapien, sit amet tincidunt diam hendrerit ac. Sed placerat ante auctor ex tempus, sit amet aliquet ipsum iaculis. Pellentesque eu turpis dapibus, venenatis leo et, luctus metus. Sed sodales ante eu felis bibendum ornare. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Sed tempor in ipsum et sodales. Ut posuere nibh aliquam nunc laoreet, a tincidunt mi dignissim. Phasellus volutpat diam bibendum, hendrerit dolor a, imperdiet nulla. Fusce malesuada leo sed est laoreet mattis. Mauris efficitur pretium augue non mollis. Quisque nec metus diam.
            Proin finibus sem non bibendum lacinia. In sodales id augue at tempus. Duis pulvinar vestibulum urna, vel placerat diam molestie eget. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Sed risus nisl, rutrum et eros condimentum, tempor facilisis est. Nulla a tempor massa. Phasellus luctus maximus finibus. Ut nec libero at nunc semper eleifend ac mollis velit.
            Aliquam scelerisque sem urna, ut pretium tortor ultricies sed. Proin at vestibulum nunc. Sed vitae pellentesque est, vitae faucibus mi. Vivamus quis dolor vitae eros fringilla pharetra. Suspendisse pharetra at mi nec imperdiet. Maecenas tincidunt quam nec lectus volutpat, vitae maximus turpis condimentum. In hac habitasse platea dictumst.
            Cras dapibus rutrum massa a dignissim. Etiam vulputate tincidunt venenatis. Maecenas turpis neque, molestie et nisl non, aliquam suscipit diam. Aenean non sem felis. Quisque eget elementum magna. Nulla neque tortor, vestibulum quis nisl sit amet, fermentum varius dolor. Praesent sit amet nunc eget neque volutpat ultricies quis eu tortor. Mauris ut consectetur odio.
            Fusce fermentum laoreet elit eu scelerisque. Nullam et orci sagittis, vehicula purus ut, euismod purus. Ut tincidunt suscipit nunc, a fermentum felis rhoncus ac. Proin at odio pharetra, tempor nisl non, posuere ante. Pellentesque malesuada gravida purus id porta. Nam tincidunt eget eros ultricies pretium. Phasellus condimentum, lectus a porttitor interdum, dolor nisl laoreet justo, et semper urna justo sit amet lorem. Nulla facilisi. Suspendisse eget nulla tristique, ullamcorper mauris quis, consequat urna. Donec bibendum laoreet diam, in vehicula elit pharetra vitae.";
        private int _returnCount;

        [UnityTest]
        public IEnumerator TestCompressEntityWithStats()
        {
            _tc.bcWrapper.AuthenticateUniversal(username, password, true, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            //Getting compression test file
            string entityValuePath = Application.dataPath;
            string resultPath = entityValuePath;
            string search = "BrainCloudClient";
            if (resultPath.Contains(search))
            {
                resultPath = resultPath.Substring(0, resultPath.LastIndexOf(search));
                resultPath += search + Path.DirectorySeparatorChar + "tests" + Path.DirectorySeparatorChar + "TestCompressionFile.txt";
            }

            string entityValueFromFile = File.ReadAllText(resultPath);
            
            _tc.bcWrapper.EntityService.CreateEntity
            (
                _defaultEntityType,
                Helpers.CreateJsonPair(_defaultEntityType, entityValueFromFile),
                new ACL(ACL.Access.None).ToJsonString(),
                _tc.ApiSuccess,
                _tc.ApiError
            );
            
            Debug.LogWarning($"UNCOMPRESSED REQUEST PACKET SIZE: {MimicPacketSize(false, entityValueFromFile)}");
            yield return _tc.StartCoroutine(_tc.Run());

            _tc.bcWrapper.EntityService.GetEntitiesByType(_defaultEntityType, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            if (_tc.m_done)
            {
                ReadResponsePacketSize(_tc.m_response, false);
            }
            yield return _tc.StartCoroutine(DeleteAllDefaultEntities());
            
            //Now we do the test over but with compression on
            _tc.bcWrapper.Client.EnableCompressedRequests(true);
            _tc.bcWrapper.Client.EnableCompressedResponses(true);
            //Now try with compressed responses and requests
            _tc.bcWrapper.EntityService.CreateEntity
            (
                _defaultEntityType,
                Helpers.CreateJsonPair(_defaultEntityType, entityValueFromFile),
                new ACL(ACL.Access.None).ToJsonString(),
                _tc.ApiSuccess,
                _tc.ApiError
            );
            Debug.LogWarning($"COMPRESSED REQUEST PACKET SIZE: {MimicPacketSize(true, entityValueFromFile)}");
            yield return _tc.StartCoroutine(_tc.Run());

            _tc.bcWrapper.EntityService.GetEntitiesByType(_defaultEntityType, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            if (_tc.m_done)
            {
                ReadResponsePacketSize(_tc.m_response, true);
            }

            yield return _tc.StartCoroutine(DeleteAllDefaultEntities());
            
        }
        
        [UnityTest]
        public IEnumerator TestCompressEntityWithResponses()
        {
            yield return TestCompressEntity(true, false);
        }
        
        [UnityTest]
        public IEnumerator TestCompressEntityWithRequests()
        {
            yield return TestCompressEntity(false, true);
        }
        
        [UnityTest]
        public IEnumerator TestCompressEntityWithResponsesAndRequests()
        {
            yield return TestCompressEntity(true, true);
        }

        [UnityTest]
        public IEnumerator TestCompressionFile()
        {
            _tc.bcWrapper.Client.EnableCompressedResponses(true);
            _tc.bcWrapper.Client.EnableCompressedRequests(true);
            _tc.bcWrapper.AuthenticateUniversal(username,password,true,_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            _tc.bcWrapper.Client.RegisterFileUploadCallbacks(FileCallbackSuccess, FileCallbackFail);
            string cloudPath = "TestFolder";
            string fileName = "testFile1.txt";
            string fileContent = "Hello, I'm a file !";
            byte[] fileData = ConvertByteFromFile(fileContent);

            _tc.bcWrapper.FileService.UploadFileFromMemory
            (
                cloudPath,
                fileName,
                true,
                true,
                fileData,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            
            yield return _tc.StartCoroutine(_tc.Run());
            yield return WaitForReturn(new[] { GetUploadId(_tc.m_response) });
            
            //Checking if cloud path is correct according to response
            var data = _tc.m_response["data"] as Dictionary<string, object>;
            var fileDetails = data["fileDetails"] as Dictionary<string, object>;
            string responseCloudPath = (string) fileDetails["cloudPath"];
            bool doesItMatch = cloudPath.Equals(responseCloudPath);
            
            LogResults("Failed to upload compressed file", _tc.successCount == 3 && doesItMatch);
            yield return null;
        }
        
        /// <summary>
        /// Creates a test file filled with garbage
        /// </summary>
        /// <param name="size">Size of the file in KB</param>
        /// <returns>Full path to the file</returns>
        private string CreateFile(int size, string name = "testFile.dat")
        {
            string path = Path.Combine(Path.GetTempPath() + name);

            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                fileStream.SetLength(1024 * size);
            }

            return path;
        }
        
        private byte[] ConvertByteFromFile(string fileContent)
        {
            if (Uri.IsWellFormedUriString(fileContent, UriKind.Absolute))
            {
                Stream info = new FileStream(fileContent, FileMode.Open);
                byte[] fileData = new Byte[(int) info.Length];
                info.Seek(0, SeekOrigin.Begin);
                info.Read(fileData, 0, (int) info.Length);
                info.Close();
                return fileData;
            }

            return Encoding.ASCII.GetBytes(fileContent);
        }
        
        private string GetUploadId(Dictionary<string, object> response)
        {
            var fileDetails = ((Dictionary<string, object>)((Dictionary<string, object>)response["data"])["fileDetails"]);
            if (fileDetails == null) return "";
            
            return (string)(fileDetails["uploadId"]);
        }
        
        private IEnumerator WaitForReturn(string[] uploadIds, int cancelTime = -1)
        {
            int count = 0;
            bool sw = true;
            Debug.Log("Waiting for file to upload...");
            BrainCloudClient client = _tc.bcWrapper.Client;
            
            client.Update();
            while (_returnCount < uploadIds.Length)
            {
                for (int i = 0; i < uploadIds.Length; i++)
                {
                    double progress = client.FileService.GetUploadProgress(uploadIds[i]);

                    if (progress > -1 && sw)
                    {
                        string logStr = "File " + (i + 1) + " Progress: " +
                                        progress + " | " +
                                        client.FileService.GetUploadBytesTransferred(uploadIds[i]) + "/" +
                                        client.FileService.GetUploadTotalBytesToTransfer(uploadIds[i]);
                        Debug.Log(logStr);
                    }

                    if (cancelTime > 0 && progress > 0.05)
                    {
                        client.FileService.CancelUpload(uploadIds[i]);
                        Debug.LogWarning("Canceling Upload...");
                    }
                }
                client.Update();
                yield return new WaitForFixedUpdate();
                sw = !sw;
                count += 150;
            }

            _returnCount = 0;
        }
        
        private void FileCallbackSuccess(string uploadId, string jsonData)
        {
            _returnCount++;
            _tc.successCount++;
        }

        private void FileCallbackFail(string uploadId, int statusCode, int reasonCode, string jsonData)
        {
            _returnCount++;
            _tc.failCount++;
            _tc.m_statusMessage = jsonData;
        }

        private IEnumerator TestCompressEntity(bool isResponsesOn, bool isRequestsOn)
        {
            var entityType = "TestCompressedEntity";
            var entityValue = "This is an entity test for compression.";
            _tc.bcWrapper.Client.EnableCompressedResponses(isResponsesOn);
            _tc.bcWrapper.Client.EnableCompressedRequests(isRequestsOn);

            _tc.bcWrapper.AuthenticateUniversal(username, password, true, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());

            _tc.bcWrapper.EntityService.CreateEntity
            (
                entityType,
                Helpers.CreateJsonPair(entityType, entityValue),
                new ACL(ACL.Access.None).ToJsonString(),
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            LogResults("Couldn't log in", _tc.successCount == 2);
        }
        
        private int MimicPacketSize(bool isCompressed, string entityValue)
        {
            //mimic data for a creating entity
            var data = new Dictionary<string, object>();
            data["entityType"] = _defaultEntityType;
            var jsonEntityData = Helpers.CreateJsonPair(_defaultEntityType, entityValue);
            var entityData = JsonReader.Deserialize<Dictionary<string, object>>(jsonEntityData);
            data["data"] = entityData;
            var jsonEntityAcl = new ACL(ACL.Access.None).ToJsonString(); 
            if (Util.IsOptionalParameterValid(jsonEntityAcl))
            {
                var acl = JsonReader.Deserialize<Dictionary<string, object>>(jsonEntityAcl);
                data["acl"] = acl;
            }
            
            string jsonRequestString = JsonWriter.Serialize(data);
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonRequestString);
            if (isCompressed)
            {
                var outputStream = new MemoryStream();
                using (var stream = new GZipStream(outputStream, CompressionMode.Compress, true))
                {
                    stream.Write(byteArray, 0, byteArray.Length);
                }
                byteArray = outputStream.ToArray();
            }
            
            return byteArray.Length;
        }
        
        private void ReadResponsePacketSize(Dictionary<string,object> response, bool isCompressed)
        {
            string jsonResponseString = JsonWriter.Serialize(response);
            string beginningText = isCompressed ? "DECOMPRESSED " : "UNCOMPRESSED ";
            byte[] responseByteArray = Encoding.UTF8.GetBytes(jsonResponseString);
            Debug.LogWarning(beginningText + "RESPONSE PACKET SIZE :" + responseByteArray.Length);
        }
        
        private IEnumerator DeleteAllDefaultEntities()
        {
            List<string> entityIds = new List<string>(0);
            //get all entities
            _tc.bcWrapper.EntityService.GetEntitiesByType(_defaultEntityType, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            Dictionary<string, object> data = (Dictionary<string, object>)_tc.m_response["data"];
            object[] temp = (object[])data["entities"];

            if (temp.Length <= 0)
            {
                yield return null;
            }

            Dictionary<string, object>[] entities = (Dictionary<string, object>[])data["entities"];

            for (int i = 0; i < entities.Length; i++)
            {
                entityIds.Add((string)entities[i]["entityId"]);
            }

            for (int i = 0; i < entityIds.Count; i++)
            {
                _tc.Reset();
                _tc.bcWrapper.EntityService.DeleteEntity(entityIds[i], -1, _tc.ApiSuccess, _tc.ApiError);
                yield return _tc.StartCoroutine(_tc.Run());
            }
        }
    }
}
