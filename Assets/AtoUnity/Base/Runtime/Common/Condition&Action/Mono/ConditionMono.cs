using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AtoGame.Base
{
    public abstract class ConditionMono : MonoBehaviour
    {
        public abstract bool CheckCondition();

        public virtual void ValidateObject()
        {

        }
    }
}