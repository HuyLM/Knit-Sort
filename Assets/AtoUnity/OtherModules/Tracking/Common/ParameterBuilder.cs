using System.Collections.Generic;
using System.Text;

namespace AtoGame.Tracking
{
    public class ParameterBuilder
    {
        private readonly Dictionary<string, object> parameters = new Dictionary<string, object>();

        public Dictionary<string, object> Params => parameters;

        public static ParameterBuilder Create()
        {
            return new ParameterBuilder();
        }

        public ParameterBuilder Add(string parameterName, object parameterValue)
        {
            if (!parameters.ContainsKey(parameterName))
            {
                parameters.Add(parameterName, parameterValue);
            }

            return this;
        }

        public string DebugLog()
        {
            StringBuilder paramLogs = new StringBuilder();
            if(Params != null && Params.Count > 0)
            {
                paramLogs.Append(" /");
                foreach(KeyValuePair<string, object> entry in Params)
                {
                    string stringValue = entry.Value == null ? "null" : entry.Value.ToString();
                    paramLogs.Append(" " + entry.Key + "=" + stringValue);
                }
            }
            return paramLogs.ToString();
        }

        public Dictionary<string, string> BuildString()
        {
            Dictionary<string, string> temp = new Dictionary<string, string>();
            foreach (var item in parameters)
            {
                temp.Add(item.Key, item.Value.ToString());
            }
            return temp;
        }

        public Dictionary<string, object> BuildObject()
        {
            Dictionary<string, object> temp = new Dictionary<string, object>();
            foreach (var item in parameters)
            {
                temp.Add(item.Key, item.Value);
            }
            return temp;
        }

#if NEWTONSOFT_ENABLE
        public Newtonsoft.Json.Linq.JObject BuildJObject()
        {
            if (parameters != null)
            {
                Newtonsoft.Json.Linq.JObject jsonData = new Newtonsoft.Json.Linq.JObject();
                foreach (var d in parameters)
                {
                    jsonData.Add(d.Key, Newtonsoft.Json.Linq.JToken.FromObject(d.Value));
                }
                return jsonData;
            }
            return new Newtonsoft.Json.Linq.JObject();
        }
#endif

#if FIREBASE_ENABLE
        public Firebase.Analytics.Parameter[] BuildFirebase()
        {
            var para = new Firebase.Analytics.Parameter[parameters.Count];
            int idx = 0;
            foreach (var item in parameters)
            {
                Firebase.Analytics.Parameter parameter = null;
                if(item.Value == null)
                {
                    parameter = new Firebase.Analytics.Parameter(item.Key, "null");
                }
                else
                {
                    System.Type type = item.Value.GetType();
                    if(type == typeof(string))
                    {
                        parameter = new Firebase.Analytics.Parameter(item.Key, item.Value.ToString());
                    }
                    else if(type == typeof(int))
                    {
                        long lo = (int)item.Value;
                        parameter = new Firebase.Analytics.Parameter(item.Key, lo);
                    }
                    else if(type == typeof(long))
                    {
                        parameter = new Firebase.Analytics.Parameter(item.Key, (long)item.Value);
                    }
                    else if(type == typeof(float))
                    {
                        float floatValue = (float)item.Value;
                        parameter = new Firebase.Analytics.Parameter(item.Key, floatValue);
                    }
                    else if(type == typeof(double))
                    {
                        parameter = new Firebase.Analytics.Parameter(item.Key, (double)item.Value);
                    }
                    else if(type == typeof(bool))
                    {
                        parameter = new Firebase.Analytics.Parameter(item.Key, item.Value.ToString());
                    }
                    else
                    {
                        parameter = new Firebase.Analytics.Parameter(item.Key, item.Value.ToString());
                    }
                }
                para[idx] = parameter;
                idx++;
            }

            return para;
        }
#endif

#if ADJUST_ENABLE
        public com.adjust.sdk.AdjustEvent BuildAdjust(string eventName)
        {
            com.adjust.sdk.AdjustEvent adjustEvent = new com.adjust.sdk.AdjustEvent(eventName);
            foreach(var item in Params)
            {
                if(item.Value == null)
                {
                    adjustEvent.addCallbackParameter(item.Key, "null");
                }
                else
                {
                   adjustEvent.addCallbackParameter(item.Key, item.Value.ToString());
                }
                
            }
            return adjustEvent;
        }
#endif

#if UNITY_TRACKING_ENABLE
        public Unity.Services.Analytics.Event BuildUnityEvent(string eventName)
        {
            Unity.Services.Analytics.CustomEvent myEvent = new Unity.Services.Analytics.CustomEvent(eventName);
            foreach(var item in parameters)
            {
                if(item.Value == null)
                {
                    myEvent.Add(item.Key, "null");
                }
                else
                {
                    myEvent.Add(item.Key, item.Value);
                }
            }
            return myEvent;
        }
#endif


    }
}
