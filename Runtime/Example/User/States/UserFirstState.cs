using Stateforge;


namespace Example.User.States;

public class UserFirstState : State<UserContext>
{
    protected override void OnEnter()
    {   
        Console.WriteLine("UserFirstState: Entered");
    }
    
    protected override void SetTransitions()
    {
        AddTransition<UserSecondState>(() => Context.Counter % 2 == 0);
    }
}