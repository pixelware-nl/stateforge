#nullable enable

namespace Stateforge
{
    public interface IStateMachine
    {
        public IState CurrentState { get; set; }
        public IStateTransition StateTransition { get; }
        
        public void Handle();
        public string DrawGizmos(IState state);
    }

    public class StateMachine<TState> : IStateMachine where TState : IState
    {
        public IState CurrentState { get; set; }
        public IStateTransition StateTransition { get; }

        public StateMachine(IStateFactory stateFactory)
        {
            CurrentState = stateFactory.GetState(typeof(TState));
            CurrentState.Enter();
            
            StateTransition = new StateTransition(stateFactory, this);
        }

        public void Handle()
        {
            StateTransition.Handle(CurrentState);
            CurrentState.Update();
        }
        
        public string DrawGizmos(IState state)
        {
            string tree = state.GetType().Name;

            if (state.ChildState != null)
            {
                tree += " > " + DrawGizmos(state.ChildState);
            }

            return tree;
        }
    }
}