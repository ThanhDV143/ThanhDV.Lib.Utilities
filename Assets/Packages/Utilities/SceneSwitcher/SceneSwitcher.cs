using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
#if UNITY_6000_3_OR_NEWER
using UnityEditor.Toolbars;
#endif
#if SCENE_SWITCHER_ADDRESSABLES
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace ThanhDV.Utilities
{
    [InitializeOnLoad]
    public static class SceneSwitcher
    {
        private static string[] _sceneNames = Array.Empty<string>();
        private static string[] _scenePaths = Array.Empty<string>();
        private static int _selectedIndex;
        private static string _lastActiveScene = "";
#if !UNITY_6000_3_OR_NEWER
        private static VisualElement _toolbarUI;
        private static VisualElement _rightContainer;
#endif
        private static string _customScenePath;

        private static bool _sceneListDirty = true;
        private static double _nextAllowedRefreshTime;

        private const float DROPDOWN_BOX_HEIGHT = 20f;
        private const string DEFAULT_SCENE_PATH = "Assets";
        private const string SCENE_PATH_PREF_KEY = "SceneSwitcher_CustomScenePath";

        private const double REFRESH_INTERVAL_SECONDS = 1.0;
#if UNITY_6000_3_OR_NEWER
        private const string MAIN_TOOLBAR_PATH = "ThanhDV.Utilities/SceneSwitcher";
#endif

        private enum SceneSource
        {
            AllScenes = 0,
            BuildSettings = 1,
            Addressables = 2,
        }

        private const string SCENE_SOURCE_PREF_KEY = "SceneSwitcher_SceneSource";
        private const string ADDRESSABLES_MISSING_HINT = "Install com.unity.addressables";

        // Addressables mode is always exposed in the cycle so users discover it exists
        // and learn they need to install the package + add the SCENE_SWITCHER_ADDRESSABLES define.
        private const int SourceCount = 3;

        private static SceneSource Source
        {
            get
            {
                var raw = EditorPrefs.GetInt(SCENE_SOURCE_PREF_KEY, 0);
                if (raw < 0 || raw >= SourceCount) raw = 0;
                return (SceneSource)raw;
            }
            set => EditorPrefs.SetInt(SCENE_SOURCE_PREF_KEY, (int)value);
        }

        private static string GetSourceLabel(SceneSource source) => source switch
        {
            SceneSource.AllScenes => "All Scenes",
            SceneSource.BuildSettings => "Build Settings",
            SceneSource.Addressables => "Addressables",
            _ => "All Scenes",
        };

        private static string GetEmptyListMessage()
        {
#if !SCENE_SWITCHER_ADDRESSABLES
            if (Source == SceneSource.Addressables)
                return ADDRESSABLES_MISSING_HINT;
#endif
            return "No Scenes";
        }

#if !SCENE_SWITCHER_ADDRESSABLES
        private static void OpenAddressablesInPackageManager()
        {
            UnityEditor.PackageManager.UI.Window.Open("com.unity.addressables");
        }
#endif

        private static string CustomScenePath
        {
            get
            {
                if (string.IsNullOrEmpty(_customScenePath))
                {
                    _customScenePath = EditorPrefs.GetString(SCENE_PATH_PREF_KEY, DEFAULT_SCENE_PATH);
                }
                return _customScenePath;
            }
            set
            {
                _customScenePath = value;
                EditorPrefs.SetString(SCENE_PATH_PREF_KEY, value);
            }
        }

        static SceneSwitcher()
        {
            RefreshSceneListIfNeeded(force: true);

            EditorSceneManager.activeSceneChangedInEditMode += (_, _) =>
            {
                UpdateSceneSelection();
#if UNITY_6000_3_OR_NEWER
                RefreshToolbarUI();
#else
                EditorApplication.delayCall += EnsureToolbarUI;
#endif
            };
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.projectChanged += OnProjectChanged;

#if !UNITY_6000_3_OR_NEWER
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            EditorApplication.delayCall += EnsureToolbarUI;
#endif
        }

#if !UNITY_6000_3_OR_NEWER
        private static void OnHierarchyChanged()
        {
            EditorApplication.delayCall += EnsureToolbarUI;
        }
#endif

        private static void OnProjectChanged()
        {
            MarkSceneListDirty();
#if UNITY_6000_3_OR_NEWER
            RefreshToolbarUI();
#else
            EditorApplication.delayCall += EnsureToolbarUI;
#endif
        }

        private static void MarkSceneListDirty()
        {
            _sceneListDirty = true;
        }

#if !UNITY_6000_3_OR_NEWER
        private static void EnsureToolbarUI()
        {
            var toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
            if (toolbarType == null) return;

            var toolbars = Resources.FindObjectsOfTypeAll(toolbarType);
            if (toolbars.Length == 0) return;

            var toolbar = toolbars[0];
            var rootField = toolbarType.GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
            if (rootField == null) return;

            var root = rootField.GetValue(toolbar) as VisualElement;
            _rightContainer = root?.Q("ToolbarZoneRightAlign");
            if (_rightContainer == null) return;

            if (_toolbarUI == null || _toolbarUI.parent != _rightContainer)
            {
                if (_toolbarUI != null)
                {
                    _toolbarUI.RemoveFromHierarchy();
                }

                _toolbarUI = new IMGUIContainer(OnGUI) { };
                _rightContainer.Add(_toolbarUI);

                _rightContainer.RegisterCallback<GeometryChangedEvent>(_ => EnsureToolbarUI());
                _rightContainer.RegisterCallback<DetachFromPanelEvent>(_ => EditorApplication.delayCall += EnsureToolbarUI);
            }
        }
#endif

#if UNITY_6000_3_OR_NEWER
        [MainToolbarElement(MAIN_TOOLBAR_PATH, defaultDockPosition = MainToolbarDockPosition.Right)]
        private static IEnumerable<MainToolbarElement> CreateMainToolbarElements()
        {
            RefreshSceneListIfNeeded();

            var isPlaying = EditorApplication.isPlaying;
            var toggleButton = new MainToolbarButton(
                new MainToolbarContent(GetSourceLabel(Source), "Cycle scene source"),
                ToggleSceneSource)
            {
                enabled = !isPlaying
            };

            var selectedSceneName = GetSelectedSceneDisplayName();
            var dropdown = new MainToolbarDropdown(
                new MainToolbarContent(selectedSceneName, "Select and open a scene"),
                ShowSceneDropdownMenu)
            {
                enabled = !isPlaying
            };

            return new MainToolbarElement[] { toggleButton, dropdown };
        }

        private static void ToggleSceneSource()
        {
            var next = ((int)Source + 1) % SourceCount;
            Source = (SceneSource)next;

            if (Source == SceneSource.AllScenes)
            {
                CustomScenePath = DEFAULT_SCENE_PATH;
            }

            MarkSceneListDirty();
            RefreshSceneListIfNeeded(force: true);
            RefreshToolbarUI();
        }

        private static void RefreshToolbarUI()
        {
            MainToolbar.Refresh(MAIN_TOOLBAR_PATH);
        }
#else
        private static void OnGUI()
        {
            RefreshSceneListIfNeeded();

            if (_sceneNames.Length == 0)
            {
                _selectedIndex = 0;
            }
            else if (_selectedIndex >= _sceneNames.Length)
            {
                _selectedIndex = 0;
            }

            var isPlaying = EditorApplication.isPlaying;

            GUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(isPlaying);
            if (GUILayout.Button(GetSourceLabel(Source), "Button", GUILayout.Height(DROPDOWN_BOX_HEIGHT)))
            {
                var next = ((int)Source + 1) % SourceCount;
                Source = (SceneSource)next;
                MarkSceneListDirty();
            }

            if (Source == SceneSource.AllScenes)
            {
                CustomScenePath = DEFAULT_SCENE_PATH;
            }

            var label = new GUIContent(GetSelectedSceneDisplayName());
            var popupStyle = new GUIStyle(EditorStyles.popup) { fixedHeight = DROPDOWN_BOX_HEIGHT };
            var rect = GUILayoutUtility.GetRect(label, popupStyle, GUILayout.Width(150), GUILayout.Height(DROPDOWN_BOX_HEIGHT));
            if (EditorGUI.DropdownButton(rect, label, FocusType.Keyboard, popupStyle))
            {
                ShowSceneDropdownMenu(rect);
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.EndHorizontal();
        }
#endif

        private static string GetSelectedSceneDisplayName()
        {
            if (_sceneNames.Length == 0)
                return GetEmptyListMessage();

            if (_selectedIndex < 0 || _selectedIndex >= _sceneNames.Length)
                SelectCurrentScene();

            if (_selectedIndex < 0 || _selectedIndex >= _sceneNames.Length)
                return _sceneNames[0];

            return _sceneNames[_selectedIndex];
        }

        private static void ShowSceneDropdownMenu(Rect dropDownRect)
        {
            RefreshSceneListIfNeeded();

            var menu = new GenericMenu();
            if (_scenePaths.Length == 0)
            {
#if !SCENE_SWITCHER_ADDRESSABLES
                if (Source == SceneSource.Addressables)
                {
                    menu.AddItem(new GUIContent(ADDRESSABLES_MISSING_HINT), false, OpenAddressablesInPackageManager);
                }
                else
#endif
                {
                    menu.AddDisabledItem(new GUIContent(GetEmptyListMessage()));
                }
                menu.DropDown(dropDownRect);
                return;
            }

            for (var index = 0; index < _scenePaths.Length; index++)
            {
                var itemIndex = index;
                var sceneName = _sceneNames[itemIndex];
                var isSelected = itemIndex == _selectedIndex;
                menu.AddItem(new GUIContent(sceneName), isSelected, () => LoadSceneAtIndex(itemIndex));
            }

            menu.DropDown(dropDownRect);
        }

        private static void RefreshSceneListIfNeeded(bool force = false)
        {
            if (!force)
            {
                if (!_sceneListDirty && EditorApplication.timeSinceStartup < _nextAllowedRefreshTime)
                    return;
            }

            _sceneListDirty = false;
            _nextAllowedRefreshTime = EditorApplication.timeSinceStartup + REFRESH_INTERVAL_SECONDS;

            BuildSceneList(out _sceneNames, out _scenePaths);
            SelectCurrentScene();
        }

        private static void BuildSceneList(out string[] sceneNames, out string[] scenePaths)
        {
            switch (Source)
            {
                case SceneSource.BuildSettings:
                    {
                        var scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).ToArray();
                        scenePaths = scenes.Select(scene => scene.path).ToArray();
                        sceneNames = scenePaths.Select(Path.GetFileNameWithoutExtension).ToArray();
                        return;
                    }
                case SceneSource.Addressables:
                    {
#if SCENE_SWITCHER_ADDRESSABLES
                        BuildAddressableSceneList(out sceneNames, out scenePaths);
#else
                        sceneNames = Array.Empty<string>();
                        scenePaths = Array.Empty<string>();
#endif
                        return;
                    }
                case SceneSource.AllScenes:
                default:
                    {
                        string path = CustomScenePath;
                        if (Directory.Exists(path))
                        {
                            var paths = Directory.GetFiles(path, "*.unity", SearchOption.AllDirectories);
                            scenePaths = paths;
                            sceneNames = paths.Select(Path.GetFileNameWithoutExtension).ToArray();
                        }
                        else
                        {
                            sceneNames = Array.Empty<string>();
                            scenePaths = Array.Empty<string>();
                        }
                        return;
                    }
            }
        }

#if SCENE_SWITCHER_ADDRESSABLES
        private static void BuildAddressableSceneList(out string[] sceneNames, out string[] scenePaths)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                sceneNames = Array.Empty<string>();
                scenePaths = Array.Empty<string>();
                return;
            }

            var paths = new List<string>();
            foreach (var group in settings.groups)
            {
                if (group == null) continue;
                foreach (var entry in group.entries)
                {
                    if (entry == null) continue;
                    if (entry.MainAssetType == typeof(SceneAsset))
                    {
                        paths.Add(entry.AssetPath);
                    }
                }
            }

            scenePaths = paths.ToArray();
            sceneNames = scenePaths.Select(Path.GetFileNameWithoutExtension).ToArray();
        }
