using Stateforge;

namespace Example.User.States;

public class UserSecondState : State<UserContext>
{
    protected override void OnEnter()
    {   
        Console.WriteLine("UserSecondState: Entered");
    }
    
    protected override void SetTransitions()
    {
        AddTransition<UserFirstState>(() => Context.Counter % 2 != 0);
    }
}