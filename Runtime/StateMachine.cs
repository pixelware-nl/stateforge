using System;
using System.Linq;
using Stateforge.Runtime.Interfaces;
using UnityEngine;

namespace Stateforge.Runtime
{
    public abstract class StateMachine<TContext> : MonoBehaviour, IStateMachine<TContext> where TContext : IContext
    {
        public IState<TContext> CurrentState { get; set; }
        public IState<TContext> PreviousState { get; set; }
        public IStateFactory<TContext> StateFactory { get; private set; }
        
        private IStateTransition<TContext> _stateTransition;
        
        private void Awake()
        {
            OnInit();
            
            TContext context = GetComponent<TContext>();
            if (context == null)
            {
                Debug.LogError($"Context of type {typeof(TContext).Name} is missing on {gameObject.name}. Please ensure it is attached.");
                return;
            }
            
            StateFactory = GetComponent<IStateFactory<TContext>>();
            if (StateFactory == null)
            {
                Debug.LogError($"StateFactory of with the context {typeof(TContext).Name} is missing on {gameObject.name}. Please ensure it is attached.");
                return;
            }
            
            StateFactory.Create(this, context);

            SetGlobalTransitions();
            
            CurrentState = StateFactory.GetState(StateFactory.GetFirstRootState().GetType());
            CurrentState.Enter();

            _stateTransition = new StateTransition<TContext>(this);
        }
        
        private void Update()
        {
            _stateTransition.Handle(CurrentState);
            CurrentState.Update();
        }

        private void FixedUpdate()
        {
            CurrentState.FixedUpdate();
        }

        private void LateUpdate()
        {
            CurrentState.LateUpdate();
        }

        /// <summary>
        /// This method is called during Awake to allow derived classes to perform any necessary initialization before the state machine starts.
        /// </summary>
        protected abstract void OnInit();
        
        /// <summary>
        /// This method is called during Awake to that allows states to switch to global states from any other state.
        /// </summary>
        protected virtual void SetGlobalTransitions() { }

        /// <summary>
        /// This function adds a global transition to the specified state. A global transition allows the state machine to transition to the specified state from any other state when the given condition is met.
        /// </summary>
        /// <param name="condition">A boolean function returning true or false.</param>
        /// <typeparam name="TState">The state of type IState</typeparam>
        protected void AddGlobalTransition<TState>(Func<bool> condition) where TState : IState<TContext>
        {
            var applicableStates = StateFactory.GetStates().Where(state => state.Key != typeof(TState));

            foreach (IState<TContext> state in applicableStates.Select(state => state.Value))
            {
                state.Transitions.Add(new Transition<TContext>(StateFactory.GetState(typeof(TState)), condition, global: true));
            }
        }
    }
}