using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using UnityEngine;

[OrderInfo("Items",
              "UsableItem",
              "Creates an item which will either show a card to use an item or use it when location is met")]
[AddComponentMenu("")]
public class UsableItem : Order
{
  [Tooltip("The feedback to play when the item gets used")]
  [SerializeField] protected MMFeedbacks useFeedback;
  [Tooltip("Whether or not to show the item card (if false the item wil be used automatically)")]
  [SerializeField] protected bool showCard = true;
  [Tooltip("Where this item will be placed on the map")]
  [SerializeField] protected LocationVariable itemLocation;
  [Tooltip("The item that will actually be used")]
  [SerializeField] protected InventoryItem item;

  public override void OnEnter()
  {
    LocationItemUsable.CreateItem(null, item, useFeedback, showCard, itemLocation);
    Continue();
  }

  public override string GetSummary()
  {
    // string cardText = showCard ? "display a usable item card" : "be used " + itemsQuantitiy + " automatically";
    // return "Create a " + item.ItemName + " usable item that will " + cardText;
    return "";
  }
}