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
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudProfanity(BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        /// <summary>
        /// Checks supplied text for profanity.
        /// </summary>
        /// <remarks>
        /// Service Name - Profanity
        /// Service Operation - ProfanityCheck
        /// </remarks>
        /// <param name="in_text">The text to check</param>
        /// <param name="in_languages">Optional comma delimited list of two character language codes</param>
        /// <param name="in_flagEmail">Optional processing of email addresses</param>
        /// <param name="in_flagPhone">Optional processing of phone numbers</param>
        /// <param name="in_flagUrls">Optional processing of urls</param>
        /// <param name="in_success">The success callback.</param>
        /// <param name="in_failure">The failure callback.</param>
        /// <param name="in_cbObject">The user object sent to the callback.</param>
        public void ProfanityCheck(
            string in_text,
            string in_languages,
            bool in_flagEmail,
            bool in_flagPhone,
            bool in_flagUrls,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProfanityText.Value] = in_text;
            if (in_languages != null)
            {
                data[OperationParam.ProfanityLanguages.Value] = in_languages;
            }
            data[OperationParam.ProfanityFlagEmail.Value] = in_flagEmail;
            data[OperationParam.ProfanityFlagPhone.Value] = in_flagPhone;
            data[OperationParam.ProfanityFlagUrls.Value] = in_flagUrls;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Profanity, ServiceOperation.ProfanityCheck, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }


        /// <summary>
        /// Replaces the characters of profanity text with a passed character(s).
        /// </summary>
        /// <remarks>
        /// Service Name - Profanity
        /// Service Operation - ProfanityReplaceText
        /// </remarks>
        /// <param name="in_text">The text to check</param>
        /// <param name="in_replaceSymbol">The text to replace individual characters of profanity text with</param>
        /// <param name="in_languages">Optional comma delimited list of two character language codes</param>
        /// <param name="in_flagEmail">Optional processing of email addresses</param>
        /// <param name="in_flagPhone">Optional processing of phone numbers</param>
        /// <param name="in_flagUrls">Optional processing of urls</param>
        /// <param name="in_success">The success callback.</param>
        /// <param name="in_failure">The failure callback.</param>
        /// <param name="in_cbObject">The user object sent to the callback.</param>
        public void ProfanityReplaceText(
            string in_text,
            string in_replaceSymbol,
            string in_languages,
            bool in_flagEmail,
            bool in_flagPhone,
            bool in_flagUrls,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProfanityText.Value] = in_text;
            data[OperationParam.ProfanityReplaceSymbol.Value] = in_replaceSymbol;
            if (in_languages != null)
            {
                data[OperationParam.ProfanityLanguages.Value] = in_languages;
            }
            data[OperationParam.ProfanityFlagEmail.Value] = in_flagEmail;
            data[OperationParam.ProfanityFlagPhone.Value] = in_flagPhone;
            data[OperationParam.ProfanityFlagUrls.Value] = in_flagUrls;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Profanity, ServiceOperation.ProfanityReplaceText, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }


        /// <summary>
        /// Checks supplied text for profanity and returns a list of bad wors.
        /// </summary>
        /// <remarks>
        /// Service Name - Profanity
        /// Service Operation - ProfanityIdentifyBadWords
        /// </remarks>
        /// <param name="in_text">The text to check</param> 
        /// <param name="in_languages">Optional comma delimited list of two character language codes</param>
        /// <param name="in_flagEmail">Optional processing of email addresses</param>
        /// <param name="in_flagPhone">Optional processing of phone numbers</param>
        /// <param name="in_flagUrls">Optional processing of urls</param>
        /// <param name="in_success">The success callback.</param>
        /// <param name="in_failure">The failure callback.</param>
        /// <param name="in_cbObject">The user object sent to the callback.</param>
        public void ProfanityIdentifyBadWords(
            string in_text,
            string in_languages,
            bool in_flagEmail,
            bool in_flagPhone,
            bool in_flagUrls,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProfanityText.Value] = in_text;
            if (in_languages != null)
            {
                data[OperationParam.ProfanityLanguages.Value] = in_languages;
            }
            data[OperationParam.ProfanityFlagEmail.Value] = in_flagEmail;
            data[OperationParam.ProfanityFlagPhone.Value] = in_flagPhone;
            data[OperationParam.ProfanityFlagUrls.Value] = in_flagUrls;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Profanity, ServiceOperation.ProfanityIdentifyBadWords, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

    }
}
