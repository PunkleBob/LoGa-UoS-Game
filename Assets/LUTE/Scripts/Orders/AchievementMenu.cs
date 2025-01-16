using UnityEngine;

[OrderInfo("Achievements",
              "Achievement List Button  ",
              "Creates a button which will toggle the achievement on/off (rather than using a nested button in popups)")]
[AddComponentMenu("")]
public class AchievementMenu : GenericButton
{
    [Tooltip("The set achievement list to toggle - this will be found in the scene if not provided")]
    [SerializeField] protected AchievementListFiller setList;

    public override void OnEnter()
    {
        if (setList != null)
        {
            AchievementListFiller.ActiveList = setList;
        }

        var list = AchievementListFiller.GetList();

        if (list == null)
        {
            Continue();
            return;
        }

        var popupIcon = SetupButton();

        UnityEngine.Events.UnityAction action = () =>
        {
            list.ShowList();
            buttonFeedback?.PlayFeedbacks();
        };

        SetAction(popupIcon, action);

        Continue();
    }
    public override string GetSummary()
    {
        return "Creates a button which will toggle the achievment list on/off (rather than using a nested button in popups)";
    }
}