using AtoGame.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AtoGame.OtherModules.HUD
{
    public class HUDManager : SingletonBind<HUDManager>
    {
        private class HUDComparer : IComparer<HUD>
        {
            public int Compare(HUD x, HUD y)
            {
                return x.Order.CompareTo(y.Order);
            }
        }

        private readonly List<HUD> huds = new List<HUD>();
        private static readonly HUDComparer comparer = new HUDComparer();
        private EventSystem eventSystem;

        private void Start()
        {
            eventSystem = EventSystem.current;
        }

        public void Add(HUD hud)
        {
            if (huds.Contains(hud))
            {
                return;
            }
            huds.Add(hud);
            huds.Sort(comparer);
        }

        public bool Remove(HUD hud)
        {
            if (Initialized)
            {
                return huds.Remove(hud);
            }
            return false;
        }

        public void IgnoreUserInput(bool ignore)
        {
            if (!eventSystem)
            {
                eventSystem = EventSystem.current;
            }
            if (eventSystem)
            {
                eventSystem.enabled = !ignore;
            }
            enabled = !ignore;
        }

        public void SetActiveAllHUD(bool active)
        {
            for(int i = 0; i < huds.Count; ++i)
            {
                huds[i].SetActive(active);
            }
        }

        private bool Back()
        {
            for (int i = huds.Count - 1; i >= 0; i--)
            {
                if (huds[i].Back())
                {
                    return true;
                }
            }
            return false;
        }

        public void Update()
        {
            if (Initialized == false)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Back();
            }
        }

        public Frame GetTopFrame()
        {
            for(int i = huds.Count - 1; i >= 0; i--)
            {
                var topFrame = huds[i].GetFrameOnTop();
                if(topFrame != null)
                {
                    if(topFrame.CanOnTop())
                    {
                        return topFrame;
                    }
                }
            }
            return null;
        }
    }
}
