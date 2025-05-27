using System;
using System.Collections.Generic;

namespace Stateforge
{
    public interface IStateFactory
    {
        public void GetFactory();
        public IState GetState(Type type);
    }
    
    public abstract class StateFactory : IStateFactory
    {
        private readonly Dictionary<Type, IState> _states = new();
        
        public void GetFactory()
        {
            SetStates();
        }

        public IState GetState(Type type)
        {
            return _states[type];
        }
        
        protected void AddState(Type type, IState state)
        {
            _states.TryAdd(type, state);
        }
        
        protected abstract void SetStates();
    }
}