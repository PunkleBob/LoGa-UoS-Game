using UnityEngine;

[OrderInfo("Logic",
              "Unlocks Node",
              "The target node selected will not be allowed to execute until this node has been completed (or not)")]
[AddComponentMenu("")]
public class UnlockNode : Order
{
  [SerializeField] public Node targetNode;
  [SerializeField] protected ComparisonOperator compare;
  [SerializeField] protected int groupCount;
  public override void OnEnter()
  {
    Continue();
  }

  public override string GetSummary()
  {
    string summary = "Unlock ";
    if (targetNode != null)
    {
      summary += targetNode._NodeName;
    }
    else
    {
      summary += "None";
    }
    if (groupCount > 0)
    {
      summary += " after " + groupCount + " nodes in this group";
    }
    else
    {
      summary += " after this node";
    }
    return summary;
  }
}