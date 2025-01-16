using UnityEngine;
[AddComponentMenu("")]
public class OptionSlider : Order
{
    [Tooltip("Notes about the option for other authors, localization, etc.")]
    [SerializeField] protected string description = "";
    [Tooltip("Text to display on the slider")]
    [SerializeField] protected string sliderLabel = "";
    [Tooltip("Hide this option if the slider has been moved previously")]
    [SerializeField] protected bool hideIfMoved;
    [Tooltip("If false, the menu option will be displayed but will not be selectable")]
    [SerializeField] protected bool interactable = true;
    [Tooltip("A custom Menu display to use to display this menu")]
    [SerializeField] protected MenuDialogue setMenuDialogue;
    [Tooltip("If true, this option will be passed to the Menu Dialogue but marked as hidden, this can be used to hide options while maintaining a Menu Shuffle.")]
    [SerializeField] protected bool hideThisOption = false;

    protected float targetFloat;
    protected bool isPopupChoice = false;
    protected bool hideOption = false;

    public MenuDialogue SetMenuDialogue { get { return setMenuDialogue; } set { setMenuDialogue = value; } }

    public virtual void SetSliderOptions(Popup popup) { }
}
