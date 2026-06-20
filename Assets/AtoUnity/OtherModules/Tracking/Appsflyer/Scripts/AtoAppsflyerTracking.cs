#if APPSFLYER_ENABLE
using AppsFlyerSDK;
#endif
using AtoGame.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
#if UNITY_IAP_ENABLE
using UnityEngine.Purchasing;
#endif

namespace AtoGame.Tracking.Appsflyer
{
    public class AtoAppsflyerTracking : SingletonFreeAlive<AtoAppsflyerTracking>
#if APPSFLYER_ENABLE
        , IAppsFlyerConversionData
#endif
    {
        public bool isDebug;
        public bool getConversionData;
        [Header("Dev key")]
        public string devKey = "dev_key";
        [Header("App Id")]
        public string appID = "";
        public string UWPAppID;
        public string macOSAppID;

        private bool available = false;

        public bool Available { get => available; }

        protected override void OnAwake()
        {
            base.OnAwake();
            available = false;
#if UNITY_IAP_ENABLE && APPSFLYER_ENABLE
            EventDispatcher.Instance.AddListener<AtoGame.IAP.EventKey.OnBoughtIap>(OnPurchasedIap);
#endif

        }
        public override void Preload()
        {
            base.Preload();
            Init();
        }

        public void SetCUID(string id)
        {
#if APPSFLYER_ENABLE
            AppsFlyer.setCustomerUserId(id);
#endif
        }

        private void Init()
        {
#if APPSFLYER_ENABLE
#if UNITY_IOS && !UNITY_EDITOR
            AppsFlyer.waitForATTUserAuthorizationWithTimeoutInterval(60);
#endif
            AppsFlyer.OnRequestResponse += AppsFlyerOnRequestResponse;
            AppsFlyer.OnInAppResponse += OnInAppResponse;

            //******************************//
            AppsFlyer.setIsDebug(isDebug);
#if UNITY_WSA_10_0 && !UNITY_EDITOR
            AppsFlyer.initSDK(devKey, UWPAppID, getConversionData ? this : null);
#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
            AppsFlyer.initSDK(devKey, macOSAppID, getConversionData ? this : null);
#else
            AppsFlyer.initSDK(devKey, appID, getConversionData ? this : null);
#endif
            //******************************//
            AppsFlyer.startSDK();
            TrackingLogger.Log($"<color=green>[AtoAppsflyerTracking] AppsFlyer Initialized: SDK Version={AppsFlyer.getSdkVersion()}</color>");
#if APPSFLYER_ADREVENUE_ENABLE
            AppsFlyerAdRevenue.start();
            AppsFlyerAdRevenue.setIsDebug(isDebug);
            TrackingLogger.Log($"<color=green>[AtoAppsflyerTracking] AppsFlyerAdRevenue Initialized: SDK Version={AppsFlyerAdRevenue.kAppsFlyerAdRevenueVersion}</color>");
#endif
#else
            TrackingLogger.Log("[AtoAppsflyerTracking] ScriptingDefine is not set: APPSFLYER_ENABLE (PlayerSetting/Scripting Define");
#endif
            available = true;
        }

#region Appsflyer Event Log
        public void LogEvent(string eventName, ParameterBuilder parameterBuilder)
        {
            DebugLog(eventName, parameterBuilder);
#if UNITY_EDITOR
            return;
#endif
            this.Log(eventName, parameterBuilder != null ? parameterBuilder.BuildString() : null);
        }

        private void Log(string eventName, Dictionary<string, string> param)
        {
#if APPSFLYER_ENABLE
            AppsFlyer.sendEvent(eventName, param);
#endif
        }


        private void DebugLog(string eventName, ParameterBuilder parameterBuilder)
        {
            string log = $"[AtoAppsflyerTracking]: EventName = " + eventName + " ";
            if(parameterBuilder != null)
            {
                log += parameterBuilder.DebugLog();
            }
            TrackingLogger.Log(log);
        }
        #endregion

        #region Appsflyer Adrevenue Log
#if APPSFLYER_ADREVENUE_ENABLE
        public void LogAdRevenue(string monetizationNetwork,
            string mediationNetwork,
            double eventRevenue,
            string revenueCurrency,
            Dictionary<string, string> additionalParameters)
        {
            AppsFlyerSDK.AppsFlyerAdRevenueMediationNetworkType type = AppsFlyerSDK.AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeIronSource;
            if(mediationNetwork == "admob")
            {
                type = AppsFlyerSDK.AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeGoogleAdMob;
            }
            else if(mediationNetwork == "ironSource")
            {
                type = AppsFlyerSDK.AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeIronSource;
            }
            else if(mediationNetwork == "applovin")
            {
                type = AppsFlyerSDK.AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeApplovinMax;
            }

            AppsFlyerAdRevenue.logAdRevenue(monetizationNetwork, type, eventRevenue, revenueCurrency, additionalParameters);
        }
#endif
        #endregion

        #region Appsflyer Callbacks

        void AppsFlyerOnRequestResponse(object sender, EventArgs e)
        {
#if APPSFLYER_ENABLE
            var args = e as AppsFlyerRequestEventArgs;
            AppsFlyer.AFLog("AppsFlyerOnRequestResponse", " status code " + args.statusCode);
#endif
        }

        void OnInAppResponse(object sender, EventArgs e)
        {
#if APPSFLYER_ENABLE
            var af_args = e as AppsFlyerRequestEventArgs;
            AppsFlyer.AFLog("AppsFlyerOnRequestResponse", " status code " + af_args.statusCode);
#endif
        }


        public void onConversionDataSuccess(string conversionData)
        {
#if APPSFLYER_ENABLE
            AppsFlyer.AFLog("onConversionDataSuccess", conversionData);
            Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
            TrackingLogger.Log("[AtoAppsflyerTracking]: OnConversionData Success");
            // add deferred deeplink logic here
#endif
        }

        public void onConversionDataFail(string error)
        {
#if APPSFLYER_ENABLE
            AppsFlyer.AFLog("onConversionDataFail", error);
            TrackingLogger.Log("[AtoAppsflyerTracking]: OnConversionData Failed: " + error);
#endif
        }

        public void onAppOpenAttribution(string attributionData)
        {
#if APPSFLYER_ENABLE
            AppsFlyer.AFLog("onAppOpenAttribution: This method was replaced by UDL. This is a fake call.", attributionData);
            Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
            // add direct deeplink logic here
            TrackingLogger.Log("[AtoAppsflyerTracking]: onAppOpenAttribution-" + attributionData.ToString());
#endif
        }

        public void onAppOpenAttributionFailure(string error)
        {
#if APPSFLYER_ENABLE
            AppsFlyer.AFLog("onAppOpenAttributionFailure: This method was replaced by UDL. This is a fake call.", error);
            TrackingLogger.Log("[AtoAppsflyerTracking]: onAppOpenAttributionFailure Failed: " + error);
#endif
        }
#endregion

#region For Validate IAP
#if UNITY_IAP_ENABLE && APPSFLYER_ENABLE

        private void OnPurchasedIap(IAP.EventKey.OnBoughtIap param)
        {
            AppsFlyerPurchaseEvent(param.Product);
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
        public static void AppsFlyerPurchaseEvent (Product product)
        {
            Dictionary<string, string> eventValue = new Dictionary<string, string> ();
            eventValue.Add ("af_revenue", GetAppsflyerRevenue (product.metadata.localizedPrice));
            eventValue.Add ("af_content_id", product.definition.id);
            eventValue.Add ("af_currency", product.metadata.isoCurrencyCode);
            AppsFlyer.sendEvent ("af_purchase", eventValue);
        }

        public static string GetAppsflyerRevenue (decimal amount) 
        {
            decimal val = decimal.Multiply (amount, 0.63m);
            return val.ToString ();
        }

#endif
#endregion
    }
}