using System;
using System.Linq;
using Stateforge.Runtime.Interfaces;
using UnityEngine;

namespace Stateforge.Runtime
{
    /// <summary>
    /// This abstract class represents a state machine that manages states and transitions based on a given context.
    /// </summary>
    public abstract class StateMachine<TContext> : MonoBehaviour, IStateMachine<TContext> where TContext : IContext
    {
        public IState<TContext> CurrentState { get; set; }
        public IState<TContext> PreviousState { get; set; }
        public IStateFactory<TContext> StateFactory { get; private set; }
        
        private IStateTransition<TContext> _stateTransition;
        
        /// <summary>
        /// The Awake method is called when the script instance is being loaded.
        /// It initializes the state machine by setting up the context, state factory, and the initial state.
        /// It also sets up global transitions and prepares the state transition handler.
        /// Derived classes can override the OnInit method to perform additional initialization before the state machine starts.
        /// </summary>
        /// <returns></returns>
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
        
        /// <summary>
        /// The Update method is called once per frame. It handles state transitions and updates the current state.
        /// </summary>
        /// <returns></returns>
        private void Update()
        {
            _stateTransition.Handle(CurrentState);
            CurrentState.Update();
        }

        /// <summary>
        /// The FixedUpdate method is called at a fixed interval and is used for physics updates. It updates the current state in a fixed time step.
        /// </summary>
        /// <returns></returns>
        private void FixedUpdate()
        {
            CurrentState.FixedUpdate();
        }

        /// <summary>
        /// The LateUpdate method is called once per frame after all Update methods have been called. It allows the current state to perform any necessary late updates.
        /// </summary>
        /// <returns></returns>
        private void LateUpdate()
        {
            CurrentState.LateUpdate();
        }

        /// <summary>
        /// This method is called during Awake to allow derived classes to perform any necessary initialization before the state machine starts.
        /// </summary>
        /// <returns></returns>
        protected abstract void OnInit();
        
        /// <summary>
        /// This method is called during Awake to that allows states to switch to global states from any other state.
        /// </summary>
        /// <returns></returns>
        protected virtual void SetGlobalTransitions() { }

        /// <summary>
        /// This function adds a global transition to the specified state. A global transition allows the state machine to transition to the specified state from any other state when the given condition is met.
        /// </summary>
        /// <param name="condition">A boolean function returning true or false.</param>
        /// <typeparam name="TState">The state of type IState</typeparam>
        /// <returns></returns>
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