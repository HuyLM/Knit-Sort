using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.SimpleFSM
{
    public abstract class SimpleState<T> where T : ISimpleContext
    {
        protected abstract void DoStart(SimpleStateMachine<T> stateMachine);
        protected abstract void DoUpdate(SimpleStateMachine<T> stateMachine);
        protected abstract void DoEnd(SimpleStateMachine<T> stateMachine);
        public virtual Color SceneGizmoColor { get => Color.white; }

        public void StartState(SimpleStateMachine<T> stateMachine)
        {
            DoStart(stateMachine);
        }

        public void UpdateState(SimpleStateMachine<T> stateMachine)
        {
            DoUpdate(stateMachine);
        }
        public void EndState(SimpleStateMachine<T> stateMachine)
        {
            DoEnd(stateMachine);
        }
    }
}
