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
    public class BrainCloudMatchMaking
    {
        private BrainCloudClient m_brainCloudClientRef;
        public BrainCloudMatchMaking(BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        /// <summary>
        /// Read match making record
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - Read
        /// </remarks>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data": null
        /// }
        /// </returns>
        public void Read(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.Read, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Sets player rating
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - SetPlayerRating
        /// </remarks>
        /// <param name="in_playerRating">
        /// The new player rating.
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data": null
        /// }
        /// </returns>
        public void SetPlayerRating(
            long in_playerRating,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MatchMakingServicePlayerRating.Value] = in_playerRating;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.SetPlayerRating, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Resets player rating
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - ResetPlayerRating
        /// </remarks>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data": null
        /// }
        /// </returns>
        public void ResetPlayerRating(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.ResetPlayerRating, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Increments player rating
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - IncrementPlayerRating
        /// </remarks>
        /// <param name="in_increment">
        /// The increment amount
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data": null
        /// }
        /// </returns>
        public void IncrementPlayerRating(
            long in_increment,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MatchMakingServicePlayerRating.Value] = in_increment;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.IncrementPlayerRating, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Decrements player rating
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - DecrementPlayerRating
        /// </remarks>
        /// <param name="in_decrement">
        /// The decrement amount
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data": null
        /// }
        /// </returns>
        public void DecrementPlayerRating(
            long in_decrement,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MatchMakingServicePlayerRating.Value] = in_decrement;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.DecrementPlayerRating, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Turns shield on
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - ShieldOn
        /// </remarks>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data": null
        /// }
        /// </returns>
        public void TurnShieldOn(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.ShieldOn, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Turns shield on for the specified number of minutes
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - ShieldOnFor
        /// </remarks>
        /// <param name="in_minutes">
        /// Number of minutes to turn the shield on for
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data": null
        /// }
        /// </returns>
        public void TurnShieldOnFor(
            int in_minutes,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MatchMakingServiceMinutes.Value] = in_minutes;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.ShieldOnFor, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Turns shield off
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - ShieldOff
        /// </remarks>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data": null
        /// }
        /// </returns>
        public void TurnShieldOff(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.ShieldOff, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Gets one oneway players
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - GetOnewayPlayers
        /// </remarks>
        /// <param name="in_rangeDelta">
        /// The range delta
        /// </param>
        /// <param name="in_numMatches">
        /// The maximum number of matches to return
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data": null
        /// }
        /// </returns>
        public void GetOneWayPlayers(
            long in_rangeDelta,
            long in_numMatches,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MatchMakingServiceRangeDelta.Value] = in_rangeDelta;
            data[OperationParam.MatchMakingServiceNumMatches.Value] = in_numMatches;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.GetOnewayPlayers, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Gets one oneway players
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - GetOnewayPlayersFilter
        /// </remarks>
        /// <param name="in_rangeDelta">
        /// The range delta
        /// </param>
        /// <param name="in_numMatches">
        /// The maximum number of matches to return
        /// </param>
        /// <param name="in_jsonExtraParms">
        /// Other parameters
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data": null
        /// }
        /// </returns>
        public void GetOneWayPlayersWithFilter(
            long in_rangeDelta,
            long in_numMatches,
            string in_jsonExtraParms,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MatchMakingServiceRangeDelta.Value] = in_rangeDelta;
            data[OperationParam.MatchMakingServiceNumMatches.Value] = in_numMatches;

            if (Util.IsOptionalParameterValid(in_jsonExtraParms))
            {
                Dictionary<string, object> extraParms = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonExtraParms);
                data[OperationParam.MatchMakingServiceExtraParams.Value] = extraParms;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.GetOnewayPlayersFilter, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Enables Match Making for the Player
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - EnableMatchMaking
        /// </remarks>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data": null
        /// }
        /// </returns>
        public void EnableMatchMaking(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.EnableMatchMaking, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}
