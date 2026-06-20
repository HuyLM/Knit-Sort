using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.FSM
{
    public abstract class Transition<T> where T : IContext
    {
        public State<T> To { get; private set; } 

        public void SetStateTo(State<T> to)
        {
            To = to;
        }

        protected abstract bool CheckCondition(StateMachine<T> stateMachine);
        public abstract void DoBeforeTransitionActions(StateMachine<T> stateMachine);
        public abstract void DoWhileTransitionActions(StateMachine<T> stateMachine);
        public abstract void DoAfterTransitionActions(StateMachine<T> stateMachine);

        public bool CheckTransition(StateMachine<T> stateMachine)
        {
            if(CheckCondition(stateMachine))
            {
                stateMachine.TransitionToState(To, this);
                return true;
            }
            return false;
        }
    }
}
