namespace Stateforge.Example.Player.States
{
    public class PlayerIdleAnimationState : State<PlayerController>
    {
        protected override void SetTransitions()
        {
            AddTransition<PlayerIdleNoAnimationState>(() => Controller.UserInput.hasReleasedJump);
        }
    }
}