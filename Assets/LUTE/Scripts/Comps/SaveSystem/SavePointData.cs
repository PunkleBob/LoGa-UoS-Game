using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// Serializable container for a Save Point's data. 
/// All data is stored as strings, and the only concrete game class it depends on is the SaveData component.
[System.Serializable]
public class SavePointData
{
    [SerializeField] protected string savePointKey;
    [SerializeField] protected string savePointDesc;
    [SerializeField] protected string sceneName;
    [SerializeField] protected List<SaveDataItem> saveDataItems = new List<SaveDataItem>();

    protected static SavePointData Create(string _savePointKey, string _savePointDesc, string _sceneName)
    {
        var savePointData = new SavePointData();

        savePointData.savePointKey = _savePointKey;
        savePointData.savePointDesc = _savePointDesc;
        savePointData.sceneName = _sceneName;

        return savePointData;
    }

    public string SavePointKey { get { return savePointKey; } set { savePointKey = value; } }
    public string SavePointDesc { get { return savePointDesc; } set { savePointDesc = value; } }
    public string SceneName { get { return sceneName; } set { sceneName = value; } }

    public List<SaveDataItem> SaveDataItems { get { return saveDataItems; } }

    /// Encodes a new Save Point to data and converts it to JSON text format.
    public static string Encode(string _savePointKey, string _savePointDesc, string _sceneName, bool settingsOnly)
    {
        var savePointData = Create(_savePointKey, _savePointDesc, _sceneName);
        var saveData = GameObject.FindObjectOfType<SaveData>();
        if (saveData != null)
        {
            saveData.Encode(savePointData.saveDataItems, settingsOnly);
        }
        return JsonUtility.ToJson(savePointData, true);
    }

    /// Decodes a Save Point from JSON text format and loads it.
    public static void Decode(string saveDataJSON)
    {
        var savePointData = JsonUtility.FromJson<SavePointData>(saveDataJSON);

        UnityAction<Scene, LoadSceneMode> onSceneLoadedAction = null;

        onSceneLoadedAction = (scene, mode) =>
        {
            // Additive scene loads and non-matching scene loads could happen if the client is using the SceneManager directly
            // Directly ignoring them is the best practice
            if (mode == LoadSceneMode.Additive || scene.name != savePointData.SceneName)
            {
                return;
            }

            SceneManager.sceneLoaded -= onSceneLoadedAction;

            // Look for the SaveData component in the scene and decode the Save Point data
            var saveData = GameObject.FindObjectOfType<SaveData>();
            if (saveData != null)
            {
                saveData.Decode(savePointData.saveDataItems);
            }

            SaveManagerSignals.DoSavePointLoaded(savePointData.savePointKey);
        };

        SceneManager.sceneLoaded += onSceneLoadedAction;
        SceneManager.LoadScene(savePointData.SceneName);
    }
}
