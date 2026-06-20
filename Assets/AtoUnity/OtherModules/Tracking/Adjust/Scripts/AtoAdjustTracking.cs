using AtoGame.Base;
using AtoGame.Mediation;

#if ADJUST_ENABLE
using com.adjust.sdk;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
#if UNITY_IAP_ENABLE
using UnityEngine.Purchasing;
#endif

namespace AtoGame.Tracking.Adjust
{
    public class AtoAdjustTracking : SingletonFreeAlive<AtoAdjustTracking>
    {
#if ADJUST_ENABLE
        public bool startManually = true;
        public string appToken;
        public AdjustEnvironment environment = AdjustEnvironment.Sandbox;
        public AdjustLogLevel logLevel = AdjustLogLevel.Info;
        public bool eventBuffering = false;
        public bool sendInBackground = false;
        public bool launchDeferredDeeplink = true;
        public bool needsCost = false;
        public bool coppaCompliant = false;
        public bool linkMe = false;
        public string defaultTracker;
        public AdjustUrlStrategy urlStrategy = AdjustUrlStrategy.Default;
        public double startDelay = 0;

        [Header("APP SECRET:")]
        [Space(5)]
        public long secretId = 0;
        public long info1 = 0;
        public long info2 = 0;
        public long info3 = 0;
        public long info4 = 0;

        [Header("ANDROID SPECIFIC FEATURES:")]
        [Space(5)]
        public bool preinstallTracking = false;
        public string preinstallFilePath;
        public bool playStoreKidsApp = false;

        [Header("iOS SPECIFIC FEATURES:")]
        [Space(5)]
        public bool adServicesInfoReading = true;
        public bool idfaInfoReading = true;
        public bool skAdNetworkHandling = true;

        [Header("Purchase Tracking")]
        public string purchaseToken = "zzzzzz";



        private bool available = false;

        public bool Available { get => available; }


        protected override void OnAwake()
        {
            base.OnAwake();
            available = false;
#if UNITY_IAP_ENABLE
            EventDispatcher.Instance.AddListener<AtoGame.IAP.EventKey.OnBoughtIap>(OnPurchasedIap);
#endif
        }


        public override void Preload()
        {
            base.Preload();
            Init();
        }

        private void Init()
        {
            //AdjustConfig adjustConfig = new AdjustConfig(this.appToken, this.environment, (this.logLevel == AdjustLogLevel.Suppress));
            AdjustConfig adjustConfig = new AdjustConfig(this.appToken, this.environment, true);
            adjustConfig.setLogLevel(this.logLevel);
            adjustConfig.setSendInBackground(this.sendInBackground);
            adjustConfig.setEventBufferingEnabled(this.eventBuffering);
            adjustConfig.setLaunchDeferredDeeplink(this.launchDeferredDeeplink);
            adjustConfig.setDefaultTracker(this.defaultTracker);
            adjustConfig.setUrlStrategy(this.urlStrategy.ToLowerCaseString());
            adjustConfig.setAppSecret(this.secretId, this.info1, this.info2, this.info3, this.info4);
            adjustConfig.setDelayStart(this.startDelay);
            adjustConfig.setNeedsCost(this.needsCost);
            adjustConfig.setPreinstallTrackingEnabled(this.preinstallTracking);
            adjustConfig.setPreinstallFilePath(this.preinstallFilePath);
            adjustConfig.setAllowAdServicesInfoReading(this.adServicesInfoReading);
            adjustConfig.setAllowIdfaReading(this.idfaInfoReading);
            adjustConfig.setCoppaCompliantEnabled(this.coppaCompliant);
            adjustConfig.setPlayStoreKidsAppEnabled(this.playStoreKidsApp);
            adjustConfig.setLinkMeEnabled(this.linkMe);
            if(!skAdNetworkHandling)
            {
                adjustConfig.deactivateSKAdNetworkHandling();
            }
            if(logLevel == AdjustLogLevel.Verbose)
            {
                adjustConfig.setLogDelegate(msg => Debug.Log(msg));
            }
            com.adjust.sdk.Adjust.start(adjustConfig);
            TrackingLogger.Log($"<color=green>[AtoAdjustTracking] Call Initialize: SDK Version={com.adjust.sdk.Adjust.getSdkVersion()}</color>");
        }

