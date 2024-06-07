# BlahSignals
BlahSignals is Unity framework that provides ECS-like architecture. 

**Disclaimer**<br/>
>The development is ended. See the successor - [BlahFramework](https://github.com/rosdvp/BlahFramework).

In the contrast to ECS there are no Entity and Component. Instead, there are Signals - pooled structs that can be used as events, commands, requests etc.

The architecture assumes that you keep game data in a separate Model instead of Components to make it easy to serialize and control data migration between different game versions.

At the same time, the architecture assumes Systems conveyor which is invoked each frame. The System-System or Unity-System communication should be handled via Signals: when something happens, one system creates a signal-event and other systems can consume it. 

The **key feature** of the framework is auto-generation of such Systems conveyor. In most ECS frameworks you have to manually control the order in which Systems are invoked. However, things become messy once you have hundred of Systems. BlahSignals allows you to mark which data each System consumes/produces, and automatically order Systems in a way that all producers will Run before all consumers inside single frame.



## Signals
The adding or reading of signals happens via BlahSignal instances. There is always only one instance per each signal type which is shared between all users. This allows you to produce a signal in one class and consume it in another one.

### One-Frame
By default, all signals are one-frame. This means, that all signal entries will be removed once you tell the framework that the frame is done.

Generaly, One-Frame signals are used for:
1) Events - tell other systems that something has happened in this frame.
2) Commands - tell to some system that something should be done.

In most cases you should prefer One-Frame signals to Not-One-Frame or Next-Frame signals.

```csharp
// By default, signals are one-frame.
// Optionally, you can implement IBlahResettable interface to automatically
// set all fields to default value once a new signal is added.
public struct Event : IBlahResettable
{
    public int X;
    public int Y;
}

// SignalsContext instance should be stored and shared between all users.
var signals = new BlahSignalsContext();

// Get a signal for Event.
BlahSignal<Event> signalEv = signals.Get<EV>();

// Create a new Event in single line.
signalEv.Add().X = 1;

// Create a new Event in multiple lines.
ref var newEv = ref signalEv.Add();
newEv.X = 1;
neWEv.Y = 2;

if (!signalEv.IsEmpty)          // At least one Event exists at this frame.
    Debug.Log(signalEv.Count);  // Get count of Events at this frame.

foreach (ref Event ev in signalEv)
    Debug.Log(ev.X); // Get all existing Events with their fields.

// Remove all one-frame signals entries.
signals.OnFrameEnd();

// Here:
// signalEv.IsEmpty == true
```

### Not-One-Frame
There still might be cases when you want signal live more than one frame.
For instance, it might be on-going request with a progress or state. This is not recommended behaviour since if your game should save the progress of a request, it will be better to put it inside the Model. 

However, for animations (and etc) the Not-One-Frame signals might be the option. These signals are not removed once you tell the framework that the frame is done, so you have to remove them manually on demand.

```csharp
public struct Event
{
    public int X;
}

// If struct implements this interface, 
// signal entries will not be removed on frame end call.
public struct Data : IBlahNotOneFrame
{
    public int X;
}

var signals = new BlahSignalsContext();
var signalEv = signals.Get<Event>();
var signalData = signals.Get<Data>();

signalEv.Add().X = 1;
signalData.Add().X = 2;

signals.OnFrameEnd();

// Here:
// signalEv.IsEmpty == true
// signalData.IsEmpty == false

foreach (ref Data data in signalData)
{
    // Remove only Data with X == 2.
    if (data.X == 2)
        signalData.DelCurrentInIteration();
}

// Alternatively, you can remove all entires of signal.
signalData.DellAll();
```
### Next-Frame
Sometimes it is not possible to process all data inside one frame.
This might happen because of cyclic dependencies between systems. 

For instance, System A adds a signal-command and consumes signal-event that command is handled. System B waits signal-command and adds signal-event. Obviously, this cannot be done inside one frame if all Systems are invoked only once per frame.

The preferable way to solve such problem is to split Systems in a smaller one, but in case you are not able to do that for certain reasons, you might want to use Next-Frame signals.

Next-Frame signal is added at current frame, but will be available for reading only after you tell the framework that the current frame is done.

