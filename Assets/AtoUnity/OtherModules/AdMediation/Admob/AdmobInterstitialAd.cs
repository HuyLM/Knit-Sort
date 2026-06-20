using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
#if ATO_ADMOB_MEDIATION_ENABLE || ATO_ADMOB_ENABLE
    public class AdmobInterstitialAd : BaseAd
    {
        private string adUnitId;

        private bool requesting;
        private readonly float[] retryTimes = { 0.1f, 1, 2, 4, 8, 16, 32, 64 }; // sec
        private int retryCounting;
        private DelayTask delayRequestTask;

        private GoogleMobileAds.Api.InterstitialAd _interstitialAd;

        public override bool IsAvailable
        {
            get
            {
                if(_interstitialAd != null && _interstitialAd.CanShowAd())
                {
                    return true;
                }
                Request();
                return false;
            }
        }

        public AdmobInterstitialAd(string adUnitId)
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
            if(_interstitialAd == null)
            {
                return;
            }
            _interstitialAd.OnAdPaid += OnAdPaid;
            _interstitialAd.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
            _interstitialAd.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;
            _interstitialAd.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
            _interstitialAd.OnAdClicked += OnAdClicked;
        }



        protected override void CallRequest()
        {
            // Clean up the old ad before loading a new one.
            if(_interstitialAd != null)
            {
                DestroyAd();
            }

            // Create our request used to load the ad.
            var adRequest = new GoogleMobileAds.Api.AdRequest();

            GoogleMobileAds.Api.InterstitialAd.Load(adUnitId, adRequest, OnInterstitialAdLoadedEvent);
        }

        protected override void CallShow()
        {
            if(IsAvailable)
            {
                _interstitialAd.Show();
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
            Debug.Log($"[AdMediation-AdmobInterstitialAd]: Request: delay={delayRequest}s, retry={retryCounting}");

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
            if(_interstitialAd != null)
            {
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }
        }

        #region Listeners

        private void OnAdFullScreenContentOpened()
        {
            OnAdShowed(new AdInfo());
            AdMediation.onInterstitialDisplayedEvent?.Invoke(new AdInfo());
            Debug.Log($"[AdMediation-AdmobInterstitialAd]: {adUnitId} got OnAdFullScreenContentOpened");
        }

        private void OnInterstitialAdLoadedEvent(GoogleMobileAds.Api.InterstitialAd ad, GoogleMobileAds.Api.LoadAdError error)
        {
            // If the operation failed with a reason.
            if(error != null)
            {
                OnInterstitialAdLoadFailedEvent(error.ToString());
                return;
            }
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if(ad == null)
            {
                OnInterstitialAdLoadFailedEvent("ad null");
                return;
            }

            // The operation completed successfully.
            _interstitialAd = ad;
            Debug.Log($"[AdMediation-AdmobInterstitialAd]: {adUnitId} got OnInterstitialAdLoadedEvent with responseInfo: " + ad.GetResponseInfo());

            CallAddEvent();

            requesting = false;
            retryCounting = 0;
            OnAdLoadSuccess(new AdInfo());
            AdMediation.onInterstitialLoadedEvent?.Invoke(new AdInfo());
        }

        private void OnInterstitialAdLoadFailedEvent(string error)
        {
            requesting = false;
            retryCounting++;
            OnAdLoadFailed(string.Empty);
            Debug.Log($"[AdMediation-AdmobInterstitialAd]: {adUnitId} got OnInterstitialAdLoadFailedEvent " + error);
            AdMediation.onInterstitialLoadFailed?.Invoke(error);
        }


        private void OnAdFullScreenContentFailed(GoogleMobileAds.Api.AdError errorInfo)
        {
            AdsEventExecutor.ExecuteInUpdate(() =>
            {
                OnAdShowFailed(errorInfo.ToString(), new AdInfo());
                AdMediation.onInterstitialFailedEvent?.Invoke(errorInfo.ToString(), new AdInfo());

                Debug.Log($"[AdMediation-AdmobInterstitialAd]: ExecuteInUpdate {adUnitId} got OnAdFullScreenContentFailed With ErrorInfo " + errorInfo.ToString());
            });
            Debug.Log($"[AdMediation-AdmobInterstitialAd]: {adUnitId} got OnAdFullScreenContentFailed With ErrorInfo " + errorInfo.ToString());
        }

        private void OnAdFullScreenContentClosed()
        {
            AdsEventExecutor.ExecuteInUpdate(() =>
            {
                OnCompleted(true, string.Empty, new AdInfo());
                AdMediation.onInterstitialCompletedEvent?.Invoke(string.Empty, new AdInfo());
                Debug.Log($"[AdMediation-AdmobInterstitialAd]: {adUnitId} got OnAdFullScreenContentClosed in ExecuteInUpdate");
            });
            Debug.Log($"[AdMediation-AdmobInterstitialAd]: {adUnitId} got OnAdFullScreenContentClosed");
        }

        private void OnAdPaid(GoogleMobileAds.Api.AdValue obj)
        {
            OnAdOpening(obj.ConvertToImpression("inter_ad", "inter_ad"));
            Debug.Log($"[AdMediation-AdmobInterstitialAd]: {adUnitId} got OnAdPaid With AdInfo " + obj.ToString());
        }


        private void OnAdClicked()
        {
            Debug.Log($"[AdMediation-AdmobInterstitialAd]: {adUnitId} got OnAdClicked");
            AdMediation.onInterstitiaClicked?.Invoke(new AdInfo());
        }
        #endregion

    }
#endif
}
