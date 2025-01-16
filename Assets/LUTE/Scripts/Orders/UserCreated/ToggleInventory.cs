using MoreMountains.InventoryEngine;
using UnityEngine;

[OrderInfo("Inventory",
              "Toggle Inventory",
              "Toggles the inventory panel on or off depending on its current state")]
[AddComponentMenu("")]
public class ToggleInventory : Order
{
  [Tooltip("The inventory to toggle")]
  [SerializeField] protected Inventory inventory;
  public override void OnEnter()
  {
    if (inventory != null)
    {
      InventoryInputManager inventoryInputManager = inventory.GetComponentInChildren<InventoryInputManager>();
      if (inventoryInputManager != null)
      {
        inventoryInputManager.ToggleInventory();
      }
    }
    Continue();
  }

  public override string GetSummary()
  {
    return inventory != null ? "Toggles the inventory panel on or off depending on its current state" : "Error: Inventory not supplied";
  }
}