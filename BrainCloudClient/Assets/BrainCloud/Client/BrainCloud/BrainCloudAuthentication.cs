// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

//----------------------------------------------------
namespace BrainCloud
{

    using System;
    using System.Collections.Generic;
    using BrainCloud.Internal;
    using BrainCloud.Common;
    using BrainCloud.JsonFx.Json;

    public class BrainCloudAuthentication
    {
        private BrainCloudClient _client;
        public bool CompressResponses { get; set; } = true;
        public string AnonymousId { get; set; }
        public string ProfileId { get; set; }
        public string AuthenticationType { get; set; }
        public BrainCloudAuthentication(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Used to create the anonymous installation id for the brainCloud profile.
        /// </summary>

        public string GenerateAnonymousId()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initialize - initializes the identity service with a saved
        /// </summary>
        /// <param name="anonymousId">The anonymous installation id that was generated for this device</param>
        /// <param name="profileId">The id of the profile id that was most recently used by the app (on this device)</param>

        public void Initialize(string profileId, string anonymousId)
        {
            ProfileId = profileId;
            AnonymousId = anonymousId;
            CompressResponses = true;
        }

        /// <summary>
        /// Used to clear the saved profile id - to use in cases when the user is
        /// </summary>

        public void ClearSavedProfileID()
        {
            ProfileId = null;
        }

        /// <summary>
        /// Get server version.
        /// </summary>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>
        public void getServerVersion(SuccessCallback success = null, FailureCallback failure = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.AuthenticateServiceAuthenticateGameId.Value] = _client.AppId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure);
            ServerCall sc = new ServerCall(ServiceName.Authenticate, ServiceOperation.GetServerVersion, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Authenticate a user anonymously with brainCloud - used for apps that don't want to bother
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - AUTHENTICATE
        /// </remarks>
        /// <param name="forceCreate">Should a new profile be created if it does not exist?</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void AuthenticateAnonymous(
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(AnonymousId, "", Common.AuthenticationType.Anonymous,
                              null, forceCreate, null, success, failure, cbObject);
        }

