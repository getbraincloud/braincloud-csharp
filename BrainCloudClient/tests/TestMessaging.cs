using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;
using BrainCloud.Common;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestMessaging : TestFixtureBase
    {
        [Test]
        public void TestDeleteMessages()
        {
            TestResult tr = new TestResult(_bc);

            string[] msgIds = {"invalidMsgId"};
            _bc.MessagingService.DeleteMessages("inbox", msgIds, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetMessageboxes()
        {
            TestResult tr = new TestResult(_bc);

            _bc.MessagingService.GetMessageboxes(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetMessageCounts()
        {
            TestResult tr = new TestResult(_bc);

            _bc.MessagingService.GetMessageCounts(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetMessages()
        {
            TestResult tr = new TestResult(_bc);

            string[] msgIds = {"invalidMsgId"};
            _bc.MessagingService.GetMessages("inbox", msgIds, tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.MESSAGE_NOT_FOUND);
        }

        [Test]
        public void TestGetMessagesPage()
        {
            TestResult tr = new TestResult(_bc);
            string profileId = _bc.Client.ProfileId;

            _bc.MessagingService.GetMessagesPage("{\"pagination\":{\"rowsPerPage\":10,\"pageNumber\":1},\"searchCriteria\":{\"$or\":[{\"message.message.from\":\"" + profileId + "\"},{\"message.message.to\":\"" + profileId + "\"}]},\"sortCriteria\":{\"mbCr\":1,\"mbUp\":-1}}", tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetMessagesPageOffset()
        {
            TestResult tr = new TestResult(_bc);

            _bc.MessagingService.GetMessagesPageOffset("invalidContext", 1, tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.DECODE_CONTEXT);
        }

        [Test]
        public void TestSendMessage()
        {
            TestResult tr = new TestResult(_bc);
            Dictionary<string, object> content = new Dictionary<string, object>();
            content.Add("Subject", "Test");
            content.Add("Text", "BlahBlah");

            string profileId = _bc.Client.ProfileId;
            string[] toProfileIds = {profileId};
            _bc.MessagingService.SendMessage(toProfileIds, content, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestSendMessageSimple()
        {
            TestResult tr = new TestResult(_bc);
            string profileId = _bc.Client.ProfileId;

            string[] toProfileIds = {profileId};
            _bc.MessagingService.SendMessageSimple(toProfileIds, "This is text", tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestMarkMessagesRead()
        {
            TestResult tr = new TestResult(_bc);

            string[] msgIds = {"invalidMsgId"};
            _bc.MessagingService.MarkMessagesRead("inbox", msgIds, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}
