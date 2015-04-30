//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using LitJson;
using BrainCloud.Internal;

#if !(DOT_NET)
using UnityEngine.SocialPlatforms;
#endif

namespace BrainCloud
{
    public class BrainCloudGamification
    {
        private BrainCloudClient m_brainCloudClientRef;
        SuccessCallback m_achievementsDelegate;

        public BrainCloudGamification(BrainCloudClient in_brainCloud)
        {
            m_brainCloudClientRef = in_brainCloud;
        }

        /// <summary>
        /// Sets the achievement awarded delegate which is called anytime
        /// an achievement is awarded
        /// </summary>
        public void SetAchievementAwardedDelegate(SuccessCallback in_delegate)
        {
            m_achievementsDelegate = in_delegate;
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
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data": {
        ///     "milestones": [
        ///       {
        ///         "gameId": "com.bitheads.unityexample",
        ///         "milestoneId": "milestone01",
        ///         "playerStatistics": {
        ///           "experiencePoints": null,
        ///           "experienceLevel": null,
        ///           "empty": true,
        ///           "statistics": {}
        ///         },
        ///         "globalStatistics": {
        ///           "statistics": {},
        ///           "empty": true
        ///         },
        ///         "playerStatisticsUnlockThresholds": {
        ///           "experiencePoints": null,
        ///           "experienceLevel": null,
        ///           "empty": true,
        ///           "statistics": {}
        ///         },
        ///         "globalStatisticsUnlockThresholds": {
        ///           "statistics": {},
        ///           "empty": true
        ///         },
        ///         "reward": {
        ///           "experiencePoints": null,
        ///           "playerStatistics": null,
        ///           "currencies": {
        ///             "gems": 10
        ///           },
        ///           "globalGameStatistics": null,
        ///           "achievement": null
        ///         },
        ///         "title": "Level 1 milestone",
        ///         "extraData": null,
        ///         "description": "Awarded when you get to player level 1",
        ///         "category": "general",
        ///         "key": {
        ///           "gameId": "com.bitheads.unityexample",
        ///           "milestoneId": "milestone01",
        ///           "primaryKey": true
        ///         }
        ///       }
        ///     ],
        ///     "achievements": [
        ///       {
        ///         "gameId": "com.bitheads.unityexample",
        ///         "achievementId": "ach01",
        ///         "facebookUrl": "http://someurl.com",
        ///         "title": "Finish Tutorial",
        ///         "imageUrl": "http://someurl.com",
        ///         "facebookGamePoints": 10,
        ///         "extraData": null,
        ///         "invisibleUntilEarned": null,
        ///         "description": "Achievement awarded when you finish the tutorial",
        ///         "key": {
        ///           "gameId": "com.bitheads.unityexample",
        ///           "achievementId": "ach01",
        ///           "primaryKey": true
        ///         }
        ///       }
        ///     ],
        ///     "xp_levels": [
        ///       {
        ///         "gameId": "com.bitheads.unityexample",
        ///         "numericLevel": 1,
        ///         "experience": 10,
        ///         "reward": {
        ///           "experiencePoints": null,
        ///           "playerStatistics": null,
        ///           "currencies": {
        ///             "gold": 1000
        ///           },
        ///           "globalGameStatistics": null,
        ///           "achievement": null
        ///         },
        ///         "facebookAction": "",
        ///         "statusTitle": "Peon",
        ///         "key": {
        ///           "gameId": "com.bitheads.unityexample",
        ///           "numericLevel": 1,
        ///           "primaryKey": true
        ///         }
        ///       },
        ///       {
        ///         "gameId": "com.bitheads.unityexample",
        ///         "numericLevel": 2,
        ///         "experience": 20,
        ///         "reward": {
        ///           "experiencePoints": null,
        ///           "playerStatistics": null,
        ///           "currencies": {
        ///             "gems": 10,
        ///             "gold": 2000
        ///           },
        ///           "globalGameStatistics": null,
        ///           "achievement": null
        ///         },
        ///         "facebookAction": "",
        ///         "statusTitle": "Jester",
        ///         "key": {
        ///           "gameId": "com.bitheads.unityexample",
        ///           "numericLevel": 2,
        ///           "primaryKey": true
        ///         }
        ///       }
        ///     ],
        ///     "quests": []
        ///   }
        /// }
        /// </returns>
        public void ReadAllGamification(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
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
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data": {
        ///     "milestones": [
        ///       {
        ///         "gameId": "com.bitheads.unityexample",
        ///         "milestoneId": "milestone01",
        ///         "playerStatistics": {
        ///           "experiencePoints": null,
        ///           "experienceLevel": null,
        ///           "empty": true,
        ///           "statistics": {}
        ///         },
        ///         "globalStatistics": {
        ///           "statistics": {},
        ///           "empty": true
        ///         },
        ///         "playerStatisticsUnlockThresholds": {
        ///           "experiencePoints": null,
        ///           "experienceLevel": null,
        ///           "empty": true,
        ///           "statistics": {}
        ///         },
        ///         "globalStatisticsUnlockThresholds": {
        ///           "statistics": {},
        ///           "empty": true
        ///         },
        ///         "reward": {
        ///           "experiencePoints": null,
        ///           "playerStatistics": null,
        ///           "currencies": {
        ///             "gems": 10
        ///           },
        ///           "globalGameStatistics": null,
        ///           "achievement": null
        ///         },
        ///         "title": "Level 1 milestone",
        ///         "extraData": null,
        ///         "description": "Awarded when you get to player level 1",
        ///         "category": "general",
        ///         "key": {
        ///           "gameId": "com.bitheads.unityexample",
        ///           "milestoneId": "milestone01",
        ///           "primaryKey": true
        ///         }
        ///       }
        ///     ]
        ///   }
        /// }
        /// </returns>
        public void ReadMilestones(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
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
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data": {
        ///     "achievements": [
        ///       {
        ///         "gameId": "com.bitheads.unityexample",
        ///         "achievementId": "ach01",
        ///         "facebookUrl": "http://someurl.com",
        ///         "title": "Finish Tutorial",
        ///         "imageUrl": "http://someurl.com",
        ///         "facebookGamePoints": 10,
        ///         "extraData": null,
        ///         "invisibleUntilEarned": null,
        ///         "description": "Achievement awarded when you finish the tutorial",
        ///         "key": {
        ///           "gameId": "com.bitheads.unityexample",
        ///           "achievementId": "ach01",
        ///           "primaryKey": true
        ///         }
        ///       },
        ///       {
        ///         "gameId": "com.bitheads.unityexample",
        ///         "achievementId": "ach02",
        ///         "facebookUrl": "http://someurl.com",
        ///         "title": "Level up",
        ///         "imageUrl": "http://someurl.com",
        ///         "facebookGamePoints": 10,
        ///         "extraData": null,
        ///         "invisibleUntilEarned": null,
        ///         "description": "Awarded when you level up for the first time.",
        ///         "key": {
        ///           "gameId": "com.bitheads.unityexample",
        ///           "achievementId": "ach02",
        ///           "primaryKey": true
        ///         }
        ///       }
        ///     ]
        ///   }
        /// }
        /// </returns>
        public void ReadAchievements(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
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
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data": {
        ///     "xp_levels": [
        ///       {
        ///         "gameId": "com.bitheads.unityexample",
        ///         "numericLevel": 1,
        ///         "experience": 10,
        ///         "reward": {
        ///           "experiencePoints": null,
        ///           "playerStatistics": null,
        ///           "currencies": {
        ///             "gold": 1000
        ///           },
        ///           "globalGameStatistics": null,
        ///           "achievement": null
        ///         },
        ///         "facebookAction": "",
        ///         "statusTitle": "Peon",
        ///         "key": {
        ///           "gameId": "com.bitheads.unityexample",
        ///           "numericLevel": 1,
        ///           "primaryKey": true
        ///         }
        ///       }
        ///     ]
        ///   }
        /// }
        /// </returns>
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
        /// <returns> The JSON returned in the callback is as follows:
        ///  {
        ///   "status": 200,
        ///   "data": {
        ///     "achievements": []
        ///   }
        /// }
        /// </returns>
        public void ReadAchievedAchievements(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
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
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data": {
        ///     "milestones": []
        ///   }
        /// }
        /// </returns>
        public void ReadCompletedMilestones(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
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
        /// <returns> The JSON returned in the callback is as follows:
        ///  {
        ///   "status": 200,
        ///   "data": {
        ///     "milestones": []
        ///   }
        /// }
        /// </returns>
        public void ReadInProgressMilestones(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
            data[OperationParam.GamificationServiceIncludeMetaData.Value] = in_includeMetaData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadInProgressMilestones, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }



        /// <summary>
        /// Method retrieves the game (aka global) statistics for the given category.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadGameStatisticsByCategory
        /// </remarks>
        /// <param name="in_category">
        /// The game statistics category
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
        ///   "status":200,
        ///   "data":{
        ///     "gameStatistics": []
        ///   }
        /// }
        /// </returns>
        public void ReadGameStatisticsByCategory(
            string in_category,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
            data[OperationParam.GamificationServiceCategory.Value] = in_category;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadGameStatisticsByCategory, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method retrieves the player statistics for the given category.
        /// </summary>
        /// <remarks>
        /// Service Name - Gamification
        /// Service Operation - ReadPlayerStatisticsByCategory
        /// </remarks>
        /// <param name="in_category">
        /// The player statistics category
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
        ///   "status":200,
        ///   "data":{
        ///     "playerStatistics": []
        ///   }
        /// }
        /// </returns>
        public void ReadPlayerStatisticsByCategory(
            string in_category,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
            data[OperationParam.GamificationServiceCategory.Value] = in_category;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ReadPlayerStatisticsByCategory, data, callback);
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
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status":200,
        ///   "data":{
        ///     "milestones": []
        ///   }
        /// }
        /// </returns>
        public void ReadMilestonesByCategory(
            string in_category,
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
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
        /// A comma separated list of achievement ids to award
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
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        public void AwardAchievements(
            string in_achievementIds,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            string[] ids = in_achievementIds.Split(new char[] {','});
            string achievementsStr = "[";
            for (int i = 0, isize = ids.Length; i < isize; ++i)
            {
                achievementsStr += (i == 0 ? "\"" : ",\"");
                achievementsStr += ids[i];
                achievementsStr += "\"";
            }
            achievementsStr += "]";

            JsonData aaa = JsonMapper.ToObject(achievementsStr);

            JsonData data = new JsonData();
            data[OperationParam.GamificationServiceAchievementsName.Value] = aaa;

            SuccessCallback successCallbacks = (SuccessCallback) AchievementAwardedSuccessCallback;
            if (in_success != null)
            {
                successCallbacks += in_success;
            }
            ServerCallback callback = BrainCloudClient.CreateServerCallback(successCallbacks, in_failure);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.AwardAchievements, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        private void AchievementAwardedSuccessCallback(string in_data, object in_obj)
        {
            JsonData incomingData = JsonMapper.ToObject(in_data);
            try
            {
                incomingData = (JsonData)incomingData[OperationParam.GamificationServiceAchievementsData.Value][OperationParam.GamificationServiceAchievementsName.Value];

                for (int i = 0; i < incomingData.Count; i++)
                {
                    AwardThirdPartyAchievements(incomingData[i].ToString());
                }

                if (m_achievementsDelegate != null)
                {
                    m_achievementsDelegate(in_data, in_obj);
                }
            }
            catch(System.Collections.Generic.KeyNotFoundException)
            {
                //do nothing.
            }
        }

        // goes through JSON response to award achievements via third party (ie game centre, facebook etc).
        // notifies achievement delegate
        internal void CheckForAchievementsToAward(string in_data, object in_obj)
        {
            JsonData incomingData = JsonMapper.ToObject(in_data);
            try
            {
                incomingData = (JsonData)incomingData[OperationParam.GamificationServiceAchievementsData.Value];
                if (incomingData[OperationParam.GamificationServiceAchievementsGranted.Value].IsArray)
                {
                    for (int i = 0; i < incomingData[OperationParam.GamificationServiceAchievementsGranted.Value].Count; i++ )
                    {
                        AwardThirdPartyAchievements(incomingData[OperationParam.GamificationServiceAchievementsGranted.Value][i].ToString());
                    }
                }

                //Let the Game Side knows about those Achievements.
                if (m_achievementsDelegate != null)
                {
                    m_achievementsDelegate(incomingData[OperationParam.GamificationServiceAchievementsGranted.Value].ToString(), in_obj);
                }
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                //do nothing.
            }
        }

        private void AwardThirdPartyAchievements(string in_achievements)
        {

#if !(DOT_NET)
            // only do it for logged in players
            UnityEngine.Social.localUser.Authenticate (success =>
            {
                if (success)
                {
                    IAchievement achievement = UnityEngine.Social.CreateAchievement();
                    achievement.id = in_achievements;
                    achievement.percentCompleted = 100.0; //progress as double
                    achievement.ReportProgress(result =>
                    {
                        if (result)
                            Console.Write("AwardThirdPartyAchievements Success");
                        else
                            Console.Write("AwardThirdPartyAchievements Failed");

                    });
                }
            });
#endif
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
        /// <returns> The JSON returned in the callback is as follows:
        ///  {
        ///   "status": 200,
        ///   "data": {
        ///     "quests": []
        ///   }
        /// }
        /// </returns>
        public void ReadQuests(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
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
        /// <returns> The JSON returned in the callback is as follows:
        ///  {
        ///   "status": 200,
        ///   "data": {
        ///     "quests": []
        ///   }
        /// }
        /// </returns>
        public void ReadCompletedQuests(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
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
        /// <returns> The JSON returned in the callback is as follows:
        ///  {
        ///   "status": 200,
        ///   "data": {
        ///     "quests": []
        ///   }
        /// }
        /// </returns>
        public void ReadInProgressQuests(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
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
        /// <returns> The JSON returned in the callback is as follows:
        ///  {
        ///   "status": 200,
        ///   "data": {
        ///     "quests": []
        ///   }
        /// }
        /// </returns>
        public void ReadNotStartedQuests(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
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
        /// <returns> The JSON returned in the callback is as follows:
        ///  {
        ///   "status": 200,
        ///   "data": {
        ///     "quests": []
        ///   }
        /// }
        /// </returns>
        public void ReadQuestsWithStatus(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
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
        /// <returns> The JSON returned in the callback is as follows:
        ///  {
        ///   "status": 200,
        ///   "data": {
        ///     "quests": []
        ///   }
        /// }
        /// </returns>
        public void ReadQuestsWithBasicPercentage(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
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
        /// <returns> The JSON returned in the callback is as follows:
        ///  {
        ///   "status": 200,
        ///   "data": {
        ///     "quests": []
        ///   }
        /// }
        /// </returns>
        public void ReadQuestsWithComplexPercentage(
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
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
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        public void ReadQuestsByCategory(
            string in_category,
            bool in_includeMetaData = false,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
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
        /// <returns> The JSON returned in the callback is as follows.
        /// {
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        public void ResetMilestones(
            string[] in_milestoneIds,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();

            JsonData milestones = new JsonData();
            for (int i = 0; i < in_milestoneIds.Length; i++)
            {
                milestones.Add(in_milestoneIds[i]);
            }

            data[OperationParam.GamificationServiceMilestones.Value] = milestones;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ResetMilestones, data, callback);
            m_brainCloudClientRef.SendRequest(sc);

        }

    }
}
