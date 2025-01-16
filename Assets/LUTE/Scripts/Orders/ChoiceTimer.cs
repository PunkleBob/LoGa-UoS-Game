using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[OrderInfo("Narrative",
             "Choice Timer",
             "Displays a timer bar and executes a target node if the player fails to select a menu option in time")]
[AddComponentMenu("")]
[ExecuteInEditMode]
public class ChoiceTimer : Order, INodeCaller
{
    [Tooltip("Length of time to display the timer for")]
    [SerializeField] protected float _duration = 1;
    [Tooltip("Node to execute when the timer expires")]
    [SerializeField] protected Node targetNode;
    [Tooltip("Choose a random node from the list to execute when the timer expires")]
    [SerializeField] protected bool randomTarget = false;
    [Tooltip("List of nodes to choose from when the timer expires if randomTarget is true")]
    [SerializeField] protected Node[] randomTargetNodes;

    public override void OnEnter()
    {
        var menu = MenuDialogue.GetMenuDialogue();

        if (menu != null &&
            targetNode != null)
        {
            if (randomTarget &&
                randomTargetNodes.Length > 0 && randomTargetNodes[0] != null)
            {
                targetNode = randomTargetNodes[Random.Range(0, randomTargetNodes.Length)];
            }
            menu.ShowTimer(_duration, targetNode);
        }

        Continue();
    }

    public override void GetConnectedNodes(ref List<Node> connectedNodes)
    {
        if (randomTarget && randomTargetNodes.Length > 0)
        {
            foreach (Node node in randomTargetNodes)
            {
                if (node != null)
                {
                    connectedNodes.Add(node);
                }
            }
        }
        else if (targetNode != null)
        {
            connectedNodes.Add(targetNode);
        }
    }

    public override string GetSummary()
    {
        if (targetNode == null)
        {
            return "Error: No target node selected";
        }

        if (randomTarget)
        {
            List<string> randomNames = GetRandomTargetNames();
            return "Random Targets: " + string.Join(", ", randomNames);
        }

        return targetNode._NodeName;
    }

    public List<string> GetRandomTargetNames()
    {
        List<string> randomTargetNames = new List<string>();

        if (randomTarget && randomTargetNodes.Length > 0)
        {
            foreach (Node node in randomTargetNodes)
            {
                if (node != null)
                {
                    randomTargetNames.Add(node._NodeName);
                }
            }
        }

        return randomTargetNames;
    }

    public bool MayCallNode(Node node)
    {
        return node == targetNode;
    }
}