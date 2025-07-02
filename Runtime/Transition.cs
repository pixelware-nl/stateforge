using System;

namespace Stateforge
{
    public interface ITransition
    {
        IState State { get; }
        Func<bool> Condition { get; }
    }
    
    public class Transition : ITransition
    {
        public IState State { get; }
        public Func<bool> Condition { get; }
        
        public Transition(IState state, Func<bool> condition)
        {
            State = state;
            Condition = condition;
        }
    }
}