using UnityEngine;
[OrderInfo("Audio",
                "Play Music",
                "Plays looping game music. If any game music is already playing, it is stopped. Game music will continue playing across scene loads")]
[AddComponentMenu("")]
public class PlayMusic : Order
{
    [Tooltip("Music sound clip to play")]
    [SerializeField] protected AudioClip musicClip;
    [Tooltip("Time to begin playing in seconds. If the audio file is compressed, the time index may be inaccurate.")]
    [SerializeField] protected float atTime;
    [SerializeField] protected bool loop = true;
    [Tooltip("Length of time to fade out previous playing music.")]
    [SerializeField] protected float fadeDuration = 1f;

    public override void OnEnter()
    {
        var soundManager = LogaManager.Instance.SoundManager;

        float startTime = Mathf.Max(0, atTime);
        soundManager.PlayMusic(musicClip, loop, fadeDuration, startTime);

        Continue();
    }

    public override string GetSummary()
    {
        if (musicClip == null)
        {
            return "Error: No music clip selected";
        }

        return "Now Playing: " + musicClip.name;
    }

    public override Color GetButtonColour()
    {
        return new Color32(224, 160, 250, 255);
    }
}
