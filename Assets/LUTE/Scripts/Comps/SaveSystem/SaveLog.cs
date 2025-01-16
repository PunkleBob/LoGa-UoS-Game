using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveLogEntry
{
    public string name;
    public string text;
}

[System.Serializable]
public class SaveLogData
{
    public  List<SaveLogEntry> entries = new List<SaveLogEntry>();
}

/// Controls the history of the save log and is used to print to UI
public class SaveLog : MonoBehaviour
{
    //Used when a line is sent to the log.
    public static event LogAddedHandler OnLogAdded;
    public delegate void LogAddedHandler(SaveLogEntry entry);

    public static void DoLogAdded(SaveLogEntry entry)
    {
        OnLogAdded?.Invoke(entry);
    }

    public static System.Action OnLogClear;
    public static void DoLogCleared()
    {
        if(OnLogClear != null)
        {
            OnLogClear();
        }
    }

    private SaveLogData logData;

    protected virtual void Awake()
    {
        logData = new SaveLogData();
        DoLogCleared();
    }
    //protected virtual void OnEnable()
    //{
    //    WriterSignals.OnWriterState += OnWriterState;
    //}

    //protected virtual void OnDisable()
    //{
    //    WriterSignals.OnWriterState -= OnWriterState;
    //}

    //protected virtual void OnWriterState(Writer writer, WriterState writerState)
    //{
    //    if (writerState == WriterState.End)
    //    {
    //        var sd = SayDialog.GetSayDialog();

    //        if (sd != null)
    //        {
    //            NarrativeLogEntry entry = new NarrativeLogEntry()
    //            {
    //                name = sd.NameText,
    //                text = sd.StoryText
    //            };
    //            AddLine(entry);
    //        }
    //    }
    //}

    public void AddLine(SaveLogEntry entry)
    {
        logData.entries.Add(entry);
        DoLogAdded(entry);
    }

    public void Clear()
    {
        logData.entries.Clear();
        DoLogCleared();
    }

    public string GetJsonHistory()
    {
        string jsonText = JsonUtility.ToJson(logData, true);
        return jsonText;
    }

    public string GetPrettyHistory(bool previousOnly = false)
    {
        string output = "\n ";
        int count;

        count = previousOnly ? logData.entries.Count - 1 : logData.entries.Count;

        for (int i = 0; i < count; i++)
        {
            output += "<b>" + logData.entries[i].name + "</b>\n";
            output += logData.entries[i].text + "\n\n";
        }
        return output;
    }

    public void LoadLogData(string _logData)
    {
        if(logData == null)
        {
            Debug.LogError("Failed to decode history save data as there is no log entry!");
            return;
        }
        logData = JsonUtility.FromJson<SaveLogData>(_logData);

        DoLogCleared();
    }
}
