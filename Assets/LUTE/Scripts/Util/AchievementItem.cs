using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AchievementItem : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI achievementTitle;
    [SerializeField] protected TextMeshProUGUI achievementDesc;
    [SerializeField] protected Image achievementImage;
    [SerializeField] protected Image achievementBackground;
    [SerializeField] protected Color unlockedColor = Color.green;
    [SerializeField] protected Color lockedColor = Color.red;


    [HideInInspector]
    public string achievementID;

    private List<MMAchievement> achievements = new List<MMAchievement>();

    public virtual void SetAchievement(string title, string description, Sprite image, string ID, List<MMAchievement> _achievements)
    {
        achievementTitle.text = title;
        achievementDesc.text = description;
        achievementImage.sprite = image;
        this.achievementID = ID;
        achievementBackground.color = lockedColor;
        achievements = _achievements;
    }

    protected virtual void Update()
    {
        foreach (MMAchievement achievement in MMAchievementManager.AchievementsList)
        {
            if (achievement.AchievementID == achievementID)
            {
                if (achievement.ProgressTarget > 1)
                {
                    if (!achievementDesc.text.Contains(achievement.ProgressCurrent + "/" + achievement.ProgressTarget))
                    {
                        achievementDesc.text += "\t\t\t\t\t" + achievement.ProgressCurrent + "/" + achievement.ProgressTarget;
                    }
                }

                if (achievement.UnlockedStatus)
                {
                    achievementDesc.text = "";
                    achievementDesc.text = achievement.Description;
                    achievementDesc.text += "\t\t\t\t\t" + achievement.ProgressCurrent + "/" + achievement.ProgressTarget;
                    achievementImage.sprite = achievement.UnlockedImage;
                    SetCompleteAchievement();
                }
                else
                {
                    achievementImage.sprite = achievement.LockedImage;
                }
            }
        }
    }

    private void SetCompleteAchievement()
    {
        //set the background color to green
        achievementBackground.color = unlockedColor;
        //set the icon to unlocked?
    }
}
