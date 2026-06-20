using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IAP_ENABLE
using UnityEngine.Purchasing;

namespace AtoGame.AtoUnity.IAP
{
    [System.Serializable]
    public class IapProductInfo
    {
        public string Id;
        public UnityEngine.Purchasing.ProductType ProductType;
        public string GoogleStoreId;
        public string AppleStoreId;
        public IapAction RestoreAction;

        public IDs GetIDs()
        {
            IDs productIds = new IDs();
            productIds.Add(Id, string.Empty);
            if(string.IsNullOrEmpty(GoogleStoreId) == false)
            {
                productIds.Add(GoogleStoreId, GooglePlay.Name);
            }
            if (string.IsNullOrEmpty(AppleStoreId) == false)
            {
                productIds.Add(AppleStoreId, MacAppStore.Name);
            }
            return productIds;
        }

        public void Restoring()
        {
            RestoreAction?.Execute(Id);
        }
    }

    public class Meta
    {
        public readonly string isoCurrencyCode;
        public readonly string localizedPriceString;
        public readonly Decimal localizedPrice;
        public readonly char symbol;

        public Meta(decimal localizedPrice, char symbol, string isoCurrencyCode)
        {
            this.isoCurrencyCode = isoCurrencyCode;
            this.localizedPrice = localizedPrice;
            if (!string.IsNullOrEmpty(isoCurrencyCode))
            {
                localizedPriceString = this.localizedPrice + " " + isoCurrencyCode;
            }

            this.symbol = symbol;
        }

        public Meta(decimal localizedPrice, string localizedPriceString, string isoCurrencyCode)
        {

            this.isoCurrencyCode = isoCurrencyCode;
            this.localizedPriceString = localizedPriceString;
            this.localizedPrice = localizedPrice;

            if (string.IsNullOrEmpty(this.localizedPriceString))
            {
                symbol = IapPurchaseController.DefaultSymbol;
            }
            else
            {
                if (!char.IsDigit(this.localizedPriceString[0]))
                {
                    symbol = this.localizedPriceString[0];
                }
                else if (!char.IsDigit(this.localizedPriceString[this.localizedPriceString.Length - 1]))
                {
                    symbol = this.localizedPriceString[this.localizedPriceString.Length - 1];
                }
                else
                {
                    symbol = IapPurchaseController.DefaultSymbol;
                }
            }

        }
    }
}
#endif