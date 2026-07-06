using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThanhDV.Utilities
{
    [InitializeOnLoad]
    internal static class PlayFromFirstSceneButton
    {
        private const string BUTTON_NAME = "SceneSwitcher_PlayFromFirstSceneButton";
        private const string PLAY_MODE_ZONE_NAME = "ToolbarZonePlayMode";
#if UNITY_6000_3_OR_NEWER
        private const string PLAY_MODE_BUTTONS_NAME = "PlayMode";
#endif
        private static readonly Color PlayTint = new Color(1f, 0.2f, 0.2f, 1f);
        private static readonly Color PlayingBackground = new Color(0x20 / 255f, 0x43 / 255f, 0x63 / 255f, 1f);

        private static Texture2D s_playIcon;
        private static Texture2D s_stopIcon;

        private const string PLAY_ICON_NAME = "PlayButton";
        private const string STOP_ICON_NAME = "PreMatQuad";

        private static EditorToolbarButton s_button;

        static PlayFromFirstSceneButton()
        {
            EditorApplication.update += Tick;
            EditorApplication.playModeStateChanged += _ => UpdateVisual();
        }

        private static void Tick()
        {
            if (s_button != null && s_button.panel != null) return;
            Inject();
        }

        private static void Inject()
        {
            var host = FindHost();
            if (host == null) return;

            var existing = host.Q<EditorToolbarButton>(BUTTON_NAME);
            if (existing != null) { s_button = existing; UpdateVisual(); return; }

            var button = new EditorToolbarButton(OnClicked)
            {
                name = BUTTON_NAME,
                icon = GetPlayIcon()
            };
            button.style.marginRight = 2;

            host.Insert(0, button);
            s_button = button;
            UpdateVisual();
        }

        private static void UpdateVisual()
        {
            if (s_button == null) return;

            var isPlaying = EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode;
            s_button.tooltip = isPlaying ? "Stop" : "Play From First Scene In Build";

            if (isPlaying)
                s_button.style.backgroundColor = PlayingBackground;
            else
                s_button.style.backgroundColor = StyleKeyword.Null;

            s_button.icon = isPlaying ? GetStopIcon() : GetPlayIcon();

            var iconImage = s_button.Q<Image>();
            if (iconImage != null) iconImage.tintColor = PlayTint;
        }

        private static void OnClicked()
        {
            if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.isPlaying = false;
                return;
            }

            PlayFromFirstScene();
        }

        private static VisualElement FindHost()
        {
#if UNITY_6000_3_OR_NEWER
            var winProp = typeof(MainToolbar).GetProperty("window", BindingFlags.NonPublic | BindingFlags.Static);
            var win = winProp?.GetValue(null) as EditorWindow;
            if (win == null || win.rootVisualElement?.panel == null) return null;

            var playMode = win.rootVisualElement.Q(PLAY_MODE_BUTTONS_NAME);
            var overlayContent = playMode?.Q("overlay-content");
            if (overlayContent == null) return null;

            foreach (var c in overlayContent.Children())
                if (c.GetType().Name == "OverlayToolbar") return c;
            return null;
#else
            var toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
            if (toolbarType == null) return null;

            var toolbars = Resources.FindObjectsOfTypeAll(toolbarType);
            if (toolbars.Length == 0) return null;

            var rootField = toolbarType.GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
            var root = rootField?.GetValue(toolbars[0]) as VisualElement;
            if (root?.panel == null) return null;

            return root.Q(PLAY_MODE_ZONE_NAME);
#endif
        }

        private static Texture2D GetPlayIcon()
        {
            if (s_playIcon == null)
                s_playIcon = EditorGUIUtility.IconContent(PLAY_ICON_NAME).image as Texture2D;
            return s_playIcon;
        }

        private static Texture2D GetStopIcon()
        {
            if (s_stopIcon == null)
                s_stopIcon = EditorGUIUtility.IconContent(STOP_ICON_NAME).image as Texture2D;
            return s_stopIcon;
        }

        private static void PlayFromFirstScene()
        {
            var scenes = EditorBuildSettings.scenes;
            if (scenes == null || scenes.Length == 0)
            {
                Debug.LogWarning("[SceneSwitcher] No scenes in Build Settings.");
                return;
            }

            var firstScene = scenes[0];
            if (string.IsNullOrEmpty(firstScene.path))
            {
                Debug.LogWarning("[SceneSwitcher] First scene path is empty.");
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            EditorSceneManager.OpenScene(firstScene.path);
            EditorApplication.isPlaying = true;
        }
    }
}
