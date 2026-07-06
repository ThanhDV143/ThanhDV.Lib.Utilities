# SceneSwitcher

Small Unity Editor utility that adds a scene dropdown to the right side of the main Toolbar. Switch scenes with one click. Also injects a red **Play From First Scene** button next to the built-in Play/Pause/Step group.

## Modes

Cycle through them by clicking the source button on the toolbar:

| Mode | What it lists |
|---|---|
| **All Scenes** | Every `*.unity` file under the configured folder (default `Assets`), scanned recursively. |
| **Build Settings** | Only scenes enabled in `Build Settings`. |
| **Addressables** | Every scene asset registered in any Addressables group. |

## How to use

1. Keep `SceneSwitcher.cs` and `PlayFromFirstSceneButton.cs` inside the editor-only asmdef (`ThanhDV.Utilities.SceneSwitcher`). Both auto-initialize on domain load.
2. Toolbar UI shows:
    * **Play From First Scene button** (red icon, left of Play/Pause/Step) — opens scene at index 0 of Build Settings and enters Play Mode. While playing, the button turns into a blue-background stop toggle; clicking it exits Play Mode and reopens the scenes you had open before.
    * **Source button** — click to cycle: All Scenes → Build Settings → Addressables → ...
    * **Scene popup** — pick a scene to open.
3. (All Scenes mode) The scanned folder defaults to `Assets`. Pref key: `SceneSwitcher_CustomScenePath`.
4. Picking a scene triggers Unity's save prompt if the current scene has unsaved changes.
5. Source button and scene popup are disabled while in Play Mode.

## Notes

* Auto-refreshes on scene change, project change, or active scene change (throttled to 1s).
* Selected mode is remembered via EditorPrefs (`SceneSwitcher_SceneSource`, int 0–2).
* Invalid stored values fall back to `All Scenes` on the next load.
* Addressable scenes are opened with `EditorSceneManager.OpenScene(path)` — the same as regular scenes. The Addressables mode is for **filtering** the list, not for changing how scenes load in the editor.
* Play From First Scene tracks the scenes you had open via `SessionState` (keys `SceneSwitcher_PrevScenes`, `SceneSwitcher_ActiveScenePath`) — state clears when the Editor closes.
* Toolbar injection uses reflection on Unity internals: `UnityEditor.Toolbars.MainToolbar.window` on Unity 6000.3+, `UnityEditor.Toolbar.m_Root` on older versions.

## Requirements

* Unity 2022.3 or newer.
* `com.unity.addressables` 1.0.0+ — only needed if you want the Addressables mode to actually list scenes; SceneSwitcher itself works without it.
