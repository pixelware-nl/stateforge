namespace Stateforge.Runtime.Interfaces
{
    public interface IStateMachine
    {
    }
    
    public interface IStateMachine<TContext> : IStateMachine where TContext : IContext
    {
        public IState<TContext> CurrentState { get; set; }
        public IState<TContext> PreviousState { get; set; }
        public IStateFactory<TContext> StateFactory { get; }
    }    
}

