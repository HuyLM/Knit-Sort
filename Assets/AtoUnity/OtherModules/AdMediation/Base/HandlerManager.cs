using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
    public abstract class HandlerManager<TMain, TInterface, TDefault>
         where TMain : new()
         where TDefault : TInterface, new()
    {
        static TMain _instance;
        public static TDefault DefaultHandler { get; } = new TDefault();
        public static TMain Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TMain();
                }
                return _instance;
            }
        }
        static TInterface _handler;
        public static void SetHandler(TInterface handler)
        {
            _handler = handler;
        }

        public static TInterface CurrentHandler
        {
            get
            {
                if (_handler == null)
                {
                    Debug.Log("<color=yellow>" + typeof(TMain) + " does not have any handler, using default handler now" + "</color>");
                    return DefaultHandler;
                }
                return _handler;
            }
        }
    }
}
