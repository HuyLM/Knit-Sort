using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AtoGame.Mediation
{
    #if ATO_MAX_MEDIATION_ENABLE
    public class MaxMRECAd : BaseMaxBannerAd
    {
        private string adUnitId;
        private MaxSdkBase.AdViewPosition position;


        public override bool IsAvailable
        {
            get
            {
                return true;
            }
        }

        public MaxMRECAd(string adUnitId, MaxSdkBase.AdViewPosition position)
        {
            this.adUnitId = adUnitId;
            this.position = position;
            CallAddEvent();
        }

        protected override void CallAddEvent()
        {
            MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
            MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdLoadFailedEvent;
            MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMRecAdClickedEvent;
            MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnMRecAdRevenuePaidEvent;
            MaxSdkCallbacks.MRec.OnAdExpandedEvent += OnMRecAdExpandedEvent;
            MaxSdkCallbacks.MRec.OnAdCollapsedEvent += OnMRecAdCollapsedEvent;
        }

        protected override void CallRequest()
        {
            Debug.Log("MaxMRECAd can't request");
        }

        protected override void CallShow()
        {
            MaxSdk.CreateMRec(adUnitId, position);
            MaxSdk.ShowMRec(adUnitId);
        }

        public void DestroyMREC()
        {
            MaxSdk.DestroyMRec(adUnitId);
            MaxSdkCallbacks.MRec.OnAdLoadedEvent -= OnMRecAdLoadedEvent;
            MaxSdkCallbacks.MRec.OnAdLoadFailedEvent -= OnMRecAdLoadFailedEvent;
            MaxSdkCallbacks.MRec.OnAdClickedEvent -= OnMRecAdClickedEvent;
            MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent -= OnMRecAdRevenuePaidEvent;
            MaxSdkCallbacks.MRec.OnAdExpandedEvent -= OnMRecAdExpandedEvent;
            MaxSdkCallbacks.MRec.OnAdCollapsedEvent -= OnMRecAdCollapsedEvent;
        }

        public void HideMREC()
        {
            MaxSdk.HideMRec(adUnitId);
        }

        public void DisplayerMREC()
        {
            MaxSdk.ShowMRec(adUnitId);
        }

        public override void Destroy()
        {
            DestroyMREC();
        }

        public override void Hide()
        {
            HideMREC();
        }

        public override void Display()
        {
            DisplayerMREC();
        }

        public override void Reload()
        {
            MaxSdk.LoadMRec(adUnitId);
        }

        #region Listners

        private void OnMRecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnCompleted(true, adInfo.Placement, adInfo.Convert());
            AdMediation.onBannerLoadedEvent?.Invoke(adInfo.Placement, adInfo.Convert());
            Debug.Log($"[AdMediation-MaxMRECAd]: {adUnitId} got OnMRecAdLoadedEvent With AdInfo " + adInfo.ToString());

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

        private void OnMRecAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            OnAdLoadFailed(errorInfo.ToString());
            AdMediation.onBannerFailedEvent?.Invoke(errorInfo.ToString());
            Debug.Log($"[AdMediation-MaxMRECAd]: {adUnitId} got OnMRecAdLoadFailedEvent With AdInfo " + errorInfo.ToString());

            Debug.Log("Waterfall Name: " + errorInfo.WaterfallInfo.Name + " and Test Name: " + errorInfo.WaterfallInfo.TestName);
            Debug.Log("Waterfall latency was: " + errorInfo.WaterfallInfo.LatencyMillis + " milliseconds");

            foreach (var networkResponse in errorInfo.WaterfallInfo.NetworkResponses)
            {
                Debug.Log("Network -> " + networkResponse.MediatedNetwork +
                      "\n...latency: " + networkResponse.LatencyMillis + " milliseconds" +
                      "\n...credentials: " + networkResponse.Credentials +
                      "\n...error: " + networkResponse.Error);
            }
        }

        private void OnMRecAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"[AdMediation-MaxMRECAd]: {adUnitId} got OnMRecAdClickedEvent With AdInfo " + adInfo.ToString());
            AdMediation.onBannerClicked?.Invoke(adInfo.Convert());
        }

        private void OnMRecAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnAdOpening(adInfo.ConvertToImpression());
            Debug.Log($"[AdMediation-MaxMRECAd]: {adUnitId} got OnMRecAdRevenuePaidEvent With AdInfo " + adInfo.ToString());
        }

        private void OnMRecAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"[AdMediation-MaxMRECAd]: {adUnitId} got OnMRecAdExpandedEvent With AdInfo " + adInfo.ToString());
        }

        private void OnMRecAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"[AdMediation-MaxMRECAd]: {adUnitId} got OnMRecAdCollapsedEvent With AdInfo " + adInfo.ToString());
        }

#endregion
    }
#endif
}
