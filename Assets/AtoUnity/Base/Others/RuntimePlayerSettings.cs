#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEditor;
#endif

namespace AtoGame.Base
{
    //https://forum.unity.com/threads/how-can-i-get-bundle-version-and-bundle-version-code-through-script.68331/#post-7917367
    public static class RuntimePlayerSettings
    {
        // List here the player settings you want to retrieve at runtime.
        // Mind that their value will be automatically overridden!
        public const string iOSBuildVersion = "434fd";
        public const int AndroidBundleVersionCode = 3;

#if UNITY_EDITOR
        /*
        // Map here your settings with the actual settings from UnityEditor
        private static readonly Dictionary<string, Func<object>> RuntimeToEditorMapping = new Dictionary<string, Func<object>>()
        {
            { nameof(iOSBuildVersion), () => PlayerSettings.iOS.buildNumber },
            { nameof(AndroidBundleVersionCode), () => PlayerSettings.Android.bundleVersionCode },
        };


        ///////////////////////////////////////////////////////////////////////
        private class Updater : UnityEditor.AssetModificationProcessor
        {
            private static readonly string playerSettingsPath = "ProjectSettings/ProjectSettings.asset";

            public static string[] OnWillSaveAssets(string[] paths)
            {
                if (paths.Contains(playerSettingsPath))
                    Update();
                return paths;
            }

            public static void Update([CallerFilePath] string sourceFilePath = "")
            {
                string text = File.ReadAllText(sourceFilePath);
                string newText = text;

                foreach (var i in RuntimeToEditorMapping)
                    newText = SetValue(newText, i.Key, i.Value());

                if (newText != text)
                {
                    File.WriteAllText(sourceFilePath, newText);
                    AssetDatabase.Refresh();
                }
            }

            private static string SetValue(string text, string paramName, object newValue)
            {
                var regexText = newValue is string ?
                    @"((?<= " + paramName + @" = "")(.*)(?="";))" :
                    @"((?<= " + paramName + @" = )(.*)(?=;))";
                var regex = new Regex(regexText, RegexOptions.Multiline);
                return regex.Replace(text, newValue.ToString());
            }
        }
        */
#endif
    }
}