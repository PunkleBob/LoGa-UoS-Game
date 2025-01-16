using System;
using UnityEngine;

public class EventHandlerInfoAttribute : Attribute
{
    public EventHandlerInfoAttribute(string category, string eventHandlerName, string helpText)
    {
        this.Category = category;
        this.EventHandlerName = eventHandlerName;
        this.HelpText = helpText;
    }

    public string Category { get; set; }
    public string EventHandlerName { get; set; }
    public string HelpText { get; set; }
}

/// <summary>
/// A node may have a relative event handler which starts executing the orders when a specific event occurs
/// you can create your own event handler by inheriting from this class and call the ExecuteNode method
/// add event handler info attribute to your class to make it appear in the editor dropdown menu
/// </summary>

[RequireComponent(typeof(Node))]
[RequireComponent(typeof(BasicFlowEngine))]
[AddComponentMenu("")]
public class EventHandler : MonoBehaviour
{
    [HideInInspector]
    [SerializeField] protected Node parentNode;

    public virtual Node ParentNode
    {
        get { return parentNode; }
        set { parentNode = value; }
    }

    public virtual bool ExecuteNode()
    {
        if (ParentNode == null)
        {
            Debug.LogError("Parent node is null");
            return false;
        }

        if (parentNode._EventHandler != this)
        {
            Debug.LogError("Parent node's event handler is not this");
            return false;
        }

        var engine = parentNode.GetEngine();

        if (engine == null || !engine.isActiveAndEnabled)
        {
            return false;
        }

        return engine.ExecuteNode(parentNode);
    }

    public virtual string GetSummary()
    {
        return "";
    }
}
