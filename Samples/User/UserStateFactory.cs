using Stateforge.Samples.User.States;
using Stateforge.Runtime;

namespace Stateforge.Samples.User
{
    public class UserStateFactory : StateFactory<UserContext>
    {
        protected override void SetStates()
        {
            AddRootState<UserIdleState>();
            AddRootState<UserMoveState>();

            AddChildState<UserIdleState, UserDefaultIdleAnimationState>();
            AddChildState<UserIdleState, UserSpecialIdleAnimationState>();
            
            AddChildState<UserMoveState, UserWalkState>();
        }
    }
}