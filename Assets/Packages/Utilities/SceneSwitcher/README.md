# SceneSwitcher

Small Unity Editor utility that adds a scene dropdown to the right side of the main Toolbar. Switch scenes with one click.

## Modes

Cycle through them by clicking the source button on the toolbar:

| Mode | What it lists |
|---|---|
| **All Scenes** | Every `*.unity` file under the configured folder (default `Assets`), scanned recursively. |
| **Build Settings** | Only scenes enabled in `Build Settings`. |
| **Addressables** | Every scene asset registered in any Addressables group. |

## How to use

1. Keep `SceneSwitcher.cs` inside the editor-only asmdef (`ThanhDV.Utilities.SceneSwitcher`). It auto-initializes on domain load.
2. Toolbar UI shows:
    * **Source button** — click to cycle: All Scenes → Build Settings → Addressables → ...
    * **Scene popup** — pick a scene to open.
3. (All Scenes mode) The scanned folder defaults to `Assets`. Pref key: `SceneSwitcher_CustomScenePath`.
4. Picking a scene triggers Unity's save prompt if the current scene has unsaved changes.
5. Controls are disabled while in Play Mode.

## Notes

* Auto-refreshes on scene change, project change, or active scene change (throttled to 1s).
* Selected mode is remembered via EditorPrefs (`SceneSwitcher_SceneSource`, int 0–2).
* Invalid stored values fall back to `All Scenes` on the next load.
* Addressable scenes are opened with `EditorSceneManager.OpenScene(path)` — the same as regular scenes. The Addressables mode is for **filtering** the list, not for changing how scenes load in the editor.

## Requirements

* Unity 2022.3 or newer.
* `com.unity.addressables` 1.0.0+ — only needed if you want the Addressables mode to actually list scenes; SceneSwitcher itself works without it.
