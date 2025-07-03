using Stateforge.Runtime;

namespace Stateforge.Samples.User.States
{
    public class UserIdleState : State<UserContext>
    {
        protected override void OnEnter()
        {
            SetChild<UserDefaultIdleAnimationState>();
        }
        
        protected override void SetTransitions()
        {
            AddTransition<UserMoveState>(() => Context.MovementDirection.x != 0.0f);
        }
    }
}