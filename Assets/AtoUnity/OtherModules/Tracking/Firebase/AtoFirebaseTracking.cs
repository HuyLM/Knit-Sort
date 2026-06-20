using AtoGame.AtoFirebase;
using AtoGame.Base;
using AtoGame.Mediation;

#if FIREBASE_ENABLE
using Firebase;
using Firebase.Analytics;
using Firebase.Crashlytics;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AtoGame.Tracking.FB
{
    public class AtoFirebaseTracking : Singleton<AtoFirebaseTracking>
    {
        private bool available;
        private event System.Action<bool> callOnAvailable;


        protected override void Initialize()
        {
            base.Initialize();

            if (Initialized)
                return;

            AtoFirebaseInitializer.AddOnFirebaseInitialized(OnFirebaseInitialized);
            available = false;
            return;
        }

        private void OnFirebaseInitialized(bool result, string errorMessage)
        {
            if(result == true)
            {
                OnFirebaseAvailable();
            }
            else
            {
                TrackingLogger.Log($"[FIREBASE-Analytics] Firebase Initialize failed: " + errorMessage);
            }
        }

        private void OnFirebaseAvailable()
        {
#if FIREBASE_ENABLE
            if (available) return;
            // Create and hold a reference to your FirebaseApp,
            // where app is a Firebase.FirebaseApp property of your application class.
            // Crashlytics will use the DefaultInstance, as well;
            // this ensures that Crashlytics is initialized.
            Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;

            // When this property is set to true, Crashlytics will report all
            // uncaught exceptions as fatal events. This is the recommended behavior.
            Crashlytics.ReportUncaughtExceptionsAsFatal = true;

            TrackingLogger.Log("[FIREBASE] OnFirebaseAvailable");
            Firebase.Analytics.FirebaseAnalytics.SetUserProperty(Firebase.Analytics.FirebaseAnalytics.UserPropertyAllowAdPersonalizationSignals, "true");
            Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            TrackingLogger.Log("[FIREBASE] OnFirebaseAvailable - After set analyticscollectionenable true");

            if (callOnAvailable != null)
            {
                TrackingLogger.Log("[FIREBASE] OnFirebaseAvailable - callOnAvailable");
                callOnAvailable.Invoke(false);
                TrackingLogger.Log("[FIREBASE] OnFirebaseAvailable - after callOnAvailable");
                callOnAvailable = null;
            }
            available = true;
#endif
        }
#if FIREBASE_ENABLE
        private void LogEvent(string eventName, params Parameter[] para)
        {
            Push((bool isPush) =>
            {
                if (para == null)
                    FirebaseAnalytics.LogEvent(eventName);
                else
                    FirebaseAnalytics.LogEvent(eventName, para);
            });
        }
#endif
    public void LogEvent(string eventName, ParameterBuilder parameterBuilder)
        {
#if FIREBASE_ENABLE
            this.LogEvent(eventName, parameterBuilder != null ? parameterBuilder.BuildFirebase() : null);
            DebugLog(eventName, parameterBuilder);
#else
            TrackingLogger.Log("FIREBASE_ENABLE flag has't defined");
#endif
        }

        private void DebugLog(string eventName, ParameterBuilder parameterBuilder)
        {
            string log = $"[FIREBASE-Analytics]:" + " EventName = " + eventName + " ";
            if(parameterBuilder != null)
            {
                log += parameterBuilder.DebugLog();
            }
            TrackingLogger.Log(log);
        }

        public void LogAdRevenue(ParameterBuilder parameterBuilder)
        {
#if FIREBASE_ENABLE
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", parameterBuilder.BuildFirebase());
#endif
        }


        public void SetUserProperty(string name, string property)
        {
#if FIREBASE_ENABLE
            Firebase.Analytics.FirebaseAnalytics.SetUserProperty(name, property);
#endif
        }
        private void Push(System.Action<bool> action)
        {
            try
            {
                if (available)
                    action.Invoke(true);
                else
                    callOnAvailable += action;
            }
            catch { throw; }
        }
    }
}
