﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Fullscreen {

    /// <summary>
    /// Defines how the plugin should handle multiple <see cref="FullscreenView"/> opened instances.
    /// </summary>
    public enum MutipleWindow {
        /// <summary>
        /// Close the fullscreen view under the cursor, if there's none a new one is created and opened
        /// </summary>
        All = 0,
        /// <summary>
        /// Allow only one open instance of each type that inherits from <see cref="FullscreenView"/>
        /// </summary>
        OneOfEachType = 1,
        /// <summary>
        /// Allow only one <see cref="FullscreenView"/> to be opened that one time.
        /// </summary>
        OnlyOne = 2,
        /// <summary>
        /// Same as <see cref="OnlyOne"/>, but opens a newly created Fullscreen View immediately.
        /// </summary>
        OnlyOneImmediate = 3
    }

    /// <summary>
    /// Define a source to get a fullscreen rect.
    /// </summary>
    public enum RectSourceMode {
        /// <summary>
        /// The full rect of the main screen.
        /// </summary>
        PrimaryScreen = 0,
        /// <summary>
        /// The full rect of the screen where the mouse pointer is. (Windows only)
        /// </summary>
        AtMousePosition = 1,
        /// <summary>
        /// A rect that spans across all the screens. (Windows only)
        /// </summary>
        VirtualSpace = 2,
        /// <summary>
        /// A rect that covers all the screen except for the taskbar.
        /// </summary>
        WorkArea = 3,
        /// <summary>
        /// A custom rect defined by <see cref="FullscreenPreferences.CustomRect"/>.
        /// </summary>
        CustomRect = 4
    }

    /// <summary>
    /// Contains settings of the Fullscreen Editor plugin
    /// </summary>
    [InitializeOnLoad]
    public static class FullscreenPreferences {

        private const string PREF_ITEM_NAME = "Fullscreen";

#if UNITY_EDITOR_OSX
		private const string PREFERENCES_MENU_ITEM = "Unity/Preferences...";
#elif UNITY_2017_2_OR_NEWER
        private const string PREFERENCES_MENU_ITEM = "Edit/Preferences...";
#endif

        internal static readonly List<string> preferenceKeys = new List<string>();

        private static int ourPrefItemIndex;
        private static EditorWindow lastWindow;
        private static readonly Type preferenceWindowType;
        private static readonly GUIContent resetSettingsContent = new GUIContent("Use Defaults", "Reset all settings to default ones");
        private static readonly PrefItem<Vector2> scroll = new PrefItem<Vector2>("Scroll", Vector2.zero, string.Empty, string.Empty);

        /// <summary>
        /// Is the window toolbar currently visible?
        /// </summary>
        public static PrefItem<bool> ToolbarVisible { get; private set; }

        /// <summary>
        /// Is Fullscreen on Play currently enabled?
        /// </summary>
        public static PrefItem<bool> FullscreenOnPlayEnabled { get; private set; }

        /// <summary>
        /// Disable <see cref="FullscreenOnPlay"/> if there is any open fullscreen view.
        /// </summary>
        public static PrefItem<bool> FullscreenOnPlayGiveWay { get; private set; }

        /// <summary>
        /// Define a source to get a fullscreen rect.
        /// </summary>
        public static PrefItem<RectSourceMode> RectSource { get; private set; }

        /// <summary>
        /// Defines how the plugin should handle multiple <see cref="FullscreenView"/> opened instances.
        /// </summary>
        public static PrefItem<MutipleWindow> MutipleWindowMode { get; private set; }

        /// <summary>
        /// Custom rect to be used with <see cref="RectSourceMode.CustomRect"/>
        /// </summary>
        public static PrefItem<Rect> CustomRect { get; private set; }

        /// <summary>
        /// Quick fix if the GameView input doesn't work properly.
        /// </summary>
        public static PrefItem<bool> GameViewInputFix { get; private set; }

        /// <summary>
        /// An instance of the prefereces window if any is open.
        /// </summary>
        public static EditorWindow PreferenceWindow { get { return (EditorWindow)Resources.FindObjectsOfTypeAll(preferenceWindowType).FirstOrDefault(); } }

        private static bool WindowsEditor { get { return Application.platform == RuntimePlatform.WindowsEditor; } }
        private static bool OSXEditor { get { return Application.platform == RuntimePlatform.OSXEditor; } }

        private static bool LinuxEditor {
            get {
#if UNITY_5_5_OR_NEWER
                return Application.platform == RuntimePlatform.LinuxEditor;
#else
                return false;
#endif
            }
        }

        static FullscreenPreferences() {
            ReloadPreferences();
            preferenceWindowType = ReflectionUtility.FindClass("UnityEditor.PreferencesWindow");

            EditorApplication.update += () => {
                if(!lastWindow && PreferenceWindow)
                    FullscreenUtility.WaitFrames(2, () => SetCustomIconOnPreferences(PreferenceWindow));
            };
        }

        private static void ReloadPreferences() {
            preferenceKeys.Clear();

            var rectSourceTooltip = string.Empty;
            var multipleWindowTooltip = string.Empty;

            rectSourceTooltip += "Controls where Fullscreen Views opens.\n\n";
            rectSourceTooltip += "Primary Screen: Fullscreen opens on the main screen;\n\n";
            rectSourceTooltip += "At Mouse Position: Fullscreen opens on the screen where the mouse pointer is (Windows only);\n\n";
            rectSourceTooltip += "Work Area: Fullscreen opens on the screen where the mouse pointer is, but won't cover the taskbar;\n\n";
            rectSourceTooltip += "Virtual Space: Fullscreen spans across all screens (Windows only);\n\n";
            rectSourceTooltip += "Custom Rect: Fullscreen opens on the given custom Rect.";

            multipleWindowTooltip += "Controls how the plugin will handle the opening and closing of Fullscreen Views.\n";
            multipleWindowTooltip += "Only One and Only One Immediate works best for single screen setups.\n\n";
            multipleWindowTooltip += "All: Close the fullscreen view under the cursor, if there's none a new one is created and opened;\n\n";
            multipleWindowTooltip += "One of Each Type: Allow only one Fullscreen View of each type, the types are Game View, Scene View, Main View and Focused View;\n\n";
            multipleWindowTooltip += "Only One: Close any open Fullscreen View before opening a new one;\n\n";
            multipleWindowTooltip += "Only One Immediate: Same as \"Only One\", but opens the newly created Fullscreen View immediately.";

            ToolbarVisible = new PrefItem<bool>("Toolbar", false, "Toolbar Visible", "Show and hide the toolbar on the top of some windows, like the Game View and Scene View.");
            FullscreenOnPlayEnabled = new PrefItem<bool>("FullscreenOnPlay", false, "Fullscreen On Play", "Open a new instance of the Fullscreen View once the play mode is started, and close if it gets paused or stopped.");
            FullscreenOnPlayGiveWay = new PrefItem<bool>("FullscreenOnPlayGiveWay", true, "Give way", "Disable fullscreen on play feature if there is any other window as fullscreen");
            RectSource = new PrefItem<RectSourceMode>("RectSource", OSXEditor ? RectSourceMode.WorkArea : RectSourceMode.AtMousePosition, "Rect Source", rectSourceTooltip);
            MutipleWindowMode = new PrefItem<MutipleWindow>("MutipleFullscreen", MutipleWindow.OnlyOneImmediate, "Multiple Fullscreen", multipleWindowTooltip);
            CustomRect = new PrefItem<Rect>("CustomRect", FullscreenRects.GetVirtualScreenRect(), "Custom Rect", string.Empty);

            GameViewInputFix = new PrefItem<bool>("GameViewInputFix", InternalEditorUtility.GetUnityVersion() < new Version(2017, 1), "Game View Input Fix", "A fix for a bug with the inputs that may happen when " +
                "two game view overlaps, this fix closes all the game views while a fullscreen view is open, and restores them once the fullscreen is closed.\n" +
                "\"Multiple Fullscreen\" must be set to \"Only One\" for this to work properly.");

            if(FullscreenUtility.MenuItemHasShortcut(Shortcut.TOOLBAR_PATH))
                ToolbarVisible.Content.text += string.Format(" ({0})", FullscreenUtility.TextifyMenuItemShortcut(Shortcut.TOOLBAR_PATH));
            if(FullscreenUtility.MenuItemHasShortcut(Shortcut.FULLSCREEN_ON_PLAY_PATH))
                FullscreenOnPlayEnabled.Content.text += string.Format(" ({0})", FullscreenUtility.TextifyMenuItemShortcut(Shortcut.FULLSCREEN_ON_PLAY_PATH));
        }

        [PreferenceItem(PREF_ITEM_NAME)]
        private static void OnGUI() {

            if(OSXEditor)
                EditorGUILayout.HelpBox("Due to OS X system limitations, the plugin will only be fullscreen if the main window is maximized and the dock is set to auto-hide, and it'll still show a thin bar at the bottom.", MessageType.Warning);
            else if(LinuxEditor)
                EditorGUILayout.HelpBox("This plugin was not tested on Linux and its behaviour is unknown.", MessageType.Warning);

            EditorGUILayout.Separator();

            scroll.Value = EditorGUILayout.BeginScrollView(scroll);

            GameViewInputFix.DoGUI();
            EditorGUILayout.Separator();
            ToolbarVisible.DoGUI();
            FullscreenOnPlayEnabled.DoGUI();

            EditorGUI.indentLevel++;
            using(new EditorGUI.DisabledGroupScope(!FullscreenOnPlayEnabled))
                FullscreenOnPlayGiveWay.DoGUI();
            EditorGUI.indentLevel--;

            EditorGUILayout.Separator();
            MutipleWindowMode.DoGUI();

            if(WindowsEditor || LinuxEditor || !IsRectModeSupported(RectSource))
                RectSource.DoGUI();

            if(!IsRectModeSupported(RectSource))
                EditorGUILayout.HelpBox("The selected Rect Source mode is not supported on this platform", MessageType.Warning);

            switch(RectSource.Value) {
                case RectSourceMode.CustomRect:
                    EditorGUI.indentLevel++;
                    CustomRect.DoGUI();

                    var customRect = CustomRect.Value;

                    if(customRect.width < 300f)
                        customRect.width = 300f;
                    if(customRect.height < 300f)
                        customRect.height = 300f;

                    CustomRect.Value = customRect;

                    EditorGUI.indentLevel--;
                    break;
            }

            EditorGUILayout.Separator();
            Shortcut.DoShortcutsGUI();
            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox("Each item has a tooltip explaining its behaviour", MessageType.Info);
            EditorGUILayout.EndScrollView();

            using(new EditorGUILayout.HorizontalScope())
                if(GUILayout.Button(resetSettingsContent, GUILayout.Width(120f)))
                    DeleteSavedValues();

            EditorGUILayout.Separator();
        }

#if UNITY_2017_2_OR_NEWER || UNITY_EDITOR_OSX
        [MenuItem(Shortcut.SETTINGS_PATH, false, 1000)]
        private static void OpenSettings() {
            if(EditorApplication.ExecuteMenuItem(PREFERENCES_MENU_ITEM))
                FullscreenUtility.WaitFrames(1, () => {
                    SetCustomIconOnPreferences(PreferenceWindow);
                    PreferenceWindow.SetPropertyValue("selectedSectionIndex", ourPrefItemIndex);
                });
            else
                Debug.LogWarning("Failed to open settings, you can find them under \"" + PREFERENCES_MENU_ITEM + "\"");
        }
#endif

        private static void DeleteSavedValues() {
            foreach(var key in preferenceKeys)
                EditorPrefs.DeleteKey(key);

            ReloadPreferences();
        }

        private static void SetCustomIconOnPreferences(EditorWindow instance) {
            try {
                if(instance == lastWindow || !instance)
                    return;

                lastWindow = instance;
                ourPrefItemIndex = 0;

                var sections = instance.GetFieldValue<IList>("m_Sections");

                foreach(var section in sections) {
                    var ourPrefContent = section.GetFieldValue<GUIContent>("content");

                    if(ourPrefContent.text == PREF_ITEM_NAME) {
                        ourPrefContent.image = EditorGUIUtility.isProSkin ? FullscreenUtility.FullscreenIconSmallDarkSkin : FullscreenUtility.FullscreenIconSmallWhiteSkin;
                        break;
                    }

                    ourPrefItemIndex++;
                }

                instance.Repaint();
            }
            catch(Exception e) {
                Debug.LogException(e);
                Debug.LogError("Failed to set custom icon on preference window");
            }
        }

        internal static bool IsRectModeSupported(RectSourceMode mode) {
            switch(mode) {
                case RectSourceMode.VirtualSpace:
                case RectSourceMode.AtMousePosition:
                    return WindowsEditor;

                case RectSourceMode.CustomRect:
                case RectSourceMode.PrimaryScreen:
                case RectSourceMode.WorkArea:
                    return WindowsEditor || OSXEditor || LinuxEditor;

                default:
                    return false;
            }
        }

    }

}