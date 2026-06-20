using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Base
{
    public class FalseConditionMono : ConditionMono
    {
        public override bool CheckCondition()
        {
            return false;
        }
    }
}
