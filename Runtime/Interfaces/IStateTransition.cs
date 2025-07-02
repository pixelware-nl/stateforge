namespace Stateforge.Interfaces;

public interface IStateTransition<TContext> where TContext : IContext
{
    public IStateMachine<TContext> StateMachine { get; }
        
    public void Handle(IState<TContext> state);
}