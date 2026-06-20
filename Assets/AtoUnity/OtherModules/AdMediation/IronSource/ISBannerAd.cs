using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
    #if ATO_IRONSOURCE_MEDIATION_ENABLE
    public class ISBannerAd : BaseAd
    {
        private string placement = string.Empty;
        private bool isCallingShow;

        private State curState;
        private IronSourceBannerSize size;
        private IronSourceBannerPosition position;
        public override bool IsAvailable
        {
            get
            {
                return true;
            }
        }


        public ISBannerAd()
        {
            curState = State.EMPTY;
            CallAddEvent();
        }


        protected override void CallAddEvent()
        {
            IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;
            IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
            IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
            IronSourceBannerEvents.onAdClickedEvent += BannerOnAdClickedEvent;
            IronSourceBannerEvents.onAdScreenPresentedEvent += BannerOnAdScreenPresentedEvent;
            IronSourceBannerEvents.onAdScreenDismissedEvent += BannerOnAdScreenDismissedEvent;
        }

        protected override void CallRequest()
        {

        }

        protected override void CallShow()
        {
            if (curState == State.EMPTY && size != null)
            {
                isCallingShow = true;
                IronSource.Agent.loadBanner(size, position);
            }
        }

        public void SetConfig(IronSourceBannerSize size, IronSourceBannerPosition position)
        {
            this.size = size;
            this.position = position;
        }

        public void DestroyBanner()
        {
            if (curState != State.EMPTY)
            {
                IronSource.Agent.destroyBanner();
                curState = State.EMPTY;
            }
        }

        public void HideBanner()
        {
            if (curState == State.SHOWING)
            {
                IronSource.Agent.hideBanner();
                curState = State.HIDING;
            }
        }

        public void DisplayBanner()
        {
            if (curState == State.HIDING)
            {
                IronSource.Agent.displayBanner();
                curState = State.SHOWING;
            }
        }

#region Listners

        void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo)
        {
            curState = State.SHOWING;
            OnCompleted(true, placement, adInfo.Convert());
            AdMediation.onBannerLoadedEvent?.Invoke(placement, adInfo.Convert());
            Debug.Log("[AdMediation-ISBannerAd]: I got BannerOnAdLoadedEvent With AdInfo " + adInfo.ToString());
        }

        void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError)
        {
            isCallingShow = false;
            OnAdLoadFailed(ironSourceError.ToString());
            AdMediation.onBannerFailedEvent?.Invoke(ironSourceError.ToString());
            Debug.Log("[AdMediation-ISBannerAd]: I got BannerOnAdLoadFailedEvent With Error " + ironSourceError.ToString());
        }

        void BannerOnAdClickedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("[AdMediation-ISBannerAd]: I got BannerOnAdClickedEvent With AdInfo " + adInfo.ToString());
            AdMediation.onBannerClicked?.Invoke(adInfo.Convert());
        }

        //Notifies the presentation of a full screen content following user click
        void BannerOnAdScreenPresentedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("[AdMediation-ISBannerAd] I got BannerOnAdScreenPresentedEvent With AdInfo " + adInfo.ToString());
        }
        //Notifies the presented screen has been dismissed
        void BannerOnAdScreenDismissedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("[AdMediation-ISBannerAd] I got BannerOnAdScreenDismissedEvent With AdInfo " + adInfo.ToString());
        }

        void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
        {
            if (isCallingShow)
            {
                isCallingShow = false;
                placement = impressionData.placement;
                OnAdOpening(impressionData.Convert());
                Debug.Log("[AdMediation-ISBannerAd] unity - script: I got ImpressionDataReadyEvent ToString(): " + impressionData.ToString());
                Debug.Log("[AdMediation-ISBannerAd] unity - script: I got ImpressionDataReadyEvent allData: " + impressionData.allData);
            }
        }

#endregion

        public enum State
        {
            EMPTY,
            SHOWING,
            HIDING,
        }
    }
#endif
}
