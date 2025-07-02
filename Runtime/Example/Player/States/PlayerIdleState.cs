using UnityEngine;

namespace Stateforge.Example.Player.States
{
    public class PlayerIdleState : State<PlayerController> 
    {
        protected override void OnEnter()
        {
            SetChild<PlayerIdleNoAnimationState>();
        }

        protected override void SetTransitions()
        {
            AddTransition<PlayerRunState>(() => Controller.UserInput.movementDirection != Vector2.zero);
        }
    }
}