using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IAP_ENABLE
using UnityEngine.Purchasing;
#endif

namespace AtoGame.IAP
{
    public partial class EventKey
    {

        public struct OnBoughtIap : Base.IEventParams
        {
#if UNITY_IAP_ENABLE
            public Product Product;
#endif
        }
    }
}
