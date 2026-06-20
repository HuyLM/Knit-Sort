using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
    #if ATO_IRONSOURCE_MEDIATION_ENABLE
    public class ISInterstitialAd : BaseAd
    {
        private bool isCallingShow;
        private string placement = string.Empty;

        private bool requesting;
        private readonly float[] retryTimes = { 0.1f, 1, 2, 5, 10, 20, 32, 64, 64, 128, 128, 256 }; // sec
        private int retryCounting;
        private DelayTask delayRequestTask;


        public override bool IsAvailable
        {
            get
            {
                if (IronSource.Agent.isInterstitialReady())
                {
                    return true;
                }
                Request();
                return false;
            }
        }

        public ISInterstitialAd()
        {
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
            IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;
            IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
            IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
            IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
            IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
            IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
            IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
            IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;
        }

        protected override void CallRequest()
        {
            IronSource.Agent.loadInterstitial();
        }

        protected override void CallShow()
        {
            if (IsAvailable)
            {
                isCallingShow = true;
                IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;
                IronSource.Agent.showInterstitial();
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
                return;
            }

            requesting = true;
            float delayRequest = GetRetryTime(retryCounting);
            Debug.Log($"[AdMediation-ISInterstitialAd]: Request: delay={delayRequest}s, retry={retryCounting}");

            delayRequestTask = new DelayTask(delayRequest, () =>
            {
                AdsEventExecutor.Remove(delayRequestTask);
                CallRequest();

            });
            delayRequestTask.Start();
            AdsEventExecutor.AddTask(delayRequestTask);
        }

#region Listeners
        void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo)
        {
            OnAdShowed(adInfo.Convert());
            AdMediation.onInterstitialDisplayedEvent?.Invoke(adInfo.Convert());
            Debug.Log("[AdMediation-ISInterstitialAd]: I got InterstitialOnAdOpenedEvent With AdInfo " + adInfo.ToString());
        }

        void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("[AdMediation-ISInterstitialAd]: I got InterstitialOnAdClosedEvent With AdInfo " + adInfo.ToString());
            Request();
        }

        void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo)
        {
            requesting = false;
            retryCounting = 0;
            OnAdLoadSuccess(adInfo.Convert());
            AdMediation.onInterstitialLoadedEvent?.Invoke(adInfo.Convert());
            Debug.Log("[AdMediation-ISInterstitialAd]: I got InterstitialOnAdReadyEvent With AdInfo " + adInfo.ToString());
        }

        void InterstitialOnAdLoadFailed(IronSourceError ironSourceError)
        {
            requesting = false;
            retryCounting++;
            AdMediation.onInterstitialLoadFailed?.Invoke(ironSourceError.ToString());
            OnAdLoadFailed(ironSourceError.ToString());
            Debug.Log("[AdMediation-ISInterstitialAd]: I got InterstitialOnAdLoadFailed With Error " + ironSourceError.ToString());
            Request();
        }
        void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
        {
            isCallingShow = false;
            OnAdShowFailed(ironSourceError.ToString(), adInfo.Convert());
            AdMediation.onInterstitialFailedEvent?.Invoke(ironSourceError.ToString(), adInfo.Convert());
            Debug.Log("[AdMediation-ISInterstitialAd]: I got InterstitialOnAdShowFailedEvent With Error " + ironSourceError.ToString() + " And AdInfo " + adInfo.ToString());
            Request();
        }

        void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("[AdMediation-ISInterstitialAd]: I got InterstitialOnAdClickedEvent With AdInfo " + adInfo.ToString());
            AdMediation.onInterstitiaClicked?.Invoke(adInfo.Convert());
        }

        void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo)
        {
            OnCompleted(true, placement, adInfo.Convert());
            AdMediation.onInterstitialCompletedEvent?.Invoke(placement, adInfo.Convert());
            placement = string.Empty;
            Debug.Log("[AdMediation-ISInterstitialAd]: I got InterstitialOnAdShowSucceededEvent With AdInfo " + adInfo.ToString());
        }

        void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
        {
            if (isCallingShow)
            {
                isCallingShow = false;
                placement = impressionData.placement;
                OnAdOpening(impressionData.Convert());
                Debug.Log("[AdMediation-ISInterstitialAd]: unity - script: I got ImpressionDataReadyEvent ToString(): " + impressionData.ToString());
                Debug.Log("[AdMediation-ISInterstitialAd]: unity - script: I got ImpressionDataReadyEvent allData: " + impressionData.allData);
            }
        }

#endregion
    }
#endif
}
