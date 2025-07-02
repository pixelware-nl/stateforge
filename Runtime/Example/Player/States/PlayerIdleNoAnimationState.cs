namespace Stateforge.Example.Player.States
{
    public class PlayerIdleNoAnimationState : State<PlayerController>
    {
        protected override void SetTransitions()
        {
            AddTransition<PlayerIdleAnimationState>(() => Controller.UserInput.hasPressedJump);
        }
    }
}