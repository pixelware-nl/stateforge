#nullable enable

using System.Collections.Generic;
using System.Linq;
using Stateforge.Runtime.Interfaces;

namespace Stateforge.Runtime
{
    /// <summary>
    /// This interface defines the contract for state transition handling within a state machine.
    /// </summary>
    public interface IStateTransition<TContext> where TContext : IContext
    {
        public IStateMachine<TContext> StateMachine { get; }
        
        public void Handle(IState<TContext> state);
    }
    
    /// <summary>
    /// This class manages state transitions within a state machine.
    /// It evaluates the transitions of the current state and performs state changes based on defined conditions.
    /// </summary>
    public class StateTransition<TContext> : IStateTransition<TContext> where TContext : IContext
    {
        public IStateMachine<TContext> StateMachine { get; }

        public StateTransition(IStateMachine<TContext> stateMachine)
        {
            StateMachine = stateMachine;
        }
        
        /// <summary>
        /// This method handles state transitions by evaluating the transitions of the current state.
        /// </summary>
        /// <param name="state">The state of type IState.</param>
        /// <returns></returns>
        public void Handle(IState<TContext> state)
        {
            Transition(state.Transitions, state);

            if (state.ChildState != null)
            {
                Handle(state.ChildState);
            }
        }

        /// <summary>
        /// This method evaluates the provided transitions and performs a state transition if a valid transition is found.
        /// </summary>
        /// <param name="transitions">A HashSet of ITransitions.</param>
        /// <param name="state">The state of type IState.</param>
        /// <returns></returns>
        private void Transition(HashSet<ITransition<TContext>> transitions, IState<TContext>? state)
        {
            ITransition<TContext>? transition = GetTransition(transitions);

            if (transition == null)
            {
                return;
            }
            
            SetState(transition.State, state);
        }

        /// <summary>
        /// This method sets the current state to the specified state, handling both root and child states appropriately.
        /// </summary>
        /// <param name="to">The IState to transition to</param>
        /// <param name="current">The IState that you are in currently</param>
        /// <returns></returns>
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

        /// <summary>
        /// The TrySetRootState method attempts to set the current state of the state machine to the specified root state.
        /// </summary>
        /// <param name="to">The IState to transition to</param>
        /// <param name="current">The IState that you are in currently</param>
        /// <returns></returns>
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

        /// <summary>
        /// This method attempts to set the current state's child state to the specified state.
        /// </summary>
        /// <param name="to">The IState to transition to</param>
        /// <param name="current">The IState that you are in currently</param>
        /// <returns></returns>
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
        
        /// <summary>
        /// The GetTransition method evaluates a set of transitions and returns the first valid transition based on its condition.
        /// </summary>
        /// <param name="transitions">A HashSet of ITransitions.</param>
        /// <returns></returns>
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