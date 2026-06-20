using System;
using System.Globalization;
using UnityEngine;

namespace AtoGame.Base
{
    public class PlayerPrefExtension
    {
        private static int lastFrame = -1;

        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
            Save();
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        }

        public static void SetEnum(string key, Enum value)
        {
            PlayerPrefs.SetString(key, value.ToString());
            Save();
        }

        public static T GetEnum<T>(string key, T defaultValue = default(T)) where T : struct
        {
            var stringValue = PlayerPrefs.GetString(key);
            return !string.IsNullOrEmpty(stringValue) ? (T)Enum.Parse(typeof(T), stringValue) : defaultValue;
        }

        public static void SetDateTime(string key, DateTime value)
        {
            PlayerPrefs.SetString(key, value.ToString("o", CultureInfo.InvariantCulture));
            Save();
        }


        public static DateTime GetDateTime(string key, DateTime defaultValue = new DateTime())
        {
            var stringValue = PlayerPrefs.GetString(key);
            return !string.IsNullOrEmpty(stringValue)
                ? DateTime.Parse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
                : defaultValue;
        }


        public static void SetTimeSpan(string key, TimeSpan value)
        {
            PlayerPrefs.SetString(key, value.ToString());
            Save();
        }


        public static TimeSpan GetTimeSpan(string key, TimeSpan defaultValue = new TimeSpan())
        {
            var stringValue = PlayerPrefs.GetString(key);

            return !string.IsNullOrEmpty(stringValue) ? TimeSpan.Parse(stringValue) : defaultValue;
        }

        public static uint ToUInt(object value)
        {
            return Convert.ToUInt32(value);
        }

        public static uint GetUInt(string key, uint defaultValue = 0)
        {
            string value = PlayerPrefs.GetString(key);
            if (!string.IsNullOrEmpty(value))
            {
                return ToUInt(value);
            }
            return defaultValue;
        }

        public static void SetUInt(string key, uint value)
        {
            PlayerPrefs.SetString(key, value.ToString());
            Save();
        }

        public static ulong ToULong(object value)
        {
            return Convert.ToUInt64(value);
        }

        public static ulong GetULong(string key, ulong defaultValue = 0)
        {
            string value = PlayerPrefs.GetString(key);
            if (!string.IsNullOrEmpty(value))
            {
                return ToULong(value);
            }
            return defaultValue;
        }

        public static void SetULong(string key, ulong value)
        {
            PlayerPrefs.SetString(key, value.ToString());
            Save();
        }

        public static float GetFloat(string key, float defaultValue = 0)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        public static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            Save();
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            Save();
        }

        public static string GetString(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            Save();
        }

        public static void Save()
        {
            if(lastFrame != Time.frameCount)
            {
                PlayerPrefs.Save();
                lastFrame = Time.frameCount;
            }
        }
    }
}
