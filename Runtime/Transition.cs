using System;
using Stateforge.Runtime.Interfaces;

namespace Stateforge.Runtime
{
    public class Transition<TContext> : ITransition<TContext> where TContext : IContext
    {
        public IState<TContext> State { get; }
        public Func<bool> Condition { get; }
        public bool Global { get; }
        
        public Transition(IState<TContext> state, Func<bool> condition, bool global = false) 
        {
            State = state;
            Condition = condition;
            Global = global;
        }
    }
}