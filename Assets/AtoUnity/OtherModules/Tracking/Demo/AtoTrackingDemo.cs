using AtoGame.Tracking.Appsflyer;
using AtoGame.Tracking.FB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Tracking
{
    public static class AtoTrackingDemo 
    {
        public static void Preload()
        {
            AtoAppsflyerTracking.Instance.Preload();
            AtoFirebaseTracking.Instance.Preload();
        }

        private static void LogEvent(string eventName, ParameterBuilder parameterBuilder)
        {
            LogAppsflyer(eventName, parameterBuilder);
            LogFirebase(eventName, parameterBuilder);
        }

        private static void LogAppsflyer(string eventName, ParameterBuilder parameterBuilder)
        {
            AtoAppsflyerTracking.Instance.LogEvent(eventName, parameterBuilder);
        }

        private static void LogFirebase(string eventName, ParameterBuilder parameterBuilder)
        {
            AtoFirebaseTracking.Instance.LogEvent(eventName, parameterBuilder);
        }

        public static void LogLogin(string loginType)
        {
            ParameterBuilder parameter = ParameterBuilder.Create().Add("method", loginType);
            LogEvent("login", parameter);
        }
    }
}
