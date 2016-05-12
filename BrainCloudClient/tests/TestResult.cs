using System.Collections.Generic;
using System.Threading;
using System;
using NUnit.Core;
using NUnit.Framework;
using JsonFx.Json;
using BrainCloud;


namespace BrainCloudTests
{
    public class TestResult
    {
        public bool m_done;
        public bool m_result;
        public int m_apiCountExpected;
        
        // if success
        public Dictionary<string, object> m_response;
        
        // if error
        public int m_statusCode;
        public int m_reasonCode;
        public string m_statusMessage;
        public int m_timeToWaitSecs = 120;
        public int m_globalErrorCount;
        public int m_networkErrorCount;

        public TestResult()
        {}

        public void Reset()
        {
            m_done = false;
            m_result = false;
            m_apiCountExpected = 0;
            m_response = null;
            m_statusCode = 0;
            m_reasonCode = 0;
            m_statusMessage = null;
            m_globalErrorCount = 0;
            m_networkErrorCount = 0;
        }

        public bool Run()
        {
            return RunExpectCount(1);
        }

        public bool RunExpectCount(int in_apiCount)
        {
            Reset();
            m_apiCountExpected = in_apiCount;

            Spin();
            
            Assert.True(m_result);
            
            return m_result;
        }


        public bool RunExpectFail(int in_expectedStatusCode, int in_expectedReasonCode)
        {
            Reset();
            Spin();

            Assert.False(m_result);
            if (in_expectedStatusCode != -1)
            {
                Assert.AreEqual(in_expectedStatusCode, m_statusCode);
            }
            if (in_expectedReasonCode != -1)
            {
                Assert.AreEqual(in_expectedReasonCode, m_reasonCode);
            }

            return !m_result;
        }

        public void ApiSuccess(string json, object cb)
        {
            m_response = JsonReader.Deserialize<Dictionary<string, object>>(json);
            m_result = true;
            --m_apiCountExpected;
            if (m_apiCountExpected <= 0)
            {
                m_done = true;
            }
        }

        public void ApiError(int statusCode, int reasonCode, string jsonError, object cb)
        {
            m_statusCode = statusCode;
            m_reasonCode = reasonCode; 
            m_statusMessage = jsonError;
            m_result = false;
            --m_apiCountExpected;
            if (m_apiCountExpected <= 0)
            {
                m_done = true;
            }
        }

        public void GlobalError(int statusCode, int reasonCode, string jsonError, object cb)
        {
            m_statusCode = statusCode;
            m_reasonCode = reasonCode;
            m_statusMessage = jsonError;
            m_result = false;
            --m_apiCountExpected;
            if (m_apiCountExpected <= 0)
            {
                m_done = true;
            }
            ++m_globalErrorCount;
        }

        public void NetworkError()
        {
            m_statusCode = StatusCodes.CLIENT_NETWORK_ERROR;
            m_reasonCode = ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT;
            m_statusMessage = "Network Error";
            m_result = false;
            --m_apiCountExpected;
            if (m_apiCountExpected <= 0)
            {
                m_done = true;
            }
            ++m_networkErrorCount;
        }

        public bool IsDone()
        {
            return m_done;
        }

        private void Spin()
        {
            long maxWait = m_timeToWaitSecs * 1000;
            while(!m_done && maxWait > 0)
            {
                BrainCloudClient.Get ().Update();
                Thread.Sleep (10);
                maxWait -= 10;
            }
        }

        public void SetTimeToWaitSecs(int secs)
        {
            m_timeToWaitSecs = secs;
        }
    }
}