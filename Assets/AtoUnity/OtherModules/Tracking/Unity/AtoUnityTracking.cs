using AtoGame.AtoUnity;
using System.Collections;
using System.Collections.Generic;
#if UNITY_TRACKING_ENABLE
using Unity.Services.Analytics;
#endif
using UnityEngine;

namespace AtoGame.Tracking.AtoUntiy
{
    public class AtoUnityTracking : Singleton<AtoUnityTracking>
    {
        private bool available;
        private event System.Action<bool> callOnAvailable;

        protected override void Initialize()
        {
            base.Initialize();

            if(Initialized)
                return;

            AtoUnityInitializer.AddOnInitialized(OnInitialized);
            available = false;
            return;
        }

        private void OnInitialized(bool result, string errorMessage)
        {
            if(result == true)
            {
                OnAvailable();
            }
            else
            {
                TrackingLogger.Log($"[Unity-Analytics] Unity Initialize failed: " + errorMessage);
            }
        }

        private void OnAvailable()
        {
#if UNITY_TRACKING_ENABLE
            if(available)
                return;

            TrackingLogger.Log("[Unity-Analytics] OnAvailable");
            Unity.Services.Analytics.AnalyticsService.Instance.StartDataCollection();
            TrackingLogger.Log("[Unity-Analytics] OnAvailable - After set StartDataCollection");

            if(callOnAvailable != null)
            {
                TrackingLogger.Log("[Unity-Analytics] OnAvailable - callOnAvailable");
                callOnAvailable.Invoke(false);
                TrackingLogger.Log("[Unity-Analytics] OnAvailable - after callOnAvailable");
                callOnAvailable = null;
            }
            available = true;
#endif
        }

        public void LogEvent(string eventName, ParameterBuilder parameterBuilder, bool forePush = false)
        {
#if UNITY_TRACKING_ENABLE
            this.LogEvent(eventName, parameterBuilder != null ? parameterBuilder.BuildUnityEvent(eventName) : null);
            DebugLog(eventName, parameterBuilder);

          
#else
            TrackingLogger.Log("Unity-Analytics flag has't defined");
#endif


        }

        private void DebugLog(string eventName, ParameterBuilder parameterBuilder)
        {
            string log = $"[Unity-Analytics]: EventName = " + eventName + " ";
            if(parameterBuilder != null)
            {
                log += parameterBuilder.DebugLog();
            }
            TrackingLogger.Log(log);
        }

#if UNITY_TRACKING_ENABLE
        private void LogEvent(string eventName, Unity.Services.Analytics.Event unityEvent, bool forcePush = false)
        {
            Push((bool isPush) =>
            {
                if(unityEvent == null)
                    Unity.Services.Analytics.AnalyticsService.Instance.RecordEvent(eventName);
                else
                    Unity.Services.Analytics.AnalyticsService.Instance.RecordEvent(unityEvent);

                if(forcePush)
                {
                    Unity.Services.Analytics.AnalyticsService.Instance.Flush();
                }
            });
        }
#endif

        private void Push(System.Action<bool> action)
        {
            try
            {
                if(available)
                    action.Invoke(true);
                else
                    callOnAvailable += action;
            }
            catch { throw; }
        }
    }
}
