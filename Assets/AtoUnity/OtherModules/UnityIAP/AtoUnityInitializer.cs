using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.AtoUnity
{
    public class AtoUnityInitializer
    {
        private static event Action<bool, string> OnInitialized;
        private static bool isInitialized;

        public static async void Initialize()
        {
            isInitialized = false;
#if UNITY_IAP_ENABLE || UNITY_TRACKING_ENABLE
            try
            {
                await Unity.Services.Core.UnityServices.InitializeAsync();
                Debug.Log("Unity Services Initialized");
                OnInitialized?.Invoke(true, string.Empty);
                OnInitialized = null;
                isInitialized = true;
            }
            catch(System.Exception ex)
            {
                Debug.LogError($"Failed to initialize Unity Services: {ex.Message}");
                OnInitialized?.Invoke(false, ex.Message);
                OnInitialized = null;
                isInitialized = true;
            }
#endif
        }

        public static void AddOnInitialized(Action<bool, string> onInitialized)
        {
            if(isInitialized == true)
            {
                onInitialized?.Invoke(true, string.Empty);
            }
            else
            {
                OnInitialized += onInitialized;
            }
        }
    }
}
