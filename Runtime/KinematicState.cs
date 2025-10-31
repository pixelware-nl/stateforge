using System;
using Stateforge.Runtime.Interfaces;
using UnityEngine;

namespace Stateforge.Runtime
{
    /// <summary>
    /// Base class for states that work with the Kinematic Character Controller asset.
    /// Provides lifecycle methods for managing velocity and rotation updates.
    /// </summary>
    /// <typeparam name="TContext">The context type that must implement IContext</typeparam>
    public abstract class KinematicState<TContext> : State<TContext> where TContext : IContext
    {
        /// <summary>
        /// Override to provide collision information for the Kinematic Character Controller.
        /// </summary>
        protected virtual object CollisionInfo => null;

        /// <summary>
        /// Override to provide character information for the Kinematic Character Controller.
        /// </summary>
        protected virtual object CharacterInfo => null;

        /// <summary>
        /// Called when the state is entered, before the first frame update.
        /// Use this to initialize velocity for the state.
        /// </summary>
        /// <param name="velocity">Reference to the current velocity vector</param>
        /// <param name="deltaTime">Time elapsed since last frame</param>
        protected virtual void OnStartVelocity(ref Vector3 velocity, float deltaTime) { }

        /// <summary>
        /// Called when the state is entered, before the first frame update.
        /// Use this to initialize rotation for the state.
        /// </summary>
        /// <param name="rotation">Reference to the current rotation quaternion</param>
        /// <param name="deltaTime">Time elapsed since last frame</param>
        protected virtual void OnStartRotation(ref Quaternion rotation, float deltaTime) { }

        /// <summary>
        /// Called during each frame update to modify the velocity.
        /// </summary>
        /// <param name="velocity">Reference to the current velocity vector</param>
        /// <param name="deltaTime">Time elapsed since last frame</param>
        protected virtual void OnUpdateVelocity(ref Vector3 velocity, float deltaTime) { }

        /// <summary>
        /// Called during each frame update to modify the rotation.
        /// </summary>
        /// <param name="rotation">Reference to the current rotation quaternion</param>
        /// <param name="deltaTime">Time elapsed since last frame</param>
        protected virtual void OnUpdateRotation(ref Quaternion rotation, float deltaTime) { }

        /// <summary>
        /// Called when the state is exited.
        /// Use this to perform final velocity modifications before leaving the state.
        /// </summary>
        /// <param name="velocity">Reference to the current velocity vector</param>
        /// <param name="deltaTime">Time elapsed since last frame</param>
        protected virtual void OnExitVelocity(ref Vector3 velocity, float deltaTime) { }

        /// <summary>
        /// Called when the state is exited.
        /// Use this to perform final rotation modifications before leaving the state.
        /// </summary>
        /// <param name="rotation">Reference to the current rotation quaternion</param>
        /// <param name="deltaTime">Time elapsed since last frame</param>
        protected virtual void OnExitRotation(ref Quaternion rotation, float deltaTime) { }

        /// <summary>
        /// Internal method to handle state entry with kinematic character controller lifecycle.
        /// Override OnEnter() in derived classes instead.
        /// </summary>
        protected sealed override void OnEnter()
        {
            Vector3 velocity = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            float deltaTime = Time.deltaTime;

            // Call velocity initialization first
            OnStartVelocity(ref velocity, deltaTime);
            
            // Call rotation initialization second
            OnStartRotation(ref rotation, deltaTime);

            // Call the derived class OnEnter
            OnKinematicEnter();
        }

        /// <summary>
        /// Internal method to handle state update with kinematic character controller lifecycle.
        /// Override OnUpdate() in derived classes instead.
        /// </summary>
        protected sealed override void OnUpdate()
        {
            float deltaTime = Time.deltaTime;

            // Update CharacterInfo (step 3)
            UpdateCharacterInfo(deltaTime);

            // Update CollisionInfo (step 4)
            UpdateCollisionInfo(deltaTime);

            Vector3 velocity = Vector3.zero;
            Quaternion rotation = Quaternion.identity;

            // Call velocity update (step 5)
            OnUpdateVelocity(ref velocity, deltaTime);

            // Call rotation update (step 6)
            OnUpdateRotation(ref rotation, deltaTime);

            // Call the derived class OnUpdate
            OnKinematicUpdate();
        }

        /// <summary>
        /// Internal method to handle state exit with kinematic character controller lifecycle.
        /// Override OnExit() in derived classes instead.
        /// </summary>
        protected sealed override void OnExit()
        {
            Vector3 velocity = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            float deltaTime = Time.deltaTime;

            // Call velocity exit logic first (step 7)
            OnExitVelocity(ref velocity, deltaTime);

            // Call rotation exit logic second (step 8)
            OnExitRotation(ref rotation, deltaTime);

            // Call the derived class OnExit
            OnKinematicExit();
        }

        /// <summary>
        /// Override this method instead of OnEnter() to provide custom state entry logic.
        /// This is called after OnStartVelocity and OnStartRotation.
        /// </summary>
        protected virtual void OnKinematicEnter() { }

        /// <summary>
        /// Override this method instead of OnUpdate() to provide custom state update logic.
        /// This is called after CharacterInfo and CollisionInfo updates, and after velocity/rotation updates.
        /// </summary>
        protected virtual void OnKinematicUpdate() { }

        /// <summary>
        /// Override this method instead of OnExit() to provide custom state exit logic.
        /// This is called after OnExitVelocity and OnExitRotation.
        /// </summary>
        protected virtual void OnKinematicExit() { }

        /// <summary>
        /// Override this method to update the CharacterInfo.
        /// Called during OnUpdate, before velocity and rotation updates.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame</param>
        protected virtual void UpdateCharacterInfo(float deltaTime) { }

        /// <summary>
        /// Override this method to update the CollisionInfo.
        /// Called during OnUpdate, after CharacterInfo update but before velocity and rotation updates.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame</param>
        protected virtual void UpdateCollisionInfo(float deltaTime) { }
    }
}
