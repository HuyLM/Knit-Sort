using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Base
{
    public abstract class ConditonObject : ScriptableObject
    {
        public abstract bool CheckCondition(object target);
    }
    public abstract class ConditonObject<T> : ScriptableObject
    {
        public abstract bool CheckCondition(T target);
    }
}
