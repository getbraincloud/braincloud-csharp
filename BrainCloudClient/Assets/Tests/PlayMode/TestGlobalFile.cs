using System.Collections;
using System.Collections.Generic;
using System.IO;
using BrainCloud.JsonFx.Json;
using NUnit.Framework;
using Tests.PlayMode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests.PlayMode
{
    public class TestGlobalFile : TestFixtureBase
    {
        private Image _img;
        private string fileId = "ed2d2924-4650-4a88-b095-94b75ce9aa18";

        [UnityTest]
        public IEnumerator TestGetGlobalFileList()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            _tc.successCount = 0;
            _tc.bcWrapper.GlobalFileService.GetGlobalFileList
                (
                    "fname",
                    false,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("failed to get global file list", _tc.successCount == 1);
            Assert.True(_tc.successCount == 1);
        }
        
        [UnityTest]
        public IEnumerator TestGetGlobalFileInfo()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            _tc.successCount = 0;
            _tc.bcWrapper.GlobalFileService.GetFileInfo
                (
                    fileId,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("Failed to Get file info, check logs for response", _tc.successCount == 1);
        }
        
        [UnityTest]
        public IEnumerator TestGlobalSimpleDownloadFile()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            _tc.bcWrapper.GlobalFileService.GetFileInfoSimple
            (
                "fname",
                "testGlobalFile.png",
                PrepareToDownloadWithFileDetails,
                _tc.ApiError
            );

            _tc.m_done = false;
            yield return _tc.StartCoroutine(_tc.Spin());
            LogResults("ERROR: Image failed to download", _img.sprite != null);
        }
        
        [UnityTest]
        public IEnumerator TestGlobalCdnUrlDownloadFile()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
        
            _tc.bcWrapper.GlobalFileService.GetGlobalCDNUrl
            (
                fileId,
                PrepareToDownloadWithCDNDetails,
                _tc.ApiError
            );

            _tc.m_done = false;
            yield return _tc.StartCoroutine(_tc.Spin());
            LogResults("ERROR: Image failed to download", _img.sprite != null);
        }

        private void PrepareToDownloadWithFileDetails(string json, object cb)
        {
            Debug.Log(json);
            string url = "";
            var response = JsonReader.Deserialize<Dictionary<string, object>>(json);
            var data = response["data"] as Dictionary<string, object>;
            var fileDetails = data["fileDetails"] as Dictionary<string, object>;
            _img = _tc.gameObject.AddComponent<Image>();
            
            url = fileDetails["url"] as string;
            _tc.StartCoroutine(DownloadFile(url));
        }
        
        private void PrepareToDownloadWithCDNDetails(string json, object cb)
        {
            Debug.Log(json);
            string url = "";
            var response = JsonReader.Deserialize<Dictionary<string, object>>(json);
            var data = response["data"] as Dictionary<string, object>;
            url = data["cdnUrl"] as string;
            _img = _tc.gameObject.AddComponent<Image>();
            
            
            _tc.StartCoroutine(DownloadFile(url));
        }        

        IEnumerator DownloadFile(string url)
        {
            UnityWebRequest wr = new UnityWebRequest(url);
            DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
            wr.downloadHandler = texDl;
            yield return wr.SendWebRequest();
            if (wr.result == UnityWebRequest.Result.Success) 
            {
                Texture2D t = texDl.texture;
                Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height),
                    Vector2.zero, 1f);
                _img.sprite = s;
            }
            _tc.m_done = true;
            
        }
    }    
}

