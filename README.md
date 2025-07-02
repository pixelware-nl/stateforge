# Stateforge
**An Unity Hierarchical State Machine**
> Author: Okan Can Özbek (2025-01-15) for Unity 6000.x

A hierarchical state machine, supporting infinite nested states. Making prototyping Unity projects easier by implementing all the boilerplate classes and code for a smooth functioning hierachical finite state machine. There could still be a few quirks in the project, if you are able to find one don't hesitate to submit a ticket, I will do my best to fix it as soon as possible. Plans for the future include adding Unit Tests, and converting this project into a Unity package. Below you can find a quick explanation on how to set the project up.

* For now there isn't an Unity package that you can install, so the best way of installing this is by directly cloning/forking the project.

## Prerequisites
The updated code for this package has been improved significantly. The previous release is now deprecated and won't be supported in the future.
1. To setup the project install it as a package into your Unity project.
2. Create the following files as an initial setup: ObjectController, and ObjectStateFactory. Subsitute Object with the name of your choosing. In this example we're going to use "Player"

## Controller setup
In our example we're going to setup a Player state machine. For this we first need to create the PlayerController. 
1. The PlayerController needs to inherit from the `Controller` class
2. The PlayerController insert the `SetupStateMachine(IStateFactory)` function
3. The PlayerController must run the `StateMachine.Handle()` inside the `Update()` function

```csharp
public class PlayerController : Controller
{
    private void Awake()
    {
        SetupStateMachine<PlayerIdleState>(new PlayerStateFactory(this));
    }

    private void Update()
    {
        StateMachine.Handle();
    }
}
```
Under the hood it inherits from `MonoBehaviour` this is a component that is also attached to your GameObject.

## State factory setup
A state factory needs to refer to a `Controller` class in our case it would be the PlayerController.
1. Initialize the `StateFactory` and pass the `PlayerController` into it
2. Initialize all the states set for the Player

```csharp
public class PlayerStateFactory : StateFactory<PlayerController>
{
    public PlayerStateFactory(PlayerController controller) : base(controller)
    {
    }

    protected override void SetStates()
    {
        // Example for adding root states
        AddRootState<PlayerIdleState>();
        AddRootState<PlayerRunState>();

        // Example for adding child states
        AddChildState<PlayerDefaultIdleState>();
        AddChildState<PlayerSpecialIdleState>();
    }
}
```

## State setup
Each state has the same base functionality, you need to define transitions and can choose between using the following three functions:
1. `OnEnter()`  - This is run when the state machine switches to said state
2. `OnUpdate()` - This happens every frame
3. `OnExit()`   - This happens when the state machine leaves the state

### Root state setup
For root states there is nothing special that you'd need to do, by default all states are root states. Unless you define a child in the `OnEnter` function.
```csharp
protected override void OnEnter()
{
    SetChild<PlayerDefaultIdleState>();
}
```

Next you also need to provide root level transitions that would cause it to switch from state to state.
This is done by adding a transition with `AddTransition<IState>(Func<bool> condition)`. It looks quite complicated, so let's break it down.
First we define to which state we want to change to. Second we pass the condition, just like an if-statement in which we can transition to.
This should be done in the `SetTransitions` function.
```csharp
protected override void SetTransitions()
{
    AddTransition<PlayerRunState>(() => Controller.UserInput.movementDirection != Vector2.zero);
}
```

Don't forget to define your controller aswel
```csharp
public class PlayerIdleState : State<PlayerController> 
{
    protected override void OnEnter()
    {
        SetChild<PlayerDefaultIdleState>();
    }

    protected override void SetTransitions()
    {
        AddTransition<PlayerRunState>(() => Controller.UserInput.movementDirection != Vector2.zero);
    }
}
```

### Child state setup
Exactly the same as the root state setup, if you add a child here it would mean you would have nested deeper.
In example if we have a Jumping state where we can access an Attack state where we access a Ground slam state it would look something like the following:
```csharp
public class PlayerJumpState : State<PlayerController> 
{
    protected override void OnEnter()
    {
        SetChild<PlayerAttackState>();
    }

    protected override void SetTransitions()
    {
        // Add transition logic here
    }
}
```
```csharp
public class PlayerAttackState : State<PlayerController> 
{
    protected override void OnEnter()
    {
        SetChild<PlayerGroundSlamState>();
    }

    protected override void SetTransitions()
    {
        // Add transition logic here
    }
}
```
## Bonus - Debugging inside the editor
If you want a quick overview of which states are active by seeing a text based tree from the root to the leaf state. You can insert the following code in the `PlayerController` script.
```csharp
// Optional: Draw Gizmos to visualize the current state in the editor
private void OnDrawGizmos()
{
    if (Application.isPlaying)
    {
        UnityEditor.Handles.Label(
            transform.position, 
            "Active: " + StateMachine.DrawGizmos(StateMachine.CurrentState)
        );
    }
}
```
