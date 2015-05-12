//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using JsonFx.Json;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudIdentity
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudIdentity(BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        /// <summary>
        /// Attach the user's Facebook credentials to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="externalId">
        /// The facebook id of the user
        /// </param>
        /// <param name="authenticationToken">
        /// The validated token from the Facebook SDK
        ///   (that will be further validated when sent to the bC service)
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Errors to watch for:  SWITCHING_PROFILES - this means that the Facebook identity you provided
        /// already points to a different profile.  You will likely want to offer the player the
        /// choice to *SWITCH* to that profile, or *MERGE* the profiles.
        ///
        /// To switch profiles, call ClearSavedProfileID() and call AuthenticateFacebook().
        /// </returns>
        public void AttachFacebookIdentity(string in_externalId, string in_authenticationToken, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.AttachIdentity(in_externalId, in_authenticationToken, OperationParam.AuthenticateServiceAuthenticateAuthFacebook.Value, in_success, in_failure);
        }

        /// <summary>
        /// Merge the profile associated with the provided Facebook credentials with the
        /// current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="externalId">
        /// The facebook id of the user
        /// </param>
        /// <param name="authenticationToken">
        /// The validated token from the Facebook SDK
        /// (that will be further validated when sent to the bC service)
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void MergeFacebookIdentity(string in_externalId, string in_authenticationToken, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.MergeIdentity(in_externalId, in_authenticationToken, OperationParam.AuthenticateServiceAuthenticateAuthFacebook.Value, in_success, in_failure);
        }

        /// <summary>
        /// Detach the Facebook identity from this profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="externalId">
        /// The facebook id of the user
        /// </param>
        /// <param name="in_continueAnon">
        /// Proceed even if the profile will revert to anonymous?
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Watch for DOWNGRADING_TO_ANONYMOUS_ERROR - occurs if you set in_continueAnon to false, and
        /// disconnecting this identity would result in the profile being anonymous (which means that
        /// the profile wouldn't be retrievable if the user loses their device)
        /// </returns>
        public void DetachFacebookIdentity(string in_externalId, bool in_continueAnon, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.DetachIdentity(in_externalId, OperationParam.AuthenticateServiceAuthenticateAuthFacebook.Value, in_continueAnon, in_success, in_failure);
        }

        /// <summary>
        /// Attach a Game Center identity to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="in_gameCenterId">
        /// The player's game center id  (use the playerID property from the local GKPlayer object)
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Errors to watch for:  SWITCHING_PROFILES - this means that the Facebook identity you provided
        /// already points to a different profile.  You will likely want to offer the player the
        /// choice to *SWITCH* to that profile, or *MERGE* the profiles.
        ///
        /// To switch profiles, call ClearSavedProfileID() and call this method again.
        /// </returns>
        public void AttachGameCenterIdentity(string in_gameCenterId, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.AttachIdentity(in_gameCenterId, "", OperationParam.AuthenticateServiceAuthenticateAuthGameCenter.Value, in_success, in_failure);
        }

        /// <summary>Merge the profile associated with the specified Game Center identity with the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="in_gameCenterId">
        /// The player's game center id  (use the playerID property from the local GKPlayer object)
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void MergeGameCenterIdentity(string in_gameCenterId, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.MergeIdentity(in_gameCenterId, "", OperationParam.AuthenticateServiceAuthenticateAuthGameCenter.Value, in_success, in_failure);
        }

        /// <summary>Detach the Game Center identity from the current profile.</summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="in_gameCenterId">
        /// The player's game center id  (use the playerID property from the local GKPlayer object)
        /// </param>
        /// <param name="in_continueAnon">
        /// Proceed even if the profile will revert to anonymous?
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Watch for DOWNGRADING_TO_ANONYMOUS_ERROR - occurs if you set in_continueAnon to false, and
        /// disconnecting this identity would result in the profile being anonymous (which means that
        /// the profile wouldn't be retrievable if the user loses their device)
        /// </returns>
        public void DetachGameCenterIdentity(string in_gameCenterId, bool in_continueAnon, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.DetachIdentity(in_gameCenterId, OperationParam.AuthenticateServiceAuthenticateAuthGameCenter.Value, in_continueAnon, in_success, in_failure);
        }

        /// <summary>
        /// Attach a Email and Password identity to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="in_email">
        /// The player's e-mail address
        /// </param>
        /// <param name="in_password">
        /// The player's password
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Errors to watch for:  SWITCHING_PROFILES - this means that the email address you provided
        /// already points to a different profile.  You will likely want to offer the player the
        /// choice to *SWITCH* to that profile, or *MERGE* the profiles.
        ///
        /// To switch profiles, call ClearSavedProfileID() and then call AuthenticateEmailPassword().
        /// </returns>
        public void AttachEmailIdentity(string in_email, string in_password, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.AttachIdentity(in_email, in_password, OperationParam.AuthenticateServiceAuthenticateAuthEmail.Value, in_success, in_failure);
        }

        /// <summary>
        // Merge the profile associated with the provided e=mail with the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="in_email">
        /// The player's e-mail address
        /// </param>
        /// <param name="in_password">
        /// The player's password
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void MergeEmailIdentity(string in_email, string in_password, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.MergeIdentity(in_email, in_password, OperationParam.AuthenticateServiceAuthenticateAuthEmail.Value, in_success, in_failure);
        }

        /// <summary>Detach the e-mail identity from the current profile
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="in_email">
        /// The player's e-mail address
        /// </param>
        /// <param name="in_continueAnon">
        /// Proceed even if the profile will revert to anonymous?
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Watch for DOWNGRADING_TO_ANONYMOUS_ERROR - occurs if you set in_continueAnon to false, and
        /// disconnecting this identity would result in the profile being anonymous (which means that
        /// the profile wouldn't be retrievable if the user loses their device)
        /// </returns>
        public void DetachEmailIdentity(string in_email, bool in_continueAnon, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.DetachIdentity(in_email, OperationParam.AuthenticateServiceAuthenticateAuthEmail.Value, in_continueAnon, in_success, in_failure);
        }

        /// <summary>
        /// Attach a Universal (userid + password) identity to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="in_userid">
        /// The player's userid
        /// </param>
        /// <param name="in_password">
        /// The player's password
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Errors to watch for:  SWITCHING_PROFILES - this means that the email address you provided
        /// already points to a different profile.  You will likely want to offer the player the
        /// choice to *SWITCH* to that profile, or *MERGE* the profiles.
        ///
        /// To switch profiles, call ClearSavedProfileID() and then call AuthenticateEmailPassword().
        /// </returns>
        public void AttachUniversalIdentity(string in_userid, string in_password, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.AttachIdentity(in_userid, in_password, OperationParam.AuthenticateServiceAuthenticateAuthUniversal.Value, in_success, in_failure);
        }

        /// <summary>
        /// Merge the profile associated with the provided e=mail with the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="in_userid">
        /// The player's userid
        /// </param>
        /// <param name="in_password">
        /// The player's password
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void MergeUniversalIdentity(string in_userid, string in_password, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.MergeIdentity(in_userid, in_password, OperationParam.AuthenticateServiceAuthenticateAuthUniversal.Value, in_success, in_failure);
        }

        /// <summary>Detach the universal identity from the current profile
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="in_userid">
        /// The player's userid
        /// </param>
        /// <param name="in_continueAnon">
        /// Proceed even if the profile will revert to anonymous?
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Watch for DOWNGRADING_TO_ANONYMOUS_ERROR - occurs if you set in_continueAnon to false, and
        /// disconnecting this identity would result in the profile being anonymous (which means that
        /// the profile wouldn't be retrievable if the user loses their device)
        /// </returns>
        public void DetachUniversalIdentity(string in_userid, bool in_continueAnon, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.DetachIdentity(in_userid, OperationParam.AuthenticateServiceAuthenticateAuthUniversal.Value, in_continueAnon, in_success, in_failure);
        }

        /// <summary>
        /// Attach a Steam (userid + steamsessionticket) identity to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="in_userid">
        /// String representation of 64 bit steam id
        /// </param>
        /// <param name="in_sessionticket">
        /// The player's session ticket (hex encoded)
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Errors to watch for:  SWITCHING_PROFILES - this means that the email address you provided
        /// already points to a different profile.  You will likely want to offer the player the
        /// choice to *SWITCH* to that profile, or *MERGE* the profiles.
        ///
        /// To switch profiles, call ClearSavedProfileID() and then call AuthenticateSteam().
        /// </returns>
        public void AttachSteamIdentity(string in_userid, string in_sessionticket, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.AttachIdentity(in_userid, in_sessionticket, OperationParam.AuthenticateServiceAuthenticateAuthSteam.Value, in_success, in_failure);
        }

        /// <summary>
        /// Merge the profile associated with the provided steam userid with the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="in_userid">
        /// String representation of 64 bit steam id
        /// </param>
        /// <param name="in_sessionticket">
        /// The player's session ticket (hex encoded)
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void MergeSteamIdentity(string in_userid, string in_sessionticket, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.MergeIdentity(in_userid, in_sessionticket, OperationParam.AuthenticateServiceAuthenticateAuthSteam.Value, in_success, in_failure);
        }

        /// <summary>Detach the steam identity from the current profile
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="in_userid">
        /// String representation of 64 bit steam id
        /// </param>
        /// <param name="in_continueAnon">
        /// Proceed even if the profile will revert to anonymous?
        /// </param>
        /// <param name="in_success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="in_failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Watch for DOWNGRADING_TO_ANONYMOUS_ERROR - occurs if you set in_continueAnon to false, and
        /// disconnecting this identity would result in the profile being anonymous (which means that
        /// the profile wouldn't be retrievable if the user loses their device)
        /// </returns>
        public void DetachSteamIdentity(string in_userid, bool in_continueAnon, SuccessCallback in_success, FailureCallback in_failure)
        {
            this.DetachIdentity(in_userid, OperationParam.AuthenticateServiceAuthenticateAuthSteam.Value, in_continueAnon, in_success, in_failure);
        }


        private void AttachIdentity(String in_externalId, string in_authenticationToken, String in_authenticationType, SuccessCallback in_success, FailureCallback in_failure)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.IdentityServiceExternalId.Value] = in_externalId;
            data[OperationParam.IdentityServiceAuthenticationType.Value] = in_authenticationType;
            data[OperationParam.AuthenticateServiceAuthenticateAuthenticationToken.Value] = in_authenticationToken;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.Attach, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        private void MergeIdentity(String in_externalId, string in_authenticationToken, String in_authenticationType, SuccessCallback in_success, FailureCallback in_failure)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.IdentityServiceExternalId.Value] = in_externalId;
            data[OperationParam.IdentityServiceAuthenticationType.Value] = in_authenticationType;
            data[OperationParam.AuthenticateServiceAuthenticateAuthenticationToken.Value] = in_authenticationToken;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.Merge, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        private void DetachIdentity(string in_externalId, string in_authenticationType, bool in_continueAnon, SuccessCallback in_success, FailureCallback in_failure)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.IdentityServiceExternalId.Value] = in_externalId;
            data[OperationParam.IdentityServiceAuthenticationType.Value] = in_authenticationType;
            data[OperationParam.IdentityServiceConfirmAnonymous.Value] = in_continueAnon;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.Detach, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}
