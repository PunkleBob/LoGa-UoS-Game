using UnityEngine;

public class EnumFlagAttribute : PropertyAttribute
{
    public string enumName;
    public EnumFlagAttribute() { }
    public EnumFlagAttribute(string name)
    {
        enumName = name;
    }
}

[EventHandlerInfo("Default",
                  "Update",
                  "Executes every chosen update method (Fixed, Late, Normal)")]
[AddComponentMenu("")]
public class UpdateEventHandler : EventHandler
{
    [System.Flags]
    public enum UpdateMessageFlags
    {
        Update = 1 << 0,
        FixedUpdate = 1 << 1,
        LateUpdate = 1 << 2,
    }

    [Tooltip("Which of the Update messages to trigger on.")]
    [SerializeField]
    [EnumFlag]
    protected UpdateMessageFlags FireOn = UpdateMessageFlags.Update;

    private void Update()
    {
        if ((FireOn & UpdateMessageFlags.Update) != 0)
        {
            ExecuteNode();
        }
    }

    private void FixedUpdate()
    {
        if ((FireOn & UpdateMessageFlags.FixedUpdate) != 0)
        {
            ExecuteNode();
        }
    }

    private void LateUpdate()
    {
        if ((FireOn & UpdateMessageFlags.LateUpdate) != 0)
        {
            ExecuteNode();
        }
    }
}