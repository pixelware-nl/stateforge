using Coding.Scripts.Components;
using Stateforge.Example.Player.States;
using UnityEngine;

namespace Stateforge.Example.Player
{
    public class PlayerController : Controller
    {
        public UserInput UserInput { get; private set; }
        
        private void Awake()
        {
            UserInput = new UserInput();
            
            SetupStateMachine<PlayerIdleState>(new PlayerStateFactory(this));
        }

        private void Update()
        {
            UserInput.Update();
            StateMachine.Handle();
        }

        // Optional: Draw Gizmos to visualize the current state in the editor
        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                UnityEditor.Handles.Label(
                    transform.position, 
                    "Active: " + StateMachine.DrawGizmos(StateMachine.CurrentState)
                );
            }
        }
    }
}