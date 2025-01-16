using System.Collections;
using UnityEngine;

[EventHandlerInfo("Default",
                  "Game Started",
                  "Executes when the game starts playing")]
[AddComponentMenu("")]
public class GameStarted : EventHandler
{
    [Tooltip("Wait for a number of frames before executing the node")]
    [SerializeField] protected int waitForFrames = 1;

    protected virtual void Start()
    {
        StartCoroutine(GameStartedCoroutine());
    }

    protected virtual IEnumerator GameStartedCoroutine()
    {
        int frameCount = waitForFrames;
        while (frameCount > 0)
        {
            yield return new WaitForEndOfFrame();
            frameCount--;
        }
        ExecuteNode();
    }

    public override string GetSummary()
    {
        return "This node will execute when the game starts playing";
    }
}
