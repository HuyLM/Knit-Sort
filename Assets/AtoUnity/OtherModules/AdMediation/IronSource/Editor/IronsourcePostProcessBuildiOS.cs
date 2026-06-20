#if ATO_IRONSOURCE_MEDIATION_ENABLE
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace AtoGame.Mediation
{
#if UNITY_IOS
    public class IronsourcePostProcessBuildiOS
    {
        private const string AdvertisingAttributionEndpoint = "https://postbacks-is.com";

        [PostProcessBuildAttribute(int.MaxValue)]
        public static void PostProcessPlist(BuildTarget buildTarget, string path)
        {
            var plistPath = Path.Combine(path, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            SetAttributionReportEndpointIfNeeded(plist);
            UpdateAppTransportSecuritySettingsIfNeeded(plist);
        }

        private static void SetAttributionReportEndpointIfNeeded(PlistDocument plist)
        {
            if (AdIronSourceSettings.Instance.SetAttributionReportEndpoint)
            {
                plist.root.SetString("NSAdvertisingAttributionReportEndpoint", AdvertisingAttributionEndpoint);
            }
            else
            {
                PlistElement attributionReportEndPoint;
                plist.root.values.TryGetValue("NSAdvertisingAttributionReportEndpoint", out attributionReportEndPoint);

                // Check if we had previously set the attribution endpoint and un-set it.
                if (attributionReportEndPoint != null && AdvertisingAttributionEndpoint.Equals(attributionReportEndPoint.AsString()))
                {
                    plist.root.values.Remove("NSAdvertisingAttributionReportEndpoint");
                }
            }
        }

        private static void UpdateAppTransportSecuritySettingsIfNeeded(PlistDocument plist)
        {
            if(AdIronSourceSettings.Instance.NeedAppTransportSecuritySettings == false)
            {
                return;
            }
            var root = plist.root.values;
            PlistElement atsRoot;
            root.TryGetValue("NSAppTransportSecurity", out atsRoot);

            if (atsRoot == null || atsRoot.GetType() != typeof(PlistElementDict))
            {
                // Add the missing App Transport Security settings for publishers if needed. 
                Debug.Log("Adding App Transport Security settings...");
                atsRoot = plist.root.CreateDict("NSAppTransportSecurity");
                atsRoot.AsDict().SetBoolean("NSAllowsArbitraryLoads", true);
            }

            var atsRootDict = atsRoot.AsDict().values;
            // Check if both NSAllowsArbitraryLoads and NSAllowsArbitraryLoadsInWebContent are present and remove NSAllowsArbitraryLoadsInWebContent if both are present.
            if (atsRootDict.ContainsKey("NSAllowsArbitraryLoads") && atsRootDict.ContainsKey("NSAllowsArbitraryLoadsInWebContent"))
            {
                Debug.Log("Removing NSAllowsArbitraryLoadsInWebContent");
                atsRootDict.Remove("NSAllowsArbitraryLoadsInWebContent");
            }
        }
    }
#endif
}
#endif
