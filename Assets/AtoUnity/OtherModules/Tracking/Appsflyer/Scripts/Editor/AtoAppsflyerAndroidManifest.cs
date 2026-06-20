using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;
namespace AtoGame.Tracking.Appsflyer
{
    public class AtoAppsflyerAndroidDocument : XmlDocument
    {
        private string path;
        protected XmlNamespaceManager nameSpaceManager;
        public readonly string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";

        public AtoAppsflyerAndroidDocument(string path)
        {
            this.path = path;
            using (var reader = new XmlTextReader(path))
            {
                reader.Read();
                Load(reader);
            }
            nameSpaceManager = new XmlNamespaceManager(NameTable);
            nameSpaceManager.AddNamespace("android", AndroidXmlNamespace);
        }

        public string Save()
        {
            return SaveAs(path);
        }

        public string SaveAs(string path)
        {
            using (var writer = new XmlTextWriter(path, new UTF8Encoding(false)))
            {
                writer.Formatting = Formatting.Indented;
                Save(writer);
            }
            return path;
        }
    }

    internal class AtoAppsflyerAndroidManifest : AtoAppsflyerAndroidDocument
    {
        private readonly XmlElement ManifestElement;
        private readonly XmlElement ApplicationElement;

        public AtoAppsflyerAndroidManifest(string path) : base(path)
        {
            ManifestElement = SelectSingleNode("/manifest") as XmlElement;
            ApplicationElement = SelectSingleNode("/manifest/application") as XmlElement;
        }

        private XmlAttribute CreateAndroidAttribute(string key, string value)
        {
            XmlAttribute attr = CreateAttribute("android", key, AndroidXmlNamespace);
            attr.Value = value;
            return attr;
        }

        internal XmlNode GetActivityWithLaunchIntent()
        {
            return
                SelectSingleNode(
                    "/manifest/application/activity[intent-filter/action/@android:name='android.intent.action.MAIN' and "
                    + "intent-filter/category/@android:name='android.intent.category.LAUNCHER']",
                    nameSpaceManager);
        }

        internal bool SetUsesCleartextTraffic()
        {
            bool changed = false;
            if (ApplicationElement.GetAttribute("usesCleartextTraffic", AndroidXmlNamespace) != "true")
            {
                ApplicationElement.SetAttribute("usesCleartextTraffic", AndroidXmlNamespace, "true");
                changed = true;
            }
            return changed;
        }

        internal bool SetHardwareAccelerated()
        {
            bool changed = false;
            var activity = GetActivityWithLaunchIntent() as XmlElement;
            if (activity.GetAttribute("hardwareAccelerated", AndroidXmlNamespace) != "true")
            {
                activity.SetAttribute("hardwareAccelerated", AndroidXmlNamespace, "true");
                changed = true;
            }
            return changed;
        }

        internal bool AddInternetPermission()
        {
            bool changed = false;
            if (SelectNodes("/manifest/uses-permission[@android:name='android.permission.INTERNET']", nameSpaceManager).Count == 0)
            {
                var elem = CreateElement("uses-permission");
                elem.Attributes.Append(CreateAndroidAttribute("name", "android.permission.INTERNET"));
                ManifestElement.AppendChild(elem);
                changed = true;
            }
            return changed;
        }

        internal bool AddAccessNetworkStatePermission()
        {
            bool changed = false;
            if (SelectNodes("/manifest/uses-permission[@android:name='android.permission.ACCESS_NETWORK_STATE']", nameSpaceManager).Count == 0)
            {
                var elem = CreateElement("uses-permission");
                elem.Attributes.Append(CreateAndroidAttribute("name", "android.permission.ACCESS_NETWORK_STATE"));
                ManifestElement.AppendChild(elem);
                changed = true;
            }
            return changed;
        }

        internal bool AddAccessWifiStatePermission()
        {
            bool changed = false;
            if (SelectNodes("/manifest/uses-permission[@android:name='android.permission.ACCESS_WIFI_STATE']", nameSpaceManager).Count == 0)
            {
                var elem = CreateElement("uses-permission");
                elem.Attributes.Append(CreateAndroidAttribute("name", "android.permission.ACCESS_WIFI_STATE"));
                ManifestElement.AppendChild(elem);
                changed = true;
            }
            return changed;
        }

        internal bool AddAccessADIDPermission()
        {
            bool changed = false;
            if (SelectNodes("/manifest/uses-permission[@android:name='com.google.android.gms.permission.AD_ID']", nameSpaceManager).Count == 0)
            {
                var elem = CreateElement("uses-permission");
                elem.Attributes.Append(CreateAndroidAttribute("name", "com.google.android.gms.permission.AD_ID"));
                ManifestElement.AppendChild(elem);
                changed = true;
            }
            return changed;
        }
    }
}