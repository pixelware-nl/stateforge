# Stateforge
**An Unity Hierarchical State Machine**
> Author: Okan Can Özbek (2025-01-15) for Unity 6000.x

A hierarchical state machine, supporting infinite nested states. Making prototyping Unity projects easier by implementing all the boilerplate classes and code for a smooth functioning hierachical finite state machine. There could still be a few quirks in the project, if you are able to find one don't hesitate to submit a ticket, I will do my best to fix it as soon as possible. Plans for the future include adding Unit Tests, and converting this project into a Unity package. Below you can find a quick explanation on how to set the project up.

* For now there isn't an Unity package that you can install, so the best way of installing this is by directly cloning/forking the project.

# Setup
1. Create a new child factory called something along the lines of `ExampleStateFactory.cs` this inherits from `StateFactory`
2. This in turn now requires the abstract function `protected abstract void SetStates()`
```csharp
public class ExampleStateFactory : StateFactory
{
    protected override void SetStates()
    {
        // Initialize your states here.
        AddState(typeof(PlayerGroundedState), new PlayerGroundedState()); // Example of initialization
        AddState(typeof(PlayerJumpState), new PlayerJumpState()); // Example of initialization
        AddState(typeof(PlayerIdleState), new PlayerIdleState()); // Example of initialization
        AddState(typeof(PlayerRunState), new PlayerRunState()); // Example of initialization
    }
}
```
> The reason we do it this way is to ensure that we keep reusing the same state instances, else it could get funky when reinitializing states.
> Also from a performance perspective it helps a lot to initialize them all before using them.

3. Next we need to define the `Controller` I called it the `Controller` since it is the heart of the State Machine. This is the point where everything comes together.
4. Create a new file called something along the lines of `ExampleController.cs` and implement the following functions like below.
```csharp
public class PlayerController : Controller
{
    private void Awake()
    {
        GetFactory(new PlayerStateFactory(this));
        GetStateMachine(typeof(PlayerLocomotion));
    }
    
    private void Update()
    {
        StateMachine.StateTransition.Handle(StateMachine.CurrentState);
        StateMachine.CurrentState.Update();
    }
}
```
> That's it, now you have setup your state machine and can start coding! Below you'll find more explanations on how to implement the state machine in depth.

# In-depth setup
1. Define your root states, so a little context. You have your root states which are all the way on the top of the chain in the hierarchy. Then come all the child states. It is important to define which of your states are the root state since it will affect how they get transitioned. Here is how you define a root state
2. Also beware that you need to implement the `protected abstract void SetTransitions()` method, this is where you define your transitions for the specific state.
```csharp
public class PlayerGroundedState : State
{
    public PlayerGroundedState(PlayerController controller) : base(controller)
    {
        EnableRootState(); // Enabling root state
    }
    
    protected override void SetTransitions()
    {
        // Define transition logic
    }
}
```
3. To define transitions we use the `AddTransition(Type toStateType, Func<bool> condition)` method. Simply said, we give the state class as a parameter and a function that returns a boolean as a second parameter.
```csharp
protected override void SetTransitions()
{
    AddTransition(typeof(PlayerJumpState), () => Input.GetKeyDown(KeyCode.Space));
}
```
4. So now we setup our State, with a transition to another state. How do we add logic to it?
* We can implement logic into our states by overriding one or multiple of the base functions (`OnEnter()`, `OnExit()`, `OnUpdate()`).
* `OnEnter()` and `OnExit` only happen when the state transitions.
* `OnUpdate()` happens every frame.

5. Lastly, transitioning to children. This might change in the future but as of now it works as followed. We need to go to our parent state, and use the `SetChild(Type stateType)` method in our `OnEnter()` method.
```csharp
public override void OnEnter()
{
    SetChild(typeof(PlayerIdleState));
}
```
> Transitioning between children work the exact same as transitioning between its parents. Remember, only the states on top of the hierarchy should implement `EnableRootState()`. The others should NOT!
> Congratulations, your state machine should now function as intended!
# Additional tips
> Just add it in like this (this is just an example you can add it however you'd like but this is how I personally implement it).
```csharp
public class PlayerController : Controller
{
    [SerializeField] private Rigidbody2D Body { get; }

    private void Awake()
    {
        GetFactory(new PlayerStateFactory(this));
        GetStateMachine(typeof(PlayerLocomotion));

        Body = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        StateMachine.StateTransition.Handle(StateMachine.CurrentState);
        StateMachine.CurrentState.Update();
    }
}
```
Also a nice to have feature is the ability to see all your active states. For this I use `OnDrawGizmos()` a nice feature from Unity. Just put this in the `Controller` and it is ready to go.
```csharp
private void OnDrawGizmos()
{
    #if UNITY_EDITOR
    if (Application.isPlaying)
    {
        // Get all the active states as a tree
        UnityEditor.Handles.Label(transform.position, "Active: " + StateMachine.GetTree(StateMachine.CurrentState));
    }
    #endif
}
```


