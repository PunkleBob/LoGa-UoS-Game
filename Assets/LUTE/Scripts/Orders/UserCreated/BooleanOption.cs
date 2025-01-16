using UnityEngine;

[OrderInfo("Menu",
              "BooleanOption",
              "Adds a checkbox to set a boolean to true or false")]
[AddComponentMenu("")]
public class BooleanOption : Order
{
  //you have a custom bool var here
  [Tooltip("Text to display on the menu button")]
  [TextArea()]
  [SerializeField] protected string text = "Option Text";
  [Tooltip("If false, the menu option will be displayed but will not be selectable")]
  [SerializeField] protected bool interactable = true;
  [Tooltip("A custom Menu display to use to display this toggle")]
  [SerializeField] protected MenuDialogue setMenuDialogue;
  [Tooltip("If true, this option will be passed to the Menu Dialogue but marked as hidden, this can be used to hide options while maintaining a Menu Shuffle.")]
  [SerializeField] protected bool hideThisOption = false;
  protected bool isPopupChoice = false;

  public override void OnEnter()
  {
    //the default behaviour of the boolean option is to set the bool to true (if it exists)
    //even if the text is empty we can still set it to the boolean name

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