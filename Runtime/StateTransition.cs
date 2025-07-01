#nullable enable

using System.Collections.Generic;
using System.Linq;

namespace Stateforge
{
    public interface IStateTransition
    {
        public IStateFactory StateFactory { get; }
        public IStateMachine StateMachine { get; }
        
        public void Handle(IState state);
    }
    
    public class StateTransition : IStateTransition
    {
        public IStateFactory StateFactory { get; }
        public IStateMachine StateMachine { get; }

        public StateTransition(IStateFactory stateFactory, IStateMachine stateMachine)
        {
            StateFactory = stateFactory;
            StateMachine = stateMachine;
        }
        
        public void Handle(IState state)
        {
            Transition(state.Transitions, state);

            if (state.ChildState != null)
            {
                Handle(state.ChildState);
            }
        }

        private void Transition(HashSet<ITransition> transitions, IState? state)
        {
            ITransition? transition = GetTransition(transitions);

            if (transition == null)
            {
                return;
            }
            
            SetState(transition.State, state);
        }

        private void SetState(IState to, IState? current)
        {
            if (TrySetRootState(to, current))
            {
                // Exit early if the root state was set
                return;
            }
            
            if (TrySetChildState(to, current))
            {
                // Exit early if the child state was set
                return;
            }
        }

        private bool TrySetRootState(IState to, IState? current)
        {
            if (current is { IsRootState: true })
            {
                if (StateMachine.CurrentState == to)
                {
                    return true;
                }
                
                StateMachine.CurrentState.Exit();
                StateMachine.CurrentState = StateFactory.GetState(to.GetType());
                StateMachine.CurrentState.Enter();
                
                return true;
            }

            return false;
        }

        private bool TrySetChildState(IState to, IState? current)
        {
            if (current?.ParentState != null)
            {
                if (current.ParentState.ChildState != null && current.ParentState.ChildState == to)
                {
                    return true;
                }
            
                current.ParentState.ChildState?.Exit();
                current.ParentState.ChildState = StateFactory.GetState(to.GetType());
                current.ParentState.ChildState.ParentState = current.ParentState;
                current.ParentState.ChildState.Enter();

                return true;
            }
            
            return false;
        }
        
        private ITransition? GetTransition(HashSet<ITransition> transitions)
        {
            if (transitions.Count == 0) 
            {
                return null;
            }

            foreach (ITransition transition in transitions.Where(transition => transition != null))
            {
                if (transition.Condition())
                {
                    return transition;
                }
            }

            return null;
        }
    }
}