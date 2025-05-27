using System;
using UnityEngine;

namespace Stateforge
{
    public interface IController
    {
        public IStateFactory stateFactory { get; set;  }
        public IStateMachine stateMachine { get; set; }
    }
    
    public abstract class Controller: MonoBehaviour, IController
    {
        public IStateFactory stateFactory { get; set; }
        public IStateMachine stateMachine { get; set; }

        protected void GetFactory(IStateFactory factory)
        {
            stateFactory = factory;
            stateFactory.GetFactory();
        }

        protected void GetStateMachine(Type initialStateType)
        {
            stateMachine = new StateMachine(stateFactory, initialStateType);
        }
    }
}