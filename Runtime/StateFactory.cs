using System;
using System.Collections.Generic;

namespace Stateforge
{
    public interface IStateFactory
    {
        public void Initialize();
        public IState GetState(Type state);
    }
    
    public abstract class StateFactory<TController> : IStateFactory where TController : IController
    {
        private readonly TController _controller;
        private readonly Dictionary<Type, State<TController>> _states = new();

        protected StateFactory(TController controller)
        {
            _controller = controller;
        }

        public void Initialize()
        {
            SetStates();

            foreach (State<TController> state in _states.Values)
            {
                state.Setup();
            }
        }
        
        public IState GetState(Type state)
        {
            return _states[state];
        }

        protected void AddRootState<TState>() where TState : State<TController>, new()
        {
            AddState<TState>(true);
        }

        protected void AddChildState<TState>() where TState : State<TController>, new()
        {
            AddState<TState>(false);
        }

        private void AddState<TState>(bool isRoot) where TState : State<TController>, new()
        {
            var instance = Activator.CreateInstance<TState>();
            instance.Create(_controller, isRoot);
            
            _states.TryAdd(typeof(TState), instance);
        }

        protected abstract void SetStates();
    }
}