using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AtoGame.Base
{
    public class UnityEventActionMono : ActionMono
    {
        [SerializeField] private UnityEvent unityEvent;
        public override void Execute(Action onCompleted = null)
        {
            unityEvent?.Invoke();
            OnComplete(onCompleted);
        }

        public override void ValidateObject()
        {
            base.ValidateObject();
            if(unityEvent == null)
            {
                Debug.Log($"{name} ValidateObject: unityEvent null", this);
            }
            else
            {
                for(int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
                {
                    var eve = unityEvent.GetPersistentTarget(i);
                    if(eve == null)
                    {
                        Debug.Log($"{name} ValidateObject: unityEvent HAS null", this);
                    }
                }
            }
        }
    }
}