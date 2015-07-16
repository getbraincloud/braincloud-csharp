using System.Collections.Generic;
using System.Threading;
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
        
        // if success
        public Dictionary<string, object> m_response;
        
        // if error
        public int m_statusCode;
        public int m_reasonCode;
        public string m_statusMessage;

        public TestResult()
        {}

        public void Reset()
        {
            m_done = false;
            m_result = false;
            m_response = null;
            m_statusCode = 0;
            m_reasonCode = 0;
            m_statusMessage = null;
        }

        public bool Run()
        {
            Reset();
            Spin();

            Assert.True(m_result);

            return m_result;
        }

        public bool RunExpectFail(int in_expectedStatusCode, int in_expectedReasonCode)
        {
            Reset();
            Spin();

            Assert.False(m_result);
            Assert.AreEqual(in_expectedStatusCode, m_statusCode);
            Assert.AreEqual(in_expectedReasonCode, m_reasonCode);

            return !m_result;
        }

        public void ApiSuccess(string json, object cb)
        {
            m_response = JsonReader.Deserialize<Dictionary<string, object>>(json);
            m_result = true;
            m_done = true;
        }

        public void ApiError(int statusCode, int reasonCode, string statusMessage, object cb)
        {
            m_statusCode = statusCode;
            m_reasonCode = reasonCode; 
            m_statusMessage = statusMessage;
            m_result = false;
            m_done = true;
        }

        public bool IsDone()
        {
            return m_done;
        }

        private void Spin()
        {
            long maxWait = 30 * 1000;
            while(!m_done && maxWait > 0)
            {
                BrainCloudClient.Get ().Update();
                Thread.Sleep (100);
                maxWait -= 100;
            }
        }
    }
}