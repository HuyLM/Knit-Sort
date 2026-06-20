
#if ATO_APPODEAL_MEDIATION_ENABLE
using AppodealStack.Monetization.Common;

using AtoGame.Mediation;
using System;
using UnityEngine;

public class AppodealMediation : SingletonFreeAlive<AppodealMediation>, IAdMediationHandler
{
    [SerializeField] private string sdkKey;
    [SerializeField] private AppodealAdUnit adUnitType = AppodealAdUnit.DEFAULT;


    private AppodealVideoRewardAd rewardAd;
    private AppodealInterstitialAd interstitialAd;
    private AppodealBannerAd bannerAd;
    private bool isInitialized;
    private Action onInitSuccess;

    protected override void OnAwake()
    {
        base.OnAwake();
        AdMediation.SetHandler(this);
    }

    public void Init(Action onCompletedInit)
    {
        this.onInitSuccess = onCompletedInit;
        isInitialized = false;

        AppodealCallbacks.AdRevenue.OnReceived += (sender, args) => {
            OnAdRevenueReceived(args.Ad);
        };


        int adTypes = 0;
        if(adUnitType.HasFlag(AppodealAdUnit.REWARDED_VIDEO))
        {
            adTypes = adTypes | AppodealAdType.RewardedVideo;
        }
        if(adUnitType.HasFlag(AppodealAdUnit.BANNER))
        {
            adTypes = adTypes | AppodealAdType.Banner;
        }
        if(adUnitType.HasFlag(AppodealAdUnit.INTERSTITIAL))
        {
            adTypes = adTypes | AppodealAdType.Interstitial;
        }
        string appKey = sdkKey;
        AppodealCallbacks.Sdk.OnInitialized += OnInitializationFinished;
        AppodealStack.Monetization.Api.Appodeal.SetLogLevel(AppodealLogLevel.Verbose);

        //AppodealStack.Monetization.Api.Appodeal.SetTesting(true);
        AppodealStack.Monetization.Api.Appodeal.Initialize(appKey, adTypes);
    }

    public void OnInitializationFinished(object sender, SdkInitializedEventArgs e) 
    {
        isInitialized = true;

        if(adUnitType.HasFlag(AppodealAdUnit.REWARDED_VIDEO))
        {
            rewardAd = new AppodealVideoRewardAd();
        }
        if(adUnitType.HasFlag(AppodealAdUnit.INTERSTITIAL))
        {
            interstitialAd = new AppodealInterstitialAd();
        }
        if(adUnitType.HasFlag(AppodealAdUnit.BANNER))
        {
            bannerAd = new AppodealBannerAd();
        }

        Debug.Log("[AdMediation-AppodealMediation] I got OnSdkInitializedEvent");

        LoadInterstitial();
        LoadRewardVideo();
        LoadBanner();
        onInitSuccess?.Invoke();
    }

    public void ShowTestSuite()
    {
        if(isInitialized)
        {
            AppodealStack.Monetization.Api.Appodeal.ShowTestScreen();
        }
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
        if(IsInterstitialAvailable())
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

    public void LoadBanner()
    {
        bannerAd?.Request();
    }

    public void ReloadBanner()
    {
    }

    public void ShowBanner(Action<string, AdInfo> onCompleted = null, Action<string, AdInfo> onFailed = null)
    {
        if(bannerAd != null)
        {
            bannerAd.Show(onCompleted, onFailed);
        }
        else
        {
            onFailed?.Invoke("bannerAd null", null);
        }
    }

    public void DestroyBanner()
    {
        if(bannerAd != null)
        {
            bannerAd.DestroyBanner();
        }
    }

    public void HideBanner()
    {
        if(bannerAd != null)
        {
            bannerAd.HideBanner();
        }
    }

    public void DisplayBanner()
    {
        if(bannerAd != null)
        {
            bannerAd.DisplayBanner();
        }
    }
    #endregion

    public void OnAdRevenueReceived(AppodealAdRevenue ad)
    {


        /* sample
         *https://docs.appodeal.com/unity/advanced/ad-revenue-callback
        //AppsFlyer
        var dict = new Dictionary<string, string>();
        dict.Add("AdUnitName", ad.AdUnitName);
        dict.Add("AdType", ad.AdType);
        AppsFlyerAdRevenue.logAdRevenue(ad.NetworkName,
            AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeAppodeal,
            ad.Revenue, ad.Currency, dict
        );

        //Adjust
        AdjustAdRevenue adRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourcePublisher);
        adRevenue.setRevenue(ad.Revenue, ad.Currency);
        adRevenue.setAdRevenueNetwork(ad.NetworkName);
        adRevenue.setAdRevenueUnit(ad.AdUnitName);
        Adjust.trackAdRevenue(adRevenue);

        //Firebase
        Firebase.Analytics.FirebaseAnalytics.LogEvent(
            Firebase.Analytics.FirebaseAnalytics.EventAdImpression,
                new Firebase.Analytics.Parameter(
                    Firebase.Analytics.FirebaseAnalytics.ParameterAdPlatform, "Appodeal"),
                new Firebase.Analytics.Parameter(
                    Firebase.Analytics.FirebaseAnalytics.ParameterAdFormat, ad.AdType),
                new Firebase.Analytics.Parameter(
                    Firebase.Analytics.FirebaseAnalytics.ParameterAdSource, ad.NetworkName),
                new Firebase.Analytics.Parameter(
                    Firebase.Analytics.FirebaseAnalytics.AdUnitName, ad.AdUnitName),
                new Firebase.Analytics.Parameter(
                    Firebase.Analytics.FirebaseAnalytics.AdCurrency, ad.Currency),
                new Firebase.Analytics.Parameter(
                    Firebase.Analytics.FirebaseAnalytics.Value, ad.Revenue)
        );
        */
    }
}
#endif