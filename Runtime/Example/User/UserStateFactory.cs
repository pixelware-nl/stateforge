using ConsoleApp.User.States;
using Stateforge;

namespace Example.User;

public class UserStateFactory : StateFactory<UserContext>
{
    protected override void SetStates()
    {
        AddRootState<UserFirstState>();
        AddRootState<UserSecondState>();
        AddRootState<UserAnyState>();
    }
}