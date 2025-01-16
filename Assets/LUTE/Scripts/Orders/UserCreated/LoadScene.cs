using UnityEngine;

[OrderInfo("Scenes",
              "LoadScene",
              "Loads a scene based on a given scene index or exact name")]
[AddComponentMenu("")]
public class LoadScene : Order
{
    [Tooltip("the exact name of the target level")]
    [SerializeField] protected string sceneName;
    [Tooltip("the index of the target level")]
    [SerializeField] protected int sceneIndex;
    public override void OnEnter()
    {
        if (!string.IsNullOrEmpty(sceneName))
            LevelSelector.LoadScene(sceneName);
        else if (sceneIndex >= 0)
        {
            LevelSelector.LoadScene(sceneIndex);
        }
        else
            Debug.LogError("No level name or index provided");
        //Continue();
    }

    public override string GetSummary()
    {
        //you can use this to return a summary of the order which is displayed in the inspector of the order
        string levelName = string.IsNullOrEmpty(this.sceneName) ? "No level name provided" : this.sceneName;
        if (sceneIndex >= 0)
        {
            levelName += " (Index: " + sceneIndex + ")";
        }
        return "Loading Scene " + levelName;
    }
}