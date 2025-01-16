using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[EventHandlerInfo
    ("Saving", 
    "Save Point Loaded",
    "SavePointLoaded is called when a save point is loaded. Use the 'new_game' key to handle game start.")]
public class SavePointLoaded : EventHandler
{
    [Tooltip("Node will execute if the save key of loaded save point matches THIS save key")]
    [SerializeField]
    protected List<string> savePointKeys = new List<string>();

    protected void OnSavePointLoaded(string _savePointKey)
    {
        for(int i = 0; i < savePointKeys.Count; i++)
        {
            var key = savePointKeys[i];
            if (string.Compare(key, _savePointKey, true) == 0)
            {
                ExecuteNode();
                return;
            }
        }
    }

    public static void NotifyEventHandlers(string _savePointKey)
    {
        // Fire any matching SavePointLoaded event handler with matching save key
        var eventHandlers = Object.FindObjectsOfType<SavePointLoaded>();
        for (int i = 0; i < eventHandlers.Length; i++)
        {
            var eventHandler = eventHandlers[i];
            eventHandler.OnSavePointLoaded(_savePointKey);
        }
    }
}