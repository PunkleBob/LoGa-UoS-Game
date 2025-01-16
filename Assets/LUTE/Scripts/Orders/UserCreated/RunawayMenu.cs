using UnityEngine;

[OrderInfo("Menu",
              "Runaway Menu",
              "Spawns a runaway menu prefab")]
[AddComponentMenu("")]
public class RunawayMenu : Order
{
  [Tooltip("The menu prefab to spawn")]
  [SerializeField] protected RunawayPopup runawayMenu;
  public override void OnEnter()
  {
    //this code gets executed as the order is called
    //some orders may not lead to another node so you can call continue if you wish to move to the next order after this one   
    //Continue();
  }

  public override string GetSummary()
  {
    //you can use this to return a summary of the order which is displayed in the inspector of the order
    return "";
  }
}