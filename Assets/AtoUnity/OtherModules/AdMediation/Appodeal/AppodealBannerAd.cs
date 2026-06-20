#if APPODEAL_ENABLE
using AppodealStack.Monetization.Api;
using AppodealStack.Monetization.Common;
using AtoGame.Mediation;
using System;
using UnityEngine;

public class AppodealBannerAd : BaseAd
{
    public override bool IsAvailable
    {
        get
        {
            return Appodeal.IsLoaded(AppodealAdType.Banner);
        }
    }

    public AppodealBannerAd()
    {
        CallAddEvent();
    }

    protected override void CallAddEvent()
    {
        AppodealCallbacks.Banner.OnLoaded += OnAdLoadedEvent;
        AppodealCallbacks.Banner.OnFailedToLoad += OnAdLoadFailedEvent;
        AppodealCallbacks.Banner.OnShown += OnAdExpandedEvent;
        AppodealCallbacks.Banner.OnShowFailed += OnAdShowFailedEvent;
        AppodealCallbacks.Banner.OnClicked += OnAdClickedEvent;
        AppodealCallbacks.Banner.OnExpired += OnExpired;
    }

    protected override void CallRequest()
    {
      
    }

    protected override void CallShow()
    {
        Appodeal.Show(AppodealShowStyle.BannerBottom);
    }

    public void DestroyBanner()
    {
        Appodeal.Destroy(AppodealAdType.Banner);
    }

    public void HideBanner()
    {
        Appodeal.HideBannerView();
    }

    public void DisplayBanner()
    {
        Appodeal.ShowBannerView(AppodealViewPosition.VerticalBottom, AppodealViewPosition.HorizontalCenter, string.Empty);
    }

    void OnAdLoadedEvent(object sender, BannerLoadedEventArgs e)
    {
        Debug.Log("Banner loaded");
        AdMediation.onBannerLoadedEvent?.Invoke(string.Empty, new AdInfo());
    }

    void OnAdLoadFailedEvent(object sender, EventArgs e)
    {
        Debug.Log("Banner failed to load");
        OnAdLoadFailed(e.ToString());
        AdMediation.onBannerFailedEvent?.Invoke(e.ToString());
    }

    private void OnAdShowFailedEvent(object sender, EventArgs e)
    {
        Debug.Log("Banner show failed");
    }

    void OnAdClickedEvent(object sender, EventArgs e)
    {
        Debug.Log("Banner clicked");
        AdMediation.onBannerClicked?.Invoke(new AdInfo());
    }

    private void OnAdExpandedEvent(object sender, EventArgs e)
    {
        Debug.Log("Banner shown");
        OnCompleted(true, string.Empty, new AdInfo());
    }


    private void OnExpired(object sender, EventArgs e)
    {
        Debug.Log("Banner expired");
    }
}
#endif