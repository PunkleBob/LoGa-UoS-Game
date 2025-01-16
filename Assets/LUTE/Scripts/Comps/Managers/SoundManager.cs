using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum AudioType
    {
        Music,
        SoundEffect
    }


    protected AudioSource audioSourceMusic;
    protected AudioSource audioSourceSoundEffect;

    void Reset()
    {
        int audioSourceCount = this.GetComponents<AudioSource>().Length;
        for (int i = 0; i < 3 - audioSourceCount; i++)
            gameObject.AddComponent<AudioSource>();
    }

    protected virtual void Awake()
    {
        Reset();
        AudioSource[] audioSources = GetComponents<AudioSource>();
        audioSourceMusic = audioSources[0];
        audioSourceSoundEffect = audioSources[1];
    }
    protected virtual void Start()
    {
        audioSourceMusic.playOnAwake = false;
        audioSourceMusic.loop = true;
    }

    /// Plays game music using an audio clip - one music clip may be played at a time.
    public void PlayMusic(AudioClip musicClip, bool loop, float fadeDuration, float atTime, bool resume = false)
    {
        if (audioSourceMusic == null || audioSourceMusic.clip == musicClip && resume == false)
        {
            return;
        }

        if (Mathf.Approximately(fadeDuration, 0f))
        {
            audioSourceMusic.clip = musicClip;
            audioSourceMusic.loop = loop;
            audioSourceMusic.time = atTime;  // May be inaccurate if the audio source is compressed http://docs.unity3d.com/ScriptReference/AudioSource-time.html BK
            audioSourceMusic.Play();
        }
        else
        {
            float startVolume = audioSourceMusic.volume;

            LeanTween.value(gameObject, startVolume, 0f, fadeDuration)
                .setOnUpdate((v) =>
                {
                    // Fade out current music
                    audioSourceMusic.volume = v;
                }).setOnComplete(() =>
                {
                    // Play new music
                    audioSourceMusic.volume = startVolume;
                    audioSourceMusic.clip = musicClip;
                    audioSourceMusic.loop = loop;
                    audioSourceMusic.time = atTime;  // May be inaccurate if the audio source is compressed http://docs.unity3d.com/ScriptReference/AudioSource-time.html BK
                    audioSourceMusic.Play();
                });
        }
    }

    public virtual void PlaySound(AudioClip soundClip, float volume)
    {
        if (volume <= 0)
        {
            // we can override the slider settings if we want to
            volume = audioSourceSoundEffect.volume;
        }
        audioSourceSoundEffect.PlayOneShot(soundClip, volume);
    }

    public virtual void SetAudioPitch(float pitch, float duration, System.Action onComplete)
    {
        if (Mathf.Approximately(duration, 0f))
        {
            audioSourceMusic.pitch = pitch;
            if (onComplete != null)
            {
                onComplete();
            }
            return;
        }

        LeanTween.value(gameObject,
            audioSourceMusic.pitch,
            pitch,
            duration).setOnUpdate((p) =>
            {
                audioSourceMusic.pitch = p;
            }).setOnComplete(() =>
            {
                if (onComplete != null)
                {
                    onComplete();
                }
            });
    }
    public virtual void SetAudioVolume(float volume, float duration, System.Action onComplete, AudioType audioType)
    {
        // Eventually switch with cases for different audio types (if needed)
        AudioSource audioSource = audioType == AudioType.Music ? audioSourceMusic : audioSourceSoundEffect;

        if (Mathf.Approximately(duration, 0f))
        {
            if (onComplete != null)
            {
                onComplete();
            }
            audioSource.volume = volume;
            return;
        }

        LeanTween.value(gameObject,
            audioSource.volume,
            volume,
            duration).setOnUpdate((v) =>
            {
                audioSource.volume = v;
            }).setOnComplete(() =>
            {
                if (onComplete != null)
                {
                    onComplete();
                }
            });
    }

    public virtual void SetSourceTime(float value)
    {
        if (!audioSourceMusic.isPlaying || audioSourceMusic.clip == null)
            return;

        audioSourceMusic.time = value * audioSourceMusic.clip.length;
    }

    public virtual void StopMusic()
    {
        audioSourceMusic.Stop();
        audioSourceMusic.clip = null;
    }

    public void StopMusic(float fadeDuration)
    {
        // If no audio is playing, return
        if (audioSourceMusic == null || !audioSourceMusic.isPlaying)
        {
            return;
        }

        if (Mathf.Approximately(fadeDuration, 0f))
        {
            // Immediately stop the music if no fade-out duration
            audioSourceMusic.Stop();
            audioSourceMusic.clip = null;
        }
        else
        {
            float startVolume = audioSourceMusic.volume;

            // Fade out the current music over the given duration
            LeanTween.value(gameObject, startVolume, 0f, fadeDuration)
                .setOnUpdate((v) =>
                {
                    // Gradually reduce the volume
                    audioSourceMusic.volume = v;
                })
                .setOnComplete(() =>
                {
                    // Once fade-out is complete, stop the music
                    audioSourceMusic.Stop();
                    // Optionally reset the volume back to the starting volume (if you'll play new music later)
                    audioSourceMusic.volume = startVolume;
                    audioSourceMusic.clip = null;
                });
        }
    }

    public virtual void PauseMusic()
    {
        audioSourceMusic.Pause();
    }

    public virtual float GetVolume(AudioType audioType)
    {
        return audioType == AudioType.Music ? audioSourceMusic.volume : audioSourceSoundEffect.volume;
    }

    public virtual AudioSource GetAudioSource()
    {
        if ((audioSourceMusic == null))
            return null;

        return audioSourceMusic;
    }

    public virtual AudioSource GetSFXSource()
    {
        if ((audioSourceSoundEffect == null))
            return null;
        return audioSourceSoundEffect;
    }
}
