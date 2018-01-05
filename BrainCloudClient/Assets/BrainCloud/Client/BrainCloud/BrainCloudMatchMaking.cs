//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;
using JsonFx.Json;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudMatchMaking
    {
        private BrainCloudClient _client;
        public BrainCloudMatchMaking(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Read match making record
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - Read
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
        public void Read(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.Read, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Sets player rating
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - SetPlayerRating
        /// </remarks>
        /// <param name="playerRating">
        /// The new player rating.
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
        public void SetPlayerRating(
            long playerRating,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MatchMakingServicePlayerRating.Value] = playerRating;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.SetPlayerRating, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Resets player rating
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - ResetPlayerRating
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
        public void ResetPlayerRating(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.ResetPlayerRating, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Increments player rating
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - IncrementPlayerRating
        /// </remarks>
        /// <param name="increment">
        /// The increment amount
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
        public void IncrementPlayerRating(
            long increment,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MatchMakingServicePlayerRating.Value] = increment;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.IncrementPlayerRating, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Decrements player rating
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - DecrementPlayerRating
        /// </remarks>
        /// <param name="decrement">
        /// The decrement amount
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
        public void DecrementPlayerRating(
            long decrement,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MatchMakingServicePlayerRating.Value] = decrement;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.DecrementPlayerRating, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Turns shield on
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - ShieldOn
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
        public void TurnShieldOn(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.ShieldOn, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Turns shield on for the specified number of minutes
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - ShieldOnFor
        /// </remarks>
        /// <param name="minutes">
        /// Number of minutes to turn the shield on for
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
        public void TurnShieldOnFor(
            int minutes,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MatchMakingServiceMinutes.Value] = minutes;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.ShieldOnFor, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Turns shield off
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - ShieldOff
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
        public void TurnShieldOff(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.ShieldOff, null, callback);
            _client.SendRequest(sc);
        }
        
        /// <summary>
        /// Increases the shield on time by specified number of minutes 
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - IncrementShieldOnFor
        /// </remarks>
        /// <param name="minutes">
        /// Number of minutes to increase the shield time for
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
        public void IncrementShieldOnFor(
            int minutes,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MatchMakingServiceMinutes.Value] = minutes;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.IncrementShieldOnFor, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Gets the shield expiry for the given player id. Passing in a null player id
        /// will return the shield expiry for the current player. The value returned is
        /// the time in UTC millis when the shield will expire.
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - GetShieldExpiry
        /// </remarks>
        /// <param name="playerId">
        /// The player id or use null to retrieve for the current player
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
        public void GetShieldExpiry(
            string playerId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (Util.IsOptionalParameterValid(playerId))
            {
                data[OperationParam.MatchMakingServicePlayerId.Value] = playerId;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.GetShieldExpiry, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Finds matchmaking enabled players
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - FIND_PLAYERS
        /// </remarks>
        /// <param name="rangeDelta">
        /// The range delta
        /// </param>
        /// <param name="numMatches">
        /// The maximum number of matches to return
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
        public void FindPlayers(
            long rangeDelta,
            long numMatches,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            FindPlayersWithAttributes(rangeDelta, numMatches, null, success, failure, cbObject);
        }

        /// <summary>
        /// Finds matchmaking enabled players with additional attributes
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - FIND_PLAYERS
        /// </remarks>
        /// <param name="rangeDelta">
        /// The range delta
        /// </param>
        /// <param name="numMatches">
        /// The maximum number of matches to return
        /// </param>
        /// <param name="jsonAttributes">
        /// Attributes match criteria
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
        public void FindPlayersWithAttributes(
            long rangeDelta,
            long numMatches,
            string jsonAttributes,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MatchMakingServiceRangeDelta.Value] = rangeDelta;
            data[OperationParam.MatchMakingServiceNumMatches.Value] = numMatches;

            if (Util.IsOptionalParameterValid(jsonAttributes))
            {
                Dictionary<string, object> attribs = JsonReader.Deserialize<Dictionary<string, object>>(jsonAttributes);
                data[OperationParam.MatchMakingServiceAttributes.Value] = attribs;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.FindPlayers, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Finds matchmaking enabled players using a cloud code filter
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - FIND_PLAYERS_USING_FILTER
        /// </remarks>
        /// <param name="rangeDelta">
        /// The range delta
        /// </param>
        /// <param name="numMatches">
        /// The maximum number of matches to return
        /// </param>
        /// <param name="jsonExtraParms">
        /// Parameters to pass to the CloudCode filter script
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
        public void FindPlayersUsingFilter(
            long rangeDelta,
            long numMatches,
            string jsonExtraParms,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            FindPlayersWithAttributesUsingFilter(rangeDelta, numMatches, null, jsonExtraParms, success, failure, cbObject);
        }

        /// <summary>
        /// Finds matchmaking enabled players using a cloud code filter 
        /// and additional attributes
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - FIND_PLAYERS_USING_FILTER
        /// </remarks>
        /// <param name="rangeDelta">
        /// The range delta
        /// </param>
        /// <param name="numMatches">
        /// The maximum number of matches to return
        /// </param>
        /// <param name="jsonAttributes">
        /// Attributes match criteria
        /// </param>
        /// <param name="jsonExtraParms">
        /// Parameters to pass to the CloudCode filter script
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
        public void FindPlayersWithAttributesUsingFilter(
            long rangeDelta,
            long numMatches,
            string jsonAttributes,
            string jsonExtraParms,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MatchMakingServiceRangeDelta.Value] = rangeDelta;
            data[OperationParam.MatchMakingServiceNumMatches.Value] = numMatches;

            if (Util.IsOptionalParameterValid(jsonAttributes))
            {
                Dictionary<string, object> attribs = JsonReader.Deserialize<Dictionary<string, object>>(jsonAttributes);
                data[OperationParam.MatchMakingServiceAttributes.Value] = attribs;
            }

            if (Util.IsOptionalParameterValid(jsonExtraParms))
            {
                Dictionary<string, object> extraParms = JsonReader.Deserialize<Dictionary<string, object>>(jsonExtraParms);
                data[OperationParam.MatchMakingServiceExtraParams.Value] = extraParms;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.FindPlayersUsingFilter, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Enables Match Making for the Player
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - EnableMatchMaking
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
        public void EnableMatchMaking(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.EnableMatchMaking, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Disables Match Making for the Player
        /// </summary>
        /// <remarks>
        /// Service Name - MatchMaking
        /// Service Operation - EnableMatchMaking
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
        public void DisableMatchMaking(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.MatchMaking, ServiceOperation.DisableMatchMaking, null, callback);
            _client.SendRequest(sc);
        }
    }
}
