using Stateforge.Interfaces;

namespace Stateforge
{
    public interface IStateTransition<TContext> where TContext : IContext
    {
        public IStateMachine<TContext> StateMachine { get; }
        
        public void Handle(IState<TContext> state);
    }
    
    public class StateTransition<TContext> : IStateTransition<TContext> where TContext : IContext
    {
        public IStateMachine<TContext> StateMachine { get; }

        public StateTransition(IStateMachine<TContext> stateMachine)
        {
            StateMachine = stateMachine;
        }
        
        public void Handle(IState<TContext> state)
        {
            Transition(state.Transitions, state);

            if (state.ChildState != null)
            {
                Handle(state.ChildState);
            }
        }

        private void Transition(HashSet<ITransition<TContext>> transitions, IState<TContext>? state)
        {
            ITransition<TContext>? transition = GetTransition(transitions);

            if (transition == null)
            {
                return;
            }
            
            SetState(transition.State, state);
        }

        private void SetState(IState<TContext> to, IState<TContext>? current)
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

        private bool TrySetRootState(IState<TContext> to, IState<TContext>? current)
        {
            if (current is { IsRootState: true })
            {
                if (StateMachine.CurrentState == to)
                {
                    return true;
                }
                
                StateMachine.CurrentState.Exit();
                StateMachine.CurrentState = StateMachine.StateFactory.GetState(to.GetType());
                StateMachine.CurrentState.Enter();
                
                return true;
            }

            return false;
        }

        private bool TrySetChildState(IState<TContext> to, IState<TContext>? current)
        {
            if (current?.ParentState != null)
            {
                if (current.ParentState.ChildState != null && current.ParentState.ChildState == to)
                {
                    return true;
                }
            
                current.ParentState.ChildState?.Exit();
                current.ParentState.ChildState = StateMachine.StateFactory.GetState(to.GetType());
                current.ParentState.ChildState.ParentState = current.ParentState;
                current.ParentState.ChildState.Enter();

                return true;
            }
            
            return false;
        }
        
        private ITransition<TContext>? GetTransition(HashSet<ITransition<TContext>> transitions)
        {
            if (transitions.Count == 0) 
            {
                return null;
            }
            
            foreach (ITransition<TContext> transition in transitions.Where(transition => transition is { Global: true }))
            {
                if (transition.Condition())
                {
                    return transition;
                }
            }
            
            foreach (ITransition<TContext> transition in transitions.Where(transition => transition is { Global: false }))
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