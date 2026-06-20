using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.FSM
{
    public abstract class State<T> where T : IContext
    {
        protected List<Transition<T>> transitions;

        protected abstract void DoStart(StateMachine<T> stateMachine);
        protected abstract void DoUpdate(StateMachine<T> stateMachine);
        protected abstract void DoEnd(StateMachine<T> stateMachine);
        public virtual Color SceneGizmoColor { get => Color.white; }

        public void StartState(StateMachine<T> stateMachine)
        {
            DoStart(stateMachine);
        }

        public void UpdateState(StateMachine<T> stateMachine)
        {
            DoUpdate(stateMachine);
            CheckTransition(stateMachine);
        }
        public void EndState(StateMachine<T> stateMachine)
        {
            DoEnd(stateMachine);
        }

        private void CheckTransition(StateMachine<T> stateMachine)
        {
            if(transitions == null)
            {
                return;
            }
            for(int i = 0; i < transitions.Count; ++i)
            {
                if (transitions[i].CheckTransition(stateMachine))
                {
                    return;
                }
            }
        }


        public void AddTransition(Transition<T> transition)
        {
            if (transitions == null)
            {
                transitions = new List<Transition<T>>();
            }
            transitions.Add(transition);
        }
    }
}
