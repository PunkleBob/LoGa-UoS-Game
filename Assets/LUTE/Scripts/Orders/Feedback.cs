using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;

[OrderInfo("Juice",
             "Play Feedback",
             "Add extra juice to your game by playing a sound effect or particle effect etc. Feedback components are created on separate game objects so that they can be reused across multiple orders.")]
[AddComponentMenu("")]
public class Feedback : Order
{
    [Tooltip("Feedback to play")]
    [SerializeField] protected MMFeedbacks feedback = null;

    [Tooltip("Whether to play the feedback immediately or wait for the next frame")]
    [SerializeField] protected bool playImmediately = true;
    [Tooltip("Whether to wait until the feedback has finished playing before executing the next command")]
    [SerializeField] protected bool waitUntilFinished = true;
    public override void OnEnter()
    {
        if (feedback == null)
        {
            Continue();
            return;
        }

        if (playImmediately)
        {
            feedback.PlayFeedbacks();

            if (waitUntilFinished)
            {
                StartCoroutine(WaitForFeedbackToFinish());
            }
            else
            {
                Continue();
            }
        }
        else
        {
            //wait until next frame to play feedback
            StartCoroutine(WaitUntilNextFrame());
        }
    }

    //ienumerator to wait until next frame
    protected virtual IEnumerator WaitUntilNextFrame()
    {
        //simply wait a frame
        yield return 0;
        feedback.PlayFeedbacks();
        if (waitUntilFinished)
        {
            StartCoroutine(WaitForFeedbackToFinish());
        }
        else
        {
            Continue();
        }
    }

    //ienumerator to wait until feedback has finished playing
    protected virtual IEnumerator WaitForFeedbackToFinish()
    {
        while (feedback.IsPlaying)
        {
            yield return null;
        }
        Continue();
    }

    public override string GetSummary()
    {
        if (feedback == null)
        {
            return "Error: No feedback selected";
        }

        return feedback.name + (playImmediately ? ": played immediately" : "") + (waitUntilFinished ? " and waits until finished" : "");
    }

    public override Color GetButtonColour()
    {
        return new Color32(255, 195, 150, 255);
    }
}
