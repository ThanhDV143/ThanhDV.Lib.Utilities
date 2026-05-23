# Singleton Utilities

Light-weight implementations of the Singleton pattern for:
- `Singleton<T>`: Plain C# (non-Unity) classes.
- `MonoSingleton<T>`: Scene (non-persistent) `MonoBehaviour`.
- `PersistentMonoSingleton<T>`: `MonoBehaviour` that survives scene loads (`DontDestroyOnLoad`).

All three variants expose:
- `Instance` — lazy, thread-safe accessor (locked on a static object).
- `Exists` — `true` once an instance has been created, without forcing instantiation.

## Variants

### Singleton<T>
- Pure C# (no Unity API for lifecycle).
- Constraint: `where T : class, new()` — requires a public parameterless constructor.
- Lazy, thread-safe via `lock`.

```csharp
public sealed class GameConfig : Singleton<GameConfig>
{
    public string CurrentLocale { get; set; } = "en";
}

// Usage
var locale = GameConfig.Instance.CurrentLocale;

if (GameConfig.Exists)
{
    // Access without triggering creation
}
```

### MonoSingleton<T>
- First access uses `FindFirstObjectByType<T>()` to locate an existing instance in the scene.
- If none is found, creates a new `GameObject` and attaches the component automatically.
- Destroyed when the scene is unloaded.

```csharp
public class AudioManager : MonoSingleton<AudioManager>
{
    public void PlayClick() { /* ... */ }
}

// Usage (in any script)
AudioManager.Instance.PlayClick();
```

### PersistentMonoSingleton<T>
- Same lookup/creation behavior as `MonoSingleton<T>`, but marked with `DontDestroyOnLoad`.
- Useful for systems: Audio, Save, Telemetry, Addressables bootstrap.

```csharp
public class SaveSystem : PersistentMonoSingleton<SaveSystem>
{
    public void Save() { /* ... */ }
}
```

## Overriding `Awake()`

`MonoSingleton<T>` and `PersistentMonoSingleton<T>` declare `Awake()` as `protected virtual`. It handles the duplicate check, instance assignment, and (for the persistent variant) `DontDestroyOnLoad`.

If you need your own initialization, override `Awake()` and call `base.Awake()` first — otherwise the singleton logic won't run.

```csharp
public class GameManager : PersistentMonoSingleton<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        // Your initialization code here
        Debug.Log("GameManager initialized!");
    }
}
```