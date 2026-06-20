using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
#if ATO_ADMOB_MEDIATION_ENABLE || ATO_ADMOB_ENABLE
    public static class AdmobHelper
    {
        public static AdInfo Convert(this GoogleMobileAds.Api.AdValue adInfo, string instanceName, string adUnit)
        {
            return new AdInfo()
            {
                adPlatform = ImpressionData.ADMOB_PLATFORM_NAME,
                auctionId = string.Empty,
                adUnit = adUnit,
                country = string.Empty,
                ab = string.Empty,
                segmentName = string.Empty,
                adNetwork = ImpressionData.ADMOB_PLATFORM_NAME,
                instanceName = instanceName,
                instanceId = string.Empty,
                revenue = adInfo.Value,
                precision = adInfo.Precision.ToString(),
                lifetimeRevenue = null,
                encryptedCPM = string.Empty
            };
        }

        public static ImpressionData ConvertToImpression(this GoogleMobileAds.Api.AdValue adInfo, string instanceName, string adUnit)
        {
            return new ImpressionData()
            {
                adPlatform = ImpressionData.ADMOB_PLATFORM_NAME,
                auctionId = string.Empty,
                adUnit = adUnit,
                country = string.Empty,
                ab = string.Empty,
                segmentName = string.Empty,
                placement = string.Empty,
                adNetwork = ImpressionData.ADMOB_PLATFORM_NAME,
                instanceName = instanceName,
                instanceId = string.Empty,
                revenue = adInfo.Value,
                precision = adInfo.Precision.ToString(),
                lifetimeRevenue = null,
                encryptedCPM = string.Empty,
                conversionValue = null,
                allData = string.Empty
            };
        }

        public static GoogleMobileAds.Api.AdSize GetBannerSize(BannerSize size)
        {
            switch(size)
            {
                case BannerSize.BANNER:
                {
                    return GoogleMobileAds.Api.AdSize.Banner;
                }
                case BannerSize.LARGE:
                {
                    return GoogleMobileAds.Api.AdSize.Leaderboard;
                }
                case BannerSize.RECTANGLE:
                {
                    return GoogleMobileAds.Api.AdSize.MediumRectangle;
                }
                default:
                {
                    return GoogleMobileAds.Api.AdSize.Banner;
                }
            }
        }

        public static GoogleMobileAds.Api.AdPosition GetBannerPosition(BannerPosition position)
        {
            switch(position)
            {
                case BannerPosition.BOTTOM_CENTER:
                {
                    return GoogleMobileAds.Api.AdPosition.Bottom;
                }
                case BannerPosition.BOTTOM_LEFT:
                {
                    return GoogleMobileAds.Api.AdPosition.BottomLeft;
                }
                case BannerPosition.BOTTOM_RIGHT:
                {
                    return GoogleMobileAds.Api.AdPosition.BottomRight;
                }
                case BannerPosition.CENTERED:
                {
                    return GoogleMobileAds.Api.AdPosition.Center;
                }
                case BannerPosition.CENTER_LEFT:
                {
                    return GoogleMobileAds.Api.AdPosition.Center;
                }
                case BannerPosition.CENTER_RIGHT:
                {
                    return GoogleMobileAds.Api.AdPosition.Center;
                }
                case BannerPosition.TOP_CENTER:
                {
                    return GoogleMobileAds.Api.AdPosition.Top;
                }
                case BannerPosition.TOP_LEFT:
                {
                    return GoogleMobileAds.Api.AdPosition.TopLeft;
                }
                case BannerPosition.TOP_RIGHT:
                {
                    return GoogleMobileAds.Api.AdPosition.TopRight;
                }
                default:
                {
                    return GoogleMobileAds.Api.AdPosition.Bottom;
                }
            }
        }
    }
#endif
}
