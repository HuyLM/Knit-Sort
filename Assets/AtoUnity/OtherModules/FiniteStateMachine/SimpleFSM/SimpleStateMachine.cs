using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.SimpleFSM
{
    public abstract class SimpleStateMachine
    {
        public abstract void Initialize(ISimpleContext context);

        public abstract void Updating();
        public virtual void Destroy()
        {

        }
    }

    public abstract class SimpleStateMachine<T> : SimpleStateMachine where T : ISimpleContext
    {
        protected T context;
        protected Dictionary<Type, SimpleState<T>> states;

        SimpleState<T> currentState;

        public T Context { get => context; }

        public override void Initialize(ISimpleContext context)
        {
            this.context = (T)context;
            // s1 create states
        }

        public override void Updating()
        {
            // Do Always Actions
            DoAlwaysActions();
            // Update Current State
            currentState?.UpdateState(this);
        }

        protected abstract void DoAlwaysActions();

        protected void SetCurrentState(Type keyState)
        {
            SimpleState<T> state = null;
            if(states == null)
            {
                return;
            }
            if(states.TryGetValue(keyState, out state))
            {
                if (currentState != null)
                {
                    currentState.EndState(this);
                }
                this.currentState = state;
                currentState.StartState(this);
            }
        }

        public void AddState(SimpleState<T> state)
        {
            if (state == null)
            {
                return;
            }
            if(states == null)
            {
                states = new Dictionary<Type, SimpleState<T>>();
            }
            if(states.ContainsKey(state.GetType()))
            {
                return;
            }
            states.Add(state.GetType(), state);
        }
    }
}
