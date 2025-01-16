using UnityEngine;

[OrderInfo("Generic",
              "Hide Object",
              "Hides a provided object in the scene")]
[AddComponentMenu("")]
public class HideObject : Order
{
  [Tooltip("The object to hide")]
  [SerializeField] protected GameObject objectToHide;
  [Tooltip("Time to wait until the object is hidden")]
  [SerializeField] protected float delay = 0f;
  public override void OnEnter()
  {
    if (objectToHide == null)
    {
      Continue();
      return;
    }

    Invoke("DelayHideObject", delay);
    Continue();
  }

  private void DelayHideObject()
  {
    objectToHide.SetActive(false);
  }

  public override string GetSummary()
  {
    if (objectToHide == null)
    {
      return "Error: Object to hide is not provided";
    }
    else
    {
      return "Hide: " + objectToHide.name + " in " + delay + " seconds";
    }
  }
}