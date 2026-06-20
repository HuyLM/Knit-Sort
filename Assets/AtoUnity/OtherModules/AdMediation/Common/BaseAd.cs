using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
    public abstract class BaseAd
    {
        private Action<string, AdInfo> onShowCompleted; // string: Placement
        private Action<string, AdInfo> onShowFailed;

        public void Show(Action<string, AdInfo> onCompleted, Action<string, AdInfo> onFailed)
        {
            this.onShowCompleted = onCompleted;
            this.onShowFailed = onFailed;
            CallShow();
        }

        public virtual void Request()
        {
            CallRequest();
        }

        protected abstract void CallShow();
        protected abstract void CallRequest();
        protected abstract void CallAddEvent();
        public abstract bool IsAvailable { get; }

        #region Callback

        protected virtual void OnAdLoadSuccess(AdInfo adInfo)
        {
            Debug.Log($"{adInfo.adUnit} LoadedSuccessfully. Ready to show");
        }

        protected void OnAdLoadFailed(string errorMsg)
        {
            Request();
        }

        protected void OnAdOpening(ImpressionData impressionData)
        {
            AdMediation.onAdRevenuePaidEvent?.Invoke(impressionData);
        }

        protected void OnAdShowFailed(string errorMsg, AdInfo adInfo)
        {
            Debug.Log($"{adInfo.adUnit} FailedToShow: {errorMsg}");
            onShowFailed?.Invoke(errorMsg, adInfo);
            Request();
        }

        protected void OnAdShowed(AdInfo adInfo)
        {
            Debug.Log($"{adInfo.adUnit} Showed");
        }

        protected void OnCompleted(bool complete, string placement, AdInfo adInfo)
        {
            Debug.Log($"{adInfo.adUnit} Closed completed={complete} Call Request new one.");
            if (complete)
            {
                onShowCompleted?.Invoke(placement, adInfo);
            }
            else
            {
                onShowFailed?.Invoke("is closed", adInfo);
            }
            Request();
        }
        #endregion
    }
}
