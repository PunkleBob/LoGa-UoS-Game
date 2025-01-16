using UnityEngine;

[OrderInfo("Achievements",
              "ShowList",
              "Shows or hides the achievement list depending if the list is active or not")]
[AddComponentMenu("")]
public class ShowList : Order
{
  [Tooltip("Sets the active list  with a reference to a list object in the scene. Will display using this list.")]
  [SerializeField] protected AchievementListFiller setList;
  [Tooltip("Custom achievement item prefab to be spawned in the list - default will be used if empty")]
  [SerializeField] protected AchievementItem customAchievementItem;
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

    list.ShowList();

    Continue();
  }

  public override string GetSummary()
  {
    return "Shows the achievement list if it is not active";
  }
}