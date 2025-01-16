using System;
using System.Collections.Generic;
using UnityEngine;

/// Supported modes for calling a node.
public enum CallMode
{
    /// <summary> Stop executing the current block after calling. </summary>
    Stop,
    /// <summary> Continue executing the current block after calling  </summary>
    Continue,
    /// <summary> Wait until the called block finishes executing, then continue executing current block. </summary>
    WaitUntilFinished,
    /// <summary> Stop executing the current block before attempting to call. This allows for circular calls within the same frame </summary>
    StopThenCall
}

/// Execute another node in the same or different engine as the order
[OrderInfo("Flow",
             "Next Node",
             "Execute another node in the same or different engine as the order")]
[AddComponentMenu("")]
public class NextNode : Order
{
    [Tooltip("Engine which contains the node to execute. If none is specified then the current engine is used")]
    [SerializeField] protected BasicFlowEngine targetEngine;

    [Tooltip("Node to start executing")]
    [SerializeField] public Node targetNode;

    [Tooltip("Order index to start executing")]
    [SerializeField] protected int startIndex;

    [Tooltip("Select if the calling node should stop or continue executing orders, or wait until the called node finishes.")]
    [SerializeField] protected CallMode callMode;

    public override void OnEnter()
    {
        if (targetNode != null)
        {
            //are we calling our own parent node?
            if (ParentNode != null && ParentNode.Equals(targetNode))
            {
                //if so, just execute the first order and ignore the call
                Continue(0);
                return;
            }

            if (targetNode.IsExecuting())
            {
                Debug.LogWarning(targetNode._NodeName + " cannot be called/executed, it is already running.");
                Continue();
                return;
            }

            Action onComplete = null;
            if (callMode == CallMode.WaitUntilFinished)
            {
                onComplete = delegate ()
                {
                    Continue();
                };
            }

            int index = startIndex;

            if (targetEngine == null || targetEngine.Equals(GetEngine()))
            {
                if (callMode == CallMode.StopThenCall)
                {
                    StopParentNode();
                }
                StartCoroutine(targetNode.Execute(index, onComplete));
            }
            else
            {
                if (callMode == CallMode.StopThenCall)
                {
                    StopParentNode();
                }
                // Execute block in another Engine
                targetEngine.ExecuteNode(targetNode, index, onComplete);
            }
        }
        if (callMode == CallMode.Stop)
        {
            StopParentNode();
        }
        else if (callMode == CallMode.Continue)
        {
            Continue();
        }
    }

    public override void GetConnectedNodes(ref List<Node> connectedNodes)
    {
        if (targetNode != null)
        {
            connectedNodes.Add(targetNode);
        }
    }

    public override string GetSummary()
    {
        string summary = "";

        if (targetNode == null)
        {
            summary = "<None>";
        }
        else
        {
            summary = targetNode._NodeName;
        }

        summary += " : " + callMode.ToString();

        return summary;
    }

    public override Color GetButtonColour()
    {
        return new Color32(58, 185, 97, 255);
    }
}
