using Stateforge;

namespace Example.User.States;

public class UserAnyState : State<UserContext>
{
    private bool _completed = false;
    
    protected override void OnEnter()
    {   
        Console.WriteLine("UserAnyState: Entered");
        Thread.Sleep(2000);
        _completed = true;
    }
    
    protected override void SetTransitions()
    {
        AddTransition<UserFirstState>(() => _completed);
    }
}