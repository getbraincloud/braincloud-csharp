using UnityEngine;
using System.Collections;

namespace BrainCloudUnity.HUD
{
	public interface IHUDElement
	{
		string GetHUDTitle();
		void OnHUDActivate();
		void OnHUDDraw();
		void OnHUDDeactivate();
	}
}