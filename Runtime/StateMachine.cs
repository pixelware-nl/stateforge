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

        protected abstract void OnInit();
        protected virtual void SetGlobalTransitions() { }

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