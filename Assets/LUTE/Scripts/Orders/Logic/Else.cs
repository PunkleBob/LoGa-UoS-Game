using UnityEngine;

[OrderInfo("Logic",
             "Else",
             "Marks the start of a order block to be executed when the preceding If statement is False")]
[AddComponentMenu("")]
public class Else : Order
{
    public override void OnEnter()
    {
        //find the matching end to this else statement
        var matchingEnd = Condition.FindMatchingEnd(this);
        if (matchingEnd != null)
        {
            //execute it if we find it
            Continue(matchingEnd.OrderIndex + 1);
        }
        else
        {
            //otherwise stop the whole node
            StopParentNode();
        }
    }

    public override bool OpenNode()
    {
        return true;
    }

    public override bool CloseNode()
    {
        return true;
    }

    public override Color GetButtonColour()
    {
        return new Color32(0, 176, 176, 255);
    }
}
