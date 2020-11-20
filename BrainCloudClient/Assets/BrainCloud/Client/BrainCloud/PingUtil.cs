//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------
#if ((UNITY_5_3_OR_NEWER) && !UNITY_WEBPLAYER && (!UNITY_IOS || ENABLE_IL2CPP)) || UNITY_2018_3_OR_NEWER
#define USE_WEB_REQUEST //Comment out to force use of old WWW class on Unity 5.3+
#endif

namespace BrainCloud
{
#if USE_WEB_REQUEST
#if UNITY_5_3
using UnityEngine.Experimental.Networking;
#else
    using UnityEngine.Networking;
#endif
#endif

    using BrainCloud.JsonFx.Json;
    using System.Collections.Generic;
    using System.Collections;
    using System;

    public static class PingUtil
    {
        #region Public
        public const int MAX_PING_CALLS = 4;
        public const int NUM_PING_CALLS_IN_PARRALLEL = 2;
        #endregion

        #region PRIVATE
        public static void PingNextItemToProcess(PingRequest in_pingRequest)
        {
            lock (in_pingRequest.targetsToProcess)
            {
                if (in_pingRequest.targetsToProcess.Count > 0)
                {
                    for (int i = 0; i < NUM_PING_CALLS_IN_PARRALLEL && in_pingRequest.targetsToProcess.Count > 0; ++i)
                    {
                        KeyValuePair<string, string> pair = in_pingRequest.targetsToProcess[0];
                        in_pingRequest.targetsToProcess.RemoveAt(0);
                        PingHost(pair.Key, pair.Value, in_pingRequest);
                    }
                }
                else if (in_pingRequest.responsePingData.Count == in_pingRequest.pingData.Count && in_pingRequest.successCallback != null)
                {
                    string pingStr = JsonWriter.Serialize(in_pingRequest.pingData);
                    in_pingRequest.clientRef.Log(in_pingRequest.name + " PINGS: " + pingStr);
                    in_pingRequest.successCallback(pingStr, in_pingRequest.callbackObject);
                    in_pingRequest.successCallback = null;
                }
            }
        }

        static void PingHost(string in_region, string in_target, PingRequest in_pingRequest)
        {
#if DOT_NET
            PingUpdateSystem(in_region, in_target, in_pingRequest);
#else       
            in_target = "http://" + in_target;

            if (in_pingRequest.clientRef.Wrapper != null)
                in_pingRequest.clientRef.Wrapper.StartCoroutine(HandlePingReponse(in_region, in_target, in_pingRequest));
#endif
        }

#if DOT_NET
        static void PingUpdateSystem(string in_region, string in_target, PingRequest in_pingRequest)
        {
            System.Net.NetworkInformation.Ping pinger = new System.Net.NetworkInformation.Ping();
            try
            {
                pinger.PingCompleted += (o, e) =>
                {
                    if (e.Error == null && e.Reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    {
                        HandlePingTimeResponse(e.Reply.RoundtripTime, in_region, in_pingRequest);
                    }
                };

                pinger.SendAsync(in_target, null);
            }
            catch (System.Net.NetworkInformation.PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }
        }
#else
        private static IEnumerator HandlePingReponse(string in_region, string in_target, PingRequest in_pingRequest)
        {
            long sentPing = DateTime.Now.Ticks;
#if USE_WEB_REQUEST
            UnityWebRequest _request = UnityWebRequest.Get(in_target);
            yield return _request.SendWebRequest();
#else
            WWWForm postForm = new WWWForm();
            WWW _request = new WWW(in_target, postForm);
#endif

            //if (_request.error == null && !_request.isNetworkError)
            {
                HandlePingTimeResponse((DateTime.Now.Ticks - sentPing) / 10000, in_region, in_pingRequest);
            }
        }
#endif

        static void HandlePingTimeResponse(long in_responseTime, string in_region, PingRequest in_pingRequest)
        {
            in_pingRequest.cachedPingResponses[in_region].Add(in_responseTime);

            if (in_pingRequest.cachedPingResponses[in_region].Count == MAX_PING_CALLS)
            {
                long totalAccumulated = 0;
                long highestValue = 0;
                foreach (var pingResponse in in_pingRequest.cachedPingResponses[in_region])
                {
                    totalAccumulated += pingResponse;
                    if (pingResponse > highestValue)
                    {
                        highestValue = pingResponse;
                    }
                }

                // accumulated ALL, now subtract the highest value
                totalAccumulated -= highestValue;
                in_pingRequest.pingData[in_region] = totalAccumulated / (in_pingRequest.cachedPingResponses[in_region].Count - 1);
            }

            PingNextItemToProcess(in_pingRequest);
        }

        public struct PingRequest
        {
            public List<KeyValuePair<string, string>> targetsToProcess;
            public Dictionary<string, long> pingData;
            public Dictionary<string, object> responsePingData;
            public Dictionary<string, List<long>> cachedPingResponses;
            public BrainCloudClient clientRef;
            public SuccessCallback successCallback;
            public object callbackObject;
            public string name;
        }
        #endregion

    }
}
