using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
#if ATO_MAX_MEDIATION_ENABLE
    public static class MaxHelper
    {
        public static AdInfo Convert(this MaxSdkBase.AdInfo adInfo)
        {
            return new AdInfo()
            {
                adPlatform = ImpressionData.MAX_PLATFORM_NAME,
                auctionId = string.Empty,
                adUnit = adInfo.AdFormat,
                country = MaxSdk.GetSdkConfiguration().CountryCode,
                ab = string.Empty,
                segmentName = string.Empty,
                adNetwork = adInfo.NetworkName,
                instanceName = adInfo.AdUnitIdentifier,
                instanceId = adInfo.AdUnitIdentifier,
                revenue = adInfo.Revenue,
                precision = adInfo.RevenuePrecision,
                lifetimeRevenue = null,
                encryptedCPM = string.Empty
            };
        }

        public static ImpressionData ConvertToImpression(this MaxSdkBase.AdInfo adInfo)
        {
            return new ImpressionData()
            {
                adPlatform = ImpressionData.MAX_PLATFORM_NAME,
                auctionId = string.Empty,
                adUnit = adInfo.AdFormat,
                country = MaxSdk.GetSdkConfiguration().CountryCode,
                ab = string.Empty,
                segmentName = string.Empty,
                placement = adInfo.Placement,
                adNetwork = adInfo.NetworkName,
                instanceName = adInfo.AdUnitIdentifier,
                instanceId = adInfo.AdUnitIdentifier,
                revenue = adInfo.Revenue,
                precision = adInfo.RevenuePrecision,
                lifetimeRevenue = null,
                encryptedCPM = string.Empty,
                conversionValue = null,
                allData = string.Empty
            };
        }

        public static MaxSdkBase.BannerPosition GetBannerPosition(BannerPosition position)
        {
            switch (position)
            {
                case BannerPosition.BOTTOM_CENTER:
                {
                    return MaxSdkBase.BannerPosition.BottomCenter;
                }
                case BannerPosition.BOTTOM_LEFT:
                {
                    return MaxSdkBase.BannerPosition.BottomLeft;
                }
                case BannerPosition.BOTTOM_RIGHT:
                {
                    return MaxSdkBase.BannerPosition.BottomRight;
                }
                case BannerPosition.CENTERED:
                {
                    return MaxSdkBase.BannerPosition.Centered;
                }
                case BannerPosition.CENTER_LEFT:
                {
                    return MaxSdkBase.BannerPosition.CenterLeft;
                }
                case BannerPosition.CENTER_RIGHT:
                {
                    return MaxSdkBase.BannerPosition.CenterRight;
                }
                case BannerPosition.TOP_CENTER:
                {
                    return MaxSdkBase.BannerPosition.TopCenter;
                }
                case BannerPosition.TOP_LEFT:
                {
                    return MaxSdkBase.BannerPosition.TopLeft;
                }
                case BannerPosition.TOP_RIGHT:
                {
                    return MaxSdkBase.BannerPosition.TopRight;
                }
                default:
                {
                    return MaxSdkBase.BannerPosition.BottomCenter;
                }
            }
        }

        public static MaxSdkBase.AdViewPosition GetViewPosition(BannerPosition position)
        {
            switch (position)
            {
                case BannerPosition.BOTTOM_CENTER:
                {
                    return MaxSdkBase.AdViewPosition.BottomCenter;
                }
                case BannerPosition.BOTTOM_LEFT:
                {
                    return MaxSdkBase.AdViewPosition.BottomLeft;
                }
                case BannerPosition.BOTTOM_RIGHT:
                {
                    return MaxSdkBase.AdViewPosition.BottomRight;
                }
                case BannerPosition.CENTERED:
                {
                    return MaxSdkBase.AdViewPosition.Centered;
                }
                case BannerPosition.CENTER_LEFT:
                {
                    return MaxSdkBase.AdViewPosition.CenterLeft;
                }
                case BannerPosition.CENTER_RIGHT:
                {
                    return MaxSdkBase.AdViewPosition.CenterRight;
                }
                case BannerPosition.TOP_CENTER:
                {
                    return MaxSdkBase.AdViewPosition.TopCenter;
                }
                case BannerPosition.TOP_LEFT:
                {
                    return MaxSdkBase.AdViewPosition.TopLeft;
                }
                case BannerPosition.TOP_RIGHT:
                {
                    return MaxSdkBase.AdViewPosition.TopRight;
                }
                default:
                {
                    return MaxSdkBase.AdViewPosition.BottomCenter;
                }
            }
        }
    }
#endif
}
