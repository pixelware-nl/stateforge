using System;
using System.Collections.Generic;
using System.Linq;
using Stateforge.Runtime.Interfaces;
using UnityEngine;

namespace Stateforge.Runtime
{
    public abstract class StateFactory<TContext> : MonoBehaviour, IStateFactory<TContext> where TContext : IContext
    {
        private IStateMachine<TContext> _stateMachine;
        private TContext _context;
        
        private readonly Dictionary<Type, IState<TContext>> _states = new();
        public List<Type> RootStates { get; private set; } = new();
        public Dictionary<Type, List<Type>> Map { get; private set; } = new();

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
        
        public IState<TContext> GetState(Type state)
        {
            return _states[state];
        }

        public IReadOnlyDictionary<Type, IState<TContext>> GetStates()
        {
            return _states.ToDictionary(kvp => kvp.Key, (kvp) => kvp.Value);
        }
        
        public IState<TContext> GetFirstRootState()
        {
            return _states.Values.FirstOrDefault(state => state.IsRootState);
        }

        protected void AddRootState<TState>() where TState : IState<TContext>, new()
        {
            if (!RootStates.Contains(typeof(TState)))
            {
                RootStates.Add(typeof(TState));
            }
            
            AddState<TState>(true);
        }

        protected void AddChildState<TParent, TChild>() where TParent : IState<TContext> where TChild : IState<TContext>, new()
        {
            if (!Map.ContainsKey(typeof(TParent)))
            {
                Map[typeof(TParent)] = new List<Type>();
            }
            
            Map[typeof(TParent)].Add(typeof(TChild));
            
            AddState<TChild>(false);
        }
        
        private void AddState<TState>(bool isRoot) where TState : IState<TContext>, new()
        {
            var instance = Activator.CreateInstance<TState>();
            instance.Create(_stateMachine, _context, isRoot);
            
            _states.TryAdd(typeof(TState), instance);
        }

        protected abstract void SetStates();
    }
}