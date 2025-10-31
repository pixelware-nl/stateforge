using Stateforge.Runtime;
using UnityEngine;

namespace Stateforge.Samples.KinematicExample.States
{
    /// <summary>
    /// Example state demonstrating how to use KinematicState with the Kinematic Character Controller.
    /// This state handles jumping movement.
    /// </summary>
    public class KinematicJumpState : KinematicState<KinematicPlayerContext>
    {
        private Vector3 jumpVelocity;
        private float jumpForce = 10f;

        protected override void OnStartVelocity(ref Vector3 velocity, float deltaTime)
        {
            // Initialize jump velocity when entering the state
            jumpVelocity = Vector3.up * jumpForce;
            velocity = jumpVelocity;
        }

        protected override void OnUpdateVelocity(ref Vector3 velocity, float deltaTime)
        {
            // Apply gravity during jump
            jumpVelocity += Physics.gravity * deltaTime;
            velocity = jumpVelocity;
        }

        protected override void OnKinematicEnter()
        {
            // Custom state entry logic after velocity/rotation initialization
            Debug.Log("Entered Jump State");
        }

        protected override void OnKinematicUpdate()
        {
            // Custom update logic after velocity/rotation updates
            // Check if we should transition to another state
        }

        protected override void SetTransitions()
        {
            // Example: Transition back to idle when grounded
            // AddTransition<KinematicIdleState>(() => IsGrounded());
        }
    }
}
