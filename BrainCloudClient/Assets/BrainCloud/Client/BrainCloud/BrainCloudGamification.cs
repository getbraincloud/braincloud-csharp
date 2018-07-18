//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using JsonFx.Json;
using BrainCloud.Internal;

#if !(DOT_NET)
using UnityEngine.SocialPlatforms;
using UnityEngine;
#endif

namespace BrainCloud
{
    public class BrainCloudGamification
    {
        private BrainCloudClient _client;

        public BrainCloudGamification(BrainCloudClient client)
        {
            _client = client;
        }


        /// <summary>
        /// Method retrieves all gamification data for the player.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
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
        public void ReadAllGamification(
            bool includeMetaData = false,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.Read, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method retrieves all milestones defined for the game.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadMilestones
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
        public void ReadMilestones(
            bool includeMetaData = false,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadMilestones, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Read all of the achievements defined for the game.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadAchievements
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
        public void ReadAchievements(
            bool includeMetaData = false,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadAchievements, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method returns all defined xp levels and any rewards associated
        /// with those xp levels.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadXpLevels
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
        public void ReadXpLevelsMetaData(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadXpLevels, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method retrives the list of achieved achievements.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadAchievedAchievements
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
        public void ReadAchievedAchievements(
            bool includeMetaData = false,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadAchievedAchievements, data, callback);
            _client.SendRequest(sc);
        }


        /// <summary>
        /// Method retrieves the list of completed milestones.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadCompleteMilestones
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
        public void ReadCompletedMilestones(
            bool includeMetaData = false,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadCompletedMilestones, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method retrieves the list of in progress milestones
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadInProgressMilestones
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
        public void ReadInProgressMilestones(
            bool includeMetaData = false,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadInProgressMilestones, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method retrieves milestones of the given category.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadMilestonesByCategory
        /// </remarks>
        /// <param name="category">
        /// The milestone category
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
        public void ReadMilestonesByCategory(
            string category,
            bool includeMetaData = false,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceCategory.Value] = category;
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadMilestonesByCategory, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method will award the achievements specified. On success, this will
        /// call AwardThirdPartyAchievement to hook into the client-side Achievement
        /// service (ie GameCentre, Facebook etc).
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - AwardAchievements
        /// </remarks>
        /// <param name="achievementIds">
        /// A collection of achievement ids to award
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
        public void AwardAchievements(
            IList<string> achievementIds,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceAchievementsName.Value] = achievementIds;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.AwardAchievements, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method retrieves all of the quests defined for the game.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadQuests
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
        public void ReadQuests(
            bool includeMetaData = false,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadQuests, data, callback);
            _client.SendRequest(sc);
        }


        /// <summary>
        ///  Method returns all completed quests.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadCompletedQuests
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
        public void ReadCompletedQuests(
            bool includeMetaData = false,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadCompletedQuests, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method returns all in progress quests.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadInProgressQuests
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
        public void ReadInProgressQuests(
            bool includeMetaData = false,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadInProgressQuests, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method returns all quests that haven't been started.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadNotStartedQuests
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
        public void ReadNotStartedQuests(
            bool includeMetaData = false,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadNotStartedQuests, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        ///  Method returns all quests with status.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadQuestsWithStatus
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
        public void ReadQuestsWithStatus(
            bool includeMetaData = false,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadQuestsWithStatus, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method returns all quests with a basic percentage.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadQuestsWithBasicPercentage
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
        public void ReadQuestsWithBasicPercentage(
            bool includeMetaData = false,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadQuestsWithBasicPercentage, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        ///  Method returns all quests with a complex percentage.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadQuestsWithComplexPercentage
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
        public void ReadQuestsWithComplexPercentage(
            bool includeMetaData = false,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadQuestsWithComplexPercentage, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method returns all quests for the given category.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadQuestsByCategory
        /// </remarks>
        /// <param name="category">
        /// The quest category
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
        public void ReadQuestsByCategory(
            string category,
            bool includeMetaData = false,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceCategory.Value] = category;
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadQuestsByCategory, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Sets the specified milestones' statuses to LOCKED.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ResetMilestones
        /// </remarks>
        /// <param name="milestoneIds">
        /// List of milestones to reset
        /// </param>
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        /// </param>
        public void ResetMilestones(
            IList<string> milestoneIds,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceMilestones.Value] = milestoneIds;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ResetMilestones, data, callback);
            _client.SendRequest(sc);

        }
    }
}
