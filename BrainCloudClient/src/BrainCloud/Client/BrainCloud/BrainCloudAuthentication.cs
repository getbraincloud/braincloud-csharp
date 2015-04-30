//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using LitJson;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudAuthentication
    {
        private BrainCloudClient m_brainCloudClientRef;

        private string m_anonymousId = "";
        private string m_profileId = "";

        public string AnonymousId
        {
            get
            {
                return m_anonymousId;
            }
            set
            {
                m_anonymousId = value;
            }
        }
        public string ProfileId
        {
            get
            {
                return m_profileId;
            }
            set
            {
                m_profileId = value;
            }
        }

        public BrainCloudAuthentication(BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        /// <summary>
        /// Generates a GUID for use as an anonymous installation id for brainCloud.  This method is provided as a convenience to the
        /// client application - but clients can override this id with a scheme of their own if they'd like (as long as the scheme in place
        /// generates unique ids per client device).
        ///<c/summary>
        /// </param>
        /// <returns> the id
        /// </returns>
        public string GenerateGUID()
        {
            Guid newID = Guid.NewGuid();

            // ensure that we do not create an empty GUID
            while (newID == Guid.Empty)
            {
                newID = Guid.NewGuid();
            }

            return newID.ToString();
        }

        /// <summary>
        /// Initialize - initializes the identity service with the saved
        /// anonymous installation id and most recently used profile id
        /// </summary>
        /// </param>
        /// <param name="in_profileId">
        /// The id of the profile id that was most recently used by the app (on this device)
        /// </param>
        /// <param name="in_anonymousId">
        /// The anonymous installation id that was generated for this device
        /// </param>
        public void Initialize(string in_profileId, string in_anonymousId)
        {
            m_anonymousId = in_anonymousId;
            m_profileId = in_profileId;
        }

        /// <summary>
        /// Used to create the anonymous installation id for the brainCloud profile.
        /// Normally only called once when the application starts for the first time.
        ///</summary>
        public void GenerateNewAnonymousID()
        {
            m_anonymousId = GenerateGUID();
        }


        /// <summary>
        /// Used to clear the saved profile id - to use in cases when the user is
        /// attempting to switch to a different game profile.
        /// </summary>
        public void ClearSavedProfileID()
        {
            m_profileId = null;  // Not sure if this is correct...
        }

        /// <summary>
        /// Authenticate a user anonymously with brainCloud - used for apps that don't want to bother
        /// the user to login, or for users who are sensitive to their privacy
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        /// </remarks>
        /// <param name="in_forceCreate">
        /// Should a new profile be created if it does not exist?
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void AuthenticateAnonymous(bool in_forceCreate, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.Authenticate(m_anonymousId, "", OperationParam.AuthenticateServiceAuthenticateAuthAnonymous.Value, in_forceCreate, in_success, in_failure);
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
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void AuthenticateFacebook(string in_externalId, string in_authenticationToken, bool in_forceCreate, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.Authenticate(in_externalId, in_authenticationToken, OperationParam.AuthenticateServiceAuthenticateAuthFacebook.Value, in_forceCreate, in_success, in_failure);
        }

        /// <summary>
        /// Authenticate the user using their Game Center id
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        /// </remarks>
        /// <param name="in_gameCenterId">
        /// The player's game center id  (use the playerID property from the local GKPlayer object)
        /// </param>
        /// <param name="forceCreate">
        /// Should a new profile be created for this user if the account does not exist?
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void AuthenticateGameCenter(string in_gameCenterId, bool in_forceCreate, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.Authenticate(in_gameCenterId, "", OperationParam.AuthenticateServiceAuthenticateAuthGameCenter.Value, in_forceCreate, in_success, in_failure);
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
        /// <param name="in_email">
        /// The e-mail address of the user
        /// </param>
        /// <param name="in_password">
        /// The password of the user
        /// </param>
        /// <param name="forceCreate">
        /// Should a new profile be created for this user if the account does not exist?
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void AuthenticateEmailPassword(string in_email, string in_password, bool in_forceCreate, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.Authenticate(in_email, in_password, OperationParam.AuthenticateServiceAuthenticateAuthEmail.Value, in_forceCreate, in_success, in_failure);
        }

        /// <summary>
        /// Authenticate the user using a userid and password (without any validation on the userid).
        /// Similar to AuthenticateEmailPassword - except that that method has additional features to
        /// allow for e-mail validation, password resets, etc.
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        /// </remarks>
        /// <param name="in_email">
        /// The e-mail address of the user
        /// </param>
        /// <param name="in_password">
        /// The password of the user
        /// </param>
        /// <param name="forceCreate">
        /// Should a new profile be created for this user if the account does not exist?
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void AuthenticateUniversal(string in_userid, string in_password, bool in_forceCreate, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.Authenticate(in_userid, in_password, OperationParam.AuthenticateServiceAuthenticateAuthUniversal.Value, in_forceCreate, in_success, in_failure);
        }

        /// <summary>
        /// Authenticate the user using a steam userid and session ticket (without any validation on the userid).
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        /// </remarks>
        /// <param name="in_userid">
        /// String representation of 64 bit steam id
        /// </param>
        /// <param name="in_sessionticket">
        /// The session ticket of the user (hex encoded)
        /// </param>
        /// <param name="forceCreate">
        /// Should a new profile be created for this user if the account does not exist?
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void AuthenticateSteam(string in_userid, string in_sessionticket, bool forceCreate, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.Authenticate(in_userid, in_sessionticket, OperationParam.AuthenticateServiceAuthenticateAuthSteam.Value, forceCreate, in_success, in_failure);
        }

        /// <summary>
        /// Authenticate the user using a google userid(email address) and google authentication token.
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Service Operation - Authenticate
        /// </remarks>
        /// <param name="in_userid">
        /// String representation of google+ userid (email)
        /// </param>
        /// <param name="in_token">
        /// The authentication token derived via the google apis.
        /// </param>
        /// <param name="forceCreate">
        /// Should a new profile be created for this user if the account does not exist?
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void AuthenticateGoogle(string in_userid, string in_token, bool forceCreate, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.Authenticate(in_userid, in_token, OperationParam.AuthenticateServiceAuthenticateAuthGoogle.Value, forceCreate, in_success, in_failure);
        }

        /// <summary>
        /// Reset Email password - Sends a password reset email to the specified address
        /// </summary>
        /// <remarks>
        /// Service Name - Authenticate
        /// Operation - ResetEmailPassword
        /// </remarks>
        /// <param name="in_externalId">
        /// The email address to send the reset email to.
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of success
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error
        /// </param>
        public void ResetEmailPassword(string in_externalId, SuccessCallback in_success, FailureCallback in_failure)
        {
            JsonData data = new JsonData();
            data[OperationParam.AuthenticateServiceAuthenticateExternalId.Value] = in_externalId;
            data[OperationParam.AuthenticateServiceAuthenticateGameId.Value] = m_brainCloudClientRef.GameId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure);
            ServerCall sc = new ServerCall(ServiceName.Authenticate, ServiceOperation.ResetEmailPassword, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }


        private void Authenticate(string in_externalId, string in_authenticationToken, string in_authenticationType, bool in_forceCreate, SuccessCallback in_success, FailureCallback in_failure)
        {
            string languageCode = Util.GetIsoCodeForCurrentLanguage();
            double utcOffset = Util.GetUTCOffsetForCurrentTimeZone();
            string countryCode = Util.GetCurrentCountryCode();

            JsonData data = new JsonData();
            data[OperationParam.AuthenticateServiceAuthenticateExternalId.Value] = in_externalId;
            data[OperationParam.AuthenticateServiceAuthenticateAuthenticationToken.Value] = in_authenticationToken;
            data[OperationParam.AuthenticateServiceAuthenticateAuthenticationType.Value] = in_authenticationType;
            data[OperationParam.AuthenticateServiceAuthenticateForceCreate.Value] = in_forceCreate;

            data[OperationParam.AuthenticateServiceAuthenticateProfileId.Value] = m_profileId;
            data[OperationParam.AuthenticateServiceAuthenticateAnonymousId.Value] = m_anonymousId;
            data[OperationParam.AuthenticateServiceAuthenticateGameId.Value] = m_brainCloudClientRef.GameId;
            data[OperationParam.AuthenticateServiceAuthenticateReleasePlatform.Value] = m_brainCloudClientRef.ReleasePlatform;
            data[OperationParam.AuthenticateServiceAuthenticateGameVersion.Value] = m_brainCloudClientRef.GameVersion;
            data[OperationParam.AuthenticateServiceAuthenticateBrainCloudVersion.Value] = Version.GetVersion();

            data[OperationParam.AuthenticateServiceAuthenticateCountryCode.Value] = countryCode;
            data[OperationParam.AuthenticateServiceAuthenticateLanguageCode.Value] = languageCode;
            data[OperationParam.AuthenticateServiceAuthenticateTimeZoneOffset.Value] = utcOffset;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure);
            ServerCall sc = new ServerCall(ServiceName.Authenticate, ServiceOperation.Authenticate, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}
