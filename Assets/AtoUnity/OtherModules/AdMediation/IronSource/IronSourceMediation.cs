using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
#if ATO_IRONSOURCE_MEDIATION_ENABLE
    public class IronSourceMediation : SingletonFreeAlive<IronSourceMediation>, IAdMediationHandler
    {

        private ISVideoRewardAd rewardAd;
        private ISInterstitialAd interstitialAd;
        private ISBannerAd bannerAd;

        private bool isInitialized;

        protected override void OnAwake()
        {
            base.OnAwake();
            AdMediation.SetHandler(this);
        }

        public void Init()
        {
            rewardAd = new ISVideoRewardAd();
            interstitialAd = new ISInterstitialAd();
            bannerAd = new ISBannerAd();

            if (AdIronSourceSettings.Instance.UseTestSuite)
            {
                IronSource.Agent.setMetaData("is_test_suite", "enable");
            }
            isInitialized = false;
            IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
            IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;

            bool manualValidateIntegration = true;
            bool manualSDKInitAPI = true;

            var developerSettings = Resources.Load<IronSourceMediationSettings>(IronSourceConstants.IRONSOURCE_MEDIATION_SETTING_NAME);
            if(developerSettings != null)
            {
                manualValidateIntegration = !developerSettings.EnableIntegrationHelper;
                manualSDKInitAPI = !developerSettings.EnableIronsourceSDKInitAPI;
            }

            Debug.Log("[AdMediation-IronSourceMediation]: unity version" + IronSource.unityVersion());
            if (manualValidateIntegration)
            {
                Debug.Log("[AdMediation-IronSourceMediation]: IronSource.Agent.validateIntegration");
                IronSource.Agent.validateIntegration();
            }

            //IronSource.Agent.setConsent(AdIronSourceSettings.Instance.HasUserConsent);
            IronSource.Agent.setMetaData("is_child_directed", AdIronSourceSettings.Instance.IsAgeRestrictedUser.ToString());
            IronSource.Agent.setMetaData("do_not_sell", AdIronSourceSettings.Instance.DoNotSell.ToString());
            if(AdIronSourceSettings.Instance.UseFacebookAd)
            {
                if (AdIronSourceSettings.Instance.IsLimitedDataUse)
                {
                    AudienceNetwork.AdSettings.SetDataProcessingOptions(new string[] { "LDU" }, 1, 1000);
                }
                else
                {
                    AudienceNetwork.AdSettings.SetDataProcessingOptions(new string[] { });
                }
            }

            if (manualSDKInitAPI)
            {
                // SDK init
                Debug.Log("[AdMediation-IronSourceMediation]: IronSource.Agent.init");
                bool defaultInit = false;
                IronSourceAdUnit adUnitType = AdIronSourceSettings.Instance.AdUnitType;
                if (adUnitType == IronSourceAdUnit.DEFAULT)
                {
                    defaultInit = true;
                }
                else
                {
                    List<string> adUnits = new List<string>();
                    if (adUnitType.HasFlag(IronSourceAdUnit.REWARDED_VIDEO))
                    {
                        adUnits.Add(IronSourceAdUnits.REWARDED_VIDEO);
                    }
                    if (adUnitType.HasFlag(IronSourceAdUnit.INTERSTITIAL))
                    {
                        adUnits.Add(IronSourceAdUnits.INTERSTITIAL);
                    }
                    if (adUnitType.HasFlag(IronSourceAdUnit.OFFERWALL))
                    {
                        adUnits.Add(IronSourceAdUnits.OFFERWALL);
                    }
                    if (adUnitType.HasFlag(IronSourceAdUnit.BANNER))
                    {
                        adUnits.Add(IronSourceAdUnits.BANNER);
                    }
                    if (adUnits.Count == 0)
                    {
                        defaultInit = true;
                    }
                    else
                    {
                        defaultInit = false;
                        IronSource.Agent.init(AdIronSourceSettings.Instance.AppKey, adUnits.ToArray());
                    }
                }
                if (defaultInit)
                {
                    IronSource.Agent.init(AdIronSourceSettings.Instance.AppKey);
                }
            }
           
        }

        void SdkInitializationCompletedEvent()
        {
            isInitialized = true;
          
            Debug.Log("[AdMediation-IronSourceMediation]: I got SdkInitializationCompletedEvent");
            IronSourceAdUnit adUnitType = AdIronSourceSettings.Instance.AdUnitType;
            if(adUnitType.HasFlag(IronSourceAdUnit.REWARDED_VIDEO))
            {
                LoadRewardVideo();
            }
            if(adUnitType.HasFlag(IronSourceAdUnit.INTERSTITIAL))
            {
                LoadInterstitial();
            }
            if(adUnitType.HasFlag(IronSourceAdUnit.OFFERWALL))
            {
            }
            if(adUnitType.HasFlag(IronSourceAdUnit.BANNER))
            {
            }
           
       
          
        }

        public void ShowTestSuite()
        {
            if(AdIronSourceSettings.Instance.UseTestSuite == false)
            {
                return;
            }
            if(isInitialized == false)
            {
                return;
            }
            IronSource.Agent.launchTestSuite();
        }

        void OnApplicationPause(bool isPaused)
        {
            IronSource.Agent.onApplicationPause(isPaused);
        }

    #region Video Reward
        public bool IsRewardVideoAvailable()
        {
            return rewardAd != null && rewardAd.IsAvailable;
        }

        public void ShowRewardVideo(Action<string, AdInfo> onCompleted = null, Action<string, AdInfo> onFailed = null)
        {
            if(IsRewardVideoAvailable())
            {
                rewardAd.Show(onCompleted, onFailed);
            }
            else
            {
                onFailed?.Invoke("Reward Ad not available", null);
            }
        }

        public void LoadRewardVideo()
        {
            rewardAd?.Request();
        }
    #endregion

    #region Interstitial
        public bool IsInterstitialAvailable()
        {
            return interstitialAd != null && interstitialAd.IsAvailable;
        }

        public void ShowInterstitial(Action<string, AdInfo> onCompleted = null, Action<string, AdInfo> onFailed = null)
        {
            if (IsInterstitialAvailable())
            {
                interstitialAd.Show(onCompleted, onFailed);
            }
            else
            {
                onFailed?.Invoke("Interstitial Ad not available", null);
            }
        }

        public void LoadInterstitial()
        {
            interstitialAd?.Request();
        }
    #endregion

    #region Banner
        public void ShowBanner(Action<string, AdInfo> onCompleted = null, Action<string, AdInfo> onFailed = null)
        {
            if(bannerAd != null)
            {
                bannerAd.SetConfig(AdIronSourceSettings.Instance.DefaultBannerSize, AdIronSourceSettings.Instance.DefaultBannerPosition);
                bannerAd.Show(onCompleted, onFailed);
            }
            else
            {
                onFailed?.Invoke("bannerAd null", null);
            }
        }

        public void ShowBanner(BannerPosition position, bool isAdaptive, BannerSize size, int width, int height, Action<string, AdInfo> onCompleted = null, Action<string, AdInfo> onFailed = null)
        {
            IronSourceBannerPosition isPosition = IronSourceHelper.GetBannerPosition(position);
            IronSourceBannerSize isBannerSize = IronSourceHelper.GetBannerSize(isAdaptive, size, width, height);
            if (bannerAd != null)
            {
                bannerAd.SetConfig(isBannerSize, isPosition);
                bannerAd.Show(onCompleted, onFailed);
            }
            else
            {
                onFailed?.Invoke("bannerAd null", null);
            }
        }

        public void DestroyBanner()
        {
            if (bannerAd != null)
            {
                bannerAd.DestroyBanner();
            }
        }

        public void HideBanner()
        {
            if (bannerAd != null)
            {
                bannerAd.HideBanner();
            }
        }

        public void DisplayBanner()
        {
            if (bannerAd != null)
            {
                bannerAd.DisplayBanner();
            }
        }
    #endregion

        private void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
        {
            //AdMediation.onAdRevenuePaidEvent?.Invoke(impressionData.Convert());
        }
    }

    [System.Flags]
    public enum IronSourceAdUnit
    {
        /// <summary>
        /// Init the ad units you’ve configured on the ironSource platform
        /// </summary>
        DEFAULT = 0,

        /// <summary>
        /// 
        /// </summary>
        REWARDED_VIDEO = 1,

        /// <summary>
        /// 
        /// </summary>
        INTERSTITIAL = 2,

        /// <summary>
        ///
        /// </remarks>
        OFFERWALL = 4,

        /// <summary>
        /// 
        /// </summary>
        BANNER = 8,
    }
#endif
}