using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BrainCloudUnity
{
    [System.Serializable]
    public struct AppIdSecretPair
    {
        [SerializeField]
        public string appName;
        [SerializeField]
        public string appId;
        [SerializeField]
        public string appSecret;

        public AppIdSecretPair(string appName, string appId, string appSecret)
        {
            this.appName = appName;
            this.appId = appId;
            this.appSecret = appSecret;
        }

        public static AppIdSecretPair[] FromDictionary(Dictionary<string, string> dict)
        {
          AppIdSecretPair [] array = new AppIdSecretPair[dict.Count];

            int index = 0;

            foreach (var item in dict)
            {
                array[index] = new AppIdSecretPair("", item.Key, item.Value);

                index++;
            }

            return array;

        }
        
        public static Dictionary<string, string> ToDictionary(AppIdSecretPair [] array)
        {
            return array.ToDictionary(item => item.appId, item => item.appSecret);
        }
    }
}