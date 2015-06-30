//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using JsonFx.Json;
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
        ///  "status": 200,
        ///  "data": {
        ///   "milestones": [
        ///    {
        ///     "id": "milestone02",
        ///     "category": "general",
        ///     "title": "Level 2 milestone",
        ///     "status": "SATISFIED",
        ///     "description": "Awarded when you get to level 2",
        ///     "gameId": "10068",
        ///     "rewards": {
        ///      "currency": {
        ///       "gold": 1000
        ///      }
        ///     },
        ///     "extraData": null,
        ///     "questId": null,
        ///     "milestoneId": "milestone02"
        ///    },
        ///    {
        ///     "id": "milestone01",
        ///     "thresholds": {
        ///      "playerStatistics": {
        ///       "experiencePoints": 0
        ///      }
        ///     },
        ///     "category": "general",
        ///     "title": "Level 1 milestone",
        ///     "status": "SATISFIED",
        ///     "description": "Awarded when you get to player level 1",
        ///     "gameId": "10068",
        ///     "rewards": {
        ///      "currency": {
        ///       "gems": 10
        ///      }
        ///     },
        ///     "extraData": null,
        ///     "questId": null,
        ///     "milestoneId": "milestone01"
        ///    }
        ///   ],
        ///   "achievements": [
        ///    {
        ///     "fbEnabled": true,
        ///     "imageUrl": null,
        ///     "status": "NOT_AWARDED",
        ///     "gameId": "10068",
        ///     "steamEnabled": false,
        ///     "extraData": null,
        ///     "achievementId": "ach01",
        ///     "invisibleUntilEarned": false,
        ///     "steamAchievementId": "",
        ///     "id": "ach01",
        ///     "appleEnabled": false,
        ///     "title": "Finish Tutorial",
        ///     "fbGamePoints": 10,
        ///     "description": "Achievement awarded when you finish the tutorial",
        ///     "appleAchievementId": ""
        ///    },
        ///    {
        ///     "fbEnabled": true,
        ///     "imageUrl": null,
        ///     "status": "NOT_AWARDED",
        ///     "gameId": "10068",
        ///     "steamEnabled": false,
        ///     "extraData": null,
        ///     "achievementId": "ach02",
        ///     "invisibleUntilEarned": false,
        ///     "steamAchievementId": "",
        ///     "id": "ach02",
        ///     "appleEnabled": false,
        ///     "title": "Level up",
        ///     "fbGamePoints": 10,
        ///     "description": "Awarded when you level up for the first time!",
        ///     "appleAchievementId": ""
        ///    }
        ///   ],
        ///   "quests": [],
        ///   "xp": {
        ///    "xpCapped": false,
        ///    "experiencePoints": 0,
        ///    "xpLevel": {
        ///     "gameId": "10068",
        ///     "level": 0,
        ///     "statusTitle": "Lesser",
        ///     "experience": 0,
        ///     "fbAction": ""
        ///    },
        ///    "experienceLevel": 0
        ///   }
        ///  }
        /// }
        /// </returns>
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
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///  "status": 200,
        ///  "data": {
        ///   "xp_levels": [
        ///    {
        ///     "level": 1,
        ///     "statusTitle": "Peon",
        ///     "experience": 0,
        ///     "fbAction": ""
        ///    },
        ///    {
        ///     "level": 2,
        ///     "statusTitle": "Small Fry",
        ///     "experience": 1000,
        ///     "fbAction": "",
        ///     "reward": {
        ///      "currency": {
        ///       "gold": 500
        ///      }
        ///     }
        ///    }
        ///   ]
        ///  }
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
            Dictionary<string, object> data = new Dictionary<string, object>();
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
            Dictionary<string, object> data = new Dictionary<string, object>();
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
            Dictionary<string, object> data = new Dictionary<string, object>();
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

            string[] achievementData = JsonReader.Deserialize<string[]>(achievementsStr);

            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceAchievementsName.Value] = achievementData;

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
            Dictionary<string, object> response = JsonReader.Deserialize<Dictionary<string, object>> (in_data);
            try
            {
                Dictionary<string, object> data = (Dictionary<string, object>) response[OperationParam.GamificationServiceAchievementsData.Value];
                //List<string> achievements = (List<string>) data[OperationParam.GamificationServiceAchievementsName.Value];
                if (((object[])data[OperationParam.GamificationServiceAchievementsName.Value]).Length > 0)
                {
                    Dictionary<string, object>[] achievements = (Dictionary<string, object>[])data[OperationParam.GamificationServiceAchievementsName.Value];
                    for (int i=0;i<achievements.Length;i++)
                    {
                        AwardThirdPartyAchievements(achievements[i]["id"].ToString());
                    }
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
            Dictionary<string, object> response = JsonReader.Deserialize<Dictionary<string, object>> (in_data);
            try
            {
                Dictionary<string, object> data = (Dictionary<string, object>) response[OperationParam.GamificationServiceAchievementsData.Value];
                List<string> achievements = (List<string>) data[OperationParam.GamificationServiceAchievementsGranted.Value];
                if (achievements != null)
                {
                    foreach (string achievement in achievements)
                    {
                        AwardThirdPartyAchievements(achievement);
                    }
                }

                //Let the Game Side knows about those Achievements.
                if (m_achievementsDelegate != null)
                {
                    m_achievementsDelegate(JsonWriter.Serialize(achievements), null);
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
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceMilestones.Value] = in_milestoneIds;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Gamification, ServiceOperation.ResetMilestones, data, callback);
            m_brainCloudClientRef.SendRequest(sc);

        }

    }
}
