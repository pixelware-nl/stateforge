using System;
using System.Collections.Generic;
using Stateforge.Runtime.Interfaces;

namespace Stateforge.Runtime
{
    /// <summary>
    /// This abstract class represents a state within a state machine.
    /// </summary>
    public abstract class State<TContext> : IState<TContext> where TContext : IContext
    {
        public bool IsRootState { get; private set; }
        
        public IState<TContext> ParentState { get; set; }
        public IState<TContext> ChildState { get; set; }
        
        public HashSet<ITransition<TContext>> Transitions { get; private set; }

        private IStateMachine<TContext> StateMachine { get; set; }
        public TContext Context { get; private set; }

        /// <summary>
        /// The Create method initializes the state with the provided state machine and context. It also sets whether the state is a root state.
        /// </summary>
        /// <param name="stateMachine">The IStateMachine that is linked to the factory</param>
        /// <param name="context">The TContext for the StateFactory</param>
        /// <param name="isRoot">Boolean to set it as a root state</param>
        /// <returns></returns>
        public void Create(IStateMachine<TContext> stateMachine, TContext context, bool isRoot = true)
        {
            StateMachine = stateMachine;
            Context = context;
            IsRootState = isRoot;
        }

        /// <summary>
        /// Set up the state by initializing its transitions. This method should be called after the state has been created and before it is used.
        /// </summary>
        /// <returns></returns>
        public void Setup()
        {
            Transitions = new HashSet<ITransition<TContext>>();
            SetTransitions();
        }
        
        /// <summary>
        /// This method is called when entering a state, triggering any entry logic defined in OnEnter.
        /// </summary>
        /// <returns></returns>
        public void Enter()
        {
            OnEnter();
        }

        /// <summary>
        /// This method is called when exiting a state, triggering any exit logic defined in OnExit.
        /// </summary>
        /// <returns></returns>
        public void Exit()
        {
            ChildState?.Exit();
            OnExit();
        }

        /// <summary>
        /// This method is called every frame while the state is active, triggering any update logic defined in OnUpdate.
        /// </summary>
        /// <returns></returns>
        public void Update()
        {
            OnUpdate();
            ChildState?.Update();
        }

        /// <summary>
        /// This method is called at a fixed interval while the state is active, triggering any fixed update logic defined in OnFixedUpdate.
        /// </summary>
        /// <returns></returns>
        public void FixedUpdate()
        {
            OnFixedUpdate();
            ChildState?.FixedUpdate();
        }

        /// <summary>
        /// This method is called every frame after all Update methods have been called, triggering any late update logic defined in OnLateUpdate.
        /// </summary>
        /// <returns></returns>
        public void LateUpdate()
        {
            OnLateUpdate();
            ChildState?.LateUpdate();
        }

        /// <summary>
        /// This function is called when entering a state, before executing any logic.
        /// </summary>
        /// <returns></returns>
        protected virtual void OnEnter() { }
        
        /// <summary>
        /// This function is called when exiting a state, before transitioning to another state.
        /// </summary>
        /// <returns></returns>
        protected virtual void OnExit() { }
        
        /// <summary>
        /// This function is called every frame while the state is active. Similar to Unity's Update method.
        /// </summary>
        /// <returns></returns>
        protected virtual void OnUpdate() { }
        
        /// <summary>
        /// This function is called at a fixed interval while the state is active. Similar to Unity's FixedUpdate method.
        /// </summary>
        /// <returns></returns>
        protected virtual void OnFixedUpdate() { }
        
        /// <summary>
        /// This function is called every frame after all Update methods have been called. Similar to Unity's LateUpdate method.
        /// </summary>
        /// <returns></returns>
        protected virtual void OnLateUpdate() { }
        
        /// <summary>
        /// The SetTransitions method is where you define all the transitions for the state.
        /// </summary>
        /// <returns></returns>
        protected abstract void SetTransitions();
        
        /// <summary>
        /// This method adds a transition to another state based on a condition.
        /// </summary>
        /// <param name="condition">A boolean function returning true or false.</param>
        /// <typeparam name="TState">The state to transition to of type IState</typeparam>
        /// <returns></returns>
        protected void AddTransition<TState>(Func<bool> condition) where TState : IState<TContext>
        {
            Transitions.Add(new Transition<TContext>(StateMachine.StateFactory.GetState(typeof(TState)), condition));
        }
        
        /// <summary>
        /// This method sets a child state for the current state and enters it, creating a new nesting level.
        /// </summary>
        /// <typeparam name="TState">The child state of type IState</typeparam>
        /// <returns></returns>
        protected void SetChild<TState>() where TState : IState<TContext>
        {
            ChildState = StateMachine.StateFactory.GetState(typeof(TState));
            ChildState.ParentState = this;
            ChildState.Enter();
        }
    }
}