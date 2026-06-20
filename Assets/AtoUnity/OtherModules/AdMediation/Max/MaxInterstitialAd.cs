using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
    #if ATO_MAX_MEDIATION_ENABLE

    public class MaxInterstitialAd : BaseAd
    {
        private string adUnitId;

        private bool requesting;
        private readonly float[] retryTimes = { 0.1f, 1, 2, 4, 8, 16, 32, 64 }; // sec
        private int retryCounting;
        private DelayTask delayRequestTask;

        public override bool IsAvailable
        {
            get
            {
                if (MaxSdk.IsInterstitialReady(adUnitId))
                {
                    return true;
                }
                Request();
                return false;
            }
        }

        public MaxInterstitialAd(string adUnitId)
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
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialAdRevenuePaidEvent;
        }

        protected override void CallRequest()
        {
            MaxSdk.LoadInterstitial(adUnitId);
        }

        protected override void CallShow()
        {
            if (IsAvailable)
            {
                MaxSdk.ShowInterstitial(adUnitId);
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
            Debug.Log($"MaxInterstitialAd Request: delay={delayRequest}s, retry={retryCounting}");

            delayRequestTask = new DelayTask(delayRequest, () =>
            {
                AdsEventExecutor.Remove(delayRequestTask);
                CallRequest();
            });
            delayRequestTask.Start();
            AdsEventExecutor.AddTask(delayRequestTask);
        }

#region Listeners

        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

            // Reset retry attempt

            requesting = false;
            retryCounting = 0;
            OnAdLoadSuccess(adInfo.Convert());
            AdMediation.onInterstitialLoadedEvent?.Invoke(adInfo.Convert());
            Debug.Log($"[AdMediation-MaxInterstitialAd]: {adUnitId} got OnInterstitialLoadedEvent With AdInfo " + adInfo.ToString());

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

        private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Interstitial ad failed to load 
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)
            /*
           retryAttempt++;
           double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

           Invoke("LoadInterstitial", (float)retryDelay);
           */

            requesting = false;
            retryCounting++;
            OnAdLoadFailed(errorInfo.ToString());
            Debug.Log($"[AdMediation-MaxInterstitialAd]: {adUnitId} got OnInterstitialLoadFailedEvent With AdInfo " + errorInfo.ToString());

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

        private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnAdShowed(adInfo.Convert());
            AdMediation.onInterstitialDisplayedEvent?.Invoke(adInfo.Convert());
            Debug.Log($"[AdMediation-MaxInterstitialAd]: {adUnitId} got OnInterstitialDisplayedEvent With AdInfo " + adInfo.ToString());
        }

        private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
            //LoadInterstitial();

            OnAdShowFailed(errorInfo.ToString(), adInfo.Convert());
            AdMediation.onInterstitialFailedEvent?.Invoke(errorInfo.ToString(), adInfo.Convert());
            Debug.Log($"[AdMediation-MaxInterstitialAd]: {adUnitId} got OnInterstitialAdFailedToDisplayEvent With AdInfo " + adInfo.ToString());
            Debug.Log($"[AdMediation-MaxInterstitialAd]: {adUnitId} got OnInterstitialAdFailedToDisplayEvent With ErrorInfo " + errorInfo.ToString());

            Request();
        }

        private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"[AdMediation-MaxInterstitialAd]: {adUnitId} got OnInterstitialClickedEvent With AdInfo " + adInfo.ToString());
        }

        private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is hidden. Pre-load the next ad.
            //LoadInterstitial();

            OnCompleted(true, adInfo.Placement, adInfo.Convert());
            AdMediation.onInterstitialCompletedEvent?.Invoke(adInfo.Placement, adInfo.Convert());
            Debug.Log($"[AdMediation-MaxInterstitialAd]: {adUnitId} got OnInterstitialHiddenEvent With AdInfo " + adInfo.ToString());
        }
        private void OnInterstitialAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnAdOpening(adInfo.ConvertToImpression());
            Debug.Log($"[AdMediation-MaxInterstitialAd]: {adUnitId} got OnInterstitialAdRevenuePaidEvent With AdInfo " + adInfo.ToString());
            // Ad revenue paid. Use this callback to track user revenue.
        }
#endregion
    }
#endif
}
