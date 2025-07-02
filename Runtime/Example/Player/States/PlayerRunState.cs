using UnityEngine;

namespace Stateforge.Example.Player.States
{
    public class PlayerRunState : State<PlayerController>
    {
        protected override void SetTransitions()
        {
            AddTransition<PlayerIdleState>(() => Controller.UserInput.movementDirection == Vector2.zero);
        }
    }
}