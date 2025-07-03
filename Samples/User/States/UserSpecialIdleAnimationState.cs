using Stateforge.Runtime;
using UnityEngine;

namespace Stateforge.Samples.User.States
{
    public class UserSpecialIdleAnimationState : State<UserContext>
    {
        private const float AnimationDuration = 3.0f;
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
            AddTransition<UserDefaultIdleAnimationState>(() => _elapsedTime >= AnimationDuration);
        }
    }
}