```csharp
struct Event 
{
    public int X;
}

var signals = new BlahSignalsContext();
var signalEv = signals.Get<Event>();

// Add signal for this frame.
signalEv.Add().X = 1;
// Add another signal for next frame.
signalEv.AddNextFrame().X = 2;

foreach (ref var ev in signalEv)
    Debug.Log(ev.X); // Only '1' will be printed.

signals.OnFrameEnd();

foreach (ref var ev in signalEv)
    Debug.Log(ev.X) // Only '2' will be printed.

```


## Systems and Groups
The systems are contained inside groups. Each system belongs to only one group. At the same time, groups are stored in BlahSystemsGroupsContext instance. You should create the instance at startup and keep it during game life-time, since it is an entry-point to your systems.

Optionally, you can provide some context to your systems from Unity world.

```csharp
// You can provide references from Unity scene to systems
// by collecting them into one class and implementing the following interface.
class MyContext : IBlahSystemsInitData
{
    public GameObject GO;
}

// Create a class for each system 
// and implement at least one of the following interfaces.
class SystemA1 : IBlahInitSystem, IBlahResumePauseSystem, IBlahRunSystem
{
    // This method is invoked:
    // 1) When systems' group becomes active first time.
    // 2) Before "Resume".
    public void Init(IBlahSystemsInitData initData)
    {
        var myContext = (MyContext) initData;
        // myContext.GO...
    }
    // This method is invoked:
    // 1) Each time systems' group becomes active.
    // 2) Before "Run".
    public void Resume() {}
    // This method is invoked:
    // 1) Each time systems' group becomes inactive.
    // 2) After "Run".
    public void Pause() {}

    // This method is invoked each time you invoke Run on SystemsContext,
    // if the group of the system is active.
    public void Run() {}
}

// class SystemA2, class SystemB1, class SystemB2 ...

// Define enum of groups.
enum EGroup
{
    A,
    B
}


var myContext = new MyContext();

// SystemsContext instance should be stored and shared between all users.
var systems = new BlahSystemsGroupsContext<EGroup>(myContext);

// Add first group of systems.
var groupA = systems.AddGroup(EGroup.A);
groupA.AddSystem<SystemA1>(); // Add system to the first group of systems.
groupB.AddSystem<SystemA2>();

// Add second group of systems.
var groupB = systems.AddGroup(EGroup.B);
groupB.AddSystem<SystemB1>(); // Add system to the second group of systems.
groupB.AddSystem<SystemB2>();

// By default, no group is active.
// After you switch active group to the first group, 
// systems' "Init" and "Resume" methods will be invoked.
systems.SwitchToGroup(EGroup.A, EGroupSwitchMode.SwitchNow);

// Invoke "Run" each frame.
// Here "Run" of systems in group A will be invoked since group A is active.
systems.Run();

// Here "Pause" will be invoked on systems of group A and "Resume" will be invoked on systems of group B.
systems.SwitchToGroup(EGroup.B, EGroupSwitchMode.SwitchNode);

// Here "Run" of systmes in group B will be invoked since it is active now.
systems.Run();

// You can deactivate all groups.
systems.SwitchToNoGroup();
```

## Auto-Order

The section above contains example of how to manually add systems to pipeline. However, the key feature is to generate the order of systems automatically. At first you should:
1) Find "BlahExampleOrderedSystemsContainer.cs" file and follow instruction inside.
2) Find "BlahExampleUnOrderedSystemsContainer.cs" file and follow instruction inside.
3) Examine example below.

```csharp
struct PurchaseCmd {}
struct PurchasedEv {}
struct MoneyChangedEv {}

class IncomeSystem : IBlahRunSystem
{
    // Tell framework that this system produces this signal,
    // so any system that consumes this signal should run after this one.
    [BlahProduce] private BlahSignal<MoneyChangedEv> _moneyChangedEv;

    public void Run()
    {
        ...
        _moneyChangedEv.Add();
    }
}

class InputSystem : IBlahRunSystem
{
    [BlahProduce] private BlahSignal<PurchaseCmd> _purchaseCmd;

    public void Run()
    {
        ...
        _purchaseCmd.Add();
    }
}

class PurchaseSystem : IBlahRunSystem
{
    // Tell framework that this system consumes this signal,
    // so this system should run after all systems that produce it.
    [BlahConsume] private BlahSignal<PurchaseCmd> _purchaseCmd;
    
    [BlahProduce] private BlahSignal<PurchasedEv> _purchasedEv;
    [BlahProduce] private BlahSignal<MoneyChangedEv> _moneyChangedEv;

    public void Run()
    {
        foreach (ref var cmd in _purchaseCmd)
        {
            ...
            _purchasedEv.Add();
            _moneyChangedEv.Add();
        }
    }
}

class UiSystem : IBlahRunSystem
{
    [BlahConsume] private BlahSignal<MoneyChangedEv> _moneyChangedEv;
    [BlahConsume] private BlahSignal<PurchasedEv> _purchasedEv;

    public void Run()
    {
        foreach (ref var ev in _moneyChangedEv)
            // Update UI for balance.
        foreach (ref var ev in _purchasedEv)
            // Update UI for purchased stuff.
    }
}


// BlahUnOrderedSystemsContainer.cs
public static class BlahUnOrderedSystemsContainer
{
    // You can add systems to this list at any order,
    // since they will be automatically ordered.
	private static readonly List<Type> SystemsToOrder = new()
	{
        typeof(InputSystem),
        typeof(UiSystem),
        typeof(IncomeSystem),
        typeof(PurchaseSystem)
    }
}

// BlahOrderedSystemsContainer.cs
// You should not modify this class manually.
// The content will be generated based on the class above.
public static class BlahOrderedSystemsContainer
{
	public static void AddOrderedSystems(BlahSystemsGroup systems)
	{
		//START-CODEGEN
        systems.Add<InputSystem>();
        systems.Add<IncomeSystem>();
        systems.Add<PurchaseSystem>();
        systems.Add<UiSystem>();
		//END-CODEGEN
    }
}


public class GameStartup
{
    private void Start()
    {
        var systems = new BlahSystemsGroupsContext<EGroup>();
        var mainGroup = systems.AddGroup(EGroup.Main);
        // Add auto-ordered systems in one of your systems groups.
        BlahOrderedSystemsContainer.AddOrderedSystems(mainGroup);
    }
}
```
Besided "BlahConsume" and "BlahProduce" attributes you might also use:
1) BlahAfterSystem(Type) attribute on system's class to force the system run after the specified one.
2) BlahProduceNextFrame attribute on signal field. The attribute is just a decoration for developers to not forget that system produce this signal as Next-Frame. It does not affect systems order.
3) BlahNotOneFrame attribute on signal field. The decoration for developers as well as BlahProduceNextFrame.

## Signals fields Injection
In case the performance hit caused by C# reflection is not an issue, you can use injection of signals into the systems' fields to avoid boilerplate code.
```csharp
class GameStartup
{
    [SerializedField]
    private MyContext _myContext;

    private BlahSignalsContext _signals;
    private BlahSystemsGroupsContext _systems;

    private void Start()
    {
        _signals = new BlahSignalsContext();

		_systems = new BlahSystemsGroupsContext<EGroup>(_myContext);

		var startupGroup = _groups.AddGroup(EGroup.Startup);
		startupGroup.AddSystem(new PreLoadingSystem());
		
		var mainGroup = _groups.AddGroup(EGroup.Main);
		BlahOrderedSystemsContainer.AddOrderedSystems(mainGroup);

		var pauseGroup = _groups.AddGroup(EGroup.Pause);
		pauseGroup.AddSystem(new GameRestartSystem());
		pauseGroup.AddSystem(new UiScreensSystem());
            
        // Create injector of signals.
		var injector = new BlahInjector(
            _signals,
		    typeof(BlahSignalsContext).GetMethod("Get"),
		    typeof(BlahSignal<>)
        );
        // Invoke "Inject" method on each group to fill Signals fields.
		injector.Inject(startupGroup.GetAllSystems());
		injector.Inject(mainGroup.GetAllSystems());
		injector.Inject(pauseGroup.GetAllSystems());
    }
}
```