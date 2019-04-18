using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using BrainCloud.Common;
using JsonFx.Json;
using System;
using System.Threading;
using System.Collections.Generic;


// namespace BrainCloudTests
// {   
//     public class RTTCallback : BrainCloud :: RTTCallback
//     {
//         BrainCloudClient m_pBC;
//         String m_expectedServiceName;
//         String m_expectedOperation;
//         int m_waitTime;
//         bool m_receivedCallback;
//         public RTTCallback(
//             BrainCloudClient pBC,
//             string expectedServiceName,
//             string expectedOperation, 
//             int waitTime)
//             {
//                 m_pBC = pBC;
//                 m_expectedServiceName = expectedServiceName;
//                 m_expectedOperation = expectedOperation;
//                 m_waitTime = waitTime;
//             }

//         public void rTTCallback(string eventJson)
//         {
//             Dictionary<string, object> data = new Dictionary<string, object>();
//             data = JsonReader.Deserialize<Dictionary<string, object>>(eventJson);
//             string service = data["service"].ToString();
//             string operation = data["operation"].ToString();

//             if(service == m_expectedServiceName && (operation == m_expectedOperation || m_expectedOperation == ""))
//             {
//                 m_receivedCallback = true;
//             }
//         }

//         bool receivedCallback()
//         {
//             TimeSpan timeToWait = TimeSpan.FromSeconds(m_waitTime);
//             DateTime startTime = DateTime.Now;
//             ///wait the time
//             while((DateTime.Now.Subtract(startTime) <= timeToWait))
//             {
//                 m_pBC.Update();
//                 if(m_receivedCallback)
//                 {
//                     return true;
//                 }
//                 Thread.Sleep(100);
//             }
//             return false; //timed out
//         }

//         void reset()
//         {
//             m_receivedCallback = false;
//         }
//     }

//     [TestFixture]
//     public class TestRTT : TestFixtureBase
//     {
// 		//these tests also test requestClientConnection because its called first in EnableRtt. Authenticates and registers automatically.
//         [Test]
//         public void TestRTTCalls()
//         {
//             TestResult tr = new TestResult(_bc);
//             _bc.Client.EnableRTT(eRTTConnectionType.WEBSOCKET, tr.ApiSuccess, tr.ApiError);
//             tr.Run();

//             RTTCallback rttcallback = new RTTCallback(_bc.Client, "event", "SEND", 10);
//             TestResult tr2 = new TestResult(_bc);
//             _bc.Client.RegisterRTTEventCallback(rttcallback);

//             TestResult trx = new TestResult(_bc);
//             _bc.Client.DisableRTT();
//             trx.Run();
//         }
//     }
// }