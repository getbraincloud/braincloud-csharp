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
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudGamification(BrainCloudClient in_brainCloud)
        {
            m_brainCloudClientRef = in_brainCloud;
        }


        /// <summary>
        /// Method retrieves all gamification data for the player.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
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
        public void ReadAllGamification(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = in_includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.Read, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method retrieves all milestones defined for the game.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadMilestones
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
        public void ReadMilestones(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = in_includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadMilestones, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Read all of the achievements defined for the game.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadAchievements
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
        public void ReadAchievements(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = in_includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadAchievements, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method returns all defined xp levels and any rewards associated
        /// with those xp levels.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadXpLevels
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
        public void ReadXpLevelsMetaData(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadXpLevels, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method retrives the list of achieved achievements.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadAchievedAchievements
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
        public void ReadAchievedAchievements(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = in_includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadAchievedAchievements, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }


        /// <summary>
        /// Method retrieves the list of completed milestones.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadCompleteMilestones
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
        public void ReadCompletedMilestones(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = in_includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadCompletedMilestones, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method retrieves the list of in progress milestones
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadInProgressMilestones
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
        public void ReadInProgressMilestones(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = in_includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadInProgressMilestones, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method retrieves milestones of the given category.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadMilestonesByCategory
        /// </remarks>
        /// <param name="in_category">
        /// The milestone category
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
        public void ReadMilestonesByCategory(
            string in_category,
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceCategory.Value] = in_category;
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = in_includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadMilestonesByCategory, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
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
        /// <param name="in_achievementIds">
        /// A collection of achievement ids to award
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
        public void AwardAchievements(
            IList<string> in_achievementIds,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceAchievementsName.Value] = in_achievementIds;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.AwardAchievements, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method retrieves all of the quests defined for the game.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadQuests
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
        public void ReadQuests(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = in_includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadQuests, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }


        /// <summary>
        ///  Method returns all completed quests.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadCompletedQuests
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
        public void ReadCompletedQuests(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = in_includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadCompletedQuests, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method returns all in progress quests.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadInProgressQuests
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
        public void ReadInProgressQuests(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = in_includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadInProgressQuests, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method returns all quests that haven't been started.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadNotStartedQuests
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
        public void ReadNotStartedQuests(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = in_includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadNotStartedQuests, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        ///  Method returns all quests with status.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadQuestsWithStatus
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
        public void ReadQuestsWithStatus(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = in_includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadQuestsWithStatus, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method returns all quests with a basic percentage.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadQuestsWithBasicPercentage
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
        public void ReadQuestsWithBasicPercentage(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = in_includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadQuestsWithBasicPercentage, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        ///  Method returns all quests with a complex percentage.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadQuestsWithComplexPercentage
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
        public void ReadQuestsWithComplexPercentage(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = in_includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadQuestsWithComplexPercentage, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method returns all quests for the given category.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadQuestsByCategory
        /// </remarks>
        /// <param name="in_category">
        /// The quest category
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
        public void ReadQuestsByCategory(
            string in_category,
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceCategory.Value] = in_category;
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = in_includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadQuestsByCategory, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Sets the specified milestones' statuses to LOCKED.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ResetMilestones
        /// </remarks>
        /// <param name="in_milestoneIds">
        /// List of milestones to reset
        /// </param>
        /// <param name="in_success">
        /// The success callback
        /// </param>
        /// <param name="in_failure">
        /// The failure callback
        /// </param>
        /// <param name="in_cbObject">
        /// The callback object
        /// </param>
        public void ResetMilestones(
            IList<string> in_milestoneIds,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceMilestones.Value] = in_milestoneIds;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ResetMilestones, data, callback);
            m_brainCloudClientRef.SendRequest(sc);

        }
    }
}
