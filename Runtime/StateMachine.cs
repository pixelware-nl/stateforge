#nullable enable
using System;

namespace Stateforge
{
    public interface IStateMachine
    {
        public IStateFactory stateFactory { get; }
        public IState currentState { get; set; }
        public IStateTransition stateTransition { get; }

        public string GetTree(IState state);
    }

    public class StateMachine : IStateMachine
    {
        public IStateFactory stateFactory { get; }
        public IState currentState { get; set; }
        public IStateTransition stateTransition { get; }

        public StateMachine(IStateFactory stateFactory, Type initialState)
        {
            this.stateFactory = stateFactory;
            this.stateFactory.GetFactory();
            
            currentState = this.stateFactory.GetState(initialState);
            currentState.Enter();
            
            stateTransition = new StateTransition(this.stateFactory, this);
        }
        
        public string GetTree(IState state)
        {
            string tree = state.GetType().Name;

            if (state.childState != null)
            {
                tree += " > " + GetTree(state.childState);
            }

            return tree;
        }
        
    }
}