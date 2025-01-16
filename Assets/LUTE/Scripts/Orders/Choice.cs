using MoreMountains.Feedbacks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[OrderInfo("Narrative",
             "Choice",
             "Displays a button in a multiple choice menu to move to next node")]
[AddComponentMenu("")]
public class Choice : Order
{
    [Tooltip("Text to display on the menu button")]
    [TextArea()]
    [SerializeField] protected string text = "Option Text";
    [Tooltip("Notes about the option text for other authors, localization, etc.")]
    [SerializeField] protected string description = "";
    [Tooltip("Node to execute when this option is selected")]
    [SerializeField] public Node targetNode;
    [Tooltip("If true, the choice will continue to the order in the list after this option is selected rather than executing the target node")]
    [SerializeField] protected bool justContinue = false;
    [Tooltip("If true, the next choice in the order list will be displayed after this choice is called")]
    [SerializeField] protected bool showNextChoice = false;
    [Tooltip("Hide this option if the target node has been executed previously")]
    [SerializeField] protected bool hideIfVisited;
    [Tooltip("If false, the menu option will be displayed but will not be selectable")]
    [SerializeField] protected bool interactable = true;
    [Tooltip("A custom Menu display to use to display this menu")]
    [SerializeField] protected MenuDialogue setMenuDialogue;
    [Tooltip("If true, this option will be passed to the Menu Dialogue but marked as hidden, this can be used to hide options while maintaining a Menu Shuffle.")]
    [SerializeField] protected bool hideThisOption = false;
    [Tooltip("If true, the menu will close when this option is selected")]
    [SerializeField] protected bool closeMenuOnSelect = true;
    [Tooltip("If true, this option will be passed to the Menu Dialogue but marked as a choice, this can be used to hide options while maintaining a Menu Shuffle.")] //to be implemented
    protected bool isPopupChoice = false;
    [Tooltip("Feedback to play when the button is selected")]
    [SerializeField] MMFeedbacks buttonFeedback;
    [SerializeField] protected AudioClip buttonSound;

    public MenuDialogue SetMenuDialogue { get { return setMenuDialogue; } set { setMenuDialogue = value; } }

    private bool hasExecuted = false;

    public override void OnEnter()
    {
        //go through the list of orders to determine if one is a popup 
        //we can then determine if we need to set the menu dialogue or to popup menu
        //if we are a popup choice, we don't need to set the menu dialogue

        if (hasExecuted)
        {
            Continue();
            ParentNode.Stop();
            return;
        }

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

            bool hideOption = (hideIfVisited && targetNode != null && targetNode.GetExecutionCount() > 0) || hideThisOption;

            var menu = MenuDialogue.GetMenuDialogue();
            if (menu != null)
            {
                menu.SetActive(true);

                string variedText = GetEngine().SubstituteVariables(text);

                menu.AddOption(variedText, interactable, hideOption, targetNode, closeMenuOnSelect, buttonFeedback, justContinue, Continue, showNextChoice, OrderIndex, ParentNode, buttonSound);
            }

            // could have a boolean of show next choice where we find next choice in order list and call it. then those choices would need to store an index of their position in the order list (order index) and we could just call the next one in the list

            if (justContinue)
            {
                if (showNextChoice)
                {
                    int currentIndex = ParentNode.OrderList.IndexOf(this);  // Get the current index of 'this'
                    if (currentIndex != -1) // Ensure 'this' is found in the list
                    {
                        // Search from the next index to the end of the list
                        var nextChoice = ParentNode.OrderList
                            .Skip(currentIndex + 1)  // Skip current index to start searching after 'this'
                            .FirstOrDefault(x => x is Choice);

                        if (nextChoice != null)
                        {
                            nextChoice.Execute();
                        }
                    }

                }
            }
            else
            {
                Continue();
            }
        }

        hasExecuted = true;
    }

    public virtual void SetMenuChoice(Popup popup)
    {
        string variedText = GetEngine().SubstituteVariables(text);
        if (popup != null)
        {
            popup.AddOption(variedText, interactable, hideIfVisited, targetNode, closeMenuOnSelect, buttonFeedback, buttonSound);
        }
    }

    public override void GetConnectedNodes(ref List<Node> connectedNodes)
    {
        if (targetNode != null)
        {
            connectedNodes.Add(targetNode);
        }
    }

    public override string GetSummary()
    {
        if (targetNode == null && justContinue == false)
        {
            return "Error: No target node selected";
        }

        if (text == "")
        {
            return "Error: No button text selected";
        }

        if (justContinue)
        {
            return "Choice continues to next Order in list";
        }

        return text + " : " + targetNode._NodeName;
    }

    public override Color GetButtonColour()
    {
        return new Color32(224, 96, 22, 255);
    }

}
