using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IAP_ENABLE
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;

namespace AtoGame.AtoUnity.IAP
{
    public class IapPurchaseController : IapSingleton<IapPurchaseController>, IDetailedStoreListener
    {
        [SerializeField] private bool enableLogs;
        [SerializeField] private bool useAppleStoreKitTestCertificate = false;
        [SerializeField] private FakeStoreUIMode fakeStoreUIMode;
        [SerializeField] private IapProductInfo[] productInfos;
        [SerializeField] private bool useBackendValidation;

        IStoreController m_StoreController;
        private IExtensionProvider m_StoreExtensionProvider;
        CrossPlatformValidator m_Validator = null;
        private Action onInitSuccessed;
        private Action onInitFailed;
        private Action<string> onBuyCompleted;
        private Action<int> onBuyFailed;
        private bool isRestoring;
        private bool isPurchasing;

        public const char DefaultSymbol = '$';
        public const string DefaultIsoCurrencyCode = "USD";
        private static readonly Meta emptyMeta = new Meta(0, DefaultSymbol, DefaultIsoCurrencyCode);

#region Initialize
        public bool IsInitialized()
        {
            var isInitialized = m_StoreController != null && m_StoreExtensionProvider != null;
            if (!isInitialized)
            {
                LogError("Not initialized.");
            }

            return isInitialized;
        }

        public void Initialize()
        {
            AtoUnityInitializer.AddOnInitialized(InitializePurchasing);
        }

        private void InitializePurchasing(bool result, string errorMessage)
        {
            if(result == false)
            {
                LogError(errorMessage);
            }
            StandardPurchasingModule standardPurchasingModule = StandardPurchasingModule.Instance();
#if UNITY_EDITOR
            standardPurchasingModule.useFakeStoreAlways = true;
#else
            standardPurchasingModule.useFakeStoreAlways = false;
#endif
            standardPurchasingModule.useFakeStoreUIMode = fakeStoreUIMode;
            var builder = ConfigurationBuilder.Instance(standardPurchasingModule);

            for (int i = 0; i < productInfos.Length; ++i)
            {
                IapProductInfo p = productInfos[i];
                builder.AddProduct(p.Id, p.ProductType, p.GetIDs());
            }

            UnityPurchasing.Initialize(this, builder);
        }

        void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Log("OnInitialized");
            isRestoring = false; 
            isPurchasing = false;
            m_StoreController = controller;
            m_StoreExtensionProvider = extensions;
            InitializeValidator();
            onInitSuccessed?.Invoke();
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
        {
            LogError($"OnInitializeFailed[Obsolete] error: {error}");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            LogError($"OnInitializeFailed error: {error} - {message}");
            onInitFailed?.Invoke();
        }

#endregion
#region Store Validator
        void InitializeValidator()
        {
            if (IsCurrentStoreSupportedByValidator())
            {
#if !UNITY_EDITOR
                var appleTangleData = useAppleStoreKitTestCertificate ? AppleStoreKitTestTangle.Data() : AppleTangle.Data();
                m_Validator = new CrossPlatformValidator(GooglePlayTangle.Data(), appleTangleData, Application.identifier);
#endif
            }
            else
            {
                var warningMsg = $"The cross-platform validator is not implemented for the currently selected store: {StandardPurchasingModule.Instance().appStore}. \n" +
                            "Build the project for Android, iOS, macOS, or tvOS and use the Google Play Store or Apple App Store. See README for more information.";
                Log(warningMsg);
            }
        }

        bool IsPurchaseValid(Product product)
        {
#if UNITY_EDITOR
            return true;
#endif
            //If we the validator doesn't support the current store, we assume the purchase is valid
            if (IsCurrentStoreSupportedByValidator())
            {
                try
                {
                    var result = m_Validator.Validate(product.receipt);
                }
                //If the purchase is deemed invalid, the validator throws an IAPSecurityException.
                catch (IAPSecurityException reason)
                {
                    Debug.Log($"Invalid receipt: {reason}");
                    return false;
                }
            }

            return true;
        }

        static bool IsCurrentStoreSupportedByValidator()
        {
            //The CrossPlatform validator only supports the GooglePlayStore and Apple's App Stores.
            return IsGooglePlayStoreSelected() || IsAppleAppStoreSelected();
        }

        static bool IsGooglePlayStoreSelected()
        {
            var currentAppStore = StandardPurchasingModule.Instance().appStore;
            return currentAppStore == AppStore.GooglePlay;
        }

        static bool IsAppleAppStoreSelected()
        {
            var currentAppStore = StandardPurchasingModule.Instance().appStore;
            return currentAppStore == AppStore.AppleAppStore ||
                   currentAppStore == AppStore.MacAppStore;
        }
#endregion
#region Store Purchasing
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            //Retrieve the purchased product
            var product = args.purchasedProduct;

            var isPurchaseValid = IsPurchaseValid(product);

            if (isPurchaseValid == true)
            {
                if(isPurchasing == true)
                {
                   
                    isPurchasing = false;
                    if (onBuyCompleted != null) onBuyCompleted.Invoke(product.definition.id);
					AtoGame.Base.EventDispatcher.Instance.Dispatch(new AtoGame.IAP.EventKey.OnBoughtIap()
                    {
                        Product = product
                    });
                }
                else
                {
                    // is restoring
                    Log($"PurchaseProcessingResult:{product.definition.id} - {isRestoring}");
                    IapProductInfo iapProductInfo = GetProductInfo(product.definition.id);
                    if(iapProductInfo != null)
                    {
                        iapProductInfo.Restoring();
                    }
                }
            }
            else
            {
               // Debug.Log("Invalid receipt, not unlocking content.");
            }

