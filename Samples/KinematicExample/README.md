# Kinematic Character Controller Integration

This folder contains an example of how to integrate Stateforge with the [Kinematic Character Controller](https://assetstore.unity.com/packages/tools/physics/kinematic-character-controller-99131) asset from the Unity Asset Store.

## Overview

The `KinematicState<TContext>` base class provides lifecycle methods specifically designed to work with the Kinematic Character Controller asset. It extends the standard `State<TContext>` class and adds support for managing velocity and rotation updates in the proper order.

## Key Features

### Lifecycle Methods

The `KinematicState` class provides the following virtual methods that you can override:

#### On State Enter
1. `OnStartVelocity(ref Vector3 velocity, float deltaTime)` - Initialize velocity when entering the state
2. `OnStartRotation(ref Quaternion rotation, float deltaTime)` - Initialize rotation when entering the state
3. `OnKinematicEnter()` - Custom state entry logic (replaces `OnEnter()`)

#### During State Update
1. `UpdateCharacterInfo(float deltaTime)` - Update character information
2. `UpdateCollisionInfo(float deltaTime)` - Update collision information
3. `OnUpdateVelocity(ref Vector3 velocity, float deltaTime)` - Update velocity each frame
4. `OnUpdateRotation(ref Quaternion rotation, float deltaTime)` - Update rotation each frame
5. `OnKinematicUpdate()` - Custom state update logic (replaces `OnUpdate()`)

#### On State Exit
1. `OnExitVelocity(ref Vector3 velocity, float deltaTime)` - Final velocity modifications before exiting
2. `OnExitRotation(ref Quaternion rotation, float deltaTime)` - Final rotation modifications before exiting
3. `OnKinematicExit()` - Custom state exit logic (replaces `OnExit()`)

### Properties

- `CollisionInfo` - Override to provide collision data for the Kinematic Character Controller
- `CharacterInfo` - Override to provide character data for the Kinematic Character Controller

## Usage Example

```csharp
using Stateforge.Runtime;
using UnityEngine;

public class KinematicWalkState : KinematicState<MyCharacterContext>
{
    protected override void OnStartVelocity(ref Vector3 velocity, float deltaTime)
    {
        // Initialize walking velocity
        velocity = Context.MoveDirection * Context.WalkSpeed;
    }

    protected override void OnUpdateVelocity(ref Vector3 velocity, float deltaTime)
    {
        // Update velocity based on input
        Vector3 targetVelocity = Context.MoveDirection * Context.WalkSpeed;
        velocity = Vector3.Lerp(velocity, targetVelocity, deltaTime * 10f);
        
        // Apply gravity
        velocity += Physics.gravity * deltaTime;
    }

    protected override void OnUpdateRotation(ref Quaternion rotation, float deltaTime)
    {
        // Rotate character to face movement direction
        if (Context.MoveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Context.MoveDirection);
            rotation = Quaternion.Slerp(rotation, targetRotation, deltaTime * 10f);
        }
    }

    protected override void SetTransitions()
    {
        AddTransition<KinematicIdleState>(() => Context.MoveDirection == Vector3.zero);
        AddTransition<KinematicJumpState>(() => Context.JumpRequested);
    }
}
```

## Integration with Kinematic Character Controller

To use these states with the actual Kinematic Character Controller asset:

1. Install the Kinematic Character Controller from the Unity Asset Store
2. Create your context class that implements `IContext` and holds a reference to the `KinematicCharacterMotor`
3. Create states that inherit from `KinematicState<YourContext>`
4. In your states, use the lifecycle methods to modify velocity and rotation
5. Apply the velocity and rotation to your `KinematicCharacterMotor` by implementing the `ICharacterController` interface

## Notes

- The `OnEnter()`, `OnUpdate()`, and `OnExit()` methods are sealed in `KinematicState`. Use `OnKinematicEnter()`, `OnKinematicUpdate()`, and `OnKinematicExit()` instead.
- All lifecycle methods are called in the specific order required by the Kinematic Character Controller workflow.
- The `ref` parameters for velocity and rotation allow you to modify the values directly.
