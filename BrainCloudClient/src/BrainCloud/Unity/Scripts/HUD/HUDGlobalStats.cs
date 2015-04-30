using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

namespace BrainCloudUnity.HUD
{
	public class HUDGlobalStats : IHUDElement
	{
		class GlobalStatistic
		{
			public string name;
			public long value;
			public string increment = "0";
		}
		IDictionary<string, GlobalStatistic> m_stats = new Dictionary<string, GlobalStatistic>();
		Vector2 m_scrollPosition = new Vector2(0,0);
		
		
		public void OnHUDActivate()
		{
			RetrieveGlobalStats();
		}
		
		public void OnHUDDeactivate()
		{}
		
		public string GetHUDTitle()
		{
			return "Global Stats";
		}

		void RetrieveGlobalStats()
		{
			m_stats.Clear ();

			BrainCloudWrapper.GetBC ().GlobalStatisticsService.ReadAllGlobalStats(
				ReadGlobalStatsSuccess, ReadGlobalStatsFailure);
		}
		
		void ReadGlobalStatsSuccess(string json, object cb)
		{
			JsonData jObj = JsonMapper.ToObject(json);
			JsonData jStats = jObj["data"]["statistics"];
			IDictionary dStats = jStats as IDictionary;
			if (dStats != null)
			{
				foreach (string key in dStats.Keys)
				{
					GlobalStatistic stat = new GlobalStatistic();
					stat.name = (string) key;
					JsonData value = (JsonData) dStats[key];
					
					// silly that LitJson can't upcast an int to a long...
					stat.value = value.IsInt ? (int) value : (long) value;
					
					m_stats[stat.name] = stat;
				}
			}
		}
		
		void ReadGlobalStatsFailure(string json, object cb)
		{
			Debug.LogError("Failed to read global statistics: " + json);
		}

		public void OnHUDDraw()
		{
			m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			
			foreach (GlobalStatistic ps in m_stats.Values)
			{
				GUILayout.BeginVertical();
				GUILayout.Space(5);
				GUILayout.EndVertical();
				
				GUILayout.BeginHorizontal();
				GUILayout.Label(ps.name, GUILayout.MinWidth(125));
				GUILayout.Box(ps.value.ToString());
				GUILayout.EndHorizontal();
				
				// increment
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				ps.increment = GUILayout.TextField(ps.increment, GUILayout.MinWidth(45));
				
				if (GUILayout.Button("Increment"))
				{
					long valueAsLong = 0;
					double valueAsDouble = 0;
					if (long.TryParse(ps.increment, out valueAsLong)
					    || double.TryParse(ps.increment, out valueAsDouble))
					{
						BrainCloudWrapper.GetBC().GlobalStatisticsService.IncrementGlobalStats(
							"{ '" + ps.name +"':" + ps.increment +"}",
							ReadGlobalStatsSuccess, ReadGlobalStatsFailure);
					}
				}
				GUILayout.EndHorizontal();
			}
			
			GUILayout.EndScrollView();
		}
		
	}
}
