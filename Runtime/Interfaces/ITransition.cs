using System;

namespace Stateforge.Runtime.Interfaces
{
    public interface ITransition<TContext> where TContext : IContext
    {
        IState<TContext> State { get; }
        Func<bool> Condition { get; }
        bool Global { get; }
    }    
}

