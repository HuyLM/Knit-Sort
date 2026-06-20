using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
    public class AdInfo 
    {
        public string adPlatform;
        public string auctionId;
        public string adUnit;
        public string country;
        public string ab;
        public string segmentName;
        public string adNetwork;
        public string instanceName;
        public string instanceId;
        public double? revenue;
        public string precision;
        public double? lifetimeRevenue;
        public string encryptedCPM;

        public override string ToString()
        {
            return "AdInfo {" +
                    "auctionId='" + auctionId + '\'' +
                    ", adUnit='" + adUnit + '\'' +
                    ", country='" + country + '\'' +
                    ", ab='" + ab + '\'' +
                    ", segmentName='" + segmentName + '\'' +
                    ", adNetwork='" + adNetwork + '\'' +
                    ", instanceName='" + instanceName + '\'' +
                    ", instanceId='" + instanceId + '\'' +
                    ", revenue=" + revenue +
                    ", precision='" + precision + '\'' +
                    ", lifetimeRevenue=" + lifetimeRevenue +
                    ", encryptedCPM='" + encryptedCPM + '\'' +
                    '}';
        }
    }


}
