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

            string[] emailAddresseses = { "recipient@mail.com" };
            string jsonServiceParams = "{\"fromAddress\":\"sender@mail.com\",\"fromName\":\"Test Mailer\",\"replyToAddress\":\"\",\"replyToName\":\"\",\"categories\":[],\"attachments\":[], \"subject\":\"Plain text email\", \"body\":\"This is a test from Unity\"}";

            _tc.bcWrapper.MailService.SendAdvancedEmailByAddresses(emailAddresseses, jsonServiceParams, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());
        }

    }
}