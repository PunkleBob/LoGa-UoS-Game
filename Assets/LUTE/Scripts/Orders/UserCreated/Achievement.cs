using MoreMountains.Feedbacks;
using System;
using UnityEngine;

[OrderInfo("Adventure",
              "Achievement",
              "Update an achievement or quest status. Requires achievement rules child object on the engine.")]
[AddComponentMenu("")]
public class Achievement : Order
{
    [Tooltip("The ID of the achievement or quest")]
    [SerializeField] protected string achievementID;
    [Tooltip("If true, will add progress to the achievement, if false, will unlock it")]
    [SerializeField] protected bool progress;
    [Tooltip("The amount of progress to add")]
    [SerializeField] protected int amount = 1;
    [Tooltip("Feedback to play when the achievement is updated")]
    [SerializeField] protected MMFeedbacks achievementFeedback;
    [Tooltip("Engine which contains the node to execute upon achievement complete. If none is specified then the current engine is used")]
    [SerializeField] protected BasicFlowEngine targetEngine;
    [Tooltip("The node to trigger after the achievement is complete")]
    [SerializeField] protected Node triggerNode;
    [Tooltip("Order index to start executing")]
    [SerializeField] protected int startIndex;
    [Tooltip("Select if the calling node should stop or continue executing orders, or wait until the called node finishes.")]
    [SerializeField] protected CallMode callMode;

    public override void OnEnter()
    {
        if (string.IsNullOrEmpty(achievementID))
        {
            Debug.LogError("Achievement ID is missing!");
            return;
        }

        var achievementRules = GetEngine().GetComponentInChildren<AchievementRules>();

        if (achievementRules == null)
        {
            return;
        }
        achievementRules.GenericEvent(achievementID, progress, amount);
        achievementFeedback?.PlayFeedbacks();
        if (triggerNode != null)
        {
            //are we calling our own parent node?
            if (ParentNode != null && ParentNode.Equals(triggerNode))
            {
                //if so, just execute the first order and ignore the call
                Continue(0);
                return;
            }

            if (triggerNode.IsExecuting())
            {
                Debug.LogWarning(triggerNode._NodeName + " cannot be called/executed, it is already running.");
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
                StartCoroutine(triggerNode.Execute(index, onComplete));
            }
            else
            {
                if (callMode == CallMode.StopThenCall)
                {
                    StopParentNode();
                }
                // Execute node in another Engine
                targetEngine.ExecuteNode(triggerNode, index, onComplete);
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
        // If no trigger node is specified, just continue
        Continue();
    }

    public override string GetSummary()
    {
        return "Achievement: " + achievementID + " " + (progress ? "Progress" : "Unlock");
    }

    public override void GetConnectedNodes(ref System.Collections.Generic.List<Node> connectedNodes)
    {
        if (triggerNode != null)
        {
            connectedNodes.Add(triggerNode);
        }
    }
}


