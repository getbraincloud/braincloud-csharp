// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

//----------------------------------------------------

namespace BrainCloud
{

    using System;
    using System.Collections.Generic;
    using System.Text;
    using BrainCloud.JsonFx.Json;
    using BrainCloud.Internal;

#if !(DOT_NET || GODOT)
    using UnityEngine.SocialPlatforms;
    using UnityEngine;
#endif

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
        /// Service Name - gamification
        /// Service Operation - READ
        /// </remarks>
        /// <param name="includeMetaData">Whether to return meta data as well</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
        /// Service Name - gamification
        /// Service Operation - READ_MILESTONES
        /// </remarks>
        /// <param name="includeMetaData">Whether to return meta data as well</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
        /// Service Name - gamification
        /// Service Operation - READ_ACHIEVEMENTS
        /// </remarks>
        /// <param name="includeMetaData">Whether to return meta data as well</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
        /// </summary>
        /// <remarks>
        /// Service Name - gamification
        /// Service Operation - READ_XP_LEVELS
        /// </remarks>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
        /// Service Name - gamification
        /// Service Operation - READ_ACHIEVED_ACHIEVEMENTS
        /// </remarks>
        /// <param name="includeMetaData">Whether to return meta data as well</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
        /// Service Name - gamification
        /// Service Operation - READ_COMPLETED_MILESTONES
        /// </remarks>
        /// <param name="includeMetaData">Whether to return meta data as well</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
        /// Service Name - gamification
        /// Service Operation - READ_IN_PROGRESS_MILESTONES
        /// </remarks>
        /// <param name="includeMetaData">Whether to return meta data as well</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
        /// Service Name - gamification
        /// Service Operation - READ_MILESTONES_BY_CATEGORY
        /// </remarks>
        /// <param name="category">The milestone category</param>
        /// <param name="includeMetaData">Whether to return meta data as well</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
        /// Method will award the achievements specified.
        /// </summary>
        /// <remarks>
        /// Service Name - gamification
        /// Service Operation - AWARD_ACHIEVEMENTS
        /// </remarks>
        /// <param name="achievementIds">Collection of achievement ids to award</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
        /// Service Name - gamification
        /// Service Operation - READ_QUESTS
        /// </remarks>
        /// <param name="includeMetaData">Whether to return meta data as well</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
        /// Method returns all completed quests.
        /// </summary>
        /// <remarks>
        /// Service Name - gamification
        /// Service Operation - READ_COMPLETED_QUESTS
        /// </remarks>
        /// <param name="includeMetaData">Whether to return meta data as well</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
        /// Method returns quests that are in progress.
        /// </summary>
        /// <remarks>
        /// Service Name - gamification
        /// Service Operation - READ_IN_PROGRESS_QUESTS
        /// </remarks>
        /// <param name="includeMetaData">Whether to return meta data as well</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
        /// Method returns quests that have not been started.
        /// </summary>
        /// <remarks>
        /// Service Name - gamification
        /// Service Operation - READ_NOT_STARTED_QUESTS
        /// </remarks>
        /// <param name="includeMetaData">Whether to return meta data as well</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
        /// Method returns quests with a status.
        /// </summary>
        /// <remarks>
        /// Service Name - gamification
        /// Service Operation - READ_QUESTS_WITH_STATUS
        /// </remarks>
        /// <param name="includeMetaData">Whether to return meta data as well</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
        /// Method returns quests with a basic percentage.
        /// </summary>
        /// <remarks>
        /// Service Name - gamification
        /// Service Operation - READ_QUESTS_WITH_BASIC_PERCENTAGE
        /// </remarks>
        /// <param name="includeMetaData">Whether to return meta data as well</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
        /// Method returns quests with a complex percentage.
        /// </summary>
        /// <remarks>
        /// Service Name - gamification
        /// Service Operation - READ_QUESTS_WITH_COMPLEX_PERCENTAGE
        /// </remarks>
        /// <param name="includeMetaData">Whether to return meta data as well</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
        /// Method returns quests for the given category.
        /// </summary>
        /// <remarks>
        /// Service Name - gamification
        /// Service Operation - READ_QUESTS_BY_CATEGORY
        /// </remarks>
        /// <param name="category">The quest category</param>
        /// <param name="includeMetaData">Whether to return meta data as well</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
    }
}
