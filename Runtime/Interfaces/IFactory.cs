namespace Stateforge.Interfaces;

public interface IStateFactory<TContext> where TContext : IContext
{
    public IState<TContext> GetState(Type state);
    public IReadOnlyDictionary<Type, IState<TContext>> GetStates();
    public void Create(IStateMachine<TContext> stateMachine, TContext context);
}