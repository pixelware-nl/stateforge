using Stateforge.Interfaces;

namespace Stateforge
{
    public class StateMachine<TContext> : IStateMachine<TContext> where TContext : IContext
    {
        public IState<TContext> CurrentState { get; set; }
        public IState<TContext> PreviousState { get; set; }
        public IStateFactory<TContext> StateFactory { get; private set; }
        
        private IStateTransition<TContext> StateTransition;

        public void Handle()
        {
            StateTransition.Handle(CurrentState);
            CurrentState.Update();
        }

        protected void Setup<TState, TStateFactory>(TContext context)
            where TState : State<TContext>
            where TStateFactory : IStateFactory<TContext>, new()
        {
            StateFactory = new TStateFactory();
            StateFactory.Create(this, context);

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