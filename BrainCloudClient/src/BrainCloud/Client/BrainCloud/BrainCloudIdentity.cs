//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudIdentity
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudIdentity(BrainCloudClient brainCloudClientRef)
        {
            m_brainCloudClientRef = brainCloudClientRef;
        }

        /// <summary>
        /// Attach the user's Facebook credentials to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="facebookId">
        /// The facebook id of the user
        /// </param>
        /// <param name="authenticationToken">
        /// The validated token from the Facebook SDK
        ///   (that will be further validated when sent to the bC service)
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Errors to watch for:  SWITCHING_PROFILES - this means that the Facebook identity you provided
        /// already points to a different profile.  You will likely want to offer the player the
        /// choice to *SWITCH* to that profile, or *MERGE* the profiles.
        ///
        /// To switch profiles, call ClearSavedProfileID() and call AuthenticateFacebook().
        /// </returns>
        public void AttachFacebookIdentity(
            string facebookId,
            string authenticationToken,
            SuccessCallback success,
            FailureCallback failure)
        {
            AttachIdentity(facebookId, authenticationToken, OperationParam.AuthenticateServiceAuthenticateAuthFacebook.Value, success, failure);
        }

        /// <summary>
        /// Merge the profile associated with the provided Facebook credentials with the
        /// current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="facebookId">
        /// The facebook id of the user
        /// </param>
        /// <param name="authenticationToken">
        /// The validated token from the Facebook SDK
        /// (that will be further validated when sent to the bC service)
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void MergeFacebookIdentity(
            string facebookId,
            string authenticationToken,
            SuccessCallback success,
            FailureCallback failure)
        {
            MergeIdentity(facebookId, authenticationToken, OperationParam.AuthenticateServiceAuthenticateAuthFacebook.Value, success, failure);
        }

        /// <summary>
        /// Detach the Facebook identity from this profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="facebookId">
        /// The facebook id of the user
        /// </param>
        /// <param name="continueAnon">
        /// Proceed even if the profile will revert to anonymous?
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Watch for DOWNGRADING_TO_ANONYMOUS_ERROR - occurs if you set continueAnon to false, and
        /// disconnecting this identity would result in the profile being anonymous (which means that
        /// the profile wouldn't be retrievable if the user loses their device)
        /// </returns>
        public void DetachFacebookIdentity(
            string facebookId,
            bool continueAnon,
            SuccessCallback success,
            FailureCallback failure)
        {
            DetachIdentity(facebookId, OperationParam.AuthenticateServiceAuthenticateAuthFacebook.Value, continueAnon, success, failure);
        }

        /// <summary>
        /// Attach a Game Center identity to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="gameCenterId">
        /// The player's game center id  (use the playerID property from the local GKPlayer object)
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Errors to watch for:  SWITCHING_PROFILES - this means that the Facebook identity you provided
        /// already points to a different profile.  You will likely want to offer the player the
        /// choice to *SWITCH* to that profile, or *MERGE* the profiles.
        ///
        /// To switch profiles, call ClearSavedProfileID() and call this method again.
        /// </returns>
        public void AttachGameCenterIdentity(
            string gameCenterId,
            SuccessCallback success,
            FailureCallback failure)
        {
            AttachIdentity(gameCenterId, "", OperationParam.AuthenticateServiceAuthenticateAuthGameCenter.Value, success, failure);
        }

        /// <summary>Merge the profile associated with the specified Game Center identity with the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="gameCenterId">
        /// The player's game center id  (use the playerID property from the local GKPlayer object)
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void MergeGameCenterIdentity(
            string gameCenterId,
            SuccessCallback success,
            FailureCallback failure)
        {
            MergeIdentity(gameCenterId, "", OperationParam.AuthenticateServiceAuthenticateAuthGameCenter.Value, success, failure);
        }

        /// <summary>Detach the Game Center identity from the current profile.</summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="gameCenterId">
        /// The player's game center id  (use the playerID property from the local GKPlayer object)
        /// </param>
        /// <param name="continueAnon">
        /// Proceed even if the profile will revert to anonymous?
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Watch for DOWNGRADING_TO_ANONYMOUS_ERROR - occurs if you set continueAnon to false, and
        /// disconnecting this identity would result in the profile being anonymous (which means that
        /// the profile wouldn't be retrievable if the user loses their device)
        /// </returns>
        public void DetachGameCenterIdentity(
            string gameCenterId,
            bool continueAnon,
            SuccessCallback success,
            FailureCallback failure)
        {
            DetachIdentity(gameCenterId, OperationParam.AuthenticateServiceAuthenticateAuthGameCenter.Value, continueAnon, success, failure);
        }

        /// <summary>
        /// Attach a Email and Password identity to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="email">
        /// The player's e-mail address
        /// </param>
        /// <param name="password">
        /// The player's password
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Errors to watch for:  SWITCHING_PROFILES - this means that the email address you provided
        /// already points to a different profile.  You will likely want to offer the player the
        /// choice to *SWITCH* to that profile, or *MERGE* the profiles.
        ///
        /// To switch profiles, call ClearSavedProfileID() and then call AuthenticateEmailPassword().
        /// </returns>
        public void AttachEmailIdentity(
            string email,
            string password,
            SuccessCallback success,
            FailureCallback failure)
        {
            AttachIdentity(email, password, OperationParam.AuthenticateServiceAuthenticateAuthEmail.Value, success, failure);
        }

        /// <summary>
        // Merge the profile associated with the provided e=mail with the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="email">
        /// The player's e-mail address
        /// </param>
        /// <param name="password">
        /// The player's password
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void MergeEmailIdentity(
            string email,
            string password,
            SuccessCallback success,
            FailureCallback failure)
        {
            MergeIdentity(email, password, OperationParam.AuthenticateServiceAuthenticateAuthEmail.Value, success, failure);
        }

        /// <summary>Detach the e-mail identity from the current profile
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="email">
        /// The player's e-mail address
        /// </param>
        /// <param name="continueAnon">
        /// Proceed even if the profile will revert to anonymous?
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Watch for DOWNGRADING_TO_ANONYMOUS_ERROR - occurs if you set continueAnon to false, and
        /// disconnecting this identity would result in the profile being anonymous (which means that
        /// the profile wouldn't be retrievable if the user loses their device)
        /// </returns>
        public void DetachEmailIdentity(
            string email,
            bool continueAnon,
            SuccessCallback success,
            FailureCallback failure)
        {
            DetachIdentity(email, OperationParam.AuthenticateServiceAuthenticateAuthEmail.Value, continueAnon, success, failure);
        }

        /// <summary>
        /// Attach a Universal (userid + password) identity to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="userid">
        /// The player's userid
        /// </param>
        /// <param name="password">
        /// The player's password
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Errors to watch for:  SWITCHING_PROFILES - this means that the email address you provided
        /// already points to a different profile.  You will likely want to offer the player the
        /// choice to *SWITCH* to that profile, or *MERGE* the profiles.
        ///
        /// To switch profiles, call ClearSavedProfileID() and then call AuthenticateEmailPassword().
        /// </returns>
        public void AttachUniversalIdentity(
            string userid,
            string password,
            SuccessCallback success,
            FailureCallback failure)
        {
            AttachIdentity(userid, password, OperationParam.AuthenticateServiceAuthenticateAuthUniversal.Value, success, failure);
        }

        /// <summary>
        /// Merge the profile associated with the provided e=mail with the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="userid">
        /// The player's userid
        /// </param>
        /// <param name="password">
        /// The player's password
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void MergeUniversalIdentity(
            string userid,
            string password,
            SuccessCallback success,
            FailureCallback failure)
        {
            MergeIdentity(userid, password, OperationParam.AuthenticateServiceAuthenticateAuthUniversal.Value, success, failure);
        }

        /// <summary>Detach the universal identity from the current profile
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="userid">
        /// The player's userid
        /// </param>
        /// <param name="continueAnon">
        /// Proceed even if the profile will revert to anonymous?
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Watch for DOWNGRADING_TO_ANONYMOUS_ERROR - occurs if you set continueAnon to false, and
        /// disconnecting this identity would result in the profile being anonymous (which means that
        /// the profile wouldn't be retrievable if the user loses their device)
        /// </returns>
        public void DetachUniversalIdentity(
            string userid,
            bool continueAnon,
            SuccessCallback success,
            FailureCallback failure)
        {
            DetachIdentity(userid, OperationParam.AuthenticateServiceAuthenticateAuthUniversal.Value, continueAnon, success, failure);
        }

        /// <summary>
        /// Attach a Steam (userid + steamsessionticket) identity to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="steamId">
        /// String representation of 64 bit steam id
        /// </param>
        /// <param name="sessionTicket">
        /// The player's session ticket (hex encoded)
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Errors to watch for:  SWITCHING_PROFILES - this means that the email address you provided
        /// already points to a different profile.  You will likely want to offer the player the
        /// choice to *SWITCH* to that profile, or *MERGE* the profiles.
        ///
        /// To switch profiles, call ClearSavedProfileID() and then call AuthenticateSteam().
        /// </returns>
        public void AttachSteamIdentity(
            string steamId,
            string sessionTicket,
            SuccessCallback success,
            FailureCallback failure)
        {
            AttachIdentity(steamId, sessionTicket, OperationParam.AuthenticateServiceAuthenticateAuthSteam.Value, success, failure);
        }

        /// <summary>
        /// Merge the profile associated with the provided steam userid with the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="steamId">
        /// String representation of 64 bit steam id
        /// </param>
        /// <param name="sessionTicket">
        /// The player's session ticket (hex encoded)
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void MergeSteamIdentity(
            string steamId,
            string sessionTicket,
            SuccessCallback success,
            FailureCallback failure)
        {
            MergeIdentity(steamId, sessionTicket, OperationParam.AuthenticateServiceAuthenticateAuthSteam.Value, success, failure);
        }

        /// <summary>Detach the steam identity from the current profile
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="steamId">
        /// String representation of 64 bit steam id
        /// </param>
        /// <param name="continueAnon">
        /// Proceed even if the profile will revert to anonymous?
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Watch for DOWNGRADING_TO_ANONYMOUS_ERROR - occurs if you set continueAnon to false, and
        /// disconnecting this identity would result in the profile being anonymous (which means that
        /// the profile wouldn't be retrievable if the user loses their device)
        /// </returns>
        public void DetachSteamIdentity(
            string steamId,
            bool continueAnon,
            SuccessCallback success,
            FailureCallback failure)
        {
            DetachIdentity(steamId, OperationParam.AuthenticateServiceAuthenticateAuthSteam.Value, continueAnon, success, failure);
        }

        /// <summary>
        /// Attach the user's Google credentials to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="googleId">
        /// The google id of the user
        /// </param>
        /// <param name="authenticationToken">
        /// The validated token from the Google SDK
        ///   (that will be further validated when sent to the bC service)
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Errors to watch for:  SWITCHING_PROFILES - this means that the Google identity you provided
        /// already points to a different profile.  You will likely want to offer the player the
        /// choice to *SWITCH* to that profile, or *MERGE* the profiles.
        ///
        /// To switch profiles, call ClearSavedProfileID() and call AuthenticateGoogle().
        /// </returns>
        public void AttachGoogleIdentity(
            string googleId,
            string authenticationToken,
            SuccessCallback success,
            FailureCallback failure)
        {
            AttachIdentity(googleId, authenticationToken, OperationParam.AuthenticateServiceAuthenticateAuthGoogle.Value, success, failure);
        }

        /// <summary>
        /// Merge the profile associated with the provided Google credentials with the
        /// current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="googleId">
        /// The Google id of the user
        /// </param>
        /// <param name="authenticationToken">
        /// The validated token from the Google SDK
        /// (that will be further validated when sent to the bC service)
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void MergeGoogleIdentity(
            string googleId,
            string authenticationToken,
            SuccessCallback success,
            FailureCallback failure)
        {
            MergeIdentity(googleId, authenticationToken, OperationParam.AuthenticateServiceAuthenticateAuthGoogle.Value, success, failure);
        }

        /// <summary>
        /// Detach the Google identity from this profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="googleId">
        /// The Google id of the user
        /// </param>
        /// <param name="continueAnon">
        /// Proceed even if the profile will revert to anonymous?
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Watch for DOWNGRADING_TO_ANONYMOUS_ERROR - occurs if you set continueAnon to false, and
        /// disconnecting this identity would result in the profile being anonymous (which means that
        /// the profile wouldn't be retrievable if the user loses their device)
        /// </returns>
        public void DetachGoogleIdentity(
            string googleId,
            bool continueAnon,
            SuccessCallback success,
            FailureCallback failure)
        {
            DetachIdentity(googleId, OperationParam.AuthenticateServiceAuthenticateAuthGoogle.Value, continueAnon, success, failure);
        }

        /// <summary>
        /// Attach the user's Twitter credentials to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="twitterId">
        /// String representation of a Twitter user ID
        /// </param>
        /// <param name="authenticationToken">
        /// The authentication token derived via the Twitter apis
        /// </param>
        /// <param name="secret">
        /// The secret given when attempting to link with Twitter
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Errors to watch for:  SWITCHING_PROFILES - this means that the Twitter identity you provided
        /// already points to a different profile.  You will likely want to offer the player the
        /// choice to *SWITCH* to that profile, or *MERGE* the profiles.
        ///
        /// To switch profiles, call ClearSavedProfileID() and call AuthenticateTwitter().
        /// </returns>
        public void AttachTwitterIdentity(
            string twitterId,
            string authenticationToken,
            string secret,
            SuccessCallback success,
            FailureCallback failure)
        {
            AttachIdentity(twitterId, authenticationToken + ":" + secret, OperationParam.AuthenticateServiceAuthenticateAuthTwitter.Value, success, failure);
        }

        /// <summary>
        /// Merge the profile associated with the provided Twitter credentials with the
        /// current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="twitterId">
        /// String representation of a Twitter user ID
        /// </param>
        /// <param name="authenticationToken">
        /// The authentication token derived via the Twitter apis
        /// </param>
        /// <param name="secret">
        /// The secret given when attempting to link with Twitter
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        public void MergeTwitterIdentity(
            string twitterId,
            string authenticationToken,
            string secret,
            SuccessCallback success,
            FailureCallback failure)
        {
            MergeIdentity(twitterId, authenticationToken + ":" + secret, OperationParam.AuthenticateServiceAuthenticateAuthTwitter.Value, success, failure);
        }

        /// <summary>
        /// Detach the Twitter identity from this profile.
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="twitterId">
        /// The Twitter id of the user
        /// </param>
        /// <param name="continueAnon">
        /// Proceed even if the profile will revert to anonymous?
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// Watch for DOWNGRADING_TO_ANONYMOUS_ERROR - occurs if you set continueAnon to false, and
        /// disconnecting this identity would result in the profile being anonymous (which means that
        /// the profile wouldn't be retrievable if the user loses their device)
        /// </returns>
        public void DetachTwitterIdentity(
            string twitterId,
            bool continueAnon,
            SuccessCallback success,
            FailureCallback failure)
        {
            DetachIdentity(twitterId, OperationParam.AuthenticateServiceAuthenticateAuthTwitter.Value, continueAnon, success, failure);
        }

        /// <summary>
        /// Switch to a Child Profile
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - SWITCH_TO_CHILD_PROFILE
        /// </remarks>
        /// <param name="childProfileId">
        /// The profileId of the child profile to switch to
        /// If null and forceCreate is true a new profile will be created
        /// </param>
        /// <param name="childGameId">
        /// The appId of the child game to switch to
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
        /// <returns>
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "vcPurchased": 0,
        ///         "experiencePoints": 0,
        ///         "xpCapped": false,
        ///         "playerName": "TestUser",
        ///         "vcClaimed": 0,
        ///         "rewards": {
        ///             "rewardDetails": {},
        ///             "rewards": {},
        ///             "currency": {
        ///                 "credits": {
        ///                     "purchased": 0,
        ///                     "balance": 0,
        ///                     "consumed": 0,
        ///                     "awarded": 0
        ///                 },
        ///                 "gold": {
        ///                     "purchased": 0,
        ///                     "balance": 0,
        ///                     "consumed": 0,
        ///                     "awarded": 0
        ///                 }
        ///             }
        ///         },
        ///         "loginCount": 1,
        ///         "server_time": 1441901094386,
        ///         "experienceLevel": 0,
        ///         "currency": {},
        ///         "statistics": {},
        ///         "id": "a17b347b-695b-431f-b1e7-5f783a562310",
        ///         "profileId": "a17t347b-692b-43ef-b1e7-5f783a566310",
        ///         "newUser": false
        ///     }
        /// }
        /// </returns>
        public void SwitchToChildProfile(
            string childProfileId,
            string childGameId,
            bool forceCreate,
            SuccessCallback success,
            FailureCallback failure)
        {
            SwitchToChildProfile(childProfileId, childGameId, forceCreate, false, success, failure);
        }

        /// <summary>
        /// Switches to the child profile of an app when only one profile exists
        /// If multiple profiles exist this returns an error
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - SWITCH_TO_CHILD_PROFILE
        /// </remarks>
        /// <param name="childGameId">
        /// The App ID of the child game to switch to
        /// </param>
        /// <param name="forceCreate">
        /// Should a new profile be created if one does not exist?
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <returns>
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "vcPurchased": 0,
        ///         "experiencePoints": 0,
        ///         "xpCapped": false,
        ///         "playerName": "TestUser",
        ///         "vcClaimed": 0,
        ///         "rewards": {
        ///             "rewardDetails": {},
        ///             "rewards": {},
        ///             "currency": {
        ///                 "credits": {
        ///                     "purchased": 0,
        ///                     "balance": 0,
        ///                     "consumed": 0,
        ///                     "awarded": 0
        ///                 },
        ///                 "gold": {
        ///                     "purchased": 0,
        ///                     "balance": 0,
        ///                     "consumed": 0,
        ///                     "awarded": 0
        ///                 }
        ///             }
        ///         },
        ///         "loginCount": 1,
        ///         "server_time": 1441901094386,
        ///         "experienceLevel": 0,
        ///         "currency": {},
        ///         "statistics": {},
        ///         "id": "a17b347b-695b-431f-b1e7-5f783a562310",
        ///         "profileId": "a17t347b-692b-43ef-b1e7-5f783a566310",
        ///         "newUser": false
        ///     }
        /// }
        /// </returns>
        public void SwitchToSingletonChildProfile(
            string childGameId,
            bool forceCreate,
            SuccessCallback success,
            FailureCallback failure)
        {
            SwitchToChildProfile(null, childGameId, forceCreate, true, success, failure);
        }

        /// <summary>
        /// Switch to a Parent Profile
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - SWITCH_TO_PARENT_PROFILE
        /// </remarks>
        /// <param name="parentLevelName">
        /// The level of the parent to switch to
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful switch
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error while switching
        /// </param>
        /// <returns>
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "profileId": "1d1h32aa-4c41-404f-bc18-29b3fg5wab8a",
        ///         "gameId": "123456"
        ///     }
        /// }
        /// </returns>
        public void SwitchToParentProfile(
            string parentLevelName,
            SuccessCallback success,
            FailureCallback failure)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.AuthenticateServiceAuthenticateLevelName.Value] = parentLevelName;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.SwitchToParentProfile, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Returns a list of all child profiles in child Apps
        /// </summary>
        /// <remarks>
        /// Service Name - Identity
        /// Service Operation - GET_CHILD_PROFILES
        /// </remarks>
        /// <param name="includeSummaryData">
        /// Whether to return the summary friend data along with this call
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "children": [
        ///             {
        ///                 "appId": "123456",
        ///                 "profileId": "b7h4c751-befd-4a89-b6da-cd55hs3b2a86",
        ///                 "profileName": "Child1",
        ///                 "summaryFriendData": null
        ///             },
        ///             {
        ///                 "appId": "123456",
        ///                 "profileId": "a17b347b-195b-45hf-b1e7-5f78g3462310",
        ///                 "profileName": "Child2",
        ///                 "summaryFriendData": null
        ///             }
        ///         ]
        ///     }
        /// }
        /// </returns>
        public void GetChildProfiles(
            bool includeSummaryData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlayerStateServiceIncludeSummaryData.Value] = includeSummaryData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.GetChildProfiles, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        private void AttachIdentity(string externalId, string authenticationToken, string authenticationType, SuccessCallback success, FailureCallback failure)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.IdentityServiceExternalId.Value] = externalId;
            data[OperationParam.IdentityServiceAuthenticationType.Value] = authenticationType;
            data[OperationParam.AuthenticateServiceAuthenticateAuthenticationToken.Value] = authenticationToken;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.Attach, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        private void MergeIdentity(string externalId, string authenticationToken, string authenticationType, SuccessCallback success, FailureCallback failure)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.IdentityServiceExternalId.Value] = externalId;
            data[OperationParam.IdentityServiceAuthenticationType.Value] = authenticationType;
            data[OperationParam.AuthenticateServiceAuthenticateAuthenticationToken.Value] = authenticationToken;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.Merge, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        private void DetachIdentity(string externalId, string authenticationType, bool continueAnon, SuccessCallback success, FailureCallback failure)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.IdentityServiceExternalId.Value] = externalId;
            data[OperationParam.IdentityServiceAuthenticationType.Value] = authenticationType;
            data[OperationParam.IdentityServiceConfirmAnonymous.Value] = continueAnon;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.Detach, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        private void SwitchToChildProfile(string childProfileId, string childGameId, bool forceCreate, bool forceSingleton, SuccessCallback success, FailureCallback failure)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (Util.IsOptionalParameterValid(childProfileId))
            {
                data[OperationParam.ServiceMessageProfileId.Value] = childProfileId;
            }

            data[OperationParam.AuthenticateServiceAuthenticateGameId.Value] = childGameId;
            data[OperationParam.AuthenticateServiceAuthenticateForceCreate.Value] = forceCreate;
            data[OperationParam.IdentityServiceForceSingleton.Value] = forceSingleton;

            data[OperationParam.AuthenticateServiceAuthenticateReleasePlatform.Value] = m_brainCloudClientRef.ReleasePlatform.ToString();
            data[OperationParam.AuthenticateServiceAuthenticateCountryCode.Value] = Util.GetCurrentCountryCode();
            data[OperationParam.AuthenticateServiceAuthenticateLanguageCode.Value] = Util.GetIsoCodeForCurrentLanguage();
            data[OperationParam.AuthenticateServiceAuthenticateTimeZoneOffset.Value] = Util.GetUTCOffsetForCurrentTimeZone();

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.SwitchToChildProfile, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}
