#if ATO_ADMOB_MEDIATION_ENABLE || (ATO_ADMOB_ENABLE && ATO_NATIVE_ADMOB_ENABLE)
using AtoGame.Mediation;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
    public class AdmobNativeAd : BaseAd
    {
        private string adUnitId;
        private GoogleMobileAds.Api.NativeAd nativeAd;

        private bool requesting;
        private readonly float[] retryTimes = { 0.1f, 1, 2, 4, 8, 16, 32, 64 }; // sec
        private int retryCounting;
        private DelayTask delayRequestTask;

        private List<Sprite> sprites;
        private bool isUsed;

        public string AdUnitID => adUnitId;
        public bool IsUsed => isUsed; 

        public override bool IsAvailable
        {
            get
            {
                if(nativeAd != null)
                {
                    return true;
                }
                Request();
                return false;
            }
        }
        public Action<string> OnLoadFail { get; set; }
        public Action<AdmobNativeAd, string> OnLoadSuccess { get; set; }
        public Action<string> OnClicked { get; set; }

        public AdmobNativeAd(string adUnitId)
        {
            this.adUnitId = adUnitId;
        }

        protected override void CallAddEvent()
        {

        }

        private float GetRetryTime(int retry)
        {
            if(retry >= 0 && retry < retryTimes.Length)
            {
                return retryTimes[retry];
            }
            return retryTimes[retryTimes.Length - 1];
        }

        public override void Request()
        {
            if(nativeAd != null)
            {
                return;
            }
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
            Debug.Log($"AdmobNativeAd Request: delay={delayRequest}s, retry={retryCounting}");

            delayRequestTask = new DelayTask(delayRequest, () =>
            {
                AdsEventExecutor.Remove(delayRequestTask);
                CallRequest();

            });
            delayRequestTask.Start();
            AdsEventExecutor.AddTask(delayRequestTask);
        }

        protected override void CallRequest()
        {
            GoogleMobileAds.Api.AdLoader adLoader = new GoogleMobileAds.Api.AdLoader.Builder(adUnitId)
            .ForNativeAd()
            .Build();

            adLoader.OnNativeAdLoaded += this.HandleNativeAdLoaded;
            adLoader.OnAdFailedToLoad += this.HandleAdFailedToLoad;
            adLoader.OnNativeAdClicked += AdLoader_OnNativeAdClicked;
            adLoader.LoadAd(new AdRequest());
        }

        private void AdLoader_OnNativeAdClicked(object sender, EventArgs e)
        {
            OnClicked?.Invoke(adUnitId);
        }

        protected override void CallShow()
        {

        }

        public void Destroy()
        {
            if(nativeAd != null && isUsed)
            {
                isUsed = false;
                nativeAd.Destroy();
                nativeAd = null;
                Request();
            }
        }

        public void SetUsed()
        {
            isUsed = true;
        }

        private void HandleAdFailedToLoad(object sender, GoogleMobileAds.Api.AdFailedToLoadEventArgs args)
        {
            AdsEventExecutor.ExecuteInUpdate(() =>
            {
                requesting = false;
                retryCounting++;
                Debug.Log($"[AdMediation-AdmobNativeAd]: {adUnitId} got HandleAdFailedToLoad {args.LoadAdError.GetMessage()}");
                OnAdLoadFailed(string.Empty);
                OnLoadFail?.Invoke(args.LoadAdError.GetMessage());
            });
        }


        private void HandleNativeAdLoaded(object sender, GoogleMobileAds.Api.NativeAdEventArgs args)
        {
            this.nativeAd = args.nativeAd;
            AdsEventExecutor.ExecuteInUpdate(() =>
            {
                isUsed = false;
                Debug.Log($"[AdMediation-AdmobNativeAd]: {adUnitId} got HandleNativeAdLoaded");
                requesting = false;
                retryCounting = 0;
                OnAdLoadSuccess(new AdInfo());
                OnLoadSuccess?.Invoke(this, adUnitId);
            });
        }

        public List<Sprite> GetSprites()
        {
            if(IsAvailable == false)
            {
                return null;
            }
            sprites = new List<Sprite>();
            var textures = GetImageTextures();
            foreach(var t in textures)
            {
                Sprite sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.one * 0.5f);
                sprites.Add(sprite);
            }
            return sprites;
        }

        #region Get

        public Texture2D GetAdChoicesLogoTexture()
        {
            if(nativeAd == null)
            {
                return null;
            }
            return nativeAd.GetAdChoicesLogoTexture();
        }

        public string GetAdvertiserText()
        {
            if(nativeAd == null)
            {
                return string.Empty;
            }
            return nativeAd.GetAdvertiserText();
        }

        public string GetBodyText()
        {
            if(nativeAd == null)
            {
                return string.Empty;
            }
            return nativeAd.GetBodyText();
        }

        public string GetCallToActionText()
        {
            if(nativeAd == null)
            {
                return string.Empty;
            }
            return nativeAd.GetCallToActionText();
        }

        public string GetHeadlineText()
        {
            if(nativeAd == null)
            {
                return string.Empty;
            }
            return nativeAd.GetHeadlineText();
        }

        public Texture2D GetIconTexture()
        {
            if(nativeAd == null)
            {
                return null;
            }
            return nativeAd.GetIconTexture();
        }

        public List<Texture2D> GetImageTextures()
        {
            if(nativeAd == null)
            {
                return null;
            }
            return nativeAd.GetImageTextures();
        }

        public string GetPrice()
        {
            if(nativeAd == null)
            {
                return string.Empty;
            }
            return nativeAd.GetPrice();
        }

        public double GetStarRating()
        {
            if(nativeAd == null)
            {
                return 0;
            }
            return nativeAd.GetStarRating();
        }

        public string GetStore()
        {
            if(nativeAd == null)
            {
                return string.Empty;
            }
            return nativeAd.GetStore();
        }

        #endregion

        #region Register
        public bool RegisterAdChoicesLogoGameObject(GameObject gameObject)
        {
            if(isUsed == true)
            {
                return false;
            }
            if(nativeAd == null)
            {
                return false;
            }
            if(!nativeAd.RegisterAdChoicesLogoGameObject(gameObject))
            {
                return false;
            }
            return true;
        }

        public bool RegisterAdvertiserTextGameObject(GameObject gameObject)
        {
            if(isUsed == true)
            {
                return false;
            }
            if(nativeAd == null)
            {
                return false;
            }
            if(!nativeAd.RegisterAdvertiserTextGameObject(gameObject))
            {
                return false;
            }
            return true;
        }

        public bool RegisterBodyTextGameObject(GameObject gameObject)
        {
            if(isUsed == true)
            {
                return false;
            }
            if(nativeAd == null)
            {
                return false;
            }
            if(!nativeAd.RegisterBodyTextGameObject(gameObject))
            {
                return false;
            }
            return true;
        }

        public bool RegisterCallToActionGameObject(GameObject gameObject)
        {
            if(isUsed == true)
            {
                return false;
            }
            if(nativeAd == null)
            {
                return false;
            }
            if(!nativeAd.RegisterCallToActionGameObject(gameObject))
            {
                return false;
            }
            return true;
        }

        public bool RegisterHeadlineTextGameObject(GameObject gameObject)
        {
            if(isUsed == true)
            {
                return false;
            }
            if(nativeAd == null)
            {
                return false;
            }
            if(!nativeAd.RegisterHeadlineTextGameObject(gameObject))
            {
                return false;
            }
            return true;
        }

        public bool RegisterIconImageGameObject(GameObject gameObject)
        {
            if(isUsed == true)
            {
                return false;
            }
            if(nativeAd == null)
            {
                return false;
            }
            if(!nativeAd.RegisterIconImageGameObject(gameObject))
            {
                return false;
            }
            return true;
        }

        public int RegisterImageGameObjects(List<GameObject> gameObjects)
        {
            if(isUsed == true)
            {
                return 0;
            }
            if(nativeAd == null)
            {
                return 0;
            }
            return nativeAd.RegisterImageGameObjects(gameObjects);
        }

        public bool RegisterPriceGameObject(GameObject gameObject)
        {
            if(isUsed == true)
            {
                return false;
            }
            if(nativeAd == null)
            {
                return false;
            }
            if(!nativeAd.RegisterPriceGameObject(gameObject))
            {
                return false;
            }
            return true;
        }

        public bool RegisterStoreGameObject(GameObject gameObject)
        {
            if(isUsed == true)
            {
                return false;
            }
            if(nativeAd == null)
            {
                return false;
            }
            if(!nativeAd.RegisterStoreGameObject(gameObject))
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
#endif