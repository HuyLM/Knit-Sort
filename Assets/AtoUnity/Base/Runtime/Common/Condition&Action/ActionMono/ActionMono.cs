using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Base
{
    public abstract class ActionMono : MonoBehaviour
    {
        [SerializeField] protected ActionMono nextAction;
        public virtual void ValidateObject() // For Editor
        {
            if(nextAction != null)
            {
                nextAction.ValidateObject();
            }
        }

        public virtual void Initialize()
        {

        }

        public void Execute()
        {
            Execute(null);
        }

        public abstract void Execute(Action onCompleted = null);

        protected virtual void OnComplete(Action onComplete)
        {
            if(nextAction != null)
            {
                nextAction.Execute(onComplete);
            }
            else
            {
                onComplete?.Invoke();
            }
        }
    }


    public abstract class ActionMono<T> : MonoBehaviour
    {
        [SerializeField] protected ActionMono<T> nextAction;
        public virtual void ValidateObject() // For Editor
        {
            if (nextAction != null)
            {
                nextAction.ValidateObject();
            }
        }

        public virtual void Initialize()
        {

        }

        public abstract void Execute(T target, Action onCompleted = null);

        protected virtual void OnComplete(T target, Action onComplete)
        {
            if (nextAction != null)
            {
                nextAction.Execute(target, onComplete);
            }
            else
            {
                onComplete?.Invoke();
            }
        }
    }
}