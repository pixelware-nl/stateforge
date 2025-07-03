using System;
using System.Collections.Generic;

namespace Stateforge.Runtime.Interfaces
{
    public interface IStateFactory<TContext> where TContext : IContext
    {
        public IState<TContext> GetState(Type state);
        public IReadOnlyDictionary<Type, IState<TContext>> GetStates();
        public IState<TContext> GetFirstRootState();
        public void Create(IStateMachine<TContext> stateMachine, TContext context);
    }
}