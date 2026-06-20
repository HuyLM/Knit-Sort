using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
     #if ATO_IRONSOURCE_MEDIATION_ENABLE
    public class ISVideoRewardAd : BaseAd
    {
        private bool getRewarded = false;
        private string placement = string.Empty;
        private bool isCallingShow;

        public override bool IsAvailable
        {
            get
            {
                if(IronSource.Agent.isRewardedVideoAvailable())
                {
                    return true;
                }
                return false;
            }
        }

        public ISVideoRewardAd()
        {
            CallAddEvent();
        }

        protected override void CallAddEvent()
        {
            IronSourceRewardedVideoEvents.onAdOpenedEvent += ReardedVideoOnAdOpenedEvent;
            IronSourceRewardedVideoEvents.onAdClosedEvent += ReardedVideoOnAdClosedEvent;
            IronSourceRewardedVideoEvents.onAdAvailableEvent += ReardedVideoOnAdAvailable;
            IronSourceRewardedVideoEvents.onAdUnavailableEvent += ReardedVideoOnAdUnavailable;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += ReardedVideoOnAdShowFailedEvent;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += ReardedVideoOnAdRewardedEvent;
            IronSourceRewardedVideoEvents.onAdClickedEvent += ReardedVideoOnAdClickedEvent;
            IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;
        }

        protected override void CallRequest()
        {
            Debug.Log("[AdMediation-ISVideoRewardAd]: ISVideoRewardAd is auto request ad");
        }

        protected override void CallShow()
        {
            if(IsAvailable)
            {
                isCallingShow = true;
                IronSource.Agent.showRewardedVideo();
            }
        }

#region Listeners

        void ReardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
        {
            getRewarded = false;
            OnAdShowed(adInfo.Convert());
            AdMediation.onVideoRewardDisplayedEvent?.Invoke(adInfo.Convert());
        }
        void ReardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
        {
            AdsEventExecutor.ExecuteInUpdate(() =>
            {
                OnCompleted(getRewarded, placement, adInfo.Convert());
                if(getRewarded)
                {
                    AdMediation.onVideoRewardCompletedEvent?.Invoke(placement, adInfo.Convert());
                }
                else
                {
                    AdMediation.onVideoRewardFailedEvent?.Invoke("is closed", adInfo.Convert());
                }
                getRewarded = false;
                placement = string.Empty;
                Debug.Log("[AdMediation-ISVideoRewardAd]: I got ReardedVideoOnAdClosedEvent in ExecuteInUpdate With AdInfo " + adInfo.ToString());
            });
            Debug.Log("[AdMediation-ISVideoRewardAd]: I got ReardedVideoOnAdClosedEvent With AdInfo " + adInfo.ToString());
        }
        void ReardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
        {
            OnAdLoadSuccess(adInfo.Convert());
            AdMediation.onVideoRewardLoadedEvent?.Invoke(adInfo.Convert());
            Debug.Log("[AdMediation-ISVideoRewardAd]: I got ReardedVideoOnAdAvailable With AdInfo " + adInfo.ToString());
        }
        void ReardedVideoOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
        {
            isCallingShow = false;
            OnAdShowFailed(ironSourceError.ToString(), adInfo.Convert());
            AdMediation.onVideoRewardFailedEvent?.Invoke(ironSourceError.ToString(), adInfo.Convert());
            Debug.Log("[AdMediation-ISVideoRewardAd]: I got RewardedVideoAdOpenedEvent With Error" + ironSourceError.ToString() + "And AdInfo " + adInfo.ToString());
        }
        void ReardedVideoOnAdRewardedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
        {
            placement = ironSourcePlacement.getPlacementName();
            getRewarded = true;
            Debug.Log("[AdMediation-ISVideoRewardAd]: I got ReardedVideoOnAdRewardedEvent With Placement" + ironSourcePlacement.ToString() + "And AdInfo " + adInfo.ToString());
        }
        void ReardedVideoOnAdClickedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
        {
            string placement = ironSourcePlacement.getPlacementName();
            Debug.Log("[AdMediation-ISVideoRewardAd]: I got ReardedVideoOnAdClickedEvent With Placement" + ironSourcePlacement.ToString() + "And AdInfo " + adInfo.ToString());
            AdMediation.onVideoRewardClicked?.Invoke(placement, adInfo.Convert());
        }
        void ReardedVideoOnAdUnavailable()
        {
            OnAdLoadFailed("ReardedVideoOnAdUnavailable");
            Debug.Log("[AdMediation-ISVideoRewardAd]: I got ReardedVideoOnAdUnavailable");
            AdMediation.onVideoRewardLoadFailedEvent?.Invoke(new AdInfo());
        }
        void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
        {
            if (isCallingShow)
            {
                isCallingShow = false;
                placement = impressionData.placement;
                OnAdOpening(impressionData.Convert());
                Debug.Log("[AdMediation-ISVideoRewardAd]: unity - script: I got ImpressionDataReadyEvent ToString(): " + impressionData.ToString());
                Debug.Log("[AdMediation-ISVideoRewardAd]: unity - script: I got ImpressionDataReadyEvent allData: " + impressionData.allData);
            }
        }

#endregion

       
    }
#endif
}
