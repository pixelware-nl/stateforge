using Stateforge.Runtime;

namespace Stateforge.Samples.User.States
{
    public class UserMoveState : State<UserContext>
    {
        protected override void OnEnter()
        {
            SetChild<UserWalkState>();
        }

        protected override void SetTransitions()
        {
            AddTransition<UserIdleState>(() => Context.MovementDirection.x == 0.0f);
        }
    }
}