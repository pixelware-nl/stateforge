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

        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnFixedUpdate() { }
        protected virtual void OnLateUpdate() { }
        
        protected abstract void SetTransitions();
        
        protected void AddTransition<TState>(Func<bool> condition) where TState : IState<TContext>
        {
            Transitions.Add(new Transition<TContext>(StateMachine.StateFactory.GetState(typeof(TState)), condition));
        }
        
        protected void SetChild<TState>() where TState : IState<TContext>
        {
            ChildState = StateMachine.StateFactory.GetState(typeof(TState));
            ChildState.ParentState = this;
            ChildState.Enter();
        }
    }
}