        /// <summary>
        /// Overloaded version AuthenticateAnonymous call, takes in more parameters. This is made as temporary
        /// fix to this service's implementation structure. The AnonymousId of the regular call tends to be null
        /// even after initializing the profileId and AnonId.This leads to an externalId missing error. This only
        /// happens when the client uses the service directly and not through the Wrapper. 
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        /// </remarks>
        /// <param name="anonymousId">
        /// provide an externalId, can be anything to keep anonymous
        /// </param>
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
            string anonymousId,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            AnonymousId = anonymousId;
            AuthenticateAnonymous(forceCreate, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user with a custom Email and Password.  Note that the client app
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - AUTHENTICATE
        /// </remarks>
        /// <param name="email">The e-mail address of the user</param>
        /// <param name="password">The password of the user</param>
        /// <param name="forceCreate">Should a new profile be created for this user if the account does not exist?</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void AuthenticateEmailPassword(
            string email,
            string password,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(email, password, Common.AuthenticationType.Email,
                              null, forceCreate, null, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user using a userid and password (without any validation on the userid).
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - AUTHENTICATE
        /// </remarks>
        /// <param name="email">The e-mail address of the user</param>
        /// <param name="password">The password of the user</param>
        /// <param name="forceCreate">Should a new profile be created for this user if the account does not exist?</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void AuthenticateUniversal(
            string userId,
            string password,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(userId, password, Common.AuthenticationType.Universal,
                              null, forceCreate, null, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user with brainCloud using their Facebook Credentials
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - AUTHENTICATE
        /// </remarks>
        /// <param name="fbUserId">The facebook id of the user</param>
        /// <param name="fbAuthToken">The validated token from the Facebook SDK (that will be further validated when sent to the bC service)</param>
        /// <param name="forceCreate">Should a new profile be created for this user if the account does not exist?</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void AuthenticateFacebook(
            string externalId,
            string authenticationToken,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(externalId, authenticationToken, Common.AuthenticationType.Facebook,
                              null, forceCreate, null, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user with brainCloud using their Facebook Limited Credentials
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        /// </remarks>
        /// <param name="externalId">
        /// The facebook Limited id of the user
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
        public void AuthenticateFacebookLimited(
            string externalId,
            string authenticationToken,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(externalId, authenticationToken, Common.AuthenticationType.FacebookLimited,
                              null, forceCreate, null, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user with brainCloud using their Oculus Credentials
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - AUTHENTICATE
        /// </remarks>
        /// <param name="oculusUserId">The oculus id of the user</param>
        /// <param name="oculusNonce">Oculus token from the Oculus SDK</param>
        /// <param name="forceCreate">Should a new profile be created for this user if the account does not exist?</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void AuthenticateOculus(
            string oculusId,
            string oculusNonce,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(oculusId, oculusNonce, Common.AuthenticationType.Oculus,
                              null, forceCreate, null, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user using their psn account id and an auth token
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        /// </remarks>
        /// <param name="accountId">
        /// The user's PSN account id
        /// </param>
        /// <param name="authToken">
        /// The user's PSN auth token
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
        public void AuthenticatePlaystationNetwork(
            string accountId,
            string authToken,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(accountId, authToken, Common.AuthenticationType.PlaystationNetwork,
                              null, forceCreate, null, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user using their psn account id and an auth token
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        /// </remarks>
        /// <param name="accountId">
        /// The user's PSN account id
        /// </param>
        /// <param name="authToken">
        /// The user's PSN auth token
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
        public void AuthenticatePlaystation5(
            string accountId,
            string authToken,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(accountId, authToken, Common.AuthenticationType.PlaystationNetwork5,
                null, forceCreate, null, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user using their Game Center id
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - AUTHENTICATE
        /// </remarks>
        /// <param name="gameCenterId">The player's game center id  (use the playerID property from the local GKPlayer object)</param>
        /// <param name="forceCreate">Should a new profile be created for this user if the account does not exist?</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void AuthenticateGameCenter(
            string gameCenterId,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(gameCenterId, "", Common.AuthenticationType.GameCenter,
                              null, forceCreate, null, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user using a steam userid and session ticket (without any validation on the userid).
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - AUTHENTICATE
        /// </remarks>
        /// <param name="userId">String representation of 64 bit steam id</param>
        /// <param name="sessionticket">The session ticket of the user (hex encoded)</param>
        /// <param name="forceCreate">Should a new profile be created for this user if the account does not exist?</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void AuthenticateSteam(
            string userId,
            string sessionticket,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(userId, sessionticket, Common.AuthenticationType.Steam,
                              null, forceCreate, null, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user using a google userid(email address) and google authentication token.
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - AUTHENTICATE
        /// </remarks>
        /// <param name="appleUserId">String of the apple accounts user Id OR email</param>
        /// <param name="identityToken">The authentication token confirming users identity</param>
        /// <param name="forceCreate">Should a new profile be created for this user if the account does not exist?</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void AuthenticateApple(
            string appleUserId,
            string identityToken,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(appleUserId, identityToken, Common.AuthenticationType.Apple,
                null, forceCreate, null, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user using a google userid(email address) and google authentication token.
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - AUTHENTICATE
        /// </remarks>
        /// <param name="googleUserId">String representation of google+ userid (email)</param>
        /// <param name="serverAuthCode">The authentication token derived via the google apis.</param>
        /// <param name="forceCreate">Should a new profile be created for this user if the account does not exist?</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void AuthenticateGoogle(
            string googleUserId,
            string serverAuthCode,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(googleUserId, serverAuthCode, Common.AuthenticationType.Google,
                null, forceCreate, null, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user using a google userid(email address) and google authentication token.
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - AUTHENTICATE
        /// </remarks>
        /// <param name="googleUserAccountEmail">String representation of google+ userid (email)</param>
        /// <param name="IdToken">The authentication token derived via the google apis.</param>
        /// <param name="forceCreate">Should a new profile be created for this user if the account does not exist?</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void AuthenticateGoogleOpenId(
            string googleUserAccountEmail,
            string IdToken,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(googleUserAccountEmail, IdToken, Common.AuthenticationType.GoogleOpenId,
                null, forceCreate, null, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user using a Twitter userid, authentication token, and secret from Twitter.
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - AUTHENTICATE
        /// </remarks>
        /// <param name="userId">String representation of Twitter userid</param>
        /// <param name="token">The authentication token derived via the Twitter apis.</param>
        /// <param name="secret">The secret given when attempting to link with Twitter</param>
        /// <param name="forceCreate">Should a new profile be created for this user if the account does not exist?</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void AuthenticateTwitter(
            string userId,
            string token,
            string secret,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(userId, token + ":" + secret, Common.AuthenticationType.Twitter,
                null, forceCreate, null, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user using a Parse userid and authentication token
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - AUTHENTICATE
        /// </remarks>
        /// <param name="userId">String representation of Parse userid</param>
        /// <param name="token">The authentication token</param>
        /// <param name="forceCreate">Should a new profile be created for this user if the account does not exist?</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void AuthenticateParse(
            string userId,
            string token,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(userId, token, Common.AuthenticationType.Parse,
                null, forceCreate, null, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user using a handoffCode
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - AUTHENTICATE
        /// </remarks>
        /// <param name="handoffCode">the code we generate in cloudcode</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void AuthenticateSettopHandoff(
            string handoffCode,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(handoffCode, "", Common.AuthenticationType.SettopHandoff,
                null, false, null, success, failure, cbObject);
        }
        /// <summary>
        /// Authenticate the user using a handoffId and authentication token
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - AUTHENTICATE
        /// </remarks>
        /// <param name="handoffId">braincloud handoff id generated from cloud script</param>
        /// <param name="securityToken">The authentication token</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void AuthenticateHandoff(
            string handoffId,
            string securityToken,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(handoffId, securityToken, Common.AuthenticationType.Handoff,
                null, false, null, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user via cloud code (which in turn validates the supplied credentials against an external system).
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - AUTHENTICATE
        /// </remarks>
        /// <param name="userId">The user id</param>
        /// <param name="token">The user token (password etc)</param>
        /// <param name="externalAuthName">The name of the cloud script to call for external authentication</param>
        /// <param name="forceCreate">Should a new profile be created for this user if the account does not exist?</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void AuthenticateExternal(
            string userId,
            string token,
            string externalAuthName,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(userId, token, Common.AuthenticationType.External,
                externalAuthName, forceCreate, null, success, failure, cbObject);
        }

        /// <summary>
        /// A generic Authenticate method that translates to the same as calling a specific one, except it takes an extraJson
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - AUTHENTICATE
        /// </remarks>
        /// <param name="authenticationType">Universal, Email, Facebook, etc</param>
        /// <param name="ids">Auth IDs structure</param>
        /// <param name="forceCreate">Should a new profile be created for this user if the account does not exist?</param>
        /// <param name="extraJson">Additional to piggyback along with the call, to be picked up by pre- or post- hooks. Leave empty string for no extraJson.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void AuthenticateAdvanced(
            AuthenticationType authenticationType,
            AuthenticationIds ids,
            bool forceCreate,
            Dictionary<string, object> extraJson,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(ids.externalId, ids.authenticationToken, authenticationType,
                ids.authenticationSubType, forceCreate, extraJson, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user for Ultra.
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - AUTHENTICATE
        /// </remarks>
        /// <param name="ultraUsername">it's what the user uses to log into the Ultra endpoint initially</param>
        /// <param name="ultraIdToken">The "id_token" taken from Ultra's JWT.</param>
        /// <param name="forceCreate">Should a new profile be created for this user if the account does not exist?</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void AuthenticateUltra(
            string ultraUsername,
            string ultraIdToken,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(ultraUsername, ultraIdToken, Common.AuthenticationType.Ultra,
                null, forceCreate, null, success, failure, cbObject);
        }

        /// <summary>
        /// Authenticate the user using their Nintendo account id and an auth token
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        /// </remarks>
        /// <param name="accountId">
        /// The user's Nintendo account id
        /// </param>
        /// <param name="authToken">
        /// The user's Nintendo auth token
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
        public void AuthenticateNintendo(
            string accountId,
            string authToken,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Authenticate(accountId, authToken, Common.AuthenticationType.Nintendo,
                null, forceCreate, null, success, failure, cbObject);
        }

        /// <summary>
        /// Reset Email password - Sends a password reset email to the specified address
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - RESET_EMAIL_PASSWORD
        /// </remarks>
        /// <param name="externalId">The email address to send the reset email to.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

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

        /// <summary>
        /// Reset Email password - Sends a password reset email to the specified address
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - RESET_EMAIL_PASSWORD_WITH_EXPIRY
        /// </remarks>
        /// <param name="externalId">The email address to send the reset email to.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void ResetEmailPasswordWithExpiry(
            string externalId,
            int tokenTtlInMinutes,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.AuthenticateServiceAuthenticateExternalId.Value] = externalId;
            data[OperationParam.AuthenticateServiceAuthenticateGameId.Value] = _client.AppId;

            data[OperationParam.AuthenticateServiceAuthenticateTokenTtlInMinutes.Value] = tokenTtlInMinutes;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure);
            ServerCall sc = new ServerCall(ServiceName.Authenticate, ServiceOperation.ResetEmailPasswordWithExpiry, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Reset Email password with service parameters - Sends a password reset email to
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - RESET_EMAIL_PASSWORD_ADVANCED
        /// </remarks>
        /// <param name="appId">the applicationId</param>
        /// <param name="emailAddress">The email address to send the reset email to.</param>
        /// <param name="serviceParams">- parameters to send to the email service. See documentation for full list. http://getbraincloud.com/apidocs/apiref/#capi-mail</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void ResetEmailPasswordAdvanced(
            string emailAddress,
            //Dictionary<string, object> serviceParams,
            string serviceParams,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.AuthenticateServiceAuthenticateGameId.Value] = _client.AppId;
            data[OperationParam.AuthenticateServiceAuthenticateEmailAddress.Value] = emailAddress;

            var jsonParams = JsonReader.Deserialize<Dictionary<string, object>>(serviceParams);
            data[OperationParam.AuthenticateServiceAuthenticateServiceParams.Value] = jsonParams;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure);
            ServerCall sc = new ServerCall(ServiceName.Authenticate, ServiceOperation.ResetEmailPasswordAdvanced, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Reset Email password with service parameters - Sends a password reset email to
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - RESET_EMAIL_PASSWORD_ADVANCED
        /// </remarks>
        /// <param name="appId">the applicationId</param>
        /// <param name="emailAddress">The email address to send the reset email to.</param>
        /// <param name="serviceParams">- parameters to send to the email service. See documentation for full list. http://getbraincloud.com/apidocs/apiref/#capi-mail</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void ResetEmailPasswordAdvancedWithExpiry(
            string emailAddress,
            //Dictionary<string, object> serviceParams,
            string serviceParams,
            int tokenTtlInMinutes,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.AuthenticateServiceAuthenticateGameId.Value] = _client.AppId;
            data[OperationParam.AuthenticateServiceAuthenticateEmailAddress.Value] = emailAddress;

            var jsonParams = JsonReader.Deserialize<Dictionary<string, object>>(serviceParams);
            data[OperationParam.AuthenticateServiceAuthenticateServiceParams.Value] = jsonParams;

            data[OperationParam.AuthenticateServiceAuthenticateTokenTtlInMinutes.Value] = tokenTtlInMinutes;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure);
            ServerCall sc = new ServerCall(ServiceName.Authenticate, ServiceOperation.ResetEmailPasswordAdvancedWithExpiry, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Resets Universal ID password
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - RESET_UNIVERSAL_ID_PASSWORD
        /// </remarks>
        /// <param name="appId">the applicationId</param>
        /// <param name="universalId">the universal Id in question</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void ResetUniversalIdPassword(
            string universalId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.AuthenticateServiceAuthenticateGameId.Value] = _client.AppId;
            data[OperationParam.AuthenticateServiceAuthenticateUniversalId.Value] = universalId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure);
            ServerCall sc = new ServerCall(ServiceName.Authenticate, ServiceOperation.ResetUniversalIdPassword, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Resets Universal ID password
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - RESET_UNIVERSAL_ID_PASSWORD_WITH_EXPIRY
        /// </remarks>
        /// <param name="appId">the applicationId</param>
        /// <param name="universalId">the universal Id in question</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void ResetUniversalIdPasswordWithExpiry(
            string universalId,
            int tokenTtlInMinutes,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.AuthenticateServiceAuthenticateGameId.Value] = _client.AppId;
            data[OperationParam.AuthenticateServiceAuthenticateUniversalId.Value] = universalId;
            data[OperationParam.AuthenticateServiceAuthenticateTokenTtlInMinutes.Value] = tokenTtlInMinutes;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure);
            ServerCall sc = new ServerCall(ServiceName.Authenticate, ServiceOperation.ResetUniversalIdPasswordWithExpiry, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Advanced Universal ID password reset using templates
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - RESET_UNIVERSAL_ID_PASSWORD_ADVANCED
        /// </remarks>
        /// <param name="appId">the applicationId</param>
        /// <param name="universalId">the universal Id in question</param>
        /// <param name="serviceParams">- parameters to send to the email service.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void ResetUniversalIdPasswordAdvanced(
            string universalId,
            string serviceParams,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.AuthenticateServiceAuthenticateGameId.Value] = _client.AppId;
            data[OperationParam.AuthenticateServiceAuthenticateUniversalId.Value] = universalId;

            var jsonParams = JsonReader.Deserialize<Dictionary<string, object>>(serviceParams);
            data[OperationParam.AuthenticateServiceAuthenticateServiceParams.Value] = jsonParams;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure);
            ServerCall sc = new ServerCall(ServiceName.Authenticate, ServiceOperation.ResetUniversalIdPasswordAdvanced, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Advanced Universal ID password reset using templates
        /// </summary>
        /// <remarks>
        /// Service Name - authenticationV2
        /// Service Operation - RESET_UNIVERSAL_ID_PASSWORD_ADVANCED_WITH_EXPIRY
        /// </remarks>
        /// <param name="appId">the applicationId</param>
        /// <param name="universalId">the universal Id in question</param>
        /// <param name="serviceParams">- parameters to send to the email service.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void ResetUniversalIdPasswordAdvancedWithExpiry(
            string universalId,
            string serviceParams,
            int tokenTtlInMinutes,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.AuthenticateServiceAuthenticateGameId.Value] = _client.AppId;
            data[OperationParam.AuthenticateServiceAuthenticateUniversalId.Value] = universalId;

            var jsonParams = JsonReader.Deserialize<Dictionary<string, object>>(serviceParams);
            data[OperationParam.AuthenticateServiceAuthenticateServiceParams.Value] = jsonParams;

            data[OperationParam.AuthenticateServiceAuthenticateTokenTtlInMinutes.Value] = tokenTtlInMinutes;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure);
            ServerCall sc = new ServerCall(ServiceName.Authenticate, ServiceOperation.ResetUniversalIdPasswordAdvancedWithExpiry, data, callback);
            _client.SendRequest(sc);
        }

        private void Authenticate(
            string externalId,
            string authenticationToken,
            AuthenticationType authenticationType,
            string externalAuthName,
            bool forceCreate,
            Dictionary<string, object> extraJson,
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
            data[OperationParam.AuthenticateServiceAuthenticateCompressResponses.Value] = CompressResponses;

            data[OperationParam.AuthenticateServiceAuthenticateProfileId.Value] = ProfileId;
            data[OperationParam.AuthenticateServiceAuthenticateAnonymousId.Value] = AnonymousId;
            data[OperationParam.AuthenticateServiceAuthenticateGameId.Value] = _client.AppId;
            data[OperationParam.AuthenticateServiceAuthenticateReleasePlatform.Value] = _client.ReleasePlatform.ToString();
            data[OperationParam.AuthenticateServiceAuthenticateGameVersion.Value] = _client.AppVersion;
            data[OperationParam.AuthenticateServiceAuthenticateBrainCloudVersion.Value] = Version.GetVersion();

#if DOT_NET
            data["clientLib"] = "csharp";
#elif GODOT
            data["clientLib"] = "csharp-godot";
#else
            data["clientLib"] = "csharp-unity";
#endif

            if (Util.IsOptionalParameterValid(externalAuthName))
            {
                data[OperationParam.AuthenticateServiceAuthenticateExternalAuthName.Value] = externalAuthName;
            }

            if (extraJson != null)
            {
                data[OperationParam.AuthenticateServiceAuthenticateExtraJson.Value] = extraJson;
            }

            data[OperationParam.AuthenticateServiceAuthenticateCountryCode.Value] = countryCode;
            data[OperationParam.AuthenticateServiceAuthenticateLanguageCode.Value] = languageCode;
            data[OperationParam.AuthenticateServiceAuthenticateTimeZoneOffset.Value] = utcOffset;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Authenticate, ServiceOperation.Authenticate, data, callback);
            if (_client.Comms.IsAuthenticateRequestInProgress())
            {
                _client.Comms.AddCallbackToAuthenticateRequest(callback);
                return;
            }
            _client.SendRequest(sc);
        }
    }
}
