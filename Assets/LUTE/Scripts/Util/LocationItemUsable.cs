using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using UnityEngine;
using UnityEngine.UI;

public class LocationItemUsable : MonoBehaviour
{
    [SerializeField] protected Image itemImage;
    protected MMFeedbacks feedback;
    protected bool showCard;
    protected LocationVariable location;
    protected InventoryItem item;
    protected Sprite imageIcon;

    private bool itemUsed = false;
    private Canvas canvas;
    private bool isSetup = false;

    protected virtual void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    protected virtual void Update()
    {
        if (isSetup)
        {
            if (CheckLocation())
            {
                //If we are at the location and we are NOT showing the card but have not picked up the item then we can pickup item
                if (!itemUsed)
                {
                    if (!showCard)
                    {
                        UseItem();
                    }
                }
            }

            bool canShowCard = CheckLocation() && !itemUsed && showCard;
            itemImage.sprite = item.Icon;
            canvas.enabled = canShowCard;
        }
    }

    public static LocationItemUsable CreateItem(LocationItemPickup customPrefab, InventoryItem item, MMFeedbacks feedbacks, bool card, LocationVariable location)
    {
        GameObject go = null;
        if (customPrefab != null)
            go = Instantiate(customPrefab.gameObject) as GameObject;
        else
        {
            GameObject containerObj = Resources.Load<GameObject>("Prefabs/UseItemCard");
            if (containerObj != null)
                go = Instantiate(containerObj) as GameObject;
        }
        go.name = "UsableItem_" + item.ItemName;

        var itemContainer = go.GetComponent<LocationItemUsable>();
        itemContainer.feedback = feedbacks;
        itemContainer.showCard = card;
        itemContainer.location = location;
        itemContainer.item = item;

        itemContainer.isSetup = true;

        return itemContainer;
    }

    public virtual void UseItem(string playerID = "Player1")
    {
        if (item != null && !itemUsed)
        {
            itemUsed = true;
            feedback?.PlayFeedbacks();
            item.Use(playerID); //should actually find the inventory properly or the player ID properly
        }

    }

    private bool CheckLocation()
    {
        return location.Evaluate(ComparisonOperator.Equals, null);
    }
}
