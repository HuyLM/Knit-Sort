using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
#if ATO_ADMOB_MEDIATION_ENABLE || ATO_ADMOB_ENABLE
    public class AdmobBannerAd : BaseAd
    {
        private string adUnitId;
        private GoogleMobileAds.Api.AdSize size;
        private GoogleMobileAds.Api.AdPosition position;
        private bool useCollapsiable;

        GoogleMobileAds.Api.BannerView _bannerView;
        private bool isFirstRequest = true;

        private bool requesting;
        private readonly float[] retryTimes = { 0.1f, 1, 2, 4, 8, 16, 32, 64 }; // sec
        private int retryCounting;
        private DelayTask delayRequestTask;

        public override bool IsAvailable
        {
            get
            {
                if(_bannerView != null)
                {
                    return true;
                }
                return false;
            }
        }

        public AdmobBannerAd(string adUnitId, GoogleMobileAds.Api.AdSize size, GoogleMobileAds.Api.AdPosition position, bool useCollapsiable)
        {
            isFirstRequest = true;
            this.useCollapsiable = useCollapsiable;
            this.adUnitId = adUnitId;
            this.size = size;
            this.position = position;

            if(_bannerView != null)
            {
                _bannerView.Destroy();
            }

            if(useCollapsiable)
            {
                size = GoogleMobileAds.Api.AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(GoogleMobileAds.Api.AdSize.FullWidth);
            }

            _bannerView = new GoogleMobileAds.Api.BannerView(adUnitId, size, position);
            CallAddEvent();

        }

        protected override void CallAddEvent()
        {
            if(_bannerView != null)
            {
                _bannerView.OnAdPaid += OnAdPaid;
                _bannerView.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
                _bannerView.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
                _bannerView.OnBannerAdLoaded += OnAdLoadedEvent;
                _bannerView.OnBannerAdLoadFailed += OnAdLoadFailedEvent;
                _bannerView.OnAdClicked += OnAdClicked;
            }
        }

        protected override void CallRequest()
        {

        }

        private void BannerRequest()
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
            Debug.Log($"AdmobBannerAd Request: delay={delayRequest}s, retry={retryCounting}");

            delayRequestTask = new DelayTask(delayRequest, () =>
            {
                AdsEventExecutor.Remove(delayRequestTask);
                CallRequest();

            });
            delayRequestTask.Start();
            AdsEventExecutor.AddTask(delayRequestTask);
        }

        private float GetRetryTime(int retry)
        {
            if(retry >= 0 && retry < retryTimes.Length)
            {
                return retryTimes[retry];
            }
            return retryTimes[retryTimes.Length - 1];
        }

        protected override void CallShow()
        {
            if(requesting == true)
            {
                return;
            }
            if(_bannerView == null)
            {
                _bannerView = new GoogleMobileAds.Api.BannerView(adUnitId, size, position);
                CallAddEvent();
            }
            if(_bannerView != null)
            {
                requesting = true;
                var adRequest = new GoogleMobileAds.Api.AdRequest();
                if(useCollapsiable)
                {
                    string positionString = "bottom";
                    switch(this.position)
                    {
                        case GoogleMobileAds.Api.AdPosition.Top:
                        {
                            positionString = "top";
                            break;
                        }
                        case GoogleMobileAds.Api.AdPosition.Bottom:
                        {
                            positionString = "bottom";
                            break;
                        }
                    }
                    adRequest.Extras.Add("collapsible", positionString);
                    if(isFirstRequest)
                    {
                        System.Guid myuuid = System.Guid.NewGuid();
                        string myuuidAsString = myuuid.ToString();
                        adRequest.Extras.Add("collapsible_request_id", myuuidAsString);
                    }
                }
                _bannerView.LoadAd(adRequest);
            }
        }

        public void DestroyBanner()
        {
            if(_bannerView != null)
            {
                _bannerView.OnAdPaid -= OnAdPaid;
                _bannerView.OnAdFullScreenContentClosed -= OnAdFullScreenContentClosed;
                _bannerView.OnAdFullScreenContentOpened -= OnAdFullScreenContentOpened;
                _bannerView.OnBannerAdLoaded -= OnAdLoadedEvent;
                _bannerView.OnBannerAdLoadFailed -= OnAdLoadFailedEvent;
                _bannerView.OnAdClicked -= OnAdClicked;
                _bannerView.Destroy();
                _bannerView = null;
                requesting = false;
            }
        }

        public void Hide()
        {
            if(_bannerView != null)
            {
                _bannerView.Hide();
            }
        }

        public void Display()
        {
            if(_bannerView != null)
            {
                _bannerView.Show();
            }
        }

        #region Listeners

        private void OnAdFullScreenContentOpened()
        {
            AdsEventExecutor.ExecuteInUpdate(() =>
            {
                Debug.Log($"[AdMediation-AdmobBannerAd]: {adUnitId} got OnAdFullScreenContentOpened");
                AdMediation.onBannerFullOpenedEvent?.Invoke();
            });
        }

        private void OnAdLoadedEvent()
        {
            AdsEventExecutor.ExecuteInUpdate(() =>
            {
                requesting = false;
                isFirstRequest = false;
                Debug.Log($"[AdMediation-AdmobBannerAd]: {adUnitId} got OnAdLoadedEvent");
                OnAdLoadSuccess(new AdInfo());
                OnCompleted(true, string.Empty, new AdInfo());
                AdMediation.onBannerLoadedEvent?.Invoke(adUnitId, new AdInfo());
            });
        }

        private void OnAdLoadFailedEvent(GoogleMobileAds.Api.LoadAdError error)
        {
            AdsEventExecutor.ExecuteInUpdate(() =>
            {
                requesting = false;
                retryCounting++;
                Debug.Log($"[AdMediation-AdmobBannerAd]: {adUnitId} got OnAdLoadFailedEvent with error {error}");
                OnAdShowFailed(error.ToString(), new AdInfo());
                AdMediation.onBannerFailedEvent?.Invoke(error.ToString());
                BannerRequest();
            });
        }

        private void OnAdFullScreenContentClosed()
        {
            AdsEventExecutor.ExecuteInUpdate(() =>
            {
                Debug.Log($"[AdMediation-AdmobBannerAd]: {adUnitId} got OnAdFullScreenContentClosed");
                AdMediation.onBannerFullClosedEvent?.Invoke();
            });
        }

        private void OnAdPaid(GoogleMobileAds.Api.AdValue obj)
        {
            Debug.Log($"[AdMediation-AdmobBannerAd]: {adUnitId} got OnAdPaid With AdInfo " + obj.ToString());
            OnAdOpening(obj.ConvertToImpression("banner_ad", "banner_ad"));
        }

        private void OnAdClicked()
        {
            Debug.Log($"[AdMediation-AdmobBannerAd]: {adUnitId} got OnAdClicked");
            AdMediation.onBannerClicked?.Invoke(new AdInfo());
        }
        #endregion

    }
#endif
}
