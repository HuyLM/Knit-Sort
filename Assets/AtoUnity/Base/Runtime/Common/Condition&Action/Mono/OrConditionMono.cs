using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AtoGame.Base
{
    public class OrConditionMono : ConditionMono
    {
        [SerializeField] protected ConditionMono[] subConditions;

        public override bool CheckCondition()
        {
            for (int i = 0; i < subConditions.Length; ++i)
            {
                if (subConditions[i].CheckCondition() == true)
                {
                    return true;
                }
            }
            return false;
        }

        public override void ValidateObject()
        {
            base.ValidateObject();
            if(subConditions == null)
            {
                Debug.Log($"{name} ValidateObject: subConditions null", this);
                return;
            }

            foreach(var c in subConditions)
            {
                if(c == null)
                {
                    Debug.Log($"{name} ValidateObject: subConditions HAS null", this);
                }
                else
                {
                    c.ValidateObject();
                }
            }
        }
    }
}