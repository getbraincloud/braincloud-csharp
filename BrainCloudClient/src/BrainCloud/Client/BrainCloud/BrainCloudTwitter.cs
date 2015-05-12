//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using JsonFx.Json;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudTwitter
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudTwitter (BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        public void AuthorizeTwitter(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Event, ServiceOperation.Send, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        public void VerifyTwitter(
            string in_token,
            string in_verifier,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.TwitterServiceVerifyToken.Value] = in_token;
            data[OperationParam.TwitterServiceVerifyVerifier.Value] = in_verifier;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Event, ServiceOperation.Send, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        public void Tweet(
            string in_token,
            string in_secret,
            string in_tweet,
            string in_picture,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.TwitterServiceTweetToken.Value] = in_token;
            data[OperationParam.TwitterServiceTweetSecret.Value] = in_secret;
            data[OperationParam.TwitterServiceTweetTweet.Value] = in_tweet;

            if (Util.IsOptionalParameterValid(in_picture))
            {
                data[OperationParam.TwitterServiceTweetPic.Value] = in_picture;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Event, ServiceOperation.Send, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}
