using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Base
{
    public class ParallelActionMono : ActionMono
    {
        [SerializeField] ActionMono[] actions;

        private int completedActionCount;
        private Action onCompleted;
        public override void Execute(Action onCompleted = null)
        {
            this.onCompleted = onCompleted;
            completedActionCount = 0;
            if(actions.Length == 0)
            {
                OnComplete(onCompleted);
            }
            else
            {
                for(int i = 0; i < actions.Length; ++i)
                {
                    actions[i].Execute(CompleteAction);
                }
            }
        }


        private void CompleteAction()
        {
            completedActionCount++;
            if(completedActionCount == actions.Length)
            {
                OnComplete(onCompleted);
            }
        }

        public override void ValidateObject()
        {
            base.ValidateObject();
            if(actions == null)
            {
                Debug.Log($"{name} ValidateObject: actions null", this);
                return;
            }

            foreach(var a in actions)
            {
                if(a == null)
                {
                    Debug.Log($"{name} ValidateObject: actions HAS null", this);
                }
                else
                {
                    a.ValidateObject();
                }
            }
        }
    }
}