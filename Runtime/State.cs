using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Stateforge
{
    public interface IState
    {
        public bool IsRootState { get; }
        
        public IState ParentState { get; set; }
        public IState ChildState { get; set; }
        
        public HashSet<ITransition> Transitions { get; }
        
        void Enter();
        void Exit();
        void Update();
    }
    
    public abstract class State<TController> : IState where TController : IController
    {
        public bool IsRootState { get; private set; }
        
        public IState ParentState { get; set; }
        public IState ChildState { get; set; }
        
        public HashSet<ITransition> Transitions { get; private set; }

        public TController Controller { get; set; }

        public void Create(TController controller, bool isRoot = true)
        {
            Controller = controller;
            IsRootState = isRoot;
        }

        public void Setup()
        {
            Transitions = new HashSet<ITransition>();
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

        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }
        protected virtual void OnUpdate() { }
        
        protected abstract void SetTransitions();
        
        protected void AddTransition<TState>(Func<bool> condition) where TState : IState
        {
            Transitions.Add(new Transition(Controller.StateFactory.GetState(typeof(TState)), condition));
        }

        protected void SetChild<TState>() where TState : IState
        {
            ChildState = Controller.StateFactory.GetState(typeof(TState));
            ChildState.ParentState = this;
            ChildState.Enter();
        }
    }
}