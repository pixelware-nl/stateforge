using System;
using System.Collections.Generic;

namespace Stateforge
{
    public interface IState
    {
        public bool isRootState { get; }
        
        public IState parentState { get; set; }
        public IState childState { get; set; }
        
        public HashSet<ITransition> transitions { get; }
        
        public IController controller { get; }
        
        void Enter();
        void Exit();
        void Update();
    }
    
    public abstract class State : IState
    {
        public bool isRootState { get; private set; }
        
        public IState parentState { get; set; }
        public IState childState { get; set; }
        
        public HashSet<ITransition> transitions { get; private set; }

        public IController controller { get; }

        protected State(IController controller)
        {
            this.controller = controller;
            SetupState();
        }
        
        public void Enter()
        {
            OnEnter();
        }

        public void Exit()
        {
            childState?.Exit();
            OnExit();
        }

        public void Update()
        {
            OnUpdate();
            childState?.Update();
        }

        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }
        protected virtual void OnUpdate() { }
        
        protected abstract void SetTransitions();
        
        protected void AddTransition(Type toStateType, Func<bool> condition)
        {
            transitions.Add(new Transition(toStateType, condition));
        }

        protected void EnableRootState()
        {
            isRootState = true;
        }

        protected void SetChild(Type childStateType)
        {
            childState = controller.stateFactory.GetState(childStateType);
            childState.parentState = this;
            childState.Enter();
        }
        
        private void SetupState()
        {
            transitions = new HashSet<ITransition>();
            SetTransitions();
        }
    }
}