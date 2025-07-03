# Stateforge
**An Unity Hierarchical State Machine**
> Author: Okan Can Özbek (2025-07-03) for Unity 6000.1.0f1

A hierarchical state machine, supporting infinite nested states. Making prototyping Unity projects easier by implementing all the boilerplate classes and code for a smooth functioning hierachical finite state machine. There could still be a few quirks in the project, if you are able to find one don't hesitate to submit a ticket, I will do my best to fix it as soon as possible. Plans for the future include adding Unit Tests, and converting this project into a Unity package. Below you can find a quick explanation on how to set the project up.

## Prerequisites
The updated code for this package has been improved significantly. The previous release is now deprecated and won't be supported in the future.
1. To setup the project install it as a package into your Unity project.
2. Create the following files as an initial setup: ObjectContext, and ObjectStateFactory and ObjectStateMachine. Subsitute "Object" with the name of your choosing. In this example we're going to use "User"

## Setting up the State Machine
1. Create a new script called `UserContext` and inherit from `StateContext`. This will be the context for your state machine.
2. Create a new script called `UserStateFactory` and inherit from `StateFactory`, important to know `StateFactory` needs to have a definition for the given context. This will be the factory for your state machine.
3. Create a new script called `UserStateMachine` and inherit from `StateMachine`. This will be the state machine itself.

Let's start with the `UserContext` class, this can be anything from a `MonoBehaviour` to a `ScriptableObject`, but it needs to inherit from the `IStateContext` interface. The context is what the state machine can access, think of: User variables, settings, movement scripts, physics, etc. Here is an example of how the `UserContext` class can look like:
```csharp
public class UserContext : MonoBehaviour, IContext
{
    public Vector2 MovementDirection { get; private set; }
    public float MovementSpeed { get; private set; } = 10.0f;

    private void Update()
    {
        MovementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}
```

Next, we need to create the `UserStateFactory` class. This class is responsible for creating the states for the state machine. It needs to inherit from `StateFactory<UserContext>`, where `UserContext` is the context we created earlier. Here is an example of how the `UserStateFactory` class can look like:
```csharp
public class UserStateFactory : StateFactory<UserContext>
{
    protected override void SetStates()
    {
        AddRootState<UserIdleState>();
        AddRootState<UserMoveState>();

        AddChildState<UserIdleState, UserDefaultIdleAnimationState>();
        AddChildState<UserIdleState, UserSpecialIdleAnimationState>();
        
        AddChildState<UserMoveState, UserWalkState>();
    }
}
```
Let's break down the code:
- `AddRootState<UserIdleState>()`: This adds a root state to the state machine. The `UserIdleState` is a state that we will define later.
- `AddChildState<UserIdleState, UserDefaultIdleAnimationState>()`: This adds a child state to the `UserIdleState`. The `UserDefaultIdleAnimationState` is a state that we will define later.

By doing it this way we can define a clear tree like structure to our hierarchical state machine.

Finally, we need to create the `UserStateMachine` class. This class is responsible for managing the state machine and its states. It needs to inherit from `StateMachine<UserContext, UserStateFactory>`, where `UserContext` is the context we created earlier and `UserStateFactory` is the factory we created earlier. Here is an example of how the `UserStateMachine` class can look like:
```csharp
public class UserStateMachine : StateMachine<UserContext>
{
    private UserContext Context { get; set; }
    
    protected override void OnInit()
    {
        Context = GetComponent<UserContext>();
    }
}
```
The only thing we need to do here is to get the context from the `UserContext` component. This will allow us to access the context from the state machine. 
If your context is not a `MonoBehaviour` you can simply set the context in the `OnInit` method like this:
```csharp
protected override void OnInit()
{
    Context = new UserContext();
}
```
Now that we have set up the context, factory, and state machine, we can start defining our states. Let's create a simple idle state and a move state.

### Defining States

Let's start with the `UserIdleState` class. This class is responsible for handling the idle state of the user. It needs to inherit from `State<UserContext>`, where `UserContext` is the context we created earlier. Here is an example of how the `UserIdleState` class can look like:
```csharp
public class UserIdleState : State<UserContext>
{
    protected override void OnEnter()
    {
        SetChild<UserDefaultIdleAnimationState>();
    }
    
    protected override void SetTransitions()
    {
        AddTransition<UserMoveState>(() => Context.MovementDirection.x != 0.0f);
    }
}
```
In this example, we are setting a child state `UserDefaultIdleAnimationState` when the state is entered. We also set a transition to the `UserMoveState` when the `MovementDirection.x` is not equal to 0.0f, meaning the user is moving horizontally.

