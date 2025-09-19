using System;
using System.Collections.Generic;
using System.Linq;
using Stateforge.Runtime.Interfaces;
using UnityEngine;

namespace Stateforge.Runtime
{
    /// <summary>
    /// This abstract class serves as a factory for creating and managing states within a state machine.
    /// </summary>
    public abstract class StateFactory<TContext> : MonoBehaviour, IStateFactory<TContext> where TContext : IContext
    {
        private IStateMachine<TContext> _stateMachine;
        private TContext _context;
        
        private readonly Dictionary<Type, IState<TContext>> _states = new();
        public List<Type> RootStates { get; private set; } = new();
        public Dictionary<Type, List<Type>> Map { get; private set; } = new();

        /// <summary>
        /// Create the state machine by setting up the states and their hierarchy. This method is called by the state machine during its initialization.
        /// </summary>
        /// <param name="stateMachine">The IStateMachine that is linked to the factory</param>
        /// <param name="context">The TContext for the StateFactory</param>
        /// <returns></returns>
        public void Create(IStateMachine<TContext> stateMachine, TContext context)
        {
            _stateMachine = stateMachine;
            _context = context;

            SetStates();

            foreach (IState<TContext> state in _states.Values)
            {
                state.Setup();
            }
        }
        
        /// <summary>
        /// Get a state by its type.
        /// </summary>
        /// <param name="state">The state of type Type</param>
        /// <returns></returns>
        public IState<TContext> GetState(Type state)
        {
            return _states[state];
        }

        /// <summary>
        /// Get all states in the state machine as a read-only dictionary.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<Type, IState<TContext>> GetStates()
        {
            return _states.ToDictionary(kvp => kvp.Key, (kvp) => kvp.Value);
        }
        
        /// <summary>
        /// Get the first root state added to the state machine. If multiple root states are added, it returns the first one.
        /// </summary>
        /// <returns></returns>
        public IState<TContext> GetFirstRootState()
        {
            return _states.Values.FirstOrDefault(state => state.IsRootState);
        }

        /// <summary>
        /// Add a root state to the state machine. Root states do not have a parent state.
        /// </summary>
        /// <typeparam name="TState">The state of type IState</typeparam>
        /// <returns></returns>
        protected void AddRootState<TState>() where TState : IState<TContext>, new()
        {
            if (!RootStates.Contains(typeof(TState)))
            {
                RootStates.Add(typeof(TState));
            }
            
            AddState<TState>(true);
        }

        /// <summary>
        /// Add a child state to a parent state. The parent state must be added first as a root state or as a child state.
        /// </summary>
        /// <typeparam name="TParent">The parent state of type IState</typeparam>
        /// <typeparam name="TChild">The child state of type IState</typeparam>
        /// <returns></returns>
        protected void AddChildState<TParent, TChild>() where TParent : IState<TContext> where TChild : IState<TContext>, new()
        {
            if (!Map.ContainsKey(typeof(TParent)))
            {
                Map[typeof(TParent)] = new List<Type>();
            }
            
            Map[typeof(TParent)].Add(typeof(TChild));
            
            AddState<TChild>(false);
        }
        
        /// <summary>
        /// Add a state to the state machine. The state can be a root state or a child state.
        /// </summary>
        /// <param name="isRoot">Boolean to set it as a root state</param>
        /// <typeparam name="TState">The state of type IState</typeparam>
        /// <returns></returns>
        private void AddState<TState>(bool isRoot) where TState : IState<TContext>, new()
        {
            var instance = Activator.CreateInstance<TState>();
            instance.Create(_stateMachine, _context, isRoot);
            
            _states.TryAdd(typeof(TState), instance);
        }

        /// <summary>
        /// Set the states and their hierarchy here by calling AddRootState and AddChildState.
        /// </summary>
        /// <returns></returns>
        protected abstract void SetStates();
    }
}