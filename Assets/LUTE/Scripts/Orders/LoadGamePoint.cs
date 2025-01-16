using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SavePoint;

[OrderInfo(
    "Saving",
    "Load Game",
    "Loads game from either default or custom save file and will load to latest save point or to specific save point with custom key.")]
public class LoadGamePoint : Order
{
    [Tooltip("The key to load the game from - if none is provided then we use default")]
    [SerializeField] protected string saveKey = LogaConstants.DefaultSaveDataKey;
    [Tooltip("If true, load the game from this specific point when using the save data file provided")]
    [SerializeField] protected bool loadCustomPoint = false;
    [Tooltip("If loading from specific point, provide the key that you wish to load; this should match the save point ID exactly.")]
    [SerializeField] protected string customKey = string.Empty;
    [Tooltip("If true, return to the previous node if we do not have a save to load. Useful for when using menus that have a load function without a current saved game.")]
    [SerializeField] protected bool returnToPreviousNode;

    public override void OnEnter()
    {
        var saveManager = LogaManager.Instance.SaveManager;

        if (loadCustomPoint && !string.IsNullOrEmpty(customKey))
        {
            if (string.IsNullOrEmpty(saveManager.StartScene))
            {
                saveManager.StartScene = SceneManager.GetActiveScene().name;
            }
            if (saveManager.HasSaveData(saveKey))
            {
                saveManager.Load(saveKey, true, customKey);
            }
            else if (returnToPreviousNode)
            {
                ReturnToPriorNode();
            }
        }
        else
        {
            if (string.IsNullOrEmpty(saveManager.StartScene))
            {
                saveManager.StartScene = SceneManager.GetActiveScene().name;
            }
            if (saveManager.HasSaveData(saveKey))
            {
                saveManager.Load(saveKey);
            }
            else if(returnToPreviousNode)
            {
                ReturnToPriorNode();
            }
        }
    }

    protected void ReturnToPriorNode()
    {
        //If we have been called by another node (such as when using a menu) and we do not have a save to load then we should return to this previous node
        //Otherwise we will be left with a blank node and no way to progress
        var engine = GetEngine();
        var nodes = engine.GetComponents<Node>();
        for (int i = 0; i < nodes.Length; i++)
        {
            var node = nodes[i];
            var orders = node.OrderList;
            foreach (var order in orders)
            {
                var nodesWithConnections = new List<Node>();
                order.GetConnectedNodes(ref nodesWithConnections);
                foreach (var connectedNode in nodesWithConnections)
                {
                    if (connectedNode == this.ParentNode)
                    {
                        engine.ExecuteNode(node);
                        Continue();
                        return;
                    }

                }
            }
        }
    }

    public override string GetSummary()
    {
        string summary = "Load game from ";
        if (saveKey != LogaConstants.DefaultSaveDataKey)
        {
            summary  += saveKey;
        }
        else
        {
            summary += "default save data";
        }
        if (loadCustomPoint)
        {
            summary += "custom point: " + customKey;
        }
        return summary;

    }

    public override Color GetButtonColour()
    {
        return new Color32(235, 191, 217, 255);
    }
}