            if(useBackendValidation == true)
            {
                return PurchaseProcessingResult.Pending;
            }
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Log("OnPurchaseFailed: " + failureDescription.ToString());
            if(isPurchasing == true)
            {
                if(onBuyFailed != null)
                {
                    onBuyFailed.Invoke((int)failureDescription.reason);
                }
                isPurchasing = false;
            }
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Log("OnPurchaseFailed: " + failureReason.ToString());
            if (isPurchasing == true)
            {
                if (onBuyFailed != null)
                {
                    onBuyFailed.Invoke((int)failureReason);
                }
                isPurchasing = false;
            }
        }

        public void ConfirmPendingPurchase(string productKey)
        {
            if (IsInitialized() && useBackendValidation)
            {
                Product product = GetProduct(productKey);
                m_StoreController.ConfirmPendingPurchase(product);
            }
        }

        public virtual void Buy(string productId, Action<string> onBuyCompleted = null, Action<int> onBuyFailed = null)
        {
            if (isPurchasing)
            {
                onBuyFailed?.Invoke(-1);
                return;
            }
            this.onBuyCompleted = onBuyCompleted;
            this.onBuyFailed = onBuyFailed;
            if (IsInitialized())
            {
                Product product = m_StoreController.products.WithID(productId);
                if (product != null)
                {
                    if (product.availableToPurchase)
                    {
                        isPurchasing = true;
                        Log($"Purchasing product asychronously: '{product.definition.id}'");
                        m_StoreController.InitiatePurchase(product);
                        return;
                    }
                    else
                    {
                        Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                    }
                }
            }
            onBuyFailed?.Invoke(-2);
        }

        public void RestorePurchases(System.Action success = null, Action onFailed = null)
        {
            if (IsInitialized() == false)
            {
                Log("RestorePurchases FAIL. Not Initialized");
                onFailed?.Invoke();
                return;
            }
            if (isRestoring == true)
            {
                Log("RestorePurchases FAIL. Restoring process is running");
                onFailed?.Invoke();
                return;
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                    Application.platform == RuntimePlatform.OSXPlayer)
            {
                Log("RestorePurchases started ...");

                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                isRestoring = true;
                apple.RestoreTransactions((result, error) =>
                {
                    if (result)
                    {
                        success.Invoke();
                    }
                    else
                    {
                        onFailed?.Invoke();
                    }
                    isRestoring = false;
                });
            }
            else
            {
                Log($"RestorePurchases FAIL. Not supported on this platform. Current = {Application.platform}");
                onFailed?.Invoke();
            }
        }

#endregion
#region Add Listener
        public void AddOnInitIapSuccessed(Action onInitSuccessed)
        {
            this.onInitSuccessed += onInitSuccessed;
        }

        public void RemoveOnInitIapSuccessed(Action onInitSuccessed)
        {
            this.onInitSuccessed -= onInitSuccessed;
        }

        public void AddOnInitIapFailed(Action onInitFailed)
        {
            this.onInitFailed += onInitFailed;
        }

        public void RemoveOnInitIapFailed(Action onInitFailed)
        {
            this.onInitFailed -= onInitFailed;
        }
#endregion
#region Logs

        public void Log(string log)
        {
            if (enableLogs)
            {
                Debug.LogFormat($"[AtoGame-IAP] - {log}");
            }
        }

        public void LogWarning(string log)
        {
            if (enableLogs)
            {
                Debug.LogWarningFormat($"[AtoGame-IAP] - {log}");
            }
        }

        public void LogError(string log)
        {
            if (enableLogs)
            {
                Debug.LogErrorFormat($"[AtoGame-IAP] - {log}");
            }
        }



#endregion
#region Others

        public bool IsOwned(string productID)
        {
            if (!IsInitialized()) return false;
            var product = m_StoreController.products.WithID(productID);
            if (product == null) return false;
            return product.hasReceipt;
        }

        public IapProductInfo GetProductInfo(string key)
        {
            for(int i = 0; i < productInfos.Length; ++i)
            {
                if(productInfos[i].Id.Equals(key))
                {
                    return productInfos[i];
                }
            }
            return null;
        }

        public Meta GetLocalPrice(string id, Decimal defaultPrice = 0, char defaultSymbol = DefaultSymbol,
        string defaultCurencyCode = DefaultIsoCurrencyCode)
        {
            if (m_StoreController != null)
            {
                Product product = m_StoreController.products.WithID(id);
                if (product != null)
                {
                    var productMetadata = m_StoreController.products.WithID(id).metadata;
                    return new Meta(productMetadata.localizedPrice, productMetadata.localizedPriceString,
                        productMetadata.isoCurrencyCode);
                }
            }

            if (defaultPrice > 0)
            {
                return new Meta(defaultPrice, defaultSymbol, defaultCurencyCode);
            }

            return emptyMeta;
        }
        public Product GetProduct(string id)
        {
            if (!IsInitialized()) return null;
            return m_StoreController.products.WithID(id);
        }

        protected override void OnAwake()
        {
        }




#endregion
    }
}

#endif