#if ATO_APPODEAL_MEDIATION_ENABLE
using AppodealStack.Monetization.Api;
using AppodealStack.Monetization.Common;

using AtoGame.Mediation;
using System;
using UnityEngine;

public class AppodealInterstitialAd : BaseAd
{
    private bool getRewarded = false;

    private bool requesting;
    private readonly float[] retryTimes = { 0.1f, 1, 2, 4, 8, 16, 32, 64 }; // sec
    private int retryCounting;
    private DelayTask delayRequestTask;

    public override bool IsAvailable
    {
        get
        {
            if(Appodeal.IsLoaded(AppodealAdType.Interstitial))
            {
                return true;
            }
            Request();
            return false;
        }
    }

    public AppodealInterstitialAd()
    {
        Appodeal.SetAutoCache(AppodealAdType.Interstitial, false);
        CallAddEvent();
    }

    private float GetRetryTime(int retry)
    {
        if(retry >= 0 && retry < retryTimes.Length)
        {
            return retryTimes[retry];
        }
        return retryTimes[retryTimes.Length - 1];
    }

    protected override void CallAddEvent()
    {
        AppodealCallbacks.Interstitial.OnLoaded += OnAdLoadedEvent;
        AppodealCallbacks.Interstitial.OnFailedToLoad += OnAdLoadFailedEvent;
        AppodealCallbacks.Interstitial.OnShown += OnAdDisplayedEvent;
        AppodealCallbacks.Interstitial.OnShowFailed += OnAdFailedToDisplayEvent;
        AppodealCallbacks.Interstitial.OnClosed += OnAdHiddenEvent;
        AppodealCallbacks.Interstitial.OnClicked += OnAdClickedEvent;
        AppodealCallbacks.Interstitial.OnExpired += OnExpired;

    }

    protected override void CallRequest()
    {
        Appodeal.Cache(AppodealAdType.Interstitial);
    }

    protected override void CallShow()
    {
        if(IsAvailable)
        {
            Appodeal.Show(AppodealShowStyle.Interstitial);
        }
    }

    public override void Request()
    {
        if(requesting)
        {
            return;
        }
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Request failed: No internet available.");
            return;
        }

        requesting = true;
        float delayRequest = GetRetryTime(retryCounting);
        Debug.Log($"{nameof(AppodealInterstitialAd)} Request: delay={delayRequest}s, retry={retryCounting}");

        delayRequestTask = new DelayTask(delayRequest, () =>
        {
            AdsEventExecutor.Remove(delayRequestTask);
            CallRequest();

        });
        delayRequestTask.Start();
        AdsEventExecutor.AddTask(delayRequestTask);
    }

    #region Listeners


    private void OnAdLoadedEvent(object sender, AdLoadedEventArgs e)
    {
        Debug.Log($"[APDUnity] [Callback] OnInterstitialLoaded(bool isPrecache:{e.IsPrecache})");

        requesting = false;
        retryCounting = 0;
        OnAdLoadSuccess(new AdInfo());
        AdMediation.onInterstitialLoadedEvent?.Invoke(new AdInfo());
    }

    private void OnAdLoadFailedEvent(object sender, EventArgs e)
    {
        Debug.Log("[APDUnity] [Callback] OnInterstitialFailedToLoad()");
        requesting = false;
        retryCounting++;
        OnAdLoadFailed(e.ToString());
        AdMediation.onInterstitialLoadFailed?.Invoke(e.ToString());
        Request();
    }

    private void OnAdFailedToDisplayEvent(object sender, EventArgs e)
    {
        Debug.Log("[APDUnity] [Callback] OnInterstitialShowFailed()");
        OnAdShowFailed(e.ToString(), new AdInfo());
        AdMediation.onInterstitialFailedEvent(e.ToString(), new AdInfo());
    }

    private void OnAdClickedEvent(object sender, EventArgs e)
    {
        Debug.Log("[APDUnity] [Callback] OnInterstitialClicked()");
        AdMediation.onInterstitiaClicked?.Invoke(new AdInfo());
    }

    private void OnAdDisplayedEvent(object sender, EventArgs e)
    {
        Debug.Log("[APDUnity] [Callback] OnInterstitialShown()");
        getRewarded = false;
        OnAdShowed(new AdInfo());
        AdMediation.onVideoRewardDisplayedEvent?.Invoke(new AdInfo());
    }

    private void OnAdHiddenEvent(object sender, EventArgs e)
    {
        Debug.Log("Interstitial closed");

        OnCompleted(true, string.Empty, new AdInfo());
        AdMediation.onInterstitialCompletedEvent?.Invoke(string.Empty, new AdInfo());
    }

    private void OnExpired(object sender, EventArgs e)
    {
        Debug.Log("[APDUnity] [Callback] OnInterstitialExpired()");
        Request();
    }

    #endregion

}
#endif
