//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudProfanity
    {
        private BrainCloudClient _client;

        public BrainCloudProfanity(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Checks supplied text for profanity.
        /// </summary>
        /// <remarks>
        /// Service Name - Profanity
        /// Service Operation - ProfanityCheck
        /// </remarks>
        /// <param name="text">The text to check</param>
        /// <param name="languages">Optional comma delimited list of two character language codes</param>
        /// <param name="flagEmail">Optional processing of email addresses</param>
        /// <param name="flagPhone">Optional processing of phone numbers</param>
        /// <param name="flagUrls">Optional processing of urls</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>
        public void ProfanityCheck(
            string text,
            string languages,
            bool flagEmail,
            bool flagPhone,
            bool flagUrls,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProfanityText.Value] = text;
            if (languages != null)
            {
                data[OperationParam.ProfanityLanguages.Value] = languages;
            }
            data[OperationParam.ProfanityFlagEmail.Value] = flagEmail;
            data[OperationParam.ProfanityFlagPhone.Value] = flagPhone;
            data[OperationParam.ProfanityFlagUrls.Value] = flagUrls;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Profanity, ServiceOperation.ProfanityCheck, data, callback);
            _client.SendRequest(sc);
        }


        /// <summary>
        /// Replaces the characters of profanity text with a passed character(s).
        /// </summary>
        /// <remarks>
        /// Service Name - Profanity
        /// Service Operation - ProfanityReplaceText
        /// </remarks>
        /// <param name="text">The text to check</param>
        /// <param name="replaceSymbol">The text to replace individual characters of profanity text with</param>
        /// <param name="languages">Optional comma delimited list of two character language codes</param>
        /// <param name="flagEmail">Optional processing of email addresses</param>
        /// <param name="flagPhone">Optional processing of phone numbers</param>
        /// <param name="flagUrls">Optional processing of urls</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>
        public void ProfanityReplaceText(
            string text,
            string replaceSymbol,
            string languages,
            bool flagEmail,
            bool flagPhone,
            bool flagUrls,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProfanityText.Value] = text;
            data[OperationParam.ProfanityReplaceSymbol.Value] = replaceSymbol;
            if (languages != null)
            {
                data[OperationParam.ProfanityLanguages.Value] = languages;
            }
            data[OperationParam.ProfanityFlagEmail.Value] = flagEmail;
            data[OperationParam.ProfanityFlagPhone.Value] = flagPhone;
            data[OperationParam.ProfanityFlagUrls.Value] = flagUrls;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Profanity, ServiceOperation.ProfanityReplaceText, data, callback);
            _client.SendRequest(sc);
        }


        /// <summary>
        /// Checks supplied text for profanity and returns a list of bad wors.
        /// </summary>
        /// <remarks>
        /// Service Name - Profanity
        /// Service Operation - ProfanityIdentifyBadWords
        /// </remarks>
        /// <param name="text">The text to check</param> 
        /// <param name="languages">Optional comma delimited list of two character language codes</param>
        /// <param name="flagEmail">Optional processing of email addresses</param>
        /// <param name="flagPhone">Optional processing of phone numbers</param>
        /// <param name="flagUrls">Optional processing of urls</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>
        public void ProfanityIdentifyBadWords(
            string text,
            string languages,
            bool flagEmail,
            bool flagPhone,
            bool flagUrls,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProfanityText.Value] = text;
            if (languages != null)
            {
                data[OperationParam.ProfanityLanguages.Value] = languages;
            }
            data[OperationParam.ProfanityFlagEmail.Value] = flagEmail;
            data[OperationParam.ProfanityFlagPhone.Value] = flagPhone;
            data[OperationParam.ProfanityFlagUrls.Value] = flagUrls;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Profanity, ServiceOperation.ProfanityIdentifyBadWords, data, callback);
            _client.SendRequest(sc);
        }

    }
}
