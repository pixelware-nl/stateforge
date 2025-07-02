using ConsoleApp.User.States;
using Stateforge;

namespace Example.User;

public class UserStateMachine : StateMachine<UserContext>
{
    private UserContext Context { get; } = new();
    
    public void Start()
    {
        Setup<UserFirstState, UserStateFactory>(Context);
    }

    public void Update()
    {
        Handle();
        Context.Counter++;
    }

    protected override void SetGlobalTransitions()
    {
        AddGlobalTransition<UserAnyState>(() => Context.Counter == 5);
    }
}