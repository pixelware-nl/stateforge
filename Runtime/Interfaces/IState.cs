using System.Collections.Generic;

namespace Stateforge.Runtime.Interfaces
{
    public interface IState<TContext> where TContext : IContext
    {
        public bool IsRootState { get; }
        public IState<TContext> ParentState { get; set; }
        public IState<TContext> ChildState { get; set; }
        
        public HashSet<ITransition<TContext>> Transitions { get; }
        
        public void Create(IStateMachine<TContext> stateMachine, TContext context, bool isRoot = true);
        public void Setup();
        public void Enter();
        public void Exit();
        public void Update();
        public void LateUpdate();
        public void FixedUpdate();
    }    
}

