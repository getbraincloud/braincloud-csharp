using UnityEngine;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudUnity.HUD
{
    public class HUDLeaderboard : IHUDElement
    {
        class LBEntry
        {
            public string playerId;
            public string name;
            public long rank;
            public long score;
        }
        List<LBEntry> m_lb = new List<LBEntry>();
        string m_lbId = "default";
        string m_score = "1000";
        bool m_showPlayerIds = false;
        Vector2 m_scrollPosition = new Vector2(0, 0);

        public void OnHUDActivate()
        { }

        public void OnHUDDeactivate()
        { }

        public string GetHUDTitle()
        {
            return "Leaderboard";
        }

        void RetrieveLeaderboard(string leaderboardId)
        {
            m_lb.Clear();

            BrainCloudLoginPF.BrainCloud.SocialLeaderboardService.GetGlobalLeaderboardPage(
                leaderboardId, BrainCloud.BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW, 0, 100,
                ReadLeaderboardSuccess, ReadLeaderboardFailure);
        }

        void PostScore(string lbId, long score)
        {
            BrainCloudLoginPF.BrainCloud.SocialLeaderboardService.PostScoreToLeaderboard(
                lbId, score, null, PostScoreSuccess, PostScoreFailure);
        }

        void PostScoreSuccess(string json, object cb)
        {
            Debug.Log("Posted score successfully... refetching new scores: " + json);

            RetrieveLeaderboard(m_lbId);
        }

        void PostScoreFailure(int statusCode, int reasonCode, string statusMessage, object cb)
        {
            Debug.LogError("Failed to post to leaderboard: " + statusMessage);
        }

        void ReadLeaderboardSuccess(string json, object cb)
        {
            Debug.Log("Leaderboard json: " + json);

            Dictionary<string, object> jObj = JsonReader.Deserialize<Dictionary<string, object>>(json);
            Dictionary<string, object> data = (Dictionary<string, object>)jObj["data"];
            List<object> entries = (List<object>)data["social_leaderboard"];

            if (entries != null)
            {
                Dictionary<string, object> jEntry = null;

                foreach (object entry in entries)
                {
                    jEntry = (Dictionary<string, object>)entry;
                    LBEntry lbe = new LBEntry();
                    lbe.playerId = (string)jEntry["playerId"];
                    lbe.name = (string)jEntry["name"];
                    lbe.rank = System.Convert.ToInt64(jEntry["rank"]);
                    lbe.score = System.Convert.ToInt64(jEntry["score"]);
                    
                    m_lb.Add(lbe);
                }
            }
        }

        void ReadLeaderboardFailure(int statusCode, int reasonCode, string statusMessage, object cb)
        {
            Debug.LogError("Failed to read leaderboard: " + statusMessage);
        }

        public void OnHUDDraw()
        {
            GUILayout.BeginVertical();
            GUILayout.Box("Leaderboard Operations");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Leaderboard Id:");
            m_lbId = GUILayout.TextField(m_lbId);
            if (GUILayout.Button("Fetch"))
            {
                RetrieveLeaderboard(m_lbId);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Score:");
            m_score = GUILayout.TextField(m_score, GUILayout.MinWidth(100));
            if (GUILayout.Button("Post"))
            {
                long scoreAsLong;
                if (long.TryParse(m_score, out scoreAsLong))
                {
                    PostScore(m_lbId, scoreAsLong);
                }
                else
                {
                    Debug.LogError("Can't parse score to long value");
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Box("Results");

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            m_showPlayerIds = GUILayout.Toggle(m_showPlayerIds, "Show Player Ids");
            GUILayout.EndHorizontal();

            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            foreach (LBEntry entry in m_lb)
            {
                string player;
                if (m_showPlayerIds)
                {
                    player = entry.playerId;
                }
                else
                {
                    player = entry.name == "" ? "(no name)" : entry.name;
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label(entry.rank.ToString() + ":");
                GUILayout.Label(player);
                GUILayout.FlexibleSpace();
                GUILayout.Label(entry.score.ToString());
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

    }
}
