using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

namespace BrainCloudUnity.HUD
{
	public class HUDPlayer : IHUDElement
	{
		SortedDictionary<string, string> m_attributes = new SortedDictionary<string, string>();
		Vector2 m_scrollPosition = new Vector2(0,0);

		public void OnHUDActivate()
		{
			BrainCloudWrapper.GetBC ().PlayerStateService.GetAttributes (GetAttributesSuccess, Failure);
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

			JsonData jObj = JsonMapper.ToObject(json);
			JsonData jStats = jObj["data"]["attributes"];
			IDictionary dStats = jStats as IDictionary;
			if (dStats != null)
			{
				foreach (string key in dStats.Keys)
				{
					string name = (string) key;
					string value = (string) dStats[key];
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
		
		void Failure(string json, object cb)
		{
			Debug.LogError("Failed: " + json);
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
				BrainCloudWrapper.GetBC ().PlayerStateService.ResetPlayer (ResetPlayerSuccess, Failure);
			}
			GUILayout.EndHorizontal ();

			GUILayout.TextArea ("Deleting your player will delete the player entirely. Player will need to reauthenticate and create new account");
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button ("Delete Player"))
			{
				BrainCloudWrapper.GetBC ().PlayerStateService.DeletePlayer (DeletePlayerSuccess, Failure);
			}
			GUILayout.EndHorizontal ();

			GUILayout.EndScrollView();

			GUILayout.EndVertical ();
		}
		
	}
}