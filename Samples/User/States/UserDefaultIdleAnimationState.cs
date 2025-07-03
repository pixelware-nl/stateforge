using Stateforge.Runtime;
using UnityEngine;

namespace Stateforge.Samples.User.States
{
    public class UserDefaultIdleAnimationState : State<UserContext>
    {
        private const float DefaultIdleDuration = 10.0f;
        private float _elapsedTime;
        
        protected override void OnEnter()
        {
            _elapsedTime = 0.0f;
        }

        protected override void OnUpdate()
        {
            _elapsedTime += Time.deltaTime;
        }
        
        protected override void SetTransitions()
        {
            AddTransition<UserSpecialIdleAnimationState>(() => _elapsedTime >= DefaultIdleDuration);
        }
    }
}