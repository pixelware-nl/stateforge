using Stateforge.Runtime;
using UnityEngine;

namespace Stateforge.Samples.User.States
{
    public class UserWalkState : State<UserContext>
    {
        protected override void OnUpdate()
        {
            Context.transform.Translate(
                Vector2.right * Context.MovementDirection.x * Context.MovementSpeed * Time.deltaTime
            );
        }
        
        protected override void SetTransitions()
        {
        }
    }
}