using UnityEngine;

#if UNITY_5_3_OR_NEWER

[OrderInfo(
    "Saving",
    "Save Point",
    "Creates a Save Point and adds it to the Save History. The player can save the Save History to persistent storage and load it again later using the Save Menu.")]
public class SavePoint : Order
{
    public enum KeyMode
    {
        /// Use the parent Node's name as the Save Point Key. N.B. If you change the Node name later it will break the save file!
        NodeName,
        /// Use a custom string for the key. This allows you to use multiple Save Points in the same Node and save files will still work if the name is changed later.
        Custom,
        /// Use both the parent Node's name as well as a custom string for the Save Point key.
        NodeNameAndCustom
    }

    public enum DescriptionMode
    {
        /// Current data and time as save point data.
        Timestamp,
        /// Custom string for saving point data.
        Custom
    }

    [Tooltip("Mark this save point as the starting point - there should only be ONE of these PER scene")]
    [SerializeField] protected bool isStartingPoint = false;
    [Tooltip("Saving Point save mode definition")]
    [SerializeField] protected KeyMode keyMode = KeyMode.NodeName;
    [Tooltip("Custom key for the Save Point")]
    [SerializeField] protected string customKey = string.Empty;
    [Tooltip("What string type to seperate the node name and custom key when using keymode.both")]
    [SerializeField] protected string keySeperator = "_";
    [Tooltip("Description mode definition")]
    [SerializeField] protected DescriptionMode descriptionMode = DescriptionMode.Timestamp;
    [Tooltip("Custom description for the Save Point")]
    [SerializeField] protected string customDescription = string.Empty;
    [Tooltip("Fire a save point loaded event when this order executes")]
    [SerializeField] protected bool fireEvent = true;
    [Tooltip("Resume the game from this save point when the save is loaded")]
    [SerializeField] protected bool resumeOnLoad = true;
    [SerializeField] protected bool autoSave = true;

    /// Mark this save point as the starting point - there should only be ONE of these PER scene
    public bool IsStartingPoint { get { return isStartingPoint; } }

    public string SavePointKey
    {
        get
        {
            if (keyMode == KeyMode.NodeName)
            {
                return ParentNode._NodeName;
            }
            else if (keyMode == KeyMode.NodeNameAndCustom)
            {
                return ParentNode._NodeName + keySeperator + customKey;
            }
            else
            {
                return customKey;
            }
        }
    }

    public string SavePointDescription
    {
        get
        {
            if (descriptionMode == DescriptionMode.Timestamp)
            {
                return System.DateTime.UtcNow.ToString("HH:mm dd MMMM, yyyy");
            }
            else
            {
                return customDescription;
            }
        }
    }
    public bool ResumeOnLoad { get { return resumeOnLoad; } }

    public override void OnEnter()
    {
        if (autoSave)
        {
            var saveManager = LogaManager.Instance.SaveManager;

            saveManager.AddSavePoint(SavePointKey, SavePointDescription, false);

            if (fireEvent)
            {
                SavePointLoaded.NotifyEventHandlers(SavePointKey);
            }
        }

        Continue();
    }

    public override string GetSummary()
    {
        if (keyMode == KeyMode.NodeName)
        {
            return "key: " + ParentNode._NodeName;
        }
        else if (keyMode == KeyMode.NodeNameAndCustom)
        {
            return "key: " + ParentNode._NodeName + keySeperator + customKey;
        }
        return "key: " + customKey;
    }

    public override Color GetButtonColour()
    {
        return new Color32(235, 191, 217, 255);
    }

    public override bool IsPropertyVisible(string propertyName)
    {
        if (propertyName == "customKey" && (keyMode != KeyMode.Custom && keyMode != KeyMode.NodeNameAndCustom))
        {
            return false;
        }
        if (propertyName == "keySeperator" && keyMode != KeyMode.NodeNameAndCustom)
        {
            return false;
        }
        if (propertyName == "customDescription" && descriptionMode != DescriptionMode.Custom)
        {
            return false;
        }
        return true;
    }
}
#endif // UNITY_5_3_OR_NEWER