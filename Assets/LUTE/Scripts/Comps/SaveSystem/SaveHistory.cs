using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// The Save History is a list of previously created Save Points, sorted chronologically.
[System.Serializable]
public class SaveHistory
{
    protected const int SaveDataVersion = 1;

    [SerializeField] protected int version = SaveDataVersion;
    [SerializeField] protected List<string> savePoints = new List<string>();
    [SerializeField] protected List<string> rewoundSavePoints = new List<string>();

    public int TotalSavePoints { get { return savePoints.Count; } }

    public int TotalRewoundSavePoints { get { return rewoundSavePoints.Count; } }

    public void AddSavePoint(string savePointKey, string savePointDesc, bool settingsOnly)
    {
        rewoundSavePoints.Clear();

        string sceneName = SceneManager.GetActiveScene().name;
        var savePointData = SavePointData.Encode(savePointKey, savePointDesc, sceneName, settingsOnly);

        savePoints.Add(savePointData);
    }

    /// Rewinds to the previous Save Point in the Save History.
    /// The latest Save Point is moved to a separate list of rewound save points.
    public void Rewind()
    {
        if (savePoints.Count > 0)
        {
            rewoundSavePoints.Add(savePoints[savePoints.Count - 1]);
            savePoints.RemoveAt(savePoints.Count - 1);
        }
    }

    /// Fast forwards to the next Save Point in the Save History.
    /// The most recently rewound Save Point is moved back to the main list of save points.
    public void FastForward()
    {
        if (rewoundSavePoints.Count > 0)
        {
            savePoints.Add(rewoundSavePoints[rewoundSavePoints.Count - 1]);
            rewoundSavePoints.RemoveAt(rewoundSavePoints.Count - 1);
        }
    }

    public void LoadLastSavePoint()
    {
        if (savePoints.Count > 0)
        {
            var savePointData = savePoints[savePoints.Count - 1];
            SavePointData.Decode(savePointData);
        }
    }

    public void LoadSavePoint(string savePointKey)
    {
        if (savePoints.Count > 0)
        {
            var savePointData = savePoints.Find(x => x.Contains(savePointKey));
            if (savePointData == null)
            {
                Debug.LogError("Save point with key " + savePointKey + " not found.");
                return;
            }
            SavePointData.Decode(savePointData);
        }
    }

    public void Clear()
    {
        savePoints.Clear();
        rewoundSavePoints.Clear();
    }
    public void ClearRewoundSavePoints()
    {
        rewoundSavePoints.Clear();
    }

    public virtual string GetDebugInfo()
    {
        string debugInfo = "Save points:\n";

        foreach (var savePoint in savePoints)
        {
            debugInfo += savePoint.Substring(0, savePoint.IndexOf(',')).Replace("\n", "").Replace("{", "").Replace("}", "") + "\n";
        }

        debugInfo += "Rewound points:\n";

        foreach (var savePoint in rewoundSavePoints)
        {
            debugInfo += savePoint.Substring(0, savePoint.IndexOf(',')).Replace("\n", "").Replace("{", "").Replace("}", "") + "\n";
        }

        return debugInfo;
    }
}