using Stateforge.Example.Player.States;

namespace Stateforge.Example.Player
{
    public class PlayerStateFactory : StateFactory<PlayerController>
    {
        public PlayerStateFactory(PlayerController controller) : base(controller)
        {
        }

        protected override void SetStates()
        {
            AddRootState<PlayerIdleState>();
            AddRootState<PlayerRunState>();
            
            AddChildState<PlayerIdleNoAnimationState>();
            AddChildState<PlayerIdleAnimationState>();
        }
    }
}