Next, we need to create the `UserDefaultIdleAnimationState` class. This class is responsible for handling the default idle animation of the user. It needs to inherit from `State<UserContext>`, where `UserContext` is the context we created earlier. Here is an example of how the `UserDefaultIdleAnimationState` class can look like:
```csharp
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
        // Imagine some animation logic playing here...
        _elapsedTime += Time.deltaTime;
    }
    
    protected override void SetTransitions()
    {
        AddTransition<UserSpecialIdleAnimationState>(() => _elapsedTime >= DefaultIdleDuration);
    }
}
```
In this example, we are setting the elapsed time to 0 when the state is entered. We also set a transition to the `UserSpecialIdleAnimationState` when the elapsed time is greater than or equal to the `DefaultIdleDuration`, meaning the user has been idle for a certain amount of time.

Next, we need to create the `UserSpecialIdleAnimationState` class. This class is responsible for handling a special idle animation of the user. It needs to inherit from `State<UserContext>`, where `UserContext` is the context we created earlier. Here is an example of how the `UserSpecialIdleAnimationState` class can look like:
```csharp
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
        // Imagine some special animation logic playing here...
        _elapsedTime += Time.deltaTime;
    }
    
    protected override void SetTransitions()
    {
        AddTransition<UserDefaultIdleAnimationState>(() => _elapsedTime >= AnimationDuration);
    }
}
```
In this example, we are setting the elapsed time to 0 when the state is entered. We also set a transition to the `UserDefaultIdleAnimationState` when the elapsed time is greater than or equal to the `AnimationDuration`, meaning the special idle animation has finished playing.

Next, we need to create the `UserMoveState` class. This class is responsible for handling the move state of the user. It needs to inherit from `State<UserContext>`, where `UserContext` is the context we created earlier. Here is an example of how the `UserMoveState` class can look like:
```csharp
public class UserMoveState : State<UserContext>
{
    protected override void OnEnter()
    {
        SetChild<UserWalkState>();
    }

    protected override void SetTransitions()
    {
        AddTransition<UserIdleState>(() => Context.MovementDirection.x == 0.0f);
    }
}
```
In this example, we are setting a child state `UserWalkState` when the state is entered. We also set a transition to the `UserIdleState` when the `MovementDirection.x` is equal to 0.0f, meaning the user is not moving horizontally.

Finally, we need to create the `UserWalkState` class. This class is responsible for handling the walk state of the user. It needs to inherit from `State<UserContext>`, where `UserContext` is the context we created earlier. Here is an example of how the `UserWalkState` class can look like:
```csharp   
public class UserWalkState : State<UserContext>
{
    protected override void OnUpdate()
    {
        Context.transform.Translate(Context.MovementDirection * Context.MovementSpeed * Time.deltaTime);
    }
    
    protected override void SetTransitions()
    {
        AddTransition<UserIdleState>(() => Context.MovementDirection.x == 0.0f);
    }
}
```
In this example, we are moving the user in the direction of the `MovementDirection` multiplied by the `MovementSpeed` and `Time.deltaTime`. We also set a transition to the `UserIdleState` when the `MovementDirection.x` is equal to 0.0f, meaning the user is not moving horizontally.

And that is it! You have successfully set up a hierarchical state machine in Unity using the Stateforge package. You can now add more states and transitions as needed to create a more complex state machine.

### Bonus
If you want to view your states within the Unity Editor, you can open the window by going to `Window > Stateforge > State Viewer`. This will allow you to see the states and transitions in a visual way, making it easier to debug and understand your state machine.

All you have to do then is to click on the GameObject with the `UserStateMachine` component attached. There you will have a quick overview of all available states and transitions. In play-mode you are able to see the current active state branch, from the root to the leaf.

![image](https://github.com/user-attachments/assets/f2dd8dca-9058-4bb7-885b-72be797a5688)

### Conclusion
Stateforge is a powerful tool for creating hierarchical state machines in Unity. It allows you to easily create and manage states and transitions, making it easier to prototype and develop your Unity projects. With the ability to view your states in the Unity Editor, you can quickly debug and understand your state machine. I hope this guide has helped you get started with Stateforge and that you find it useful in your Unity projects. If you have any questions or feedback, feel free to reach out!
