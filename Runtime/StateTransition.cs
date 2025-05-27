#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stateforge
{
    public interface IStateTransition
    {
        public IStateFactory stateFactory { get; }
        public IStateMachine stateMachine { get; }
        
        public void Handle(IState state);
    }
    
    public class StateTransition : IStateTransition
    {
        public IStateFactory stateFactory { get; }
        public IStateMachine stateMachine { get; }

        public StateTransition(IStateFactory stateFactory, IStateMachine stateMachine)
        {
            this.stateFactory = stateFactory;
            this.stateMachine = stateMachine;
        }
        
        public void Handle(IState state)
        {
            Transition(state.transitions, state, state.isRootState);

            if (state?.childState != null)
            {
                Handle(state.childState);
            }
        }

        private void Transition(HashSet<ITransition> transitions, IState? state, bool isRootState = false)
        {
            ITransition? transition = GetTransition(transitions);

            if (transition == null)
            {
                return;
            }
            
            SetState(transition.toStateType, state, isRootState);
        }

        private void SetState(Type toStateType, IState? state, bool isRootState = false)
        {
            if (isRootState)
            {
                if (stateMachine.currentState.GetType() == toStateType)
                {
                    return;
                }
                
                stateMachine.currentState.Exit();
                stateMachine.currentState = stateFactory.GetState(toStateType);
                stateMachine.currentState.Enter();
                
                return;
            }
            
            if (state?.parentState != null)
            {
                if (state.parentState.childState != null && state.parentState.childState.GetType() == toStateType)
                {
                    return;
                }
            
                state.parentState.childState?.Exit();
                state.parentState.childState = stateFactory.GetState(toStateType);
                state.parentState.childState.parentState = state.parentState;
                state.parentState.childState.Enter();
            }
        }
        
        private ITransition? GetTransition(HashSet<ITransition> transitions)
        {
            if (transitions.Count == 0) 
            {
                return null;
            }

            foreach (ITransition transition in transitions.Where(transition => transition != null))
            {
                if (transition.condition() && transition.toStateType != typeof(State))
                {
                    return transition;
                }
            }

            return null;
        }
    }
}