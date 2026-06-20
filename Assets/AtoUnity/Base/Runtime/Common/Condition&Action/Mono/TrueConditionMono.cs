using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Base
{
    public class TrueConditionMono : ConditionMono
    {
        public override bool CheckCondition()
        {
            return true;
        }
    }
}