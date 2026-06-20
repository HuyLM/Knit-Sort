using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Base
{
    public abstract class ActionObject : ScriptableObject
    {
        public abstract void Execute(object user = null, Action onCompleted = null);
    }

    public abstract class ActionObject<T> : ScriptableObject
    {
        public abstract void Execute(T target, object user = null, Action onCompleted = null);
    }
}