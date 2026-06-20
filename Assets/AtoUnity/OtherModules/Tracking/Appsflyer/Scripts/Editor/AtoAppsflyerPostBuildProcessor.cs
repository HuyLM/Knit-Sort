using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;
namespace AtoGame.Tracking.Appsflyer
{
    public class AtoAppsflyerPostBuildProcessor : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder { get { return 1; } }
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            TrackingLogger.Log("[AtoAppsflyerPostBuildProcessor] Appsflyer Post Build Processor is patching manifest file and gradle file...");
#if APPSFLYER_ENABLE
            PatchAndroidManifest(path);
            PatchBuildGradle(path);
            PatchGradleProperty(path);
#endif
        }

        private void PatchAndroidManifest(string root)
        {
            var manifestFilePath = GetManifestFilePath(root);
            var manifest = new AtoAppsflyerAndroidManifest(manifestFilePath);

            var changed = false;

            TrackingLogger.Log("[AtoAppsflyerPostBuildProcessor] Add INTERNET permission");
            changed = manifest.AddInternetPermission() || changed;
            TrackingLogger.Log("[AtoAppsflyerPostBuildProcessor] Add ACCESS_NETWORK_STATE permission");
            changed = manifest.AddAccessNetworkStatePermission() || changed;
            TrackingLogger.Log("[AtoAppsflyerPostBuildProcessor] Add ACCESS_WIFI_STATE permission");
            changed = manifest.AddAccessWifiStatePermission() || changed;

            TrackingLogger.Log("[AtoAppsflyerPostBuildProcessor] PatchAndroidManifest: " + PlayerSettings.Android.targetSdkVersion);
            if (PlayerSettings.Android.targetSdkVersion > AndroidSdkVersions.AndroidApiLevel30)
            {
                TrackingLogger.Log("[AtoAppsflyerPostBuildProcessor] Add com.google.android.gms.permission.AD_ID permission");
                changed = manifest.AddAccessADIDPermission() || changed;
            }

            if (changed)
            {
                manifest.Save();
            }
        }

        private void PatchBuildGradle(string root)
        {
            var gradleFilePath = GetGradleFilePath(root);
        }

        private void PatchGradleProperty(string root)
        {
            var gradlePropertyFilePath = GetGradlePropertyFilePath(root);
        }

        private string CombinePaths(string[] paths)
        {
            var path = "";
            foreach (var item in paths)
            {
                path = Path.Combine(path, item);
            }
            return path;
        }

        private string GetManifestFilePath(string root)
        {
            string[] comps = { root, "src", "main", "AndroidManifest.xml" };
            return CombinePaths(comps);
        }

        private string GetGradleFilePath(string root)
        {
            string[] comps = { root, "build.gradle" };
            return CombinePaths(comps);
        }

        private string GetGradlePropertyFilePath(string root)
        {
#if UNITY_2019_3_OR_NEWER
            string[] compos = { root, "..", "gradle.properties" };
#else
        string[] compos = {root, "gradle.properties"};
#endif
            return CombinePaths(compos);
        }
    }
}