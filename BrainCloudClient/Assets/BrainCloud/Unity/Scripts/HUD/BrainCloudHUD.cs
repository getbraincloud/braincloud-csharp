using UnityEngine;
using System.Collections.Generic;

namespace BrainCloudUnity.HUD
{
    public class BrainCloudHUD : MonoBehaviour
    {
        protected int m_activeScreen;
        protected List<IHUDElement> m_screens;
        protected string[] m_screenNames;

        protected bool m_enableUI = true;
        public bool EnableUI
        {
            get { return m_enableUI; }
            set
            {
                if (!m_enableUI && value)
                {
                    m_screens[m_activeScreen].OnHUDActivate();
                }
                else if (m_enableUI && !value)
                {
                    m_screens[m_activeScreen].OnHUDDeactivate();
                }
                m_enableUI = value;
            }
        }

        protected bool m_minimzed = true;
        public bool Minimized
        {
            get { return m_minimzed; }
            set
            {
                if (!m_minimzed && value)
                {
                    m_screens[m_activeScreen].OnHUDActivate();
                }
                else if (m_minimzed && !value)
                {
                    m_screens[m_activeScreen].OnHUDDeactivate();
                }
                m_minimzed = value;
            }
        }

        void Update()
        {
        }

        void Start()
        {
            m_screens = new List<IHUDElement>();
            m_screens.Add(new HUDInfo());
            m_screens.Add(new HUDPlayer());
            m_screens.Add(new HUDPlayerStats());
            m_screens.Add(new HUDGlobalStats());
            m_screens.Add(new HUDLeaderboard());
            m_activeScreen = 0;

            m_screenNames = new string[m_screens.Count];
            for (int i = 0, ilen = m_screens.Count; i < ilen; ++i)
            {
                m_screenNames[i] = m_screens[i].GetHUDTitle();
            }
        }

        void OnGUI()
        {
            if (!EnableUI)
            {
                return;
            }
            if (Minimized)
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                /** // Debug menu appears to have an issue in the current verion of Unity. Look into adjusting
				if (GUILayout.Button ("bC Debug"))
				{
					Minimized = false;
				}
				*/
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.Window(0, new Rect(0, 0, 350, Screen.height), OnWindow, m_screens[m_activeScreen].GetHUDTitle());
            }
        }

        void OnWindow(int id)
        {
            m_screens[m_activeScreen].OnHUDDraw();

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            int newSelection = GUILayout.SelectionGrid(m_activeScreen, m_screenNames, 3);
            if (newSelection != m_activeScreen)
            {
                m_screens[m_activeScreen].OnHUDDeactivate();
                m_activeScreen = newSelection;
                m_screens[m_activeScreen].OnHUDActivate();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close"))
            {
                Minimized = true;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }
}
