
namespace BrainCloudUnity.HUD
{

using UnityEngine;
using System.Collections;

    public interface IHUDElement
    {
        string GetHUDTitle();
        void OnHUDActivate();
        void OnHUDDraw();
        void OnHUDDeactivate();
    }
}