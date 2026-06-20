using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.AtoUnity.IAP
{
    public abstract class IapAction : ScriptableObject
    {
        public abstract void Execute(string target, object user = null, Action onCompleted = null);
    }
}
