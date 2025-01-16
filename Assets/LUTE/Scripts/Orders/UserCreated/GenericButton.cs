using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;

[OrderInfo("Menu",
              "GenericButton",
              "Creates a button which allows for generic event inputs")]
[AddComponentMenu("")]
public class GenericButton : Order
{
    [Tooltip("Custom icon to display for this menu")]
    [SerializeField] protected Sprite customButtonIcon;
    [Tooltip("A custom popup class to use to display this menu - if one is in the scene it will be used instead")]
    [SerializeField] protected PopupIcon setIconButton;
    [Tooltip("If true, the popup icon will be displayed, otherwise it will be hidden")]
    [SerializeField] protected bool showIcon = true;
    [Tooltip("The feedbacks to play when the button is clicked")]
    [SerializeField] protected MMFeedbacks buttonFeedback;
    [Tooltip("The event to call when the button is clicked")]
    [SerializeField] protected UnityEngine.Events.UnityEvent buttonEvent;
    [SerializeField] protected AudioClip buttonSound;
    public override void OnEnter()
    {
        var popupIcon = SetupButton();

        // Add additional functionality to button event
        UnityAction extendedAction = () =>
        {
            // Call the original button event
            buttonEvent.Invoke();
            // Play the feedbacks
            buttonFeedback?.PlayFeedbacks();
        };

        SetAction(popupIcon, extendedAction.Invoke);

        Continue();
    }

    protected virtual PopupIcon SetupButton()
    {
        if (setIconButton != null)
        {
            PopupIcon.ActivePopupIcon = setIconButton;
        }
        var popupIcon = PopupIcon.GetPopupIcon();
        if (popupIcon != null)
        {
            if (customButtonIcon != null)
            {
                popupIcon.SetIcon(customButtonIcon);
            }
        }
        if (showIcon)
        {
            popupIcon.SetActive(true);
        }

        return popupIcon;
    }

    protected virtual void SetAction(PopupIcon popup, UnityAction action)
    {
        if (popup == null || action == null)
        {
            return;
        }

        action += () =>
        {
            if (buttonSound != null)
            {
                LogaManager.Instance.SoundManager.PlaySound(buttonSound, -1);
            }
        };

        popup.SetAction(action);
        popup.MoveToNextOption();
    }

    public override string GetSummary()
    {
        return "Creates a button which allows for generic event inputs";
    }
}