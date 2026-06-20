using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AtoGame.Mediation
{
#if ATO_IRONSOURCE_MEDIATION_ENABLE
    [CreateAssetMenu(fileName = "AdIronSourceSettings", menuName = "Data/AdMediation/IronSource/AdIronSourceSettings")]
    public class AdIronSourceSettings : ScriptableObject
    {
        [Header("App Key")]
        [SerializeField] private string androidAppKey = "";
        [SerializeField] private string iosAppKey = "";
        [Header("Test App Key")]
        [SerializeField] private string androidTestAppKey = "85460dcd";
        [SerializeField] private string iosTestAppKey = "8545d445";
        [SerializeField] private bool useTestAd = false;
        private const string otherAppKey = "unexpected_platform";
        [Space()]
        [Header("Mediation")]
        [SerializeField] private IronSourceAdUnit adUnitType = IronSourceAdUnit.DEFAULT;
        [Space()]
        [Header("Test Suite")]
        [SerializeField] private bool useTestSuite = false;
        [Space()]
        [Header("Banner")]
        [SerializeField] private IronSourceBannerPosition defaultBannerPosition = IronSourceBannerPosition.BOTTOM;
        [SerializeField] private bool useAdaptiveBanner;
        [SerializeField] private BannerSize bannerSize;
        [SerializeField] private int bannerWidth;
        [SerializeField] private int bannerHeight;
        [Header("Privacy")]
        [SerializeField] private bool hasUserConsent = true;
        [SerializeField] private bool isAgeRestrictedUser = false;
        [SerializeField] private bool doNotSell = false;
        [SerializeField] private bool isLimitedDataUse = false;
        [SerializeField] private bool useFacebookAd = false;

        [Space()]
        [Header("iOS Build")]
        [SerializeField] private bool setAttributionReportEndpoint = true;
        [SerializeField] private bool needAppTransportSecuritySettings = false; // AdColony, ByteDance, Fyber, Google, GoogleAdManager, HyprMX, InMobi, IronSource, Smaato

        private IronSourceBannerSize defaultIronSourceBannerSize;

        private static AdIronSourceSettings instance;

        public static AdIronSourceSettings Instance
        {
            get
            {
#if UNITY_EDITOR
                if (instance == null)
                {
                    string settingsFilePath = "Assets/AdMediation/IronSource/Resources/AdIronSourceSettings.asset";
                    var settingsDir = Path.GetDirectoryName(settingsFilePath);
                    if (!Directory.Exists(settingsDir))
                    {
                        Directory.CreateDirectory(settingsDir);
                    }

                    instance = AssetDatabase.LoadAssetAtPath<AdIronSourceSettings>(settingsFilePath);
                    if (instance != null) return instance;

                    instance = CreateInstance<AdIronSourceSettings>();
                    AssetDatabase.CreateAsset(instance, settingsFilePath);
                }
#else
                if (instance == null)
                {
                    instance = Resources.Load<AdIronSourceSettings>("AdIronSourceSettings");
                }
#endif
                return instance;
            }
        }
        public string AppKey
        {
            get
            {
                if (useTestAd)
                {
#if UNITY_ANDROID
                    return androidTestAppKey;
#elif UNITY_IPHONE
                    return iosTestAppKey;
#else
                    return otherAppKey;
#endif
                }
#if UNITY_ANDROID
                return androidAppKey;
#elif UNITY_IPHONE
                return iosAppKey;
#else
                return otherAppKey;
#endif
            }
        }
        public IronSourceAdUnit AdUnitType => adUnitType;
        public bool UseTestSuite => useTestSuite;
        public IronSourceBannerSize DefaultBannerSize
        {
            get
            {
                if(defaultIronSourceBannerSize == null)
                {
                    defaultIronSourceBannerSize = IronSourceHelper.GetBannerSize(useAdaptiveBanner, bannerSize, bannerWidth, bannerHeight);
                }
                return defaultIronSourceBannerSize;
            }
        }
        public IronSourceBannerPosition DefaultBannerPosition => defaultBannerPosition;
        public bool HasUserConsent => hasUserConsent;
        public bool IsAgeRestrictedUser => isAgeRestrictedUser;
        public bool DoNotSell => doNotSell;
        public bool IsLimitedDataUse => isLimitedDataUse;
        public bool SetAttributionReportEndpoint => setAttributionReportEndpoint;
        public bool NeedAppTransportSecuritySettings => needAppTransportSecuritySettings;
        public bool UseFacebookAd => useFacebookAd;
    }
#endif
}
