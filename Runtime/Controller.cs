using UnityEngine;

namespace Stateforge
{
    public interface IController
    {
        public IStateFactory StateFactory { get; set; }
        public IStateMachine StateMachine { get; set; }
    }
    
    public abstract class Controller: MonoBehaviour, IController
    {
        public IStateFactory StateFactory { get; set; }
        public IStateMachine StateMachine { get; set; }
        
        protected void SetupStateMachine<TState>(IStateFactory stateFactory) where TState : IState
        {
            StateFactory = stateFactory;
            StateFactory.Initialize();
            
            StateMachine = new StateMachine<TState>(StateFactory);
        }
    }
}