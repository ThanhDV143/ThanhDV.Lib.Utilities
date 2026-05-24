# UIAdaptation

Utility scripts to adapt UI & 2D camera to any screen ratio. 

## Components

- **`UIScaler`** — Configures Unity's `CanvasScaler`. Sets `ScaleWithScreenSize` + auto-picks `matchWidthOrHeight` (0 = match width, 1 = match height) by comparing the actual screen ratio with `referenceResolution`.
- **`UISafeZone`** — Snaps a `RectTransform`'s anchors to `Screen.safeArea` (avoids notches / home indicator on modern iOS/Android).
- **`PersistentOrthographicCamera`** — Adjusts an orthographic `Camera.orthographicSize` so the horizontal world view stays consistent across aspect ratios.

## Runtime re-adaptation

| Component | Re-adapts on screen change? | How |
|---|---|---|
| `UIScaler` | ✅ Automatic | Hooks into Unity's `OnRectTransformDimensionsChange` — zero per-frame cost, fires on rotation / window resize / parent layout change. |
| `UISafeZone` | ✅ Automatic | Same `OnRectTransformDimensionsChange` mechanism. |
| `PersistentOrthographicCamera` | ⚙️ Opt-in via `autoReadapt` | Cameras don't have RectTransform callbacks, so this polls `Screen.width/height` in `Update`. Default off — leave off if the game is rotation-locked. |

For rotation-locked games (most mobile titles) you don't need `autoReadapt`; the one-time Awake setup is enough.