using UnityEngine;

namespace AtoGame.Mediation
{
#if ATO_MAX_MEDIATION_ENABLE
    public class MaxAppOpenAd : BaseAd
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
                if(MaxSdk.IsAppOpenAdReady(adUnitId))
                {
                    return true;
                }
                Request();
                return false;
            }
        }

        public MaxAppOpenAd(string adUnitId)
        {
            this.adUnitId = adUnitId;
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
            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAppOpenLoadedEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += OnAppOpenLoadFailedEvent;
            MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent += OnAppOpenDisplayedEvent;
            MaxSdkCallbacks.AppOpen.OnAdClickedEvent += OnAppOpenClickedEvent;
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenHiddenEvent;
            MaxSdkCallbacks.AppOpen.OnAdDisplayFailedEvent += OnAppOpenAdFailedToDisplayEvent;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += OnAppOpenAdRevenuePaidEvent;
        }

        protected override void CallRequest()
        {
            MaxSdk.LoadAppOpenAd(adUnitId);
        }

        protected override void CallShow()
        {
            if(IsAvailable)
            {
                MaxSdk.ShowAppOpenAd(adUnitId);
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
            Debug.Log($"MaxAppOpenAd Request: delay={delayRequest}s, retry={retryCounting}");

            delayRequestTask = new DelayTask(delayRequest, () =>
            {
                AdsEventExecutor.Remove(delayRequestTask);
                CallRequest();
            });
            delayRequestTask.Start();
            AdsEventExecutor.AddTask(delayRequestTask);
        }

        #region Listeners

        private void OnAppOpenLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // AppOpen ad is ready for you to show. MaxSdk.IsAppOpenReady(adUnitId) now returns 'true'

            // Reset retry attempt

            requesting = false;
            retryCounting = 0;
            OnAdLoadSuccess(adInfo.Convert());
            AdMediation.onAppOpenLoadedEvent?.Invoke(adInfo.Convert());
            Debug.Log($"[AdMediation-MaxAppOpenAd]: {adUnitId} got OnAppOpenLoadedEvent With AdInfo " + adInfo.ToString());

            Debug.Log("Waterfall Name: " + adInfo.WaterfallInfo.Name + " and Test Name: " + adInfo.WaterfallInfo.TestName);
            Debug.Log("Waterfall latency was: " + adInfo.WaterfallInfo.LatencyMillis + " milliseconds");
            string waterfallInfoStr = "";
            foreach(var networkResponse in adInfo.WaterfallInfo.NetworkResponses)
            {
                waterfallInfoStr = "Network -> " + networkResponse.MediatedNetwork +
                                   "\n...adLoadState: " + networkResponse.AdLoadState +
                                   "\n...latency: " + networkResponse.LatencyMillis + " milliseconds" +
                                   "\n...credentials: " + networkResponse.Credentials;
            }
            Debug.Log(waterfallInfoStr);
        }

        private void OnAppOpenLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // AppOpen ad failed to load 
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)
            /*
           retryAttempt++;
           double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

           Invoke("LoadAppOpen", (float)retryDelay);
           */

            requesting = false;
            retryCounting++;
            AdMediation.onAppOpenLoadFailed?.Invoke(errorInfo.ToString());
            OnAdLoadFailed(errorInfo.ToString());
            Debug.Log($"[AdMediation-MaxAppOpenAd]: {adUnitId} got OnAppOpenLoadFailedEvent With AdInfo " + errorInfo.ToString());

            Debug.Log("Waterfall Name: " + errorInfo.WaterfallInfo.Name + " and Test Name: " + errorInfo.WaterfallInfo.TestName);
            Debug.Log("Waterfall latency was: " + errorInfo.WaterfallInfo.LatencyMillis + " milliseconds");

            foreach(var networkResponse in errorInfo.WaterfallInfo.NetworkResponses)
            {
                Debug.Log("Network -> " + networkResponse.MediatedNetwork +
                      "\n...latency: " + networkResponse.LatencyMillis + " milliseconds" +
                      "\n...credentials: " + networkResponse.Credentials +
                      "\n...error: " + networkResponse.Error);
            }

            Request();
        }

        private void OnAppOpenDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnAdShowed(adInfo.Convert());
            AdMediation.onAppOpenOpenedEvent?.Invoke(adInfo.Convert());
            Debug.Log($"[AdMediation-MaxAppOpenAd]: {adUnitId} got OnAppOpenDisplayedEvent With AdInfo " + adInfo.ToString());
        }

        private void OnAppOpenAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // AppOpen ad failed to display. AppLovin recommends that you load the next ad.
            //LoadAppOpen();

            OnAdShowFailed(errorInfo.ToString(), adInfo.Convert());
            AdMediation.onAppOpenFailedEvent?.Invoke(errorInfo.ToString(), adInfo.Convert());
            Debug.Log($"[AdMediation-MaxAppOpenAd]: {adUnitId} got OnAppOpenAdFailedToDisplayEvent With AdInfo " + adInfo.ToString());
            Debug.Log($"[AdMediation-MaxAppOpenAd]: {adUnitId} got OnAppOpenAdFailedToDisplayEvent With ErrorInfo " + errorInfo.ToString());

            Request();
        }

        private void OnAppOpenClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"[AdMediation-MaxAppOpenAd]: {adUnitId} got OnAppOpenClickedEvent With AdInfo " + adInfo.ToString());
            AdMediation.onAppOpenClicked?.Invoke(adInfo.Convert());
        }

        private void OnAppOpenHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // AppOpen ad is hidden. Pre-load the next ad.
            //LoadAppOpen();

            OnCompleted(true, adInfo.Placement, adInfo.Convert());
            AdMediation.onAppOpenCompletedEvent?.Invoke(adInfo.Placement, adInfo.Convert());
            Debug.Log($"[AdMediation-MaxAppOpenAd]: {adUnitId} got OnAppOpenHiddenEvent With AdInfo " + adInfo.ToString());
        }
        private void OnAppOpenAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnAdOpening(adInfo.ConvertToImpression());
            Debug.Log($"[AdMediation-MaxAppOpenAd]: {adUnitId} got OnAppOpenAdRevenuePaidEvent With AdInfo " + adInfo.ToString());
            // Ad revenue paid. Use this callback to track user revenue.
        }
        #endregion
    }
#endif
}
