using Stateforge.Interfaces;

namespace Stateforge
{
    public abstract class StateFactory<TContext> : IStateFactory<TContext> where TContext : IContext
    {
        private IStateMachine<TContext> _stateMachine;
        private TContext _context;
        
        private readonly Dictionary<Type, IState<TContext>> _states = new();

        public void Create(IStateMachine<TContext> stateMachine, TContext context)
        {
            _stateMachine = stateMachine;
            _context = context;

            SetStates();

            foreach (IState<TContext> state in _states.Values)
            {
                state.Setup();
            }
        }
        
        public IState<TContext> GetState(Type state)
        {
            return _states[state];
        }

        public IReadOnlyDictionary<Type, IState<TContext>> GetStates()
        {
            return _states.ToDictionary(kvp => kvp.Key, (kvp) => kvp.Value);
        }

        protected void AddRootState<TState>() where TState : IState<TContext>, new()
        {
            AddState<TState>(true);
        }

        protected void AddChildState<TState>() where TState : IState<TContext>, new()
        {
            AddState<TState>(false);
        }

        private void AddState<TState>(bool isRoot) where TState : IState<TContext>, new()
        {
            var instance = Activator.CreateInstance<TState>();
            instance.Create(_stateMachine, _context, isRoot);
            
            _states.TryAdd(typeof(TState), instance);
        }

        protected abstract void SetStates();
    }
}