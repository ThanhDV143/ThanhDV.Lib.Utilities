# EventBus

Lightweight in-process event bus for Unity. Lets unrelated components communicate without direct references. Supports:

1. Events with data (`Action<T>`)
2. Marker events without data (`Action` keyed by generic type T)

Two ways to use it:
- **Singleton** — access the static `EventBus.Instance` from anywhere.
- **Dependency Injection** — depend on the `IEventBus` abstraction, register the singleton instance into your DI container (VContainer / Zenject / etc.).

Both modes share the same underlying instance and listener table.

## How to use

1. Define event types (struct/class). Add fields only if you need to pass data.
2. Register / unregister in `OnEnable` / `OnDisable` (or `OnDestroy`).
3. Post events from anywhere — via `EventBus.Instance` or an injected `IEventBus`.

⚠️:
* Type `T` is the dictionary key. For no-data events an internal cached key `FullTypeName_NoData` is used.
* Exceptions inside listeners are caught and logged so other listeners still run.
* Intended for main Unity thread (not fully thread-safe for concurrent writes).
* The singleton implementation is thread-safe using lock mechanism.

## Shared event types

The examples below reuse these two event types:

```csharp
// With data
public struct EnemyDied
{
    public int EnemyId;
    public Vector3 Position;
    public EnemyDied(int id, Vector3 pos) { EnemyId = id; Position = pos; }
}

// No data (marker)
public struct WaveCleared { }
```

## Example 1 — Singleton (default)

```csharp
using UnityEngine;
using ThanhDV.Utilities;

public class ScoreSystem : MonoBehaviour
{
    void OnEnable()
    {
        EventBus.Instance.Register<EnemyDied>(OnEnemyDied);
        EventBus.Instance.Register<WaveCleared>(OnWaveCleared);
    }

    void OnDisable()
    {
        EventBus.Instance.Unregister<EnemyDied>(OnEnemyDied);
        EventBus.Instance.Unregister<WaveCleared>(OnWaveCleared);
    }

    void OnEnemyDied(EnemyDied e) => Debug.Log($"Enemy {e.EnemyId} died at {e.Position}");
    void OnWaveCleared() => Debug.Log("Wave cleared!");
}

public class Enemy : MonoBehaviour
{
    public int Id;
    public void Die() => EventBus.Instance.Post(new EnemyDied(Id, transform.position));
}

public class WaveManager : MonoBehaviour
{
    public void Clear() => EventBus.Instance.Post<WaveCleared>();
}
```

## Example 2 — Dependency Injection

Register the existing singleton instance into your DI container so both worlds (legacy code calling `EventBus.Instance` and new code receiving `IEventBus`) share the same listener table.

### VContainer

```csharp
using VContainer;
using VContainer.Unity;
using ThanhDV.Utilities;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // Share the same instance with EventBus.Instance — avoids "two buses"
        builder.RegisterInstance<IEventBus>(EventBus.Instance);
    }
}
```

### Zenject

```csharp
using Zenject;
using ThanhDV.Utilities;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IEventBus>().FromInstance(EventBus.Instance).AsSingle();
    }
}
```

### Consumer with constructor injection

```csharp
using ThanhDV.Utilities;
using VContainer;

public class ScoreSystem : IStartable, System.IDisposable
{
    private readonly IEventBus _bus;

    [Inject]
    public ScoreSystem(IEventBus bus) => _bus = bus;

    public void Start()
    {
        _bus.Register<EnemyDied>(OnEnemyDied);
        _bus.Register<WaveCleared>(OnWaveCleared);
    }

    public void Dispose()
    {
        _bus.Unregister<EnemyDied>(OnEnemyDied);
        _bus.Unregister<WaveCleared>(OnWaveCleared);
    }

    private void OnEnemyDied(EnemyDied e) => UnityEngine.Debug.Log($"Enemy {e.EnemyId} died at {e.Position}");
    private void OnWaveCleared() => UnityEngine.Debug.Log("Wave cleared!");
}
```

### Consumer with field injection (MonoBehaviour)

```csharp
using UnityEngine;
using VContainer;
using ThanhDV.Utilities;

public class Enemy : MonoBehaviour
{
    [Inject] private IEventBus _bus;
    public int Id;

    public void Die() => _bus.Post(new EnemyDied(Id, transform.position));
}
```

## Unit testing with the interface

Because consumers depend on `IEventBus`, you can substitute a mock without touching the singleton:

```csharp
// Using NSubstitute
[Test]
public void Enemy_Die_PublishesEnemyDied()
{
    var bus = Substitute.For<IEventBus>();
    var enemy = new GameObject().AddComponent<Enemy>();
    // Inject the mock manually (or via test-time container)
    typeof(Enemy).GetField("_bus", BindingFlags.NonPublic | BindingFlags.Instance)
                 .SetValue(enemy, bus);
    enemy.Id = 42;

    enemy.Die();

    bus.Received(1).Post(Arg.Is<EnemyDied>(e => e.EnemyId == 42));
}
```

## Pitfalls

- **Don't register `IEventBus` with `Lifetime.Singleton` resolved via `new EventBus()`** — that creates a second bus that doesn't share listeners with `EventBus.Instance`. Always register the static instance: `RegisterInstance<IEventBus>(EventBus.Instance)`.
- **Mixing singleton calls and injected calls is fine** — both paths reach the same `_delegates` dictionary, so a listener registered via `EventBus.Instance` will receive events posted via an injected `IEventBus`, and vice versa.
- **Remember to unregister** — listeners hold references; failing to unregister causes leaks for as long as the bus lives (which is the whole app session).
