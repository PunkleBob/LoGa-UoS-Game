using UnityEngine;

/// Waits for period of time before executing the next order in the list
[OrderInfo("Flow",
             "Wait",
             "Waits for period of time before executing the next order in the list")]
[AddComponentMenu("")]
[ExecuteInEditMode]
public class Wait : Order
{
    [Tooltip("Length of time to wait for")]
    [SerializeField] protected float _duration = 1f;

    protected virtual void OnWaitFinalised()
    {
        Continue();
    }

    public override void OnEnter()
    {
        Invoke("OnWaitFinalised", _duration);
    }
    public override string GetSummary()
    {
        return _duration.ToString() + " seconds";
    }

    //get the button colour once styling has been implemented
}