#endif

        private static void SelectCurrentScene()
        {
            var currentScene = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().path);

            if (_sceneNames.Length == 0)
            {
                _selectedIndex = 0;
                _lastActiveScene = currentScene;
                return;
            }

            var index = Array.IndexOf(_sceneNames, currentScene);
            _selectedIndex = index == -1 ? 0 : index;
            _lastActiveScene = currentScene;
        }

        private static void UpdateSceneSelection()
        {
            var currentScene = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().path);
            if (currentScene == _lastActiveScene) return;
            _lastActiveScene = currentScene;
            SelectCurrentScene();
        }

        private static void LoadSceneAtIndex(int sceneIndex)
        {
            if (sceneIndex < 0 || sceneIndex >= _scenePaths.Length)
                return;

            var scenePath = _scenePaths[sceneIndex];

            if (!string.IsNullOrEmpty(scenePath))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    _selectedIndex = sceneIndex;
                    EditorSceneManager.OpenScene(scenePath);
#if UNITY_6000_3_OR_NEWER
                    RefreshToolbarUI();
#endif
                }
            }
            else
            {
                Debug.Log("<color=red>[SceneSwitcher] Scene path is empty!!!");
            }
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.ExitingPlayMode || state == PlayModeStateChange.EnteredEditMode)
            {
#if UNITY_6000_3_OR_NEWER
                RefreshToolbarUI();
#else
                EditorApplication.delayCall += EnsureToolbarUI;
#endif
            }
        }
    }
}