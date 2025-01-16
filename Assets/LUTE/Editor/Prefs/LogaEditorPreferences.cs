using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class LogaEditorPreferences
{
    private static bool prefsLoaded = false;
    private const string HIDE_ICON_KEY = "hideIconInHierarchy";
    private const string USE_LOGS_KEY = "useLogs";

    public static bool hideIconInHierarchy;
    public static bool useLogs;

    static LogaEditorPreferences()
    {
        LoadOnScript();
    }

    [SettingsProvider]
    public static SettingsProvider CreateSettingsProvider()
    {
        // First parameter is the path in the Settings window.
        // Second parameter is the scope of this setting: it only appears in the Project Settings window.
        var provider = new SettingsProvider("Project/LoGa", SettingsScope.Project)
        {
            // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
            guiHandler = (searchContext) => PreferencesGUI()

            // // Populate the search keywords to enable smart search filtering and label highlighting:
            // keywords = new HashSet<string>(new[] { "Number", "Some String" })
        };

        return provider;
    }

    private static void PreferencesGUI()
    {
        // Load the preferences
        if (!prefsLoaded)
        {
            LoadOnScript();
        }

        hideIconInHierarchy = EditorGUILayout.Toggle("Hide Engine Icon in Hierarchy", hideIconInHierarchy);

        EditorGUILayout.Space();

        useLogs = EditorGUILayout.Toggle("Use Logs", useLogs);
        LogaConstants.UseLogs = useLogs;

        EditorGUILayout.Space();

        if (LogaEditorResources.Add == null)
            EditorGUILayout.HelpBox("Resources need to be regenerated!", MessageType.Error);


        if (GUILayout.Button(new GUIContent("Select Editor Resources SO", "If icons are not showing correctly you may need to reassign the references in the LogaEditorResources. Button below will locate it.")))
        {
            var ids = AssetDatabase.FindAssets("t:LogaEditorResources");
            if (ids.Length > 0)
            {
                var p = AssetDatabase.GUIDToAssetPath(ids[0]);
                var asset = AssetDatabase.LoadAssetAtPath<LogaEditorResources>(p);
                Selection.activeObject = asset;
            }
            else
            {
                Debug.LogError("No LogaEditorResources found!");
            }
        }

        // Save the preferences
        if (GUI.changed)
        {
            EditorPrefs.SetBool(HIDE_ICON_KEY, hideIconInHierarchy);
            EditorPrefs.SetBool(USE_LOGS_KEY, useLogs);
        }
    }

    public static void LoadOnScript()
    {
        hideIconInHierarchy = EditorPrefs.GetBool(HIDE_ICON_KEY, false);
        useLogs = EditorPrefs.GetBool(USE_LOGS_KEY, false);
        LogaConstants.UseLogs = useLogs;
        prefsLoaded = true;
    }
}