        #region Event Log
        public void LogEvent(string eventName, ParameterBuilder parameterBuilder)
        {
            DebugLog(eventName, parameterBuilder);
#if UNITY_EDITOR
            return;
#endif


            AdjustEvent adjustEvent = null;
            if(parameterBuilder != null)
            {
                adjustEvent = parameterBuilder.BuildAdjust(eventName);
            }
            else
            {
                adjustEvent = new AdjustEvent(eventName);
            }
            Log(adjustEvent);
        }

        private void Log(AdjustEvent adjustEvent)
        {
            com.adjust.sdk.Adjust.trackEvent(adjustEvent);
        }

        private void DebugLog(string eventName, ParameterBuilder parameterBuilder)
        {
            StringBuilder paramLogs = new StringBuilder();
            if(parameterBuilder != null && parameterBuilder.Params.Count > 0)
            {
                paramLogs.Append(" /");
                foreach(KeyValuePair<string, object> entry in parameterBuilder.Params)
                {
                    paramLogs.Append(" " + entry.Key + "=" + entry.Value.ToString());
                }
            }
            TrackingLogger.Log("[AtoAdjustTracking] EventName = " + eventName + paramLogs.ToString());
        }

        #endregion


        #region Adrevenue Log
#if ADJUST_ENABLE
        public static void LogAdRevenue(string adNetwork,
      string adPlatform,
      double eventRevenue,
      string revenueCurrency,
      string placement)
        {
            string type = AdjustConfig.AdjustAdRevenueSourceIronSource;
            if(adPlatform == ImpressionData.ADMOB_PLATFORM_NAME)
            {
                type = AdjustConfig.AdjustAdRevenueSourceAdMob;
            }
            else if(adPlatform == ImpressionData.IRONSOURCE_PLATFORM_NAME)
            {
                type = AdjustConfig.AdjustAdRevenueSourceIronSource;
            }
            else if(adPlatform == ImpressionData.MAX_PLATFORM_NAME)
            {
                type = AdjustConfig.AdjustAdRevenueSourceAppLovinMAX;
            }


            AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(type);
            adjustAdRevenue.setRevenue(eventRevenue, "USD");
            // optional fields
            adjustAdRevenue.setAdRevenueNetwork(adNetwork);
            adjustAdRevenue.setAdRevenueUnit(revenueCurrency);
            adjustAdRevenue.setAdRevenuePlacement(placement);
            // track Adjust ad revenue
            com.adjust.sdk.Adjust.trackAdRevenue(adjustAdRevenue);
        }
#endif
        #endregion

        #region For Validate IAP
#if UNITY_IAP_ENABLE && ADJUST_ENABLE

        private void OnPurchasedIap(IAP.EventKey.OnBoughtIap param)
        {
            PurchaseEvent(param.Product);
        }

        /// <summary>
        /// Gọi hàm này sau khi thực hiện purchase thành công. (dành riêng cho game có IAP chiếm trên 20%)
        /// if (validPurchase) {
        ///     Unlock the appropriate content here.
        ///     Tracking.Appsflyer.AtoAppsflyerTracking.AppsFlyerPurchaseEvent(e.purchasedProduct);
        /// }
        /// return PurchaseProcessingResult.Complete;
        /// </summary>
        /// <param name="product"></param>
        public static void PurchaseEvent (Product product)
        {
            AdjustEvent adjustEvent = new AdjustEvent(Instance.purchaseToken);
            adjustEvent.addCallbackParameter("aj_revenue", GetAppsflyerRevenue(product.metadata.localizedPrice));
            adjustEvent.addCallbackParameter("aj_content_id", product.definition.id);
            adjustEvent.addCallbackParameter("aj_currency", product.metadata.isoCurrencyCode);
            adjustEvent.setTransactionId(product.transactionID);
            adjustEvent.setRevenue((double)product.metadata.localizedPrice, product.metadata.isoCurrencyCode);
            com.adjust.sdk.Adjust.trackEvent(adjustEvent);
        }

        public static string GetAppsflyerRevenue (decimal amount) 
        {
            decimal val = decimal.Multiply (amount, 0.63m);
            return val.ToString ();
        }

#endif
        #endregion
#endif
    }

}
