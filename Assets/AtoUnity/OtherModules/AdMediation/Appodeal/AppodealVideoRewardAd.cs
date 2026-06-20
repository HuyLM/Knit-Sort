#if ATO_APPODEAL_MEDIATION_ENABLE
using AppodealStack.Monetization.Api;
using AppodealStack.Monetization.Common;

using AtoGame.Mediation;
using System;
using UnityEngine;

public class AppodealVideoRewardAd : BaseAd
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
            if(Appodeal.IsLoaded(AppodealAdType.RewardedVideo))
            {
                return true;
            }
            Request();
            return false;
        }
    }

    public AppodealVideoRewardAd()
    {
        Appodeal.SetAutoCache(AppodealAdType.RewardedVideo, false);
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
        AppodealCallbacks.RewardedVideo.OnLoaded += OnRewardedAdLoadedEvent;
        AppodealCallbacks.RewardedVideo.OnFailedToLoad += OnRewardedAdLoadFailedEvent;
        AppodealCallbacks.RewardedVideo.OnShown += OnRewardedAdDisplayedEvent;
        AppodealCallbacks.RewardedVideo.OnShowFailed += OnRewardedAdFailedToDisplayEvent;
        AppodealCallbacks.RewardedVideo.OnClosed += OnRewardedAdHiddenEvent;
        AppodealCallbacks.RewardedVideo.OnFinished += OnRewardedAdReceivedRewardEvent;
        AppodealCallbacks.RewardedVideo.OnClicked += OnRewardedAdClickedEvent;
        AppodealCallbacks.RewardedVideo.OnExpired += OnRewardedVideoExpired;

    }

    protected override void CallRequest()
    {
        Appodeal.Cache(AppodealAdType.RewardedVideo);
    }

    protected override void CallShow()
    {
        if(IsAvailable)
        {
            Appodeal.Show(AppodealShowStyle.RewardedVideo);
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
        Debug.Log($"MaxVideoRewardAd Request: delay={delayRequest}s, retry={retryCounting}");

        delayRequestTask = new DelayTask(delayRequest, () =>
        {
            AdsEventExecutor.Remove(delayRequestTask);
            CallRequest();

        });
        delayRequestTask.Start();
        AdsEventExecutor.AddTask(delayRequestTask);
    }

    #region Listeners
  

    private void OnRewardedAdLoadedEvent(object sender, AdLoadedEventArgs e)
    {
        Debug.Log($"[APDUnity] [Callback] OnRewardedVideoLoaded(bool isPrecache:{e.IsPrecache})");

        requesting = false;
        retryCounting = 0;
        OnAdLoadSuccess(new AdInfo());
        AdMediation.onVideoRewardLoadedEvent?.Invoke(new AdInfo());
    }

    private void OnRewardedAdLoadFailedEvent(object sender, EventArgs e)
    {
        Debug.Log("[APDUnity] [Callback] OnRewardedVideoFailedToLoad()");
        requesting = false;
        retryCounting++;
        OnAdLoadFailed(e.ToString());
        AdMediation.onVideoRewardLoadFailedEvent?.Invoke(new AdInfo());
        Request();
    }

    private void OnRewardedAdFailedToDisplayEvent(object sender, EventArgs e)
    {
        Debug.Log("[APDUnity] [Callback] OnRewardedVideoShowFailed()");
        OnAdShowFailed(e.ToString(), new AdInfo());
        AdMediation.onVideoRewardFailedEvent(e.ToString(), new AdInfo());
    }

    private void OnRewardedAdClickedEvent(object sender, EventArgs e)
    {
        Debug.Log("[APDUnity] [Callback] OnRewardedVideoClicked()");
        AdMediation.onVideoRewardClicked?.Invoke(string.Empty, new AdInfo());
    }

    private void OnRewardedAdDisplayedEvent(object sender, EventArgs e)
    {
        Debug.Log("[APDUnity] [Callback] OnRewardedVideoShown()");
        getRewarded = false;
        OnAdShowed(new AdInfo());
        AdMediation.onVideoRewardDisplayedEvent?.Invoke(new AdInfo());
    }

    private void OnRewardedAdHiddenEvent(object sender, RewardedVideoClosedEventArgs e)
    {
        Debug.Log($"[APDUnity] [Callback] OnRewardedVideoClosed(bool finished:{e.Finished})");
        AdsEventExecutor.ExecuteInUpdate(() =>
        {
            Debug.Log($"[APDUnity] [Callback] OnRewardedVideoClosed(bool finished:{e.Finished}) --- In AdsEventExecutor");
            OnCompleted(getRewarded, string.Empty, new AdInfo());
            if(getRewarded)
            {
                AdMediation.onVideoRewardCompletedEvent?.Invoke(string.Empty, new AdInfo());
            }
            else
            {
                AdMediation.onVideoRewardFailedEvent?.Invoke("is closed", new AdInfo());
            }
            getRewarded = false;
        });
    }

    private void OnRewardedAdReceivedRewardEvent(object sender, RewardedVideoFinishedEventArgs e)
    {
        getRewarded = true;
        Debug.Log($"[APDUnity] [Callback] OnRewardedVideoFinished(double amount:{e.Amount}, string name:{e.Currency})");
    }

    private void OnRewardedVideoExpired(object sender, EventArgs e)
    {
        Debug.Log("[APDUnity] [Callback] OnRewardedVideoExpired()");
        Request();
    }

    #endregion

}
#endif