namespace Stateforge
{
    public interface IState<TContext> where TContext : IContext
    {
        public bool IsRootState { get; }
        public IState<TContext> ParentState { get; set; }
        public IState<TContext> ChildState { get; set; }
        
        public HashSet<ITransition<TContext>> Transitions { get; }
        
        public void Create(IStateMachine<TContext> stateMachine, TContext context, bool isRoot = true);
        public void Setup();
        
        public void Enter();
        public void Exit();
        public void Update();
    }
    
    public abstract class State<TContext> : IState<TContext> where TContext : IContext
    {
        public bool IsRootState { get; private set; }
        
        public IState<TContext> ParentState { get; set; }
        public IState<TContext> ChildState { get; set; }
        
        public HashSet<ITransition<TContext>> Transitions { get; private set; }

        private IStateMachine<TContext> StateMachine { get; set; }
        public TContext Context { get; private set; }

        public void Create(IStateMachine<TContext> stateMachine, TContext context, bool isRoot = true)
        {
            StateMachine = stateMachine;
            Context = context;
            IsRootState = isRoot;
        }

        public void Setup()
        {
            Transitions = new HashSet<ITransition<TContext>>();
            SetTransitions();
        }
        
        public void Enter()
        {
            OnEnter();
        }

        public void Exit()
        {
            ChildState?.Exit();
            OnExit();
        }

        public void Update()
        {
            OnUpdate();
            ChildState?.Update();
        }

        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }
        protected virtual void OnUpdate() { }
        
        protected abstract void SetTransitions();
        
        protected void AddTransition<TState>(Func<bool> condition) where TState : IState<TContext>
        {
            Console.WriteLine(StateMachine.StateFactory.GetType().Name);
            Transitions.Add(new Transition<TContext>(StateMachine.StateFactory.GetState(typeof(TState)), condition));
        }

        protected void SetChild<TState>() where TState : IState<TContext>
        {
            ChildState = StateMachine.StateFactory.GetState(typeof(TState));
            ChildState.ParentState = this;
            ChildState.Enter();
        }
    }
}