// Use this to be notified of save events
using System.Data;

public static class SaveManagerSignals 
{
    //Sent just after a save point if loaded
    public static event SavePointLoadedHandler OnSavePointLoaded;
    public delegate void SavePointLoadedHandler(string savePointKey);

    public static void DoSavePointLoaded(string savePointKey)
    {
        OnSavePointLoaded?.Invoke(savePointKey);
    }

    // Sent just after a save point is added
    public static event SavePointAddedHandler OnSavePointAdded;
    public delegate void SavePointAddedHandler(string savePointKey, string savePointDescription);
    public static void DoSavePointAdded(string savePointKey, string savePointDescription)
    {
        OnSavePointAdded?.Invoke(savePointKey, savePointDescription);
    }

    //Sent when save hitory is reset
    public static event SaveHistoryResetHandler OnSaveHistoryReset;
    public delegate void SaveHistoryResetHandler();
    public static void DoSaveHistoryReset()
    {
        OnSaveHistoryReset?.Invoke();
    }
}
