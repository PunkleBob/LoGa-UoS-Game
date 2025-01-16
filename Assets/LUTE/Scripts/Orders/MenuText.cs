using UnityEngine;

[OrderInfo("Menu",
             "Text",
             "Displays a text box in a menu - useful for setting titles on pause menus, etc.")]
[AddComponentMenu("")]
public class MenuText : Order
{
    [Tooltip("Text to display on the menu")]
    [TextArea()]
    [SerializeField] protected string text = "";
    [Tooltip("If true, this option will be passed to the Menu Dialogue but marked as hidden, this can be used to hide options while maintaining a Menu Shuffle.")]
    [SerializeField] protected bool hideThisOption = false;

    protected bool isPopupChoice = false;

    public override void OnEnter()
    {
        //go through the list of orders to determine if one is a popup 
        //we can then determine if we need to set the menu dialogue or to popup menu
        //if we are a popup choice, we don't need to set the menu dialogue

        var orders = ParentNode.OrderList;
        if (orders.Count > 0)
        {
            foreach (Order order in orders)
            {
                if (order is PopupMenu)
                {
                    isPopupChoice = true;
                }
            }
        }

        if (!isPopupChoice)
        {
            //for now we return if we are not a popup choice as this order only concerns menus which are set using below method
            return;
        }

        Continue();
    }

    public virtual void SetMenuText(Popup popup)
    {
        if (popup != null)
        {
            popup.AddMenuText(text, hideThisOption);
        }
    }

    public override string GetSummary()
    {
        return text;
    }

    // public override Color GetButtonColor()
    // {
    //     return new Color32(235, 191, 217, 255);
    // }
}
