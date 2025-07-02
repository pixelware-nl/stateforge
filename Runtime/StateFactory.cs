namespace Stateforge
{
    public interface IStateFactory<TContext> where TContext : IContext
    {
        public IState<TContext> GetState(Type state);
        public IReadOnlyDictionary<Type, IState<TContext>> GetStates();
        public void Create(IStateMachine<TContext> stateMachine, TContext context);
    }
    
    public abstract class StateFactory<TContext> : IStateFactory<TContext> where TContext : IContext
    {
        private IStateMachine<TContext> _stateMachine;
        private TContext _context;
        
        private readonly Dictionary<Type, IState<TContext>> _states = new();

        public void Create(IStateMachine<TContext> stateMachine, TContext context)
        {
            _stateMachine = stateMachine;
            _context = context;

            Console.WriteLine("Entered, statemachine name: " + _stateMachine.GetType().Name);
            
            SetStates();
            
            Console.WriteLine("List of states:");
            foreach (var state in _states)
            {
                Console.WriteLine($"- {state.Key.Name}");
            }

            foreach (IState<TContext> state in _states.Values)
            {
                state.Setup();
            }

            Console.WriteLine($"Set states, current states: ");
            foreach (var state in _states)
            {
                Console.WriteLine($"- {state.Key.Name}");
            }
        }
        
        public IState<TContext> GetState(Type state)
        {
            Console.WriteLine($"Getting state: {state.Name}");
            
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