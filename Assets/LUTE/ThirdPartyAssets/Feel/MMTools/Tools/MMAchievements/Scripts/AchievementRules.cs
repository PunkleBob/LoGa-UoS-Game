using MoreMountains.Tools;
using UnityEngine;

public class AchievementRules : MMAchievementRules
{
    public void GenericEvent(string eventName, bool progress, int amount = 1)
    {
        if (progress)
            MMAchievementManager.AddProgress(eventName, amount);
        else
            MMAchievementManager.UnlockAchievement(eventName);
    }
}
