using System;
using Stateforge.Runtime.Interfaces;

namespace Stateforge.Runtime
{
    /// <summary>
    /// This class represents a transition between states in a state machine.
    /// It contains the target state, a condition for the transition, and a flag indicating if it's a global transition.
    /// </summary>
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