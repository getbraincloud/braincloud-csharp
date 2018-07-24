using UnityEngine;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudUnity.HUD
{
	public class HUDPlayer : IHUDElement
	{
		SortedDictionary<string, string> m_attributes = new SortedDictionary<string, string>();
		Vector2 m_scrollPosition = new Vector2(0,0);

		public void OnHUDActivate()
		{
			BrainCloudLoginPF.BrainCloud.PlayerStateService.GetAttributes (GetAttributesSuccess, Failure);
		}
		
		public void OnHUDDeactivate()
		{

		}
		
		public string GetHUDTitle()
		{
			return "Player";
		}

		void GetAttributesSuccess(string json, object cb)
		{
			m_attributes.Clear ();

            Dictionary<string, object> jObj = JsonReader.Deserialize<Dictionary<string, object>>(json);
            Dictionary<string, object> data = (Dictionary<string, object>)jObj["data"];
            Dictionary<string, object> stats = (Dictionary<string, object>)data["attributes"];
            
			if (stats != null)
			{
				foreach (string key in stats.Keys)
				{
					string name = key;
					string value = (string)stats[key];
					m_attributes[name] = value;
				}
			}
		}

		void ResetPlayerSuccess(string json, object cb)
		{
			// probably need to refresh game state... todo add a callback handler
		}

		void DeletePlayerSuccess(string json, object cb)
		{
			// definitely need to refresh game state... todo add a callback handler
		}
		
		void Failure(int statusCode, int reasonCode, string statusMessage, object cb)
		{
			Debug.LogError("Failed: " + statusMessage);
		}
		
		public void OnHUDDraw()
		{
			m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			foreach (string key in m_attributes.Keys)
			{
				GUILayout.Label(key);
			}
			GUILayout.EndVertical();
			GUILayout.BeginVertical();
			foreach (string value in m_attributes.Values)
			{
				GUILayout.Box(value);
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();

			//spacer
			GUILayout.BeginVertical();
			GUILayout.Space(5);
			GUILayout.EndVertical();

			GUILayout.TextArea ("Reseting your player will delete all player data but will keep identities intact.");
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button ("Reset Player"))
			{
				BrainCloudLoginPF.BrainCloud.PlayerStateService.ResetUser (ResetPlayerSuccess, Failure);
			}
			GUILayout.EndHorizontal ();

			GUILayout.TextArea ("Deleting your player will delete the player entirely. Player will need to reauthenticate and create new account");
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button ("Delete Player"))
			{
				BrainCloudLoginPF.BrainCloud.PlayerStateService.DeleteUser (DeletePlayerSuccess, Failure);
			}
			GUILayout.EndHorizontal ();

			GUILayout.EndScrollView();

			GUILayout.EndVertical ();
		}
		
	}
}