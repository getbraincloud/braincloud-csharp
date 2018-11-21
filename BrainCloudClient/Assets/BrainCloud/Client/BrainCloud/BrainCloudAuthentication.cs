//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using BrainCloud.Internal;
using BrainCloud.Common;

namespace BrainCloud
{
    public class BrainCloudAuthentication
    {
        private BrainCloudClient _client;

        public string AnonymousId { get; set; }
        public string ProfileId { get; set; }

        public BrainCloudAuthentication(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Used to create the anonymous installation id for the brainCloud profile.
        /// </summary>
        /// <returns>A unique Anonymous ID</returns>
        public string GenerateAnonymousId()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initialize - initializes the identity service with a saved
        /// anonymous installation id and most recently used profile id
        /// </summary>
        /// <param name="profileId">
        /// The id of the profile id that was most recently used by the app (on this device)
        /// </param>
        /// <param name="anonymousId">
        /// The anonymous installation id that was generated for this device
        /// </param>
        public void Initialize(string profileId, string anonymousId)
        {
            AnonymousId = anonymousId;
            ProfileId = profileId;
        }

        /// <summary>
        /// Used to clear the saved profile id - to use in cases when the user is
        /// attempting to switch to a different app profile.
        /// </summary>
        public void ClearSavedProfileID()
        {
            ProfileId = null;
        }

        /// <summary>
        /// Authenticate a user anonymously with brainCloud - used for apps that don't want to bother
        /// the user to login, or for users who are sensitive to their privacy
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        /// </remarks>
        /// <param name="forceCreate">
        /// Should a new profile be created if it does not exist?
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user supplied callback object
        /// </param>
        public void AuthenticateAnonymous(
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(AnonymousId, "", AuthenticationType.Anonymous,
                              null, forceCreate, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user with a custom Email and Password.  Note that the client app
        /// is responsible for collecting (and storing) the e-mail and potentially password
        /// (for convenience) in the client data.  For the greatest security,
        /// force the user to re-enter their password at each login.
        /// (Or at least give them that option).
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        ///
        /// Note that the password sent from the client to the server is protected via SSL.
        /// </remarks>
        /// <param name="email">
        /// The e-mail address of the user
        /// </param>
        /// <param name="password">
        /// The password of the user
        /// </param>
        /// <param name="forceCreate">
        /// Should a new profile be created for this user if the account does not exist?
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user supplied callback object
        /// </param>
        public void AuthenticateEmailPassword(
            string email,
            string password,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(email, password, AuthenticationType.Email,
                              null, forceCreate, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user using a userId and password (without any validation on the userId).
        /// Similar to AuthenticateEmailPassword - except that that method has additional features to
        /// allow for e-mail validation, password resets, etc.
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        /// </remarks>
        /// <param name="email">
        /// The e-mail address of the user
        /// </param>
        /// <param name="password">
        /// The password of the user
        /// </param>
        /// <param name="forceCreate">
        /// Should a new profile be created for this user if the account does not exist?
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user supplied callback object
        /// </param>
        public void AuthenticateUniversal(
            string userId,
            string password,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(userId, password, AuthenticationType.Universal,
                              null, forceCreate, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user with brainCloud using their Facebook Credentials
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        /// </remarks>
        /// <param name="externalId">
        /// The facebook id of the user
        /// </param>
        /// <param name="authenticationToken">
        /// The validated token from the Facebook SDK (that will be further
        /// validated when sent to the bC service)
        /// </param>
        /// <param name="forceCreate">
        /// Should a new profile be created for this user if the account does not exist?
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user supplied callback object
        /// </param>
        public void AuthenticateFacebook(
            string externalId,
            string authenticationToken,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(externalId, authenticationToken, AuthenticationType.Facebook,
                              null, forceCreate, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user using their Game Center id
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        /// </remarks>
        /// <param name="gameCenterId">
        /// The user's game center id  (use the profileID property from the local GKPlayer object)
        /// </param>
        /// <param name="forceCreate">
        /// Should a new profile be created for this user if the account does not exist?
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user supplied callback object
        /// </param>
        public void AuthenticateGameCenter(
            string gameCenterId,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(gameCenterId, "", AuthenticationType.GameCenter,
                              null, forceCreate, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user using a steam userId and session ticket (without any validation on the userId).
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        /// </remarks>
        /// <param name="userId">
        /// String representation of 64 bit steam id
        /// </param>
        /// <param name="sessionticket">
        /// The session ticket of the user (hex encoded)
        /// </param>
        /// <param name="forceCreate">
        /// Should a new profile be created for this user if the account does not exist?
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user supplied callback object
        /// </param>
        public void AuthenticateSteam(
            string userId,
            string sessionticket,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(userId, sessionticket, AuthenticationType.Steam,
                              null, forceCreate, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user using a google userId(email address) and google authentication token.
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        /// </remarks>
        /// <param name="userId">
        /// String representation of google+ userId (email)
        /// </param>
        /// <param name="token">
        /// The authentication token derived via the google apis.
        /// </param>
        /// <param name="forceCreate">
        /// Should a new profile be created for this user if the account does not exist?
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user supplied callback object
        /// </param>
        public void AuthenticateGoogle(
            string userId,
            string token,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(userId, token, AuthenticationType.Google,
                null, forceCreate, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user using a Twitter userId, authentication token, and secret from twitter.
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        /// </remarks>
        /// <param name="userId">
        /// String representation of a Twitter user ID
        /// </param>
        /// <param name="token">
        /// The authentication token derived via the Twitter apis
        /// </param>
        /// <param name="secret">
        /// The secret given when attempting to link with Twitter
        /// </param>
        /// <param name="forceCreate">
        /// Should a new profile be created for this user if the account does not exist?
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user supplied callback object
        /// </param>
        public void AuthenticateTwitter(
            string userId,
            string token,
            string secret,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(userId, token + ":" + secret, AuthenticationType.Twitter,
                null, forceCreate, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user using a Pase userId and authentication token
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        /// </remarks>
        /// <param name="userId">
        /// String representation of Parse user ID
        /// </param>
        /// <param name="token">
        /// The authentication token
        /// </param>
        /// <param name="forceCreate">
        /// Should a new profile be created for this user if the account does not exist?
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user supplied callback object
        /// </param>
        public void AuthenticateParse(
            string userId,
            string token,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(userId, token, AuthenticationType.Parse,
                null, forceCreate, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user using a handoffId and authentication token
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        /// </remarks>
        /// <param name="handoffId">
        /// braincloud handoffId that is generated from cloud script
        /// </param>
        /// <param name="securityToken">
        /// The authentication token
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user supplied callback object
        /// </param>
        public void AuthenticateHandoff(
            string handoffId,
            string securityToken,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(handoffId, securityToken, AuthenticationType.Handoff,
                null, false, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user via cloud code (which in turn validates the supplied credentials against an external system).
        /// This allows the developer to extend brainCloud authentication to support other backend authentication systems.
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        /// </remarks>
        /// <param name="userId">
        /// The user id
        /// </param>
        /// <param name="token">
        /// The user token (password etc)
        /// </param>
        /// /// <param name="externalAuthName">
        /// The name of the cloud script to call for external authentication
        /// </param>
        /// <param name="forceCreate">
        /// Should a new profile be created for this user if the account does not exist?
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user supplied callback object
        /// </param>
        public void AuthenticateExternal(
            string userId,
            string token,
            string externalAuthName,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(userId, token, AuthenticationType.External,
                externalAuthName, forceCreate, success, failure, cbObject);
        }

        /// <summary>
        /// Reset Email password - Sends a password reset email to the specified address
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Operation - ResetEmailPassword
        /// </remarks>
        /// <param name="externalId">
        /// The email address to send the reset email to.
        /// </param>
        /// <param name="success">
        /// The method to call in event of success
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error
        /// </param>
        /// <param name="cbObject">
        /// The user supplied callback object
        /// </param>
        public void ResetEmailPassword(
            string externalId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.AuthenticateServiceAuthenticateExternalId.Value] = externalId;
            data[OperationParam.AuthenticateServiceAuthenticateGameId.Value] = _client.AppId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure);
            ServerCall sc = new ServerCall(ServiceName.Authenticate, ServiceOperation.ResetEmailPassword, data, callback);
            _client.SendRequest(sc);
        }


        private void Authenticate(
            string externalId,
            string authenticationToken,
            AuthenticationType authenticationType,
            string externalAuthName,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            string languageCode = _client.LanguageCode;
            double utcOffset = Util.GetUTCOffsetForCurrentTimeZone();
            string countryCode = _client.CountryCode;

            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.AuthenticateServiceAuthenticateExternalId.Value] = externalId;
            data[OperationParam.AuthenticateServiceAuthenticateAuthenticationToken.Value] = authenticationToken;
            data[OperationParam.AuthenticateServiceAuthenticateAuthenticationType.Value] = authenticationType.ToString();
            data[OperationParam.AuthenticateServiceAuthenticateForceCreate.Value] = forceCreate;

            data[OperationParam.AuthenticateServiceAuthenticateProfileId.Value] = ProfileId;
            data[OperationParam.AuthenticateServiceAuthenticateAnonymousId.Value] = AnonymousId;
            data[OperationParam.AuthenticateServiceAuthenticateGameId.Value] = _client.AppId;
            data[OperationParam.AuthenticateServiceAuthenticateReleasePlatform.Value] = _client.ReleasePlatform.ToString();
            data[OperationParam.AuthenticateServiceAuthenticateGameVersion.Value] = _client.AppVersion;
            data[OperationParam.AuthenticateServiceAuthenticateBrainCloudVersion.Value] = Version.GetVersion();

#if DOT_NET
            data["clientLib"] = "csharp";
#else
            data["clientLib"] = "csharp-unity";
#endif

            if (Util.IsOptionalParameterValid(externalAuthName))
            {
                data[OperationParam.AuthenticateServiceAuthenticateExternalAuthName.Value] = externalAuthName;
            }
            data[OperationParam.AuthenticateServiceAuthenticateCountryCode.Value] = countryCode;
            data[OperationParam.AuthenticateServiceAuthenticateLanguageCode.Value] = languageCode;
            data[OperationParam.AuthenticateServiceAuthenticateTimeZoneOffset.Value] = utcOffset;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Authenticate, ServiceOperation.Authenticate, data, callback);
            _client.SendRequest(sc);
        }
    }
}
