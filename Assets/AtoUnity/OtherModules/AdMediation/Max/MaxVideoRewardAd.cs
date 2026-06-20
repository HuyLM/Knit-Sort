using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
#if ATO_MAX_MEDIATION_ENABLE
    public class MaxVideoRewardAd : BaseAd
    {
        private bool getRewarded = false;
        private string adUnitId;

        private bool requesting;
        private readonly float[] retryTimes = { 0.1f, 1, 2, 4, 8, 16, 32, 64 }; // sec
        private int retryCounting;
        private DelayTask delayRequestTask;

        public override bool IsAvailable
        {
            get
            {
                if(MaxSdk.IsRewardedAdReady(adUnitId))
                {
                    return true;
                }
                Request();
                return false;
            }
        }

        public MaxVideoRewardAd(string adUnitId)
        {
            this.adUnitId = adUnitId;
            CallAddEvent();
        }

        private float GetRetryTime(int retry)
        {
            if (retry >= 0 && retry < retryTimes.Length)
            {
                return retryTimes[retry];
            }
            return retryTimes[retryTimes.Length - 1];
        }

        protected override void CallAddEvent()
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        }

        protected override void CallRequest()
        {
            MaxSdk.LoadRewardedAd(adUnitId);
        }

        protected override void CallShow()
        {
            if (IsAvailable)
            {
                MaxSdk.ShowRewardedAd(adUnitId);
            }
        }

        public override void Request()
        {
            if (requesting)
            {
                return;
            }
            if (Application.internetReachability == NetworkReachability.NotReachable)
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
        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            getRewarded = false;
            OnAdShowed(adInfo.Convert());
            AdMediation.onVideoRewardDisplayedEvent?.Invoke(adInfo.Convert());
            Debug.Log($"[AdMediation-MaxVideoRewardAd]: {adUnitId} got OnRewardedAdDisplayedEvent With AdInfo " + adInfo.ToString());
        }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            requesting = false;
            retryCounting = 0;
            OnAdLoadSuccess(adInfo.Convert());
            AdMediation.onVideoRewardLoadedEvent?.Invoke(adInfo.Convert());
            Debug.Log($"[AdMediation-MaxVideoRewardAd]: {adUnitId} got OnRewardedAdLoadedEvent With AdInfo " + adInfo.ToString());

            Debug.Log("Waterfall Name: " + adInfo.WaterfallInfo.Name + " and Test Name: " + adInfo.WaterfallInfo.TestName);
            Debug.Log("Waterfall latency was: " + adInfo.WaterfallInfo.LatencyMillis + " milliseconds");
            string waterfallInfoStr = "";
            foreach (var networkResponse in adInfo.WaterfallInfo.NetworkResponses)
            {
                waterfallInfoStr = "Network -> " + networkResponse.MediatedNetwork +
                                   "\n...adLoadState: " + networkResponse.AdLoadState +
                                   "\n...latency: " + networkResponse.LatencyMillis + " milliseconds" +
                                   "\n...credentials: " + networkResponse.Credentials;
            }
            Debug.Log(waterfallInfoStr);
        }

        private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            requesting = false;
            retryCounting++;
            OnAdLoadFailed(errorInfo.ToString());
            AdMediation.onVideoRewardLoadFailedEvent?.Invoke(new AdInfo());
            Debug.Log($"[AdMediation-MaxVideoRewardAd]: {adUnitId} got OnRewardedAdLoadFailedEvent With ErrorInfo " + errorInfo.ToString());

            Debug.Log("Waterfall Name: " + errorInfo.WaterfallInfo.Name + " and Test Name: " + errorInfo.WaterfallInfo.TestName);
            Debug.Log("Waterfall latency was: " + errorInfo.WaterfallInfo.LatencyMillis + " milliseconds");

            foreach (var networkResponse in errorInfo.WaterfallInfo.NetworkResponses)
            {
                Debug.Log("Network -> " + networkResponse.MediatedNetwork +
                      "\n...latency: " + networkResponse.LatencyMillis + " milliseconds" +
                      "\n...credentials: " + networkResponse.Credentials +
                      "\n...error: " + networkResponse.Error);
            }

            Request();
        }



        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            OnAdShowFailed(errorInfo.ToString(), adInfo.Convert());
            AdMediation.onVideoRewardFailedEvent(errorInfo.ToString(), adInfo.Convert());
         
            Debug.Log($"[AdMediation-MaxVideoRewardAd]: {adUnitId} got OnRewardedAdFailedToDisplayEvent With AdInfo " + adInfo.ToString());
            Debug.Log($"[AdMediation-MaxVideoRewardAd]: {adUnitId} got OnRewardedAdFailedToDisplayEvent With ErrorInfo " + errorInfo.ToString());
        }

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) 
        {
            Debug.Log($"[AdMediation-MaxVideoRewardAd]: {adUnitId} got OnRewardedAdClickedEvent With AdInfo " + adInfo.ToString());
        }

        private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            AdsEventExecutor.ExecuteInUpdate(() =>
            {
                OnCompleted(getRewarded, adInfo.Placement, adInfo.Convert());
                if(getRewarded)
                {
                    AdMediation.onVideoRewardCompletedEvent?.Invoke(adInfo.Placement, adInfo.Convert());
                }
                else
                {
                    AdMediation.onVideoRewardFailedEvent?.Invoke("is closed", adInfo.Convert());
                }
                getRewarded = false;
                Debug.Log($"[AdMediation-MaxVideoRewardAd]: {adUnitId} got OnRewardedAdHiddenEvent in ExecuteInUpdate With AdInfo " + adInfo.ToString());
            });
            Debug.Log($"[AdMediation-MaxVideoRewardAd]: {adUnitId} got OnRewardedAdHiddenEvent With AdInfo " + adInfo.ToString());
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            getRewarded = true;
            Debug.Log($"[AdMediation-MaxVideoRewardAd]: {adUnitId} got OnRewardedAdReceivedRewardEvent With AdInfo " + adInfo.ToString());
        }

        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnAdOpening(adInfo.ConvertToImpression());
            Debug.Log($"[AdMediation-MaxVideoRewardAd]: {adUnitId} got OnRewardedAdRevenuePaidEvent With AdInfo " + adInfo.ToString());
        }

#endregion
    }
#endif
}
