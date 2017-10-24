using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BrainCloud;

namespace BrainCloudUnity.HUD
{
	public class HUDInfo : IHUDElement
	{
		public void OnHUDActivate()
		{}

		public void OnHUDDeactivate()
		{}

		public string GetHUDTitle()
		{
			return "brainCloud";
		}

		public void OnHUDDraw()
		{
			GUILayout.BeginVertical ();
			
			BrainCloudClient bcc = BrainCloudLoginPF.BrainCloud.Client;
			GUILayout.Box ("Connection Details");
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			GUILayout.Label ("Authenticated");
			GUILayout.Label ("Session");
			GUILayout.Label ("Profile Id");
			GUILayout.Label ("Anonymous Id");
			GUILayout.Label ("Game Id");
			GUILayout.Label ("Game Version");
			GUILayout.Label ("Client Version");
			GUILayout.Label ("Platform");
			GUILayout.EndVertical ();
			GUILayout.BeginVertical();
			GUILayout.Label (bcc.Authenticated ? "True" : "False");
			GUILayout.Label (bcc.SessionID);
			GUILayout.Label (bcc.AuthenticationService.ProfileId);
			GUILayout.Label (bcc.AuthenticationService.AnonymousId);
			GUILayout.Label (bcc.AppId);
			GUILayout.Label (bcc.AppVersion);
			GUILayout.Label (bcc.BrainCloudClientVersion);
			GUILayout.Label (bcc.ReleasePlatform.ToString());
			GUILayout.EndVertical ();
			GUILayout.EndHorizontal();

			GUILayout.Box ("Portal Links");
			GUILayout.BeginHorizontal ();
			//GUILayout.FlexibleSpace();
			if (GUILayout.Button ("Player Monitoring"))
			{
				Application.OpenURL(BrainCloudSettings.Instance.ServerURL +"/admin/dashboard#/monitoring/summary");
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.EndVertical ();
		}
	}
}