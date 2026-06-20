using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Base
{
    public class SequenceActionMono : ActionMono
    {
        [SerializeField] ActionMono[] actions;

        private int curIndex;
        private Action onCompleted;
        public override void Execute(Action onCompleted = null)
        {
            this.onCompleted = onCompleted;
            curIndex = 0;
            actions[curIndex].Execute(ExecuteNext);
        }


        private void ExecuteNext()
        {
            curIndex++;
            if(curIndex < actions.Length)
            {
                actions[curIndex].Execute(ExecuteNext);
            }
            else
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
