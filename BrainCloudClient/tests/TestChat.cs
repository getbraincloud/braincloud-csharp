using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;
using BrainCloud.Common;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestChat : TestFixtureBase
    {
        [Test]
        public void TestGetInvalidChannelId()
        {
            TestResult tr = new TestResult(_bc);

            _bc.ChatService.GetChannelId("gl", "invalid", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.CHANNEL_NOT_FOUND);
        }

        [Test]
        public void TestGetChannelInfo()
        {
            // Get channel id
            string channelId = GetChannelId();

            TestResult tr = new TestResult(_bc);

            // Get channel info
            _bc.ChatService.GetChannelInfo(channelId, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestChannelConnect()
        {
            // Get channel id
            string channelId = GetChannelId();

            TestResult tr = new TestResult(_bc);

            // Get channel info
            _bc.ChatService.ChannelConnect(channelId, 50, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetSubscribedChannels()
        {
            // Get channel id
            string channelId = GetChannelId();

            TestResult tr = new TestResult(_bc);

            // Get channel info
            _bc.ChatService.GetSubscribedChannels("gl", tr.ApiSuccess, tr.ApiError);
            tr.Run();

            // Verify that our channel is present in the channel list
            var channels = ((object[])((Dictionary<string, object>)tr.m_response["data"])["channels"]);
            int i = 0;
            for (; i < channels.Length; ++i)
            {
                string curChannelId = (string)((Dictionary<string, object>)channels[i])["id"];
                if (channelId == curChannelId) break;
            }

            Assert.IsFalse(i == channels.Length);
        }

        [Test]
        public void TestPostChatMessage()
        {
            // Get channel id
            string channelId = GetChannelId();
            PostChatMessage(channelId);
        }

        [Test]
        public void TestGetChatMessage()
        {
            // Get channel id
            string channelId = GetChannelId();
            string msgId = (string)PostChatMessage(channelId)["msgId"];

            TestResult tr = new TestResult(_bc);

            // Get channel info
            _bc.ChatService.GetChatMessage(channelId, msgId, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Assert.IsTrue((string)((Dictionary<string, object>)((Dictionary<string, object>)tr.m_response["data"])["content"])["text"] == "Hello World!");
        }

        [Test]
        public void TestUpdateChatMessage()
        {
            // Get channel id
            string channelId = GetChannelId();
            var message = PostChatMessage(channelId);
            string msgId = (string)message["msgId"];
            int version = 1;

            TestResult tr = new TestResult(_bc);

            // Get channel info
            _bc.ChatService.UpdateChatMessage(channelId, msgId, version, "Hello World! edited", null, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetRecentMessages()
        {
            // Get channel id
            string channelId = GetChannelId();
            string msgId = (string)PostChatMessage(channelId)["msgId"];

            TestResult tr = new TestResult(_bc);

            // Get recent messages
            _bc.ChatService.GetRecentChatMessages(channelId, 50, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            // Verify that our channel is present in the channel list
            var messages = ((object[])((Dictionary<string, object>)tr.m_response["data"])["messages"]);
            int i = 0;
            for (; i < messages.Length; ++i)
            {
                string curMsgId = (string)((Dictionary<string, object>)messages[i])["msgId"];
                if (msgId == curMsgId) break;
            }

            Assert.IsFalse(i == messages.Length);
        }

        [Test]
        public void TestDeleteChatMessage()
        {
            // Get channel id
            string channelId = GetChannelId();
            var message = PostChatMessage(channelId);
            string msgId = (string)message["msgId"];
            int version = 1;

            TestResult tr = new TestResult(_bc);

            // Get channel info
            _bc.ChatService.DeleteChatMessage(channelId, msgId, version, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        // Helpers
        private string GetChannelId()
        {
            TestResult tr = new TestResult(_bc);
            
            _bc.ChatService.GetChannelId("gl", "valid", tr.ApiSuccess, tr.ApiError);
            tr.Run();
            return (string)((Dictionary<string, object>)tr.m_response["data"])["channelId"];
        }

        [Test]
        public void TestChannelDisconnect()
        {
            // Get channel id
            string channelId = GetChannelId();

            TestResult tr = new TestResult(_bc);

            _bc.ChatService.ChannelConnect(channelId, 50, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.ChatService.ChannelDisconnect(channelId, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestPostChatMessageSimple()
        {
            TestResult tr = new TestResult(_bc);
            
            // Get channel id
            string channelId = GetChannelId();

            _bc.ChatService.PostChatMessageSimple(channelId, "Hello World!", true, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        private Dictionary<string, object> PostChatMessage(string channelId)
        {
            TestResult tr = new TestResult(_bc);
            
            _bc.ChatService.PostChatMessage(channelId, "Hello World!", null, true, tr.ApiSuccess, tr.ApiError);
            tr.Run();
            return ((Dictionary<string, object>)tr.m_response["data"]);
        }
    }
}
