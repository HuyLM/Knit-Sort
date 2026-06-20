using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
#if ATO_IRONSOURCE_MEDIATION_ENABLE
    public static class IronSourceHelper
    {
        public static AdInfo Convert(this IronSourceAdInfo adInfo)
        {
            return new AdInfo()
            {
                adPlatform = "ironSource",
                auctionId = adInfo.auctionId,
                adUnit = adInfo.adUnit,
                country = adInfo.country,
                ab = adInfo.ab,
                segmentName = adInfo.segmentName,
                adNetwork = adInfo.adNetwork,
                instanceName = adInfo.instanceName,
                instanceId = adInfo.instanceId,
                revenue = adInfo.revenue,
                precision = adInfo.precision,
                lifetimeRevenue = adInfo.lifetimeRevenue,
                encryptedCPM = adInfo.encryptedCPM
            };
        }

        public static ImpressionData Convert(this IronSourceImpressionData adInfo)
        {
            return new ImpressionData()
            {
                adPlatform = "ironSource",
                auctionId = adInfo.auctionId,
                adUnit = adInfo.adUnit,
                country = adInfo.country,
                ab = adInfo.ab,
                segmentName = adInfo.segmentName,
                placement = adInfo.placement,
                adNetwork = adInfo.adNetwork,
                instanceName = adInfo.instanceName,
                instanceId = adInfo.instanceId,
                revenue = adInfo.revenue,
                precision = adInfo.precision,
                lifetimeRevenue = adInfo.lifetimeRevenue,
                encryptedCPM = adInfo.encryptedCPM,
                conversionValue = adInfo.conversionValue,
                allData = adInfo.allData
            };
        }

        public static IronSourceBannerPosition GetBannerPosition(BannerPosition position)
        {
            switch(position)
            {
                case BannerPosition.BOTTOM_CENTER:
                {
                    return IronSourceBannerPosition.BOTTOM;
                }
                case BannerPosition.BOTTOM_LEFT:
                {
                    return IronSourceBannerPosition.BOTTOM;
                }
                case BannerPosition.BOTTOM_RIGHT:
                {
                    return IronSourceBannerPosition.BOTTOM;
                }
                case BannerPosition.CENTERED:
                {
                    return IronSourceBannerPosition.BOTTOM;
                }
                case BannerPosition.CENTER_LEFT:
                {
                    return IronSourceBannerPosition.BOTTOM;
                }
                case BannerPosition.CENTER_RIGHT:
                {
                    return IronSourceBannerPosition.BOTTOM;
                }
                case BannerPosition.TOP_CENTER:
                {
                    return IronSourceBannerPosition.TOP;
                }
                case BannerPosition.TOP_LEFT:
                {
                    return IronSourceBannerPosition.TOP;
                }
                case BannerPosition.TOP_RIGHT:
                {
                    return IronSourceBannerPosition.TOP;
                }
                default:
                {
                    return IronSourceBannerPosition.BOTTOM;
                }
            }
        }

        public static IronSourceBannerSize GetBannerSize(bool isAdaptive, BannerSize size, int width, int height)
        {
            IronSourceBannerSize ironSourceBannerSize = null;
            if (size == BannerSize.CUSTOM)
            {
                ironSourceBannerSize = new IronSourceBannerSize(width, height);
            }
            else if (size == BannerSize.BANNER)
            {
                ironSourceBannerSize = IronSourceBannerSize.BANNER;
            }
            else if (size == BannerSize.LARGE)
            {
                ironSourceBannerSize = IronSourceBannerSize.LARGE;
            }
            else if (size == BannerSize.RECTANGLE)
            {
                ironSourceBannerSize = IronSourceBannerSize.RECTANGLE;
            }
            else if (size == BannerSize.SMART)
            {
                ironSourceBannerSize = IronSourceBannerSize.SMART;
            }
            ironSourceBannerSize.SetAdaptive(isAdaptive);
            return ironSourceBannerSize;
        }
    }
#endif
}
