using Stateforge.Runtime.Interfaces;
using UnityEngine;

namespace Stateforge.Samples.KinematicExample
{
    /// <summary>
    /// Example context for kinematic character controller states.
    /// This would typically hold references to the character motor and other relevant data.
    /// </summary>
    public class KinematicPlayerContext : MonoBehaviour, IContext
    {
        // Reference to the kinematic character motor (from the asset)
        // public KinematicCharacterMotor Motor { get; private set; }

        public Vector3 MoveInput { get; private set; }
        public bool JumpRequested { get; private set; }

        private void Update()
        {
            // Capture player input
            MoveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            JumpRequested = Input.GetButtonDown("Jump");
        }
    }
}
