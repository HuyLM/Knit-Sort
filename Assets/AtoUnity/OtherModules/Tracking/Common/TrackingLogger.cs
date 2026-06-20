using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Tracking
{
    public static class TrackingLogger
    {
       public static void Log(string log)
       {
#if TRACKING_LOG_ENABLE
            Debug.Log(log);
#endif
        }
    }
}
