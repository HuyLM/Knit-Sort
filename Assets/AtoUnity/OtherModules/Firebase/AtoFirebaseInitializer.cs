#if FIREBASE_ENABLE
using Firebase;
using Firebase.Extensions;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.AtoFirebase
{
    public class AtoFirebaseInitializer
    {
        private static event Action<bool, string> OnFirebaseInitialized;
        private static bool isInitialized; 


        public static void InitializeFirebase()
        {
            isInitialized = false;
#if FIREBASE_ENABLE
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if(task.Result == DependencyStatus.Available)
                {
                    // Firebase is initialized and ready to use.
                    Debug.Log("Firebase initialized successfully.");

                    // Invoke the event to notify listeners.
                    OnFirebaseInitialized?.Invoke(true, string.Empty);
                    OnFirebaseInitialized = null;
                    isInitialized = true;
                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
                    OnFirebaseInitialized?.Invoke(false, task.Result.ToString());
                    OnFirebaseInitialized = null;
                    isInitialized = true;
                }
            });
#endif
        }

        public static void AddOnFirebaseInitialized(Action<bool, string> onInitialized)
        {
            if(isInitialized == true)
            {
                onInitialized?.Invoke(true, string.Empty);
            }
            else
            {
                OnFirebaseInitialized += onInitialized;
            }
        }

    }
}
