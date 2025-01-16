using MoreMountains.InventoryEngine;
using UnityEngine;

[OrderInfo("Inventory",
              "InventoryMenu",
              "Creates a button which will toggle the inventory on/off (rather than using a nested button in popups)")]
[AddComponentMenu("")]
public class InventoryMenu : GenericButton
{
    [Tooltip("The inventory to toggle")]
    [SerializeField] protected Inventory inventory;

    public override void OnEnter()
    {
        if (inventory == null)
        {
            inventory = Inventory.FindInventory("MainInventory", "Player1");
            if (inventory == null)
            {
                Continue();
                return;
            }
        }

        var popupIcon = SetupButton();

        UnityEngine.Events.UnityAction action = () =>
        {
            InventoryInputManager inventoryInputManager = inventory.GetComponentInChildren<InventoryInputManager>();
            if (inventoryInputManager != null)
            {
                inventoryInputManager.ToggleInventory();
            }
        };

        SetAction(popupIcon, action);

        Continue();
    }

    public override string GetSummary()
    {
        return "Creates a button which will toggle the inventory on/off (rather than using a nested button in popups)";
    }
}