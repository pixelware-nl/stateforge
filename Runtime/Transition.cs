using System;

namespace Stateforge
{
    public interface ITransition
    {
        Type toStateType { get; }
        Func<bool> condition { get; }
    }
    
    public class Transition : ITransition
    {
        public Type toStateType { get; private set; }
        public Func<bool> condition { get; private set; }
        
        public Transition(Type toStateType, Func<bool> condition)
        {
            this.toStateType = toStateType;
            this.condition = condition;
        }
    }
}