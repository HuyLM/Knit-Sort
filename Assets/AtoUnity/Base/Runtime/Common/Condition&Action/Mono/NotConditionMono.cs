using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Base
{
    public class NotConditionMono : ConditionMono
    {
        [SerializeField] private ConditionMono condition;

        public override bool CheckCondition()
        {
            return condition.CheckCondition() == false;
        }

        public override void ValidateObject()
        {
            base.ValidateObject();
            if(condition == null)
            {
                Debug.Log($"{name} ValidateObject: condition null", this);
                return;
            }
            else
            {
                condition.ValidateObject();
            }
        }
    }
}