using System;
using System.Collections.Generic;
using Stateforge.Runtime.Interfaces;

namespace Stateforge.Runtime
{
    public abstract class State<TContext> : IState<TContext> where TContext : IContext
    {
        public bool IsRootState { get; private set; }
        
        public IState<TContext> ParentState { get; set; }
        public IState<TContext> ChildState { get; set; }
        
        public HashSet<ITransition<TContext>> Transitions { get; private set; }

        private IStateMachine<TContext> StateMachine { get; set; }
        public TContext Context { get; private set; }

        public void Create(IStateMachine<TContext> stateMachine, TContext context, bool isRoot = true)
        {
            StateMachine = stateMachine;
            Context = context;
            IsRootState = isRoot;
        }

        public void Setup()
        {
            Transitions = new HashSet<ITransition<TContext>>();
            SetTransitions();
        }
        
        public void Enter()
        {
            OnEnter();
        }

        public void Exit()
        {
            ChildState?.Exit();
            OnExit();
        }

        public void Update()
        {
            OnUpdate();
            ChildState?.Update();
        }

        public void FixedUpdate()
        {
            OnFixedUpdate();
            ChildState?.FixedUpdate();
        }

        public void LateUpdate()
        {
            OnLateUpdate();
            ChildState?.LateUpdate();
        }

        /// <summary>
        /// This function is called when entering a state, before executing any logic.
        /// </summary>
        protected virtual void OnEnter() { }
        
        /// <summary>
        /// This function is called when exiting a state, before transitioning to another state.
        /// </summary>
        protected virtual void OnExit() { }
        
        /// <summary>
        /// This function is called every frame while the state is active. Similar to Unity's Update method.
        /// </summary>
        protected virtual void OnUpdate() { }
        
        /// <summary>
        /// This function is called at a fixed interval while the state is active. Similar to Unity's FixedUpdate method.
        /// </summary>
        protected virtual void OnFixedUpdate() { }
        
        /// <summary>
        /// This function is called every frame after all Update methods have been called. Similar to Unity's LateUpdate method.
        /// </summary>
        protected virtual void OnLateUpdate() { }
        
        /// <summary>
        /// The SetTransitions method is where you define all the transitions for the state.
        /// </summary>
        protected abstract void SetTransitions();
        
        /// <summary>
        /// This method adds a transition to another state based on a condition.
        /// </summary>
        /// <param name="condition">A boolean function returning true or false.</param>
        /// <typeparam name="TState">The state to transition to of type IState</typeparam>
        protected void AddTransition<TState>(Func<bool> condition) where TState : IState<TContext>
        {
            Transitions.Add(new Transition<TContext>(StateMachine.StateFactory.GetState(typeof(TState)), condition));
        }
        
        /// <summary>
        /// This method sets a child state for the current state and enters it, creating a new nesting level.
        /// </summary>
        /// <typeparam name="TState">The child state of type IState</typeparam>
        protected void SetChild<TState>() where TState : IState<TContext>
        {
            ChildState = StateMachine.StateFactory.GetState(typeof(TState));
            ChildState.ParentState = this;
            ChildState.Enter();
        }
    }
}