//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

namespace BrainCloud
{

using System.Collections.Generic;
using BrainCloud.Internal;
using BrainCloud.Common;
using System;

    public class BrainCloudIdentity
    {
        private BrainCloudClient _client;

        public BrainCloudIdentity(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Attach the user's Facebook credentials to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void AttachFacebookIdentity(
            string facebookId,
            string authenticationToken,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            AttachIdentity(facebookId, authenticationToken, AuthenticationType.Facebook, success, failure, cbObject);
        }

        /// <summary>
        /// Merge the profile associated with the provided Facebook credentials with the
        /// current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void MergeFacebookIdentity(
            string facebookId,
            string authenticationToken,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            MergeIdentity(facebookId, authenticationToken, AuthenticationType.Facebook, success, failure, cbObject);
        }

        /// <summary>
        /// Detach the Facebook identity from this profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void DetachFacebookIdentity(
            string facebookId,
            bool continueAnon,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            DetachIdentity(facebookId, AuthenticationType.Facebook, continueAnon, success, failure, cbObject);
        }

        /// <summary>
        /// Attach the user's Oculus credentials to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="oculusId">
        /// The oculus id of the user
        /// </param>
        /// <param name="oculusNonce">
        /// token from the Oculus SDK
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void AttachOculusIdentity(
            string oculusId,
            string oculusNonce,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            AttachIdentity(oculusId, oculusNonce, AuthenticationType.Oculus, success, failure, cbObject);
        }

        /// <summary>
        /// Merge the profile associated with the provided Oculus credentials with the
        /// current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="oculusId">
        /// The oculus id of the user
        /// </param>
        /// <param name="oculusNonce">
        /// token from the Oculus SDK
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void MergeOculusIdentity(
            string oculusId,
            string oculusNonce,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            MergeIdentity(oculusId, oculusNonce, AuthenticationType.Oculus, success, failure, cbObject);
        }

        /// <summary>
        /// Detach the Facebook identity from this profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="oculusId">
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void DetachOculusIdentity(
            string oculusId,
            bool continueAnon,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            DetachIdentity(oculusId, AuthenticationType.Oculus, continueAnon, success, failure, cbObject);
        }


                /// <summary>
        /// Attach the user's FacebookLimited credentials to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="facebookLimitedId">
        /// The facebook Limited id of the user
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void AttachFacebookLimitedIdentity(
            string facebookLimitedId,
            string authenticationToken,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            AttachIdentity(facebookLimitedId, authenticationToken, AuthenticationType.FacebookLimited, success, failure, cbObject);
        }

        /// <summary>
        /// Merge the profile associated with the provided Facebook Limited credentials with the
        /// current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="facebookLimitedId">
        /// The facebook Limited id of the user
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void MergeFacebookLimitedIdentity(
            string facebookLimitedId,
            string authenticationToken,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            MergeIdentity(facebookLimitedId, authenticationToken, AuthenticationType.FacebookLimited, success, failure, cbObject);
        }

        /// <summary>
        /// Detach the FacebookLimited identity from this profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="facebookLimitedId">
        /// The facebook Limited id of the user
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void DetachFacebookLimitedIdentity(
            string facebookLimitedId,
            bool continueAnon,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            DetachIdentity(facebookLimitedId, AuthenticationType.FacebookLimited, continueAnon, success, failure, cbObject);
        }

        /// <summary>
        /// Attach the user's PSN credentials to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="psnAccountId">
        /// The PSN account id of the user
        /// </param>
        /// <param name="authenticationToken">
        /// The validated token from the Playstation SDK
        ///   (that will be further validated when sent to the bC service)
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void AttachPlaystationNetworkIdentity(
            string psnAccountId,
            string authenticationToken,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            AttachIdentity(psnAccountId, authenticationToken, AuthenticationType.PlaystationNetwork, success, failure, cbObject);
        }

        /// <summary>
        /// Merge the profile associated with the provided PSN credentials with the
        /// current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="psnAccountId">
        /// The psn account id of the user
        /// </param>
        /// <param name="authenticationToken">
        /// The validated token from the Playstation SDK
        /// (that will be further validated when sent to the bC service)
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void MergePlaystationNetworkIdentity(
            string psnAccountId,
            string authenticationToken,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            MergeIdentity(psnAccountId, authenticationToken, AuthenticationType.PlaystationNetwork, success, failure, cbObject);
        }

        /// <summary>
        /// Detach the PSN identity from this profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="psnAccountId">
        /// The PSN Account id of the user
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void DetachPlaystationNetworkIdentity(
            string psnAccountId,
            bool continueAnon,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            DetachIdentity(psnAccountId, AuthenticationType.PlaystationNetwork, continueAnon, success, failure, cbObject);
        }

        /// <summary>
        /// Attach a Game Center identity to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="gameCenterId">
        /// The user's game center id  (use the playerID property from the local GKPlayer object)
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void AttachGameCenterIdentity(
            string gameCenterId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            AttachIdentity(gameCenterId, "", AuthenticationType.GameCenter, success, failure, cbObject);
        }

        /// <summary>Merge the profile associated with the specified Game Center identity with the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="gameCenterId">
        /// The user's game center id  (use the playerID property from the local GKPlayer object)
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void MergeGameCenterIdentity(
            string gameCenterId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            MergeIdentity(gameCenterId, "", AuthenticationType.GameCenter, success, failure, cbObject);
        }

        /// <summary>Detach the Game Center identity from the current profile.</summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="gameCenterId">
        /// The user's game center id  (use the playerID property from the local GKPlayer object)
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void DetachGameCenterIdentity(
            string gameCenterId,
            bool continueAnon,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            DetachIdentity(gameCenterId, AuthenticationType.GameCenter, continueAnon, success, failure, cbObject);
        }

        /// <summary>
        /// Attach a Email and Password identity to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="email">
        /// The user's e-mail address
        /// </param>
        /// <param name="password">
        /// The user's password
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void AttachEmailIdentity(
            string email,
            string password,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            AttachIdentity(email, password, AuthenticationType.Email, success, failure, cbObject);
        }

        /// <summary>
        // Merge the profile associated with the provided e=mail with the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="email">
        /// The user's e-mail address
        /// </param>
        /// <param name="password">
        /// The user's password
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void MergeEmailIdentity(
            string email,
            string password,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            MergeIdentity(email, password, AuthenticationType.Email, success, failure, cbObject);
        }

        /// <summary>Detach the e-mail identity from the current profile
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="email">
        /// The user's e-mail address
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void DetachEmailIdentity(
            string email,
            bool continueAnon,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            DetachIdentity(email, AuthenticationType.Email, continueAnon, success, failure, cbObject);
        }

        /// <summary>
        /// Attach a Universal (userId + password) identity to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="userId">
        /// The user's userId
        /// </param>
        /// <param name="password">
        /// The user's password
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void AttachUniversalIdentity(
            string userId,
            string password,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            AttachIdentity(userId, password, AuthenticationType.Universal, success, failure, cbObject);
        }

        /// <summary>
        /// Merge the profile associated with the provided e=mail with the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="userId">
        /// The user's userId
        /// </param>
        /// <param name="password">
        /// The user's password
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void MergeUniversalIdentity(
            string userId,
            string password,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            MergeIdentity(userId, password, AuthenticationType.Universal, success, failure, cbObject);
        }

        /// <summary>Detach the universal identity from the current profile
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="userId">
        /// The user's userId
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void DetachUniversalIdentity(
            string userId,
            bool continueAnon,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            DetachIdentity(userId, AuthenticationType.Universal, continueAnon, success, failure, cbObject);
        }

        /// <summary>
        /// Attach a Steam (userId + steamsessionticket) identity to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="steamId">
        /// String representation of 64 bit steam id
        /// </param>
        /// <param name="sessionTicket">
        /// The user's session ticket (hex encoded)
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void AttachSteamIdentity(
            string steamId,
            string sessionTicket,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            AttachIdentity(steamId, sessionTicket, AuthenticationType.Steam, success, failure, cbObject);
        }

        /// <summary>
        /// Merge the profile associated with the provided steam userId with the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="steamId">
        /// String representation of 64 bit steam id
        /// </param>
        /// <param name="sessionTicket">
        /// The user's session ticket (hex encoded)
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void MergeSteamIdentity(
            string steamId,
            string sessionTicket,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            MergeIdentity(steamId, sessionTicket, AuthenticationType.Steam, success, failure, cbObject);
        }

        /// <summary>Detach the steam identity from the current profile
        /// </summary>
        /// <remarks>
        /// Service Name - identity
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void DetachSteamIdentity(
            string steamId,
            bool continueAnon,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            DetachIdentity(steamId, AuthenticationType.Steam, continueAnon, success, failure, cbObject);
        }

        /// <summary>
        /// Attach the user's Google credentials to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="googleUserId">
        /// String representation of google+ userId. Gotten with calls like RequestUserId
        /// </param>
        /// <param name="serverAuthCode">
        /// The validated token from the Google SDK
        ///   (that will be further validated when sent to the bC service)
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void AttachGoogleIdentity(
            string googleUserId,
            string serverAuthCode,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            AttachIdentity(googleUserId, serverAuthCode, AuthenticationType.Google, success, failure, cbObject);
        }

        /// <summary>
        /// Merge the profile associated with the provided Google credentials with the
        /// current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="googleUserId">
        /// String representation of google+ userId. Gotten with calls like RequestUserId
        /// </param>
        /// <param name="serverAuthCode">
        /// The validated token from the Google SDK
        /// (that will be further validated when sent to the bC service)
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void MergeGoogleIdentity(
            string googleUserId,
            string serverAuthCode,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            MergeIdentity(googleUserId, serverAuthCode, AuthenticationType.Google, success, failure, cbObject);
        }

        /// <summary>
        /// Detach the Google identity from this profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="googleUserId">
        /// String representation of google+ userId. Gotten with calls like RequestUserId
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void DetachGoogleIdentity(
            string googleUserId,
            bool continueAnon,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            DetachIdentity(googleUserId, AuthenticationType.Google, continueAnon, success, failure, cbObject);
        }

            /// <summary>
        /// Attach the user's Google credentials to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="googleUserAccountEmail">
        /// The email associated with the google user
        /// </param>
        /// <param name="IdToken">
        /// The id token of the google account. Can get with calls like requestIdToken
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void AttachGoogleOpenIdIdentity(
            string googleUserAccountEmail,
            string IdToken,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            AttachIdentity(googleUserAccountEmail, IdToken, AuthenticationType.GoogleOpenId, success, failure, cbObject);
        }

        /// <summary>
        /// Merge the profile associated with the provided Google credentials with the
        /// current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="googleUserAccountEmail">
        /// The email associated with the google user
        /// </param>
        /// <param name="IdToken">
        /// The id token of the google account. Can get with calls like requestIdToken
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void MergeGoogleOpenIdIdentity(
            string googleUserAccountEmail,
            string IdToken,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            MergeIdentity(googleUserAccountEmail, IdToken, AuthenticationType.GoogleOpenId, success, failure, cbObject);
        }

        /// <summary>
        /// Detach the Google identity from this profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="googleUserAccountEmail">
        /// The email associated with the google user
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void DetachGoogleOpenIdIdentity(
            string googleUserAccountEmail,
            bool continueAnon,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            DetachIdentity(googleUserAccountEmail, AuthenticationType.GoogleOpenId, continueAnon, success, failure, cbObject);
        }

               /// <summary>
        /// Attach the user's Apple credentials to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="appleUserId">
        /// This can be the user id OR the email of the user for the account
        /// </param>
        /// <param name="identityToken">
        /// The token confirming the user's identity
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void AttachAppleIdentity(
            string appleUserId,
            string identityToken,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            AttachIdentity(appleUserId, identityToken, AuthenticationType.Apple, success, failure, cbObject);
        }

        /// <summary>
        /// Merge the profile associated with the provided Apple credentials with the
        /// current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="appleUserId">
        /// This can be the user id OR the email of the user for the account
        /// </param>
        /// <param name="identityToken">
        /// The token confirming the user's identity
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void MergeAppleIdentity(
            string appleUserId,
            string identityToken,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            MergeIdentity(appleUserId, identityToken, AuthenticationType.Apple, success, failure, cbObject);
        }

        /// <summary>
        /// Detach the Apple identity from this profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="appleUserId">
        /// This can be the user id OR the email of the user for the account
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void DetachAppleIdentity(
            string appleUserId,
            bool continueAnon,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            DetachIdentity(appleUserId, AuthenticationType.Apple, continueAnon, success, failure, cbObject);
        }

        /// <summary>
        /// Attach the user's Twitter credentials to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void AttachTwitterIdentity(
            string twitterId,
            string authenticationToken,
            string secret,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            AttachIdentity(twitterId, authenticationToken + ":" + secret, AuthenticationType.Twitter, success, failure, cbObject);
        }

        /// <summary>
        /// Merge the profile associated with the provided Twitter credentials with the
        /// current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void MergeTwitterIdentity(
            string twitterId,
            string authenticationToken,
            string secret,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            MergeIdentity(twitterId, authenticationToken + ":" + secret, AuthenticationType.Twitter, success, failure, cbObject);
        }

        /// <summary>
        /// Detach the Twitter identity from this profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void DetachTwitterIdentity(
            string twitterId,
            bool continueAnon,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            DetachIdentity(twitterId, AuthenticationType.Twitter, continueAnon, success, failure, cbObject);
        }

        /// <summary>
        /// Attach the user's Parse credentials to the current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Attach
        /// </remarks>
        /// <param name="parseId">
        /// The Parse id of the user
        /// </param>
        /// <param name="authenticationToken">
        /// The validated token from Parse
        ///   (that will be further validated when sent to the bC service)
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void AttachParseIdentity(
            string parseId,
            string authenticationToken,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            AttachIdentity(parseId, authenticationToken, AuthenticationType.Parse, success, failure, cbObject);
        }

        /// <summary>
        /// Merge the profile associated with the provided Parse credentials with the
        /// current profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Merge
        /// </remarks>
        /// <param name="parseId">
        /// The Parse id of the user
        /// </param>
        /// <param name="authenticationToken">
        /// The validated token from Parse
        /// (that will be further validated when sent to the bC service)
        /// </param>
        /// <param name="success">
        /// The method to call in event of successful login
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error during authentication
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void MergeParseIdentity(
            string parseId,
            string authenticationToken,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            MergeIdentity(parseId, authenticationToken, AuthenticationType.Parse, success, failure, cbObject);
        }

        /// <summary>
        /// Detach the Parse identity from this profile.
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - Detach
        /// </remarks>
        /// <param name="parseId">
        /// The Parse id of the user
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void DetachParseIdentity(
            string parseId,
            bool continueAnon,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            DetachIdentity(parseId, AuthenticationType.Parse, continueAnon, success, failure, cbObject);
        }

        /// <summary>
        /// Switch to a Child Profile
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - SWITCH_TO_CHILD_PROFILE
        /// </remarks>
        /// <param name="childProfileId">
        /// The profileId of the child profile to switch to
        /// If null and forceCreate is true a new profile will be created
        /// </param>
        /// <param name="childAppId">
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void SwitchToChildProfile(
            string childProfileId,
            string childAppId,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            SwitchToChildProfile(childProfileId, childAppId, forceCreate, false, success, failure, cbObject);
        }

        /// <summary>
        /// Switches to the child profile of an app when only one profile exists
        /// If multiple profiles exist this returns an error
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - SWITCH_TO_CHILD_PROFILE
        /// </remarks>
        /// <param name="childAppId">
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void SwitchToSingletonChildProfile(
            string childAppId,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            SwitchToChildProfile(null, childAppId, forceCreate, true, success, failure, cbObject);
        }

        /// <summary>
        /// Attaches a univeral id to the current profile with no login capability.
        /// </summary>
        /// <param name="externalId">
        /// User ID
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
        public void AttachNonLoginUniversalId(
            string externalId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.IdentityServiceExternalId.Value] = externalId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.AttachNonLoginUniversalId, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Updates univeral id of the current profile.
        /// </summary>
        /// <param name="externalId">
        /// User ID
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
        public void UpdateUniversalIdLogin(
            string externalId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.IdentityServiceExternalId.Value] = externalId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.UpdateUniversalIdLogin, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Attach a new identity to a parent app
        /// </summary>
        /// <param name="externalId">
        /// User ID
        /// </param>
        /// <param name="authenticationToken">
        /// Password or client side token
        /// </param>
        /// <param name="authenticationType">
        /// Type of authentication
        /// </param>
        /// <param name="externalAuthName">
        /// Optional - if using AuthenticationType of external
        /// </param>
        /// <param name="forceCreate">
        /// If the profile does not exist, should it be created?
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
        public void AttachParentWithIdentity(
            string externalId,
            string authenticationToken,
            AuthenticationType authenticationType,
            string externalAuthName,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.IdentityServiceExternalId.Value] = externalId;
            data[OperationParam.AuthenticateServiceAuthenticateAuthenticationToken.Value] = authenticationToken;
            data[OperationParam.IdentityServiceAuthenticationType.Value] = authenticationType.ToString();

            if (Util.IsOptionalParameterValid(externalAuthName))
                data[OperationParam.AuthenticateServiceAuthenticateExternalAuthName.Value] = externalAuthName;
            
            data[OperationParam.AuthenticateServiceAuthenticateForceCreate.Value] = forceCreate;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.AttachParentWithIdentity, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Switch to a Parent Profile
        /// </summary>
        /// <remarks>
        /// Service Name - identity
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
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void SwitchToParentProfile(
            string parentLevelName,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.AuthenticateServiceAuthenticateLevelName.Value] = parentLevelName;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.SwitchToParentProfile, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Detaches parent from this user's profile
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - DETACH_PARENT
        /// </remarks>
        /// <param name="success">
        /// The method to call in event of successful switch
        /// </param>
        /// <param name="failure">
        /// The method to call in the event of an error while switching
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void DetachParent(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.DetachParent, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Returns a list of all child profiles in child Apps
        /// </summary>
        /// <remarks>
        /// Service Name - identity
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
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieve list of identities
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - GET_IDENTITIES
        /// </remarks>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void GetIdentities(SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.GetIdentities, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieve list of expired identities
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - GET_EXPIRED_IDENTITIES
        /// </remarks>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void GetExpiredIdentities(SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.GetExpiredIdentities, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Refreshes an identity for this user
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - REFRESH_IDENTITY
        /// </remarks>
        /// <param name="externalId">
        /// User ID
        /// </param>
        /// <param name="authenticationToken">
        /// Password or client side token
        /// </param>
        /// <param name="authenticationType">
        /// Type of authentication
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
        public void RefreshIdentity(
            string externalId,
            string authenticationToken,
            AuthenticationType authenticationType,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.IdentityServiceExternalId.Value] = externalId;
            data[OperationParam.AuthenticateServiceAuthenticateAuthenticationToken.Value] = authenticationToken;
            data[OperationParam.IdentityServiceAuthenticationType.Value] = authenticationType.ToString();

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.RefreshIdentity, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Allows email identity email address to be changed
        /// </summary>
        /// <remarks>
        /// Service Name - identity
        /// Service Operation - REFRESH_IDENTITY
        /// </remarks>
        /// <param name="oldEmailAddress">
        /// Old email address
        /// </param>
        /// <param name="password">
        /// Password for identity
        /// </param>
        /// <param name="newEmailAddress">
        /// New email address
        /// </param>
        /// <param name="updateContactEmail">
        /// Whether to update contact email in profile
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
        public void ChangeEmailIdentity(
            string oldEmailAddress,
            string password,
            string newEmailAddress,
            bool updateContactEmail,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.IdentityServiceOldEmailAddress.Value] = oldEmailAddress;
            data[OperationParam.AuthenticateServiceAuthenticateAuthenticationToken.Value] = password;
            data[OperationParam.IdentityServiceNewEmailAddress.Value] = newEmailAddress;
            data[OperationParam.IdentityServiceUpdateContactEmail.Value] = updateContactEmail;


            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.ChangeEmailIdentity, data, callback);
            _client.SendRequest(sc);
        }
        

        /// <summary>
        /// Attaches a peer identity to this user's profile
        /// </summary>
        /// <param name="peer">
        /// Name of the peer to connect to
        /// </param>
        /// <param name="externalId">
        /// User ID
        /// </param>
        /// <param name="authenticationToken">
        /// Password or client side token
        /// </param>
        /// <param name="authenticationType">
        /// Type of authentication
        /// </param>
        /// <param name="externalAuthName">
        /// Optional - if using AuthenticationType of external
        /// </param>
        /// <param name="forceCreate">
        /// If the profile does not exist, should it be created?
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
        public void AttachPeerProfile(
            string peer,
            string externalId,
            string authenticationToken,
            AuthenticationType authenticationType,
            string externalAuthName,
            bool forceCreate,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.IdentityServiceExternalId.Value] = externalId;
            data[OperationParam.AuthenticateServiceAuthenticateAuthenticationToken.Value] = authenticationToken;
            data[OperationParam.IdentityServiceAuthenticationType.Value] = authenticationType.ToString();

            if(Util.IsOptionalParameterValid(externalAuthName))
                data[OperationParam.AuthenticateServiceAuthenticateExternalAuthName.Value] = externalAuthName;

            data[OperationParam.Peer.Value] = peer;
            data[OperationParam.AuthenticateServiceAuthenticateForceCreate.Value] = forceCreate;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.AttachPeerProfile, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Detaches a peer identity from this user's profile
        /// </summary>
        /// <param name="peer">
        /// Name of the peer to connect to
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
        public void DetachPeer(
            string peer,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.Peer.Value] = peer;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.DetachPeer, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves a list of attached peer profiles
        /// </summary>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void GetPeerProfiles(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.GetPeerProfiles, null, callback);
            _client.SendRequest(sc);
        }

        [Obsolete("This has been deprecated, use AttachBlockChainIdentity instead. Removal on Match 1, 2022")]
        public void AttachBlockChain(
            string blockchainConfig,
            string publicKey,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.BlockChainConfig.Value] = blockchainConfig;
            data[OperationParam.PublicKey.Value] = publicKey;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.AttachBlockChain, data, callback);
            _client.SendRequest(sc);
        }

        [Obsolete("This has been deprecated, use DetachBlockChainIdentity instead. Removal on Match 1, 2022")]
        public void DetachBlockChain(
            string blockchainConfig,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.BlockChainConfig.Value] = blockchainConfig;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.DetachBlockChain, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Attach blockchain
        /// </summary>
        /// <param name="blockchainConfig">
        /// 
        /// </param>
        /// <param name="publicKey">
        /// 
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
        public void AttachBlockChainIdentity(
            string blockchainConfig,
            string publicKey,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.BlockChainConfig.Value] = blockchainConfig;
            data[OperationParam.PublicKey.Value] = publicKey;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.AttachBlockChain, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// detach blockchain
        /// </summary>
        /// <param name="blockchainConfig">
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
        public void DetachBlockChainIdentity(
            string blockchainConfig,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.BlockChainConfig.Value] = blockchainConfig;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.DetachBlockChain, data, callback);
            _client.SendRequest(sc);
        }


        #region Private Methods

        private void AttachIdentity(string externalId, string authenticationToken, AuthenticationType authenticationType, SuccessCallback success, FailureCallback failure,
            object cbObject)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.IdentityServiceExternalId.Value] = externalId;
            data[OperationParam.IdentityServiceAuthenticationType.Value] = authenticationType.ToString();
            data[OperationParam.AuthenticateServiceAuthenticateAuthenticationToken.Value] = authenticationToken;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.Attach, data, callback);
            _client.SendRequest(sc);
        }

        private void MergeIdentity(string externalId, string authenticationToken, AuthenticationType authenticationType, SuccessCallback success, FailureCallback failure,
            object cbObject)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.IdentityServiceExternalId.Value] = externalId;
            data[OperationParam.IdentityServiceAuthenticationType.Value] = authenticationType.ToString();
            data[OperationParam.AuthenticateServiceAuthenticateAuthenticationToken.Value] = authenticationToken;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.Merge, data, callback);
            _client.SendRequest(sc);
        }

        private void DetachIdentity(string externalId, AuthenticationType authenticationType, bool continueAnon, SuccessCallback success, FailureCallback failure,
            object cbObject)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.IdentityServiceExternalId.Value] = externalId;
            data[OperationParam.IdentityServiceAuthenticationType.Value] = authenticationType.ToString();
            data[OperationParam.IdentityServiceConfirmAnonymous.Value] = continueAnon;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.Detach, data, callback);
            _client.SendRequest(sc);
        }

        private void SwitchToChildProfile(string childProfileId, string childAppd, bool forceCreate, bool forceSingleton, SuccessCallback success, FailureCallback failure,
            object cbObject)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (Util.IsOptionalParameterValid(childProfileId))
            {
                data[OperationParam.ProfileId.Value] = childProfileId;
            }

            data[OperationParam.AuthenticateServiceAuthenticateGameId.Value] = childAppd;
            data[OperationParam.AuthenticateServiceAuthenticateForceCreate.Value] = forceCreate;
            data[OperationParam.IdentityServiceForceSingleton.Value] = forceSingleton;

            data[OperationParam.AuthenticateServiceAuthenticateReleasePlatform.Value] = _client.ReleasePlatform.ToString();
            data[OperationParam.AuthenticateServiceAuthenticateCountryCode.Value] = Util.GetCurrentCountryCode();
            data[OperationParam.AuthenticateServiceAuthenticateLanguageCode.Value] = Util.GetIsoCodeForCurrentLanguage();
            data[OperationParam.AuthenticateServiceAuthenticateTimeZoneOffset.Value] = Util.GetUTCOffsetForCurrentTimeZone();

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Identity, ServiceOperation.SwitchToChildProfile, data, callback);
            _client.SendRequest(sc);
        }

        #endregion
    }
}
