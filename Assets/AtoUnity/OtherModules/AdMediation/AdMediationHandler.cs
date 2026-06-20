using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
    public interface IAdMediationHandler
    {
        void Init(Action onCompletedInit);
        void ShowTestSuite();
        bool IsRewardVideoAvailable();
        void ShowRewardVideo(Action<string, AdInfo> onCompleted = null, Action<string, AdInfo> onFailed = null);
        void LoadRewardVideo();
        bool IsInterstitialAvailable();
        void ShowInterstitial(Action<string, AdInfo> onCompleted = null, Action<string, AdInfo> onFailed = null);
        void LoadInterstitial();
        void ReloadBanner();
        void ShowBanner(Action<string, AdInfo> onCompleted = null, Action<string, AdInfo> onFailed = null);
        void DestroyBanner();
        void HideBanner();
        void DisplayBanner();
    }

    public class DefaultAdMediationHandler : IAdMediationHandler
    {
        public void Init(Action onCompletedInit)
        {
        }
        public void ShowTestSuite()
        {
        }
        public bool IsRewardVideoAvailable()
        {
            return false;
        }
        public void ShowRewardVideo(Action<string, AdInfo> onCompleted = null, Action<string, AdInfo> onFailed = null)
        {
        }
        public void LoadRewardVideo()
        {
        }
        public bool IsInterstitialAvailable()
        {
            return false;
        }
        public void ShowInterstitial(Action<string, AdInfo> onCompleted = null, Action<string, AdInfo> onFailed = null)
        {
        }
        public void LoadInterstitial()
        {
        }
        public void ReloadBanner()
        {
        }
        public void ShowBanner(Action<string, AdInfo> onCompleted = null, Action<string, AdInfo> onFailed = null)
        {
        }

        public void DestroyBanner()
        {
        }

        public void DisplayBanner()
        {
        }

        public void HideBanner()
        {
        }
    }
}
