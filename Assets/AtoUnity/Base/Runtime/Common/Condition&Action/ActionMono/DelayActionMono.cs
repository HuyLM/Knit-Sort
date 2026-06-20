using AtoGame.Base;
using AtoGame.Base.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Base
{
    public class DelayActionMono : ActionMono
    {
        [SerializeField] private float delayTime; // second
	[SerializeField] private bool canRestart = false;

        private Countdowner countdowner;
        private Action onCompleted;
        public override void Execute(Action onCompleted = null)
        {
            if(countdowner.IsCountdowning())
            {
                Debug.LogError($"{gameObject.name} Delay Action Mono is countdowning", this);
		if(canRestart)
		{
			countdowner.StartCountdown(delayTime);
		}
                return;
            }    
            this.onCompleted = onCompleted;
            countdowner.StartCountdown(delayTime);
        }

        private void Update()
        {
            if(countdowner.IsCountdowning())
            {
                countdowner.Countdowning(Time.deltaTime);
                if(countdowner.IsTimeOut())
                {
                    OnComplete(onCompleted);
                }
            }
        }

        public override void ValidateObject()
        {
            base.ValidateObject();
            if(delayTime <= 0)
            {
                Debug.Log($"{name} ValidateObject: delayTime <= 0", this);
            }
        }
    }
}
