# Lifetime Binding

Lightweight memory management add-on for Unity's Addressable Asset System.

## Requirements

- Unity 2020.3 or higher
- `com.unity.addressables` package installed
- (Optional) UniTask

## Features

- **Lifetime Binding** - bind Addressable handles to a `GameObject` or any `IReleaseEvent` (incl. non-MonoBehaviour owners). Built-in triggers: GameObject destroy, particle finish, manual.
- **Preloading** - preload assets by address/label with progress, retrieve synchronously, dispose by binding. Coroutine + UniTask APIs.

## Usage

### Bind an Addressable handle to a GameObject

```csharp
using ThanhDV.Utilities;
using UnityEngine.AddressableAssets;

var handle = Addressables
    .LoadAssetAsync<GameObject>("MyPrefab")
    .BindTo(gameObject);

await handle.Task;
// When gameObject is destroyed, the handle is released automatically.
```

### Release on particle finish

```csharp
var fx = Instantiate(explosionPrefab);
var releaseEvent = fx.AddComponent<ParticleSystemBasedReleaseEvent>();

Addressables
    .LoadAssetAsync<AudioClip>("ExplosionSfx")
    .BindTo(releaseEvent);
```

### Custom release trigger

```csharp
using System;
using MyUtilities.LifetimeBinding;
using UnityEngine;

public sealed class TimerBasedReleaseEvent : MonoBehaviour, IReleaseEvent
{
    [SerializeField] private float duration = 5f;

    private void Start() => Invoke(nameof(Fire), duration);
    private void Fire() => Dispatched?.Invoke();

    public event Action Dispatched;
}
```

### Preload assets, then access synchronously

```csharp
using MyUtilities.LifetimeBinding;
using MyUtilities.LifetimeBinding.Preloading;

var preloader = new AddressablePreloader().BindTo(gameObject);

// Preload (coroutine)
yield return preloader.PreloadKey<GameObject>("Enemy");
yield return preloader.PreloadKeys<GameObject>(new[] { "Bullet", "Pickup" });

// Or with UniTask
await preloader.PreloadKeyAsync<GameObject>("Enemy");

// Retrieve synchronously
var enemyPrefab = preloader.GetAsset<GameObject>("Enemy");
var allBullets = preloader.GetAssets<GameObject>("BulletsLabel");
```

## Notes

- **`BindTo(gameObject)` releases on Destroy, not on Disable.** If you need different timing, implement `IReleaseEvent` yourself.
- **UniTask is optional.** The `*Async` methods (e.g. `PreloadKeyAsync`) are only compiled when a supported UniTask package is installed. Coroutine APIs are always available.

## Credits

Based on [Addler](https://github.com/Haruma-K/Addler) by Haruma-K (MIT License).