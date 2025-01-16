using TMPro;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    [OrderInfo("Util",
             "Set Text Info Panel",
             "Sets text on an info panel the reveals hit")]
    [AddComponentMenu("")]
    public class SetTextInfoPanel : Order
    {
        [Tooltip("Text to add to the info panel")]
        [TextArea(15, 20)]
        [SerializeField] protected string textToAdd;
        [Tooltip("Text alignment for the info panel")]
        [SerializeField] protected TextAlignmentOptions textAlignment;
        [Space]
        [Tooltip("Whether to override the default button behaviour on the info panel")]
        [SerializeField] protected UnityEngine.Events.UnityEvent buttonAction;
        [Tooltip("A custom text info panel to use")]
        [SerializeField] protected TextInfoPanel textInfoPanel;

        public override void OnEnter()
        {
            if (textInfoPanel == null)
            {
                textInfoPanel = TextInfoPanel.GetInfoPanel();
            }
            if (textInfoPanel != null)
            {
                textInfoPanel.SetInfo(textToAdd, buttonAction == null ? null : buttonAction, textAlignment);
                textInfoPanel.ToggleMenu();
            }
            Continue();
        }
    }
}
