using UnityEngine;

[OrderInfo("Menu",
              "BooleanOptionDemoMap",
              "Adds a checkbox to set the demo map boolean to true or false")]
[AddComponentMenu("")]
public class BooleanOptionCustom : BooleanOption
{
    public override void OnEnter()
    {
        bool demoMap = GetEngine().DemoMapMode;

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
            if (setMenuDialogue != null)
            {
                MenuDialogue.SetMenuDialogue(setMenuDialogue);
            }

            var menu = MenuDialogue.GetMenuDialogue();
            if (menu != null)
            {
                menu.SetActive(true);
                UnityEngine.Events.UnityAction<bool> action = (bool value) =>
                {
                    //when doing the parent class you will need to find the relevant variable and set it
                    GetEngine().DemoMapMode = value;
                };
                menu.AddOptionToggle(interactable, demoMap, hideThisOption, action, text);
            }

            Continue();
        }
    }

    public virtual void SetMenuChoice(Popup popup)
    {
        bool demoMap = GetEngine().DemoMapMode;
        if (popup != null)
        {
            UnityEngine.Events.UnityAction<bool> action = (bool value) =>
            {
                //when doing the parent class you will need to find the relevant variable and set it
                GetEngine().DemoMapMode = value;
            };
            popup.AddOptionToggle(interactable, demoMap, hideThisOption, action, text);
        }
    }

    public override string GetSummary()
    {
        return "Setting the demo map boolean to true or false";
    }
}