using UnityEngine;

namespace AtoGame.Mediation
{
#if ATO_ADMOB_MEDIATION_ENABLE || ATO_ADMOB_ENABLE
    public class AdmobVideoRewardAd : BaseAd
    {
        private bool getRewarded = false;
        private string adUnitId;

        private bool requesting;
        private readonly float[] retryTimes = { 0.1f, 1, 2, 4, 8, 16, 32, 64 }; // sec
        private int retryCounting;
        private DelayTask delayRequestTask;

        private GoogleMobileAds.Api.RewardedAd _rewardedAd;

        public override bool IsAvailable
        {
            get
            {
                if (_rewardedAd != null && _rewardedAd.CanShowAd())
                {
                    return true;
                }
                Request();
                return false;
            }
        }

        public AdmobVideoRewardAd(string adUnitId)
        {
            this.adUnitId = adUnitId;
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
            if (_rewardedAd == null)
            {
                return;
            }
            _rewardedAd.OnAdPaid += OnAdPaid;
            _rewardedAd.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
            _rewardedAd.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;
            _rewardedAd.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
            _rewardedAd.OnAdClicked += OnAdClicked;
        }



        protected override void CallRequest()
        {
            // Clean up the old ad before loading a new one.
            if (_rewardedAd != null)
            {
                DestroyAd();
            }
            Debug.Log("Loading rewarded ad.");

            // Create our request used to load the ad.
            var adRequest = new GoogleMobileAds.Api.AdRequest();

            GoogleMobileAds.Api.RewardedAd.Load(adUnitId, adRequest, OnRewardedAdLoadedEvent);
        }

        protected override void CallShow()
        {
            if (IsAvailable)
            {
                Debug.Log("Showing rewarded ad.");
                _rewardedAd.Show(OnRewardedAdReceivedRewardEvent);
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
            Debug.Log($"AdmobVideoRewardAd Request: delay={delayRequest}s, retry={retryCounting}");

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
            if (_rewardedAd != null)
            {
                Debug.Log("Destroying rewarded ad.");
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }
        }

        #region Listeners

        private void OnAdFullScreenContentOpened()
        {
            getRewarded = false;
            OnAdShowed(new AdInfo());
            AdMediation.onVideoRewardDisplayedEvent?.Invoke(new AdInfo());
            Debug.Log($"[AdMediation-AdmobVideoRewardAd]: {adUnitId} got OnRewardedAdDisplayedEvent");
        }

        private void OnRewardedAdLoadedEvent(GoogleMobileAds.Api.RewardedAd ad, GoogleMobileAds.Api.LoadAdError error)
        {
            // If the operation failed with a reason.
            if (error != null)
            {
                OnRewardedAdLoadFailedEvent(error.ToString());
                return;
            }
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                OnRewardedAdLoadFailedEvent("null ad");
                return;
            }

            // The operation completed successfully.
            _rewardedAd = ad;
            Debug.Log($"[AdMediation-AdmobVideoRewardAd]: {adUnitId} got OnRewardedAdLoadedEvent with responseInfo: " + ad.GetResponseInfo());

            CallAddEvent();

            requesting = false;
            retryCounting = 0;
            OnAdLoadSuccess(new AdInfo());
            AdMediation.onVideoRewardLoadedEvent?.Invoke(new AdInfo());
        }

        private void OnRewardedAdLoadFailedEvent(string error)
        {
            requesting = false;
            retryCounting++;
            OnAdLoadFailed(string.Empty);
            Debug.Log($"[AdMediation-AdmobVideoRewardAd]: {adUnitId} got OnRewardedAdLoadFailedEvent with error {error}");
            AdMediation.onVideoRewardLoadFailedEvent?.Invoke(new AdInfo());
        }


        private void OnAdFullScreenContentFailed(GoogleMobileAds.Api.AdError errorInfo)
        {
            OnAdShowFailed(errorInfo.ToString(), new AdInfo());
            AdMediation.onVideoRewardFailedEvent?.Invoke(errorInfo.ToString(), new AdInfo());

            Debug.Log($"[AdMediation-AdmobVideoRewardAd]: {adUnitId} got OnAdFullScreenContentFailed With ErrorInfo " + errorInfo.ToString());
        }

        private void OnAdFullScreenContentClosed()
        {
            AdsEventExecutor.ExecuteInUpdate(() =>
            {
                OnCompleted(getRewarded, string.Empty, new AdInfo());
                if (getRewarded)
                {
                    AdMediation.onVideoRewardCompletedEvent?.Invoke(string.Empty, new AdInfo());
                }
                else
                {
                    AdMediation.onVideoRewardFailedEvent?.Invoke("is closed", new AdInfo());
                }
                getRewarded = false;
                Debug.Log($"[AdMediation-AdmobVideoRewardAd]: {adUnitId} got OnAdFullScreenContentClosed in ExecuteInUpdate");
            });
            Debug.Log($"[AdMediation-AdmobVideoRewardAd]: {adUnitId} got OnAdFullScreenContentClosed");
        }

        private void OnRewardedAdReceivedRewardEvent(GoogleMobileAds.Api.Reward reward)
        {
            getRewarded = true;
            Debug.Log($"[AdMediation-AdmobVideoRewardAd]: {adUnitId} got OnRewardedAdReceivedRewardEvent");
        }

        private void OnAdPaid(GoogleMobileAds.Api.AdValue obj)
        {
            OnAdOpening(obj.ConvertToImpression("reward_ad", "reward_ad"));
            Debug.Log($"[AdMediation-AdmobVideoRewardAd]: {adUnitId} got OnAdPaid With AdInfo " + obj.ToString());
        }

        private void OnAdClicked()
        {
            Debug.Log($"[AdMediation-AdmobVideoRewardAd]: {adUnitId} got OnAdClicked");
            AdMediation.onVideoRewardClicked?.Invoke(adUnitId, new AdInfo());

            #endregion
        }
    }
#endif
}
