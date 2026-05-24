# Debug Drawing Extension

Utility extension for drawing debug geometry to visualize gameplay logic, collisions, directions, areas of effect, etc.

## API Groups

- **`DebugExt`** — `Debug.DrawLine` / `Debug.DrawRay` based. Visible in Play Mode. Supports `duration` + `depthTest`.
- **`GizmosExt`** — `Gizmos.*` based. Only inside `OnDrawGizmos` / `OnDrawGizmosSelected`.
- **`MethodsDebug`** — reflection helpers.

All drawing methods are wrapped in `#if UNITY_EDITOR` — no-op in builds.

## Basic Usage

```csharp
using ThanhDV.Utilities;
using UnityEngine;

public class DebugExample : MonoBehaviour
{
    void Update()
    {
        DebugExt.DrawArrow(transform.position, transform.forward * 3, Color.red);
    }

    void OnDrawGizmosSelected()
    {
        GizmosExt.DrawArrow(transform.position, transform.forward * 3, Color.red);
    }
}
```

## `DebugExt` — API

```csharp
DrawPoint(Vector3 position, Color color, float scale = 1.0f, float duration = 0, bool depthTest = true);
DrawPoint(Vector3 position, float scale = 1.0f, float duration = 0, bool depthTest = true);

DrawBounds(Bounds bounds, Color color, float duration = 0, bool depthTest = true);
DrawBounds(Bounds bounds, float duration = 0, bool depthTest = true);

DrawLocalCube(Transform transform, Vector3 size, Color color, Vector3 center = default, float duration = 0, bool depthTest = true);
DrawLocalCube(Transform transform, Vector3 size, Vector3 center = default, float duration = 0, bool depthTest = true);
DrawLocalCube(Matrix4x4 space, Vector3 size, Color color, Vector3 center = default, float duration = 0, bool depthTest = true);
DrawLocalCube(Matrix4x4 space, Vector3 size, Vector3 center = default, float duration = 0, bool depthTest = true);

DrawCircle(Vector3 position, Vector3 up, Color color, float radius = 1.0f, float duration = 0, bool depthTest = true);
DrawCircle(Vector3 position, Color color, float radius = 1.0f, float duration = 0, bool depthTest = true);
DrawCircle(Vector3 position, Vector3 up, float radius = 1.0f, float duration = 0, bool depthTest = true);
DrawCircle(Vector3 position, float radius = 1.0f, float duration = 0, bool depthTest = true);

DrawWireSphere(Vector3 position, Color color, float radius = 1.0f, float duration = 0, bool depthTest = true);
DrawWireSphere(Vector3 position, float radius = 1.0f, float duration = 0, bool depthTest = true);

DrawCylinder(Vector3 start, Vector3 end, Color color, float radius = 1, float duration = 0, bool depthTest = true);
DrawCylinder(Vector3 start, Vector3 end, float radius = 1, float duration = 0, bool depthTest = true);

DrawCone(Vector3 position, Vector3 direction, Color color, float angle = 45, float duration = 0, bool depthTest = true);
DrawCone(Vector3 position, Vector3 direction, float angle = 45, float duration = 0, bool depthTest = true);
DrawCone(Vector3 position, Color color, float angle = 45, float duration = 0, bool depthTest = true);
DrawCone(Vector3 position, float angle = 45, float duration = 0, bool depthTest = true);

DrawArrow(Vector3 position, Vector3 direction, Color color, float duration = 0, bool depthTest = true);
DrawArrow(Vector3 position, Vector3 direction, float duration = 0, bool depthTest = true);

DrawCapsule(Vector3 start, Vector3 end, Color color, float radius = 1, float duration = 0, bool depthTest = true);
DrawCapsule(Vector3 start, Vector3 end, float radius = 1, float duration = 0, bool depthTest = true);
```

## `GizmosExt` — API

No `DrawWireSphere` — use Unity's built-in `Gizmos.DrawWireSphere(center, radius)`.

```csharp
DrawPoint(Vector3 position, Color color, float scale = 1.0f);
DrawPoint(Vector3 position, float scale = 1.0f);

DrawBounds(Bounds bounds, Color color);
DrawBounds(Bounds bounds);

DrawLocalCube(Transform transform, Vector3 size, Color color, Vector3 center = default);
DrawLocalCube(Transform transform, Vector3 size, Vector3 center = default);
DrawLocalCube(Matrix4x4 space, Vector3 size, Color color, Vector3 center = default);
DrawLocalCube(Matrix4x4 space, Vector3 size, Vector3 center = default);

DrawCircle(Vector3 position, Vector3 up, Color color, float radius = 1.0f);
DrawCircle(Vector3 position, Color color, float radius = 1.0f);
DrawCircle(Vector3 position, Vector3 up, float radius = 1.0f);
DrawCircle(Vector3 position, float radius = 1.0f);

DrawCylinder(Vector3 start, Vector3 end, Color color, float radius = 1.0f);
DrawCylinder(Vector3 start, Vector3 end, float radius = 1.0f);

DrawCone(Vector3 position, Vector3 direction, Color color, float angle = 45);
DrawCone(Vector3 position, Vector3 direction, float angle = 45);
DrawCone(Vector3 position, Color color, float angle = 45);
DrawCone(Vector3 position, float angle = 45);

DrawArrow(Vector3 position, Vector3 direction, Color color);
DrawArrow(Vector3 position, Vector3 direction);

DrawCapsule(Vector3 start, Vector3 end, Color color, float radius = 1);
DrawCapsule(Vector3 start, Vector3 end, float radius = 1);
```

## `MethodsDebug` — API

```csharp
string MethodsOfObject(System.Object obj, bool includeInfo = false);   // throws ArgumentNullException if obj is null
string MethodsOfType(System.Type type, bool includeInfo = false);      // throws ArgumentNullException if type is null
```
