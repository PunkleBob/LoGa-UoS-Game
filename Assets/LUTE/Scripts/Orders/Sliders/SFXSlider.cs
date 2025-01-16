namespace LoGaCulture.LUTE
{
    [OrderInfo("Menu", "SFX Option Slider", "Displays a slider in a menu to adjust the SFX volume")]
    public class SFXSlider : OptionSlider
    {
        private void Start()
        {
            if (sliderLabel.Length <= 0)
            {
                sliderLabel = "SFX volume";
            }

            targetFloat = LogaManager.Instance.SoundManager.GetVolume(SoundManager.AudioType.SoundEffect);

            hideOption = (hideIfMoved && targetFloat < 0) || hideThisOption;
        }
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
                if (setMenuDialogue != null)
                {
                    MenuDialogue.SetMenuDialogue(setMenuDialogue);
                }

                var menu = MenuDialogue.GetMenuDialogue();
                if (menu != null)
                {
                    menu.SetActive(true);

                    UnityEngine.Events.UnityAction<float> action = (float value) =>
                    {
                        LogaManager.Instance.SoundManager.SetAudioVolume(value, 0, null, SoundManager.AudioType.SoundEffect);
                    };

                    menu.AddOptionSlider(interactable, targetFloat, hideOption, action, sliderLabel);
                }

                Continue();
            }
        }

        public override void SetSliderOptions(Popup popup)
        {
            if (popup != null)
            {
                UnityEngine.Events.UnityAction<float> action = (float value) =>
                {
                    LogaManager.Instance.SoundManager.SetAudioVolume(value, 0, null, SoundManager.AudioType.SoundEffect);
                };

                popup.AddOptionSlider(interactable, targetFloat, hideOption, action, sliderLabel);
            }
        }

        public override string GetSummary()
        {
            if (!string.IsNullOrEmpty(sliderLabel))
            {
                return sliderLabel + ": interactable: " + interactable;
            }
            else
            {
                return interactable.ToString();
            }
        }
    }
}
