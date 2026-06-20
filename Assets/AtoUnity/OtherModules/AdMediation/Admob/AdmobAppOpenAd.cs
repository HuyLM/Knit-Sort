using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
#if ATO_ADMOB_MEDIATION_ENABLE || ATO_ADMOB_ENABLE
    public class AdmobAppOpenAd : BaseAd
    {
        private string adUnitId;
        private bool requesting;
        private readonly float[] retryTimes = { 0.1f, 1, 2, 4, 8, 16, 32, 64 }; // sec
        private int retryCounting;
        private DelayTask delayRequestTask;

        private GoogleMobileAds.Api.AppOpenAd _appOpenAd;

        public override bool IsAvailable
        {
            get
            {
                if(_appOpenAd != null && _appOpenAd.CanShowAd())
                {
                    return true;
                }
                Request();
                return false;
            }
        }

        public AdmobAppOpenAd(string adUnitId)
        {
            this.adUnitId = adUnitId;
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
            if(_appOpenAd == null)
            {
                return;
            }
            _appOpenAd.OnAdPaid += OnAdPaid;
            _appOpenAd.OnAdImpressionRecorded += OnAdImpressionRecorded;
            _appOpenAd.OnAdClicked += OnAdClicked;
            _appOpenAd.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
            _appOpenAd.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;
            _appOpenAd.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
        }

        protected override void CallRequest()
        {
            // Clean up the old ad before loading a new one.
            if(_appOpenAd != null)
            {
                DestroyAd();
            }
            Debug.Log("[AdMediation-AdmobAppOpenAd]: requesting appopen ad.");

            // Create our request used to load the ad.
            var adRequest = new GoogleMobileAds.Api.AdRequest();

            GoogleMobileAds.Api.AppOpenAd.Load(adUnitId, adRequest, OnAppOpenAdLoadedEvent);
        }

        protected override void CallShow()
        {
            if(IsAvailable)
            {
                _appOpenAd.Show();
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
                return;
            }

            requesting = true;
            float delayRequest = GetRetryTime(retryCounting);
            Debug.Log($"[AdMediation-AdmobAppOpenAd]: Request: delay={delayRequest}s, retry={retryCounting}");

            delayRequestTask = new DelayTask(delayRequest, () =>
            {
                AdsEventExecutor.Remove(delayRequestTask);
                CallRequest();

            });
            delayRequestTask.Start();
            AdsEventExecutor.AddTask(delayRequestTask);
        }


        public void DestroyAd()
        {
            if(_appOpenAd != null)
            {
                Debug.Log("[AdMediation-AdmobAppOpenAd]: Destroying app open ad.");
                _appOpenAd.Destroy();
                _appOpenAd = null;
            }
        }

        #region Listeners

        private void OnAdFullScreenContentOpened()
        {
            OnAdShowed(new AdInfo());
            Debug.Log($"[AdMediation-AdmobAppOpenAd]: {adUnitId} got OnAdFullScreenContentOpened");
            AdMediation.onAppOpenOpenedEvent?.Invoke(new AdInfo());
        }

        private void OnAppOpenAdLoadedEvent(GoogleMobileAds.Api.AppOpenAd ad, GoogleMobileAds.Api.LoadAdError error)
        {
            // If the operation failed with a reason.
            if(error != null)
            {
                OnAppOpenAdLoadFailedEvent(error.ToString());
                return;
            }
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if(ad == null)
            {
                OnAppOpenAdLoadFailedEvent("null ad");
                return;
            }

            // The operation completed successfully.
            _appOpenAd = ad;
            Debug.Log($"[AdMediation-AdmobAppOpenAd]: {adUnitId} got OnAppOpenAdLoadedEvent with responseInfo: " + ad.GetResponseInfo());

            CallAddEvent();

            requesting = false;
            retryCounting = 0;
            OnAdLoadSuccess(new AdInfo());
            AdMediation.onAppOpenLoadedEvent?.Invoke(new AdInfo());
        }

        private void OnAppOpenAdLoadFailedEvent(string error)
        {
            requesting = false;
            retryCounting++;
            Debug.Log($"[AdMediation-AdmobAppOpenAd]: {adUnitId} got OnAppOpenAdLoadFailedEvent {error}");
            AdMediation.onAppOpenLoadFailed?.Invoke(error);
            OnAdLoadFailed(string.Empty);
        }


        private void OnAdFullScreenContentFailed(GoogleMobileAds.Api.AdError errorInfo)
        {
            OnAdShowFailed(errorInfo.ToString(), new AdInfo());

            Debug.Log($"[AdMediation-AdmobAppOpenAd]: {adUnitId} got OnAdFullScreenContentFailed With ErrorInfo " + errorInfo.ToString());
            AdMediation.onAppOpenFailedEvent(errorInfo.ToString(), new AdInfo());

        }

        private void OnAdFullScreenContentClosed()
        {
            AdsEventExecutor.ExecuteInUpdate(() =>
            {
                OnCompleted(true, string.Empty, new AdInfo());
                Debug.Log($"[AdMediation-AdmobAppOpenAd]: {adUnitId} got OnAdFullScreenContentClosed");
                AdMediation.onAppOpenCompletedEvent?.Invoke(string.Empty, new AdInfo());
            });
         
        }

        private void OnAdPaid(GoogleMobileAds.Api.AdValue obj)
        {
            OnAdOpening(obj.ConvertToImpression("open_ad", "open_ad"));
            Debug.Log($"[AdMediation-AdmobAppOpenAd]: {adUnitId} got OnAdPaid With AdInfo " + obj.ToString());
        }

        private void OnAdImpressionRecorded()
        {
            Debug.Log($"[AdMediation-AdmobAppOpenAd]: {adUnitId} got OnAdImpressionRecorded");
        }

        private void OnAdClicked()
        {
            Debug.Log($"[AdMediation-AdmobAppOpenAd]: {adUnitId} got OnAdClicked");
            AdMediation.onAppOpenClicked?.Invoke(new AdInfo());
        }

        #endregion
    }
#endif
}
