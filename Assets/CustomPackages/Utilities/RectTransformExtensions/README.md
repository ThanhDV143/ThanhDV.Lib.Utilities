# RectTransform Extensions

Utility extension methods for Unity `RectTransform` to quickly set anchors, pivot, and offsets via concise calls.

## How to use

1. Add the namespace: `using ThanhDV.Utilities;`
2. Call the extension methods on any `RectTransform`.

## API

### Anchors

```csharp
public static void SetAnchor(
    this RectTransform source,
    AnchorPresets align,
    int? offsetX = null,
    int? offsetY = null);
```

- `align` — one of the `AnchorPresets` enum values (see below).
- `offsetX`, `offsetY` — optional new `anchoredPosition.x` / `.y`. When omitted (`null`), the current `anchoredPosition` component is preserved.

**`AnchorPresets` values:**

| Group | Values |
|---|---|
| Corner | `TopLeft`, `TopCenter`, `TopRight`, `MiddleLeft`, `MiddleCenter`, `MiddleRight`, `BottomLeft`, `BottomCenter`, `BottomRight` |
| Horizontal stretch | `HorStretchTop`, `HorStretchMiddle`, `HorStretchBottom` |
| Vertical stretch | `VertStretchLeft`, `VertStretchCenter`, `VertStretchRight` |
| Full | `StretchAll` |

### Pivot

```csharp
public static void SetPivot(this RectTransform source, PivotPresets preset);
```

**`PivotPresets` values:**

`TopLeft`, `TopCenter`, `TopRight`, `MiddleLeft`, `MiddleCenter`, `MiddleRight`, `BottomLeft`, `BottomCenter`, `BottomRight`.

### Offsets

```csharp
public static void SetOffset(this RectTransform rt, float left, float right, float top, float bottom);
public static void SetOffsetLeft(this RectTransform rt, float left);
public static void SetOffsetRight(this RectTransform rt, float right);
public static void SetOffsetTop(this RectTransform rt, float top);
public static void SetOffsetBottom(this RectTransform rt, float bottom);
```

`SetOffset` writes both `offsetMin` and `offsetMax`. The single-side variants modify only one edge.

## Example

```csharp
using UnityEngine;
using ThanhDV.Utilities;

public class UISetup : MonoBehaviour
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private RectTransform bottomBar;
    [SerializeField] private RectTransform topBadge;

    private void Awake()
    {
        // Center the panel — keep its current anchoredPosition
        panel.SetAnchor(AnchorPresets.MiddleCenter);
        panel.SetPivot(PivotPresets.MiddleCenter);

        // Stretch bottomBar across the bottom edge and pad it 16px on each side
        bottomBar.SetAnchor(AnchorPresets.HorStretchBottom);
        bottomBar.SetPivot(PivotPresets.BottomCenter);
        bottomBar.SetOffset(left: 16f, right: 16f, top: 120f, bottom: 0f);

        // Pin a badge to the top-right with a 12px,12px offset
        topBadge.SetAnchor(AnchorPresets.TopRight, offsetX: -12, offsetY: -12);
        topBadge.SetPivot(PivotPresets.TopRight);
    }
}
```

## Notes

- `SetAnchor` and `SetPivot` return `void` — they don't chain. Call them on consecutive lines.
- Omit `offsetX` / `offsetY` to preserve the existing `anchoredPosition` instead of zeroing it.
- `SetOffset(left, right, top, bottom)` follows Unity's convention where `offsetMax` stores **negated** right/top values internally; the method does the negation for you.
