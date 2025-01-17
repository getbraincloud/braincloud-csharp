using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using BrainCloud;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class TestMail : TestFixtureBase
    {        
        [UnityTest]
        public IEnumerator TestSendAdvancedEmailByAddresses()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            string[] emailAddresseses = { "bc-client-team@bitheads.com", "test@bitheads.com" };
            string jsonServiceParams = "{\"fromAddress\":\"bc-client-team@bitheads.com\",\"fromName\":\"BC Client Team\",\"replyToAddress\":\"bc-client-team@bitheads.com\",\"replyToName\":\"Optional ReplyTo\",\"templateId\":\"d-www-xxx-yyy-zzz\",\"dynamicData\":{\"user\":{\"firstName\":\"John\",\"lastName\":\"Doe\"},\"resetLink\":\"www.dummuyLink.io\"},\"categories\":[\"category1\",\"category2\"],\"attachments\":[{\"content\":\"VGhpcyBhdHRhY2htZW50IHRleHQ=\",\"filename\":\"attachment.txt\"}]}";

            _tc.bcWrapper.MailService.SendAdvancedEmailByAddresses(emailAddresseses, jsonServiceParams, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());
        }

    }
}