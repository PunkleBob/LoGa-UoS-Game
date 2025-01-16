using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;

[OrderInfo("Inventory",
              "AddItem",
              "Adds a new item to the inventory")]
[AddComponentMenu("")]
public class AddItem : Order
{
    [Tooltip("The item to add to the inventory")]
    [SerializeField] protected InventoryItem item;
    [Tooltip("The amount of the item to add to the inventory")]
    [SerializeField] protected int amount = 1;
    [Tooltip("Optional Feedback to play when giving the item")]
    [SerializeField] protected MMFeedbacks feedback;
    [Tooltip("If this item should persist across scenes and sytem or should be used in this instance")]
    [SerializeField] protected bool persistentItem = false;
    [Tooltip("If the item is already in the inventory, should we add to it or ignore it?")]
    [SerializeField] protected bool addIfAlreadyInInventory = true;
    private ItemPicker itemPicker;
    public override void OnEnter()
    {
        //this code gets executed as the order is called
        if (item == null)
        {
            Debug.LogError("No item picker set in the AddItem order");
            return;
        }

        if (!addIfAlreadyInInventory)
        {
            var inventory = item.TargetInventory("Player1");
            if (inventory != null && inventory.InventoryContains(item.ItemID).Count > 0)
            {
                // If there is an inventory and there inventory already contains this item then move on
                Continue();
                return;
            }
        }

        //we need a serialised item to actually add to the inventory so we must instantiate it
        itemPicker = GetEngine().gameObject.AddComponent<ItemPicker>();
        itemPicker.Item = item;
        //we add the item to the inventory
        itemPicker.Quantity = amount;
        Invoke("Add", 0.0f);
        //some orders may not lead to another node so you can call continue if you wish to move to the next order after this one   
        Continue();
    }

    private void Add()
    {
        feedback?.PlayFeedbacks();
        itemPicker.Pick();
        if (persistentItem)
            MMGameEvent.Trigger("Save");
    }

    public override string GetSummary()
    {
        //you can use this to return a summary of the order which is displayed in the inspector of the order
        return item != null ? "Adding " + amount + " " + item.ItemName + "(s)" : "Error: No item set";
    }

    public override Color GetButtonColour()
    {
        return new Color32(184, 210, 235, 255);
    }
}