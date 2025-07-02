namespace Stateforge.Interfaces;

public interface IStateMachine<TContext> where TContext : IContext
{
    public IState<TContext> CurrentState { get; set; }
    public IState<TContext> PreviousState { get; set; }
    public IStateFactory<TContext> StateFactory { get; }

    public void Handle();
}