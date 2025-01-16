using UnityEngine;

/// Container for saved data items.
/// Data and the type are stored as strings which are typically strings in JSON representing save objects.
[System.Serializable]
public class SaveDataItem
{
    [SerializeField] protected string data = "";
    [SerializeField] protected string type = "";

    public virtual string Data { get { return data; } }
    public virtual string Type { get { return type; } }

    /// Creates a new SaveDataItem with the given data and type.
    public static SaveDataItem Create(string _type, string _data)
    {
        var item = new SaveDataItem();
        item.type = _type;
        item.data = _data;

        return item;
    }
}