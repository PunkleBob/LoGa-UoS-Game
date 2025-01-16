using UnityEngine;

[OrderInfo("Logic",
             "Stop",
             "Stop executing the Node that contains this order.")]
[AddComponentMenu("")]
public class Stop : Order
{
    public override void OnEnter()
    {
        StopParentNode();
    }
}