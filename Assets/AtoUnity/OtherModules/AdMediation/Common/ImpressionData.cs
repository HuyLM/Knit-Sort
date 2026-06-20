using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
    public class ImpressionData
    {

        public static string ADMOB_PLATFORM_NAME = "Admob";
        public static string MAX_PLATFORM_NAME = "applovin";
        public static string IRONSOURCE_PLATFORM_NAME = "ironSource";


        public string adPlatform;
        public string auctionId;
        public string adUnit;
        public string country;
        public string ab;
        public string segmentName;
        public string placement;
        public string adNetwork;
        public string instanceName;
        public string instanceId;
        public double? revenue;
        public string precision;
        public double? lifetimeRevenue;
        public string encryptedCPM;
        public int? conversionValue;
        public string allData;
        public override string ToString()
        {
            return "ImpressionData{" +
                    "auctionId='" + auctionId + '\'' +
                    ", adUnit='" + adUnit + '\'' +
                    ", country='" + country + '\'' +
                    ", ab='" + ab + '\'' +
                    ", segmentName='" + segmentName + '\'' +
                    ", placement='" + placement + '\'' +
                    ", adNetwork='" + adNetwork + '\'' +
                    ", instanceName='" + instanceName + '\'' +
                    ", instanceId='" + instanceId + '\'' +
                    ", revenue=" + revenue +
                    ", precision='" + precision + '\'' +
                    ", lifetimeRevenue=" + lifetimeRevenue +
                    ", encryptedCPM='" + encryptedCPM + '\'' +
                    ", conversionValue=" + conversionValue +
                    '}';
        }
    }
}
