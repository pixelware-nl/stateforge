namespace Stateforge
{
    public interface IStateMachine<TContext> where TContext : IContext
    {
        public IState<TContext> CurrentState { get; set; }
        public IStateTransition<TContext> StateTransition { get; }
        public IStateFactory<TContext> StateFactory { get; }

        public void Handle();
    }

    public class StateMachine<TContext> : IStateMachine<TContext> where TContext : IContext
    {
        public IState<TContext> CurrentState { get; set; }
        public IStateTransition<TContext> StateTransition { get; private set; }
        public IStateFactory<TContext> StateFactory { get; private set; }

        public void Handle()
        {
            StateTransition.Handle((IState<TContext>)CurrentState);
            CurrentState.Update();
        }

        protected void Setup<TState, TStateFactory>(TContext context, TStateFactory stateFactory)
            where TState : State<TContext>
            where TStateFactory : IStateFactory<TContext>
        {
            StateFactory = stateFactory;
            StateFactory.Create(this, context);
            Console.WriteLine("Created factory");

            SetGlobalTransitions();

            CurrentState = StateFactory.GetState(typeof(TState));
            CurrentState.Enter();

            StateTransition = new StateTransition<TContext>(this);
        }

        protected virtual void SetGlobalTransitions() { }

        protected void AddGlobalTransition<TState>(Func<bool> condition) where TState : IState<TContext>
        {
            var applicableStates = StateFactory.GetStates().Where(state => state.Key != typeof(TState));

            foreach (IState<TContext> state in applicableStates.ToDictionary().Values)
            {
                state.Transitions.Add(new Transition<TContext>(StateFactory.GetState(typeof(TState)), condition, global: true));
            }
        }
    }
}