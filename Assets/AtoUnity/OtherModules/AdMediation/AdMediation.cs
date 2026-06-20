using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
    public class AdMediation : HandlerManager<AdMediation, IAdMediationHandler, DefaultAdMediationHandler>
    {
        #region IAdMediationHandler
        public static void Init(Action onCompletedInit = null)
        {
            AdsEventExecutor.Initialize();
            CurrentHandler.Init(onCompletedInit);
        }
        public static void ShowTestSuite()
        {
            CurrentHandler.ShowTestSuite();
        }
        public static bool IsRewardVideoAvailable()
        {
            return CurrentHandler.IsRewardVideoAvailable();
        }
        public static void ShowRewardVideo(Action<string, AdInfo> onCompleted = null, Action<string, AdInfo> onFailed = null)
        {
            CurrentHandler.ShowRewardVideo(onCompleted, onFailed);
        }
        public static void LoadRewardVideo()
        {
            CurrentHandler.LoadRewardVideo();
        }
        public static bool IsInterstitialAvailable()
        {
            return CurrentHandler.IsInterstitialAvailable();
        }
        public static void ShowInterstitial(Action<string, AdInfo> onCompleted = null, Action<string, AdInfo> onFailed = null)
        {
            CurrentHandler.ShowInterstitial(onCompleted, onFailed);
        }
        public static void LoadInterstitial()
        {
            CurrentHandler.LoadInterstitial();
        }
        public static void ShowBanner(Action<string, AdInfo> onCompleted = null, Action<string, AdInfo> onFailed = null)
        {
            CurrentHandler.ShowBanner(onCompleted, onFailed);
        }
        public static void ReloadBanner()
        {
            CurrentHandler.ReloadBanner();
        }

        public static void DestroyBanner()
        {
            CurrentHandler.DestroyBanner();
        }
        public static void DisplayBanner()
        {
            CurrentHandler.DisplayBanner();
        }
        public static void HideBanner()
        {
            CurrentHandler.HideBanner();
        }
        #endregion

        public static Action<ImpressionData> onAdRevenuePaidEvent;
        // video reward
        public static Action<AdInfo> onVideoRewardLoadedEvent;
        public static Action<AdInfo> onVideoRewardLoadFailedEvent;
        public static Action<AdInfo> onVideoRewardDisplayedEvent;
        public static Action<string, AdInfo> onVideoRewardCompletedEvent;
        public static Action<string, AdInfo> onVideoRewardFailedEvent;
        public static Action<string, AdInfo> onVideoRewardClicked;
        // interstitial
        public static Action<AdInfo> onInterstitialLoadedEvent;
        public static Action<string> onInterstitialLoadFailed;
        public static Action<AdInfo> onInterstitialDisplayedEvent;
        public static Action<string, AdInfo> onInterstitialCompletedEvent;
        public static Action<string, AdInfo> onInterstitialFailedEvent;
        public static Action<AdInfo> onInterstitiaClicked;
        // banner
        public static Action<string, AdInfo> onBannerLoadedEvent;
        public static Action<string> onBannerFailedEvent;
        public static Action onBannerFullOpenedEvent;
        public static Action onBannerFullClosedEvent;
        public static Action<AdInfo> onBannerClicked;
        // app open ad
        public static Action<AdInfo> onAppOpenLoadedEvent;
        public static Action<string> onAppOpenLoadFailed;
        public static Action<AdInfo> onAppOpenOpenedEvent;
        public static Action<string, AdInfo> onAppOpenFailedEvent;
        public static Action<string, AdInfo> onAppOpenCompletedEvent;
        public static Action<AdInfo> onAppOpenClicked;

        //

        #region Extend Params
        private static Dictionary<string, object> ExtendParams = new Dictionary<string, object>();

        public static void AddExtendParams(string key, object value)
        {
            if(ExtendParams.ContainsKey(key))
            {
                ExtendParams[key] = value;
            }
            else
            {
                ExtendParams.Add(key, value);
            }
        }

        public static int GetExtendParams(string key, int defaultValue)
        {
            if(ExtendParams.ContainsKey(key))
            {
                object value;
                AdMediation.ExtendParams.TryGetValue(key, out value);
                if(value == null)
                {
                    return defaultValue;
                }
                if(value is int i)
                    return i;
                if(int.TryParse(value.ToString(), out int result))
                {
                    return result;
                }
                return defaultValue;
            }
            else
            {
                return defaultValue;
            }
        }

        public static float GetExtendParams(string key, float defaultValue)
        {
            if(ExtendParams.ContainsKey(key))
            {
                object value;
                AdMediation.ExtendParams.TryGetValue(key, out value);
                if(value == null)
                {
                    return defaultValue;
                }
                if(value is float i)
                    return i;
                if(float.TryParse(value.ToString(), out float result))
                {
                    return result;
                }
                return defaultValue;
            }
            else
            {
                return defaultValue;
            }
        }

        public static bool GetExtendParams(string key, bool defaultValue)
        {
            if(ExtendParams.ContainsKey(key))
            {
                object value;
                AdMediation.ExtendParams.TryGetValue(key, out value);
                if(value == null)
                {
                    return defaultValue;
                }
                if(value is bool i)
                    return i;
                if(bool.TryParse(value.ToString(), out bool result))
                {
                    return result;
                }
                return defaultValue;
            }
            else
            {
                return defaultValue;
            }
        }

        public static string GetExtendParams(string key, string defaultValue)
        {
            if(ExtendParams.ContainsKey(key))
            {
                object value;
                AdMediation.ExtendParams.TryGetValue(key, out value);
                if(value == null)
                {
                    return defaultValue;
                }
                if(value is string i)
                    return i;
               
                return value.ToString();
            }
            else
            {
                return defaultValue;
            }
        }
        #endregion Extend Params
    }
}
