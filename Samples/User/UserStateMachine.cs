using Stateforge.Runtime;

namespace Stateforge.Samples.User
{
    public class UserStateMachine : StateMachine<UserContext>
    {
        private UserContext Context { get; set; }
        
        protected override void OnInit()
        {
            Context = GetComponent<UserContext>();
        }
    }
}