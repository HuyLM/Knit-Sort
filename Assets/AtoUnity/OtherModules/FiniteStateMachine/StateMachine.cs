using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.FSM
{
    public abstract class StateMachine
    {
        public abstract void Initialize(IContext context);
        public abstract void Updating(float deltaTime);
        public virtual void Destroy()
        {

        }
        public abstract void Start();
    }

    public abstract class StateMachine<T> : StateMachine where T : IContext
    {
        protected T context;
        State<T> currentState;
        protected List<Transition<T>> anyStateTransitions;
        protected State<T> startState;


        public T Context { get => context; }

        public override void Initialize(IContext context)
        {
            this.context = (T)context;
            // s1 create states
            // s2 create trasitions
            // s3 link states to transitions
            // init 
        }

        public override void Updating(float deltaTime)
        {
            // Do Always Actions
            DoAlwaysActions();
            // Transition From Any States
            CheckTransitionFromAnyStates();
            // Update Current State
            currentState?.UpdateState(this);
        }

        public override void Start()
        {
            if(startState != null)
            {
                StartStartState(startState);
            }
        }

        protected abstract void DoAlwaysActions();
        private void CheckTransitionFromAnyStates()
        {
            if (anyStateTransitions == null)
            {
                return;
            }
            for (int i = 0; i < anyStateTransitions.Count; ++i)
            {
                if (anyStateTransitions[i].CheckTransition(this))
                {
                    return;
                }
            }
        }

        public void TransitionToState(State<T> nextState, Transition<T> transition)
        {
            if (nextState != null && nextState != currentState && currentState != null)
            {
                transition?.DoBeforeTransitionActions(this);
                currentState.EndState(this);
                transition?.DoWhileTransitionActions(this);
                SetCurrentState(nextState);
                currentState.StartState(this);
                transition?.DoAfterTransitionActions(this);
            }
        }

        private void SetCurrentState(State<T> currentState)
        {
            this.currentState = currentState;
        }

        protected void StartStartState(State<T> startState)
        {
            SetCurrentState(startState);
            startState.StartState(this);
        }

        void OnDrawGizmos(Transform transform)
        {
            if (currentState != null)
            {
                Gizmos.color = currentState.SceneGizmoColor;
                Gizmos.DrawWireSphere(transform.position, 0.5f);
            }
        }
    }
}
