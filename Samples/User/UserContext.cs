using Stateforge.Runtime.Interfaces;
using UnityEngine;

namespace Stateforge.Samples.User
{
    public class UserContext : MonoBehaviour, IContext
    {
        public Vector2 MovementDirection { get; private set; }
        public float MovementSpeed { get; private set; } = 10.0f;

        private void Update()
        {
            MovementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
    }
}