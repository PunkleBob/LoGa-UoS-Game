using System.Collections.Generic;
using UnityEngine;

public class AudioWriter : MonoBehaviour, IWriterListener
{
    public enum AudioMode
    {
        Beeps,
        SoundEffect,
    }

    [SerializeField] protected List<AudioClip> beepSounds = new List<AudioClip>();

    [SerializeField] protected AudioSource targetAudioSource;

    [Range(0, 1)]
    [SerializeField] protected float volume = 1f;

    [Tooltip("Sound effect to play on user input (e.g. a click)")]
    [SerializeField] protected AudioClip inputSound;

    [Tooltip("Type of sound effect to play when writing text")]
    [SerializeField] protected AudioMode audioMode = AudioMode.Beeps;

    [Tooltip("Loop the audio when in Sound Effect mode. Has no effect in Beeps mode.")]
    [SerializeField] protected bool loop = true;

    // When true, a beep will be played on every written character glyph
    protected bool playBeeps;
    protected float nextBeepTime;

    protected float targetVolume = 0f;

    // True when a voiceover clip is playing
    protected bool playingVoiceover = false;

    public bool IsPlayingVoiceOver { get { return playingVoiceover; } }

    public float GetSecondsRemaining()
    {
        if (IsPlayingVoiceOver)
        {
            return targetAudioSource.isPlaying ? targetAudioSource.clip.length - targetAudioSource.time : 0f;
        }
        else
        {
            return 0F;
        }
    }
    protected virtual void SetAudioMode(AudioMode mode)
    {
        audioMode = mode;
    }

    protected virtual void Awake()
    {
        // Need to do this in Awake rather than Start due to init order issues
        if (targetAudioSource == null)
        {
            targetAudioSource = GetComponent<AudioSource>();
            if (targetAudioSource == null)
            {
                targetAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        targetAudioSource.volume = 0f;
    }

    protected virtual void Play(AudioClip audioClip)
    {
        if (targetAudioSource == null || audioMode == AudioMode.SoundEffect && audioClip == null || audioMode == AudioMode.Beeps && beepSounds.Count <= 0)
        {
            return;
        }

        playingVoiceover = false;
        targetAudioSource.volume = 0;
        targetVolume = volume;

        if (audioClip != null)
        {
            targetAudioSource.clip = audioClip;
            targetAudioSource.loop = loop;
            targetAudioSource.Play();
        }
        else if (audioMode == AudioMode.Beeps)
        {
            targetAudioSource.clip = null;
            targetAudioSource.loop = false;
            playBeeps = true;
        }
    }

    protected virtual void Pause()
    {
        if (targetAudioSource == null)
        {
            return;
        }

        // There's an audible click if you call audioSource.Pause() so instead just drop the volume to 0.
        targetVolume = 0f;
    }

    protected virtual void Stop()
    {
        if (targetAudioSource == null)
        {
            return;
        }

        // There's an audible click if you call audioSource.Stop() so instead we just switch off
        // looping and let the audio stop automatically at the end of the clip
        targetVolume = 0f;
        targetAudioSource.loop = false;
        playBeeps = false;
        playingVoiceover = false;
    }

    protected virtual void Resume()
    {
        if (targetAudioSource == null)
        {
            return;
        }

        targetVolume = volume;
    }

    protected virtual void Update()
    {
        targetAudioSource.volume = Mathf.MoveTowards(targetAudioSource.volume, targetVolume, Time.deltaTime * 5f);
    }

    #region IWriterListener implementation
    public virtual void OnInput()
    {
        if (inputSound != null)
        {
            // Assumes we're playing a 2D sound
            AudioSource.PlayClipAtPoint(inputSound, Vector3.zero);
        }
    }

    public virtual void OnGlyph()
    {
        if (playingVoiceover)
        {
            return;
        }

        if (playBeeps && beepSounds.Count > 0)
        {
            if (!targetAudioSource.isPlaying)
            {
                if (nextBeepTime < Time.realtimeSinceStartup)
                {
                    targetAudioSource.clip = beepSounds[Random.Range(0, beepSounds.Count)];

                    if (targetAudioSource.clip != null)
                    {
                        targetAudioSource.loop = false;
                        targetVolume = volume;
                        targetAudioSource.Play();

                        float extend = targetAudioSource.clip.length;
                        nextBeepTime = Time.realtimeSinceStartup + extend;
                    }
                }
            }
        }
    }

    public virtual void OnVoiceover(AudioClip voiceoverClip)
    {
        if (targetAudioSource == null)
        {
            return;
        }

        playingVoiceover = true;

        targetAudioSource.volume = volume;
        targetVolume = volume;
        targetAudioSource.loop = false;
        targetAudioSource.clip = voiceoverClip;
        targetAudioSource.Play();
    }


    public virtual void OnStart(AudioClip audioClip)
    {
        if (playingVoiceover)
        {
            return;
        }
        Play(audioClip);
    }

    public virtual void OnPause()
    {
        if (playingVoiceover)
        {
            return;
        }
        Pause();
    }

    public virtual void OnResume()
    {
        if (playingVoiceover)
        {
            return;
        }
        Resume();
    }

    public virtual void OnEnd(bool stopAudio)
    {
        if (stopAudio)
        {
            Stop();
        }
    }

    public void OnAllWordsWritten()
    {
    }
    #endregion
}