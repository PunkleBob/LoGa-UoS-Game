using UnityEngine;

namespace LoGaCulture.LUTE
{
    [OrderInfo("Audio",
             "Play Sound",
             "Plays a once-off sound effect. Multiple sound effects can be played at the same time.")]
    [AddComponentMenu("")]
    public class PlaySound : Order
    {
        [Tooltip("Sound effect clip to play")]
        [SerializeField] protected AudioClip soundClip;

        [Range(-1, 1)]
        [Tooltip("Volume level of the sound effect. Use -1 to match the volume of the current SFX player.")]
        [SerializeField] protected float volume = 1;

        [Tooltip("Wait until the sound has finished playing before continuing execution.")]
        [SerializeField] protected bool waitUntilFinished;

        protected virtual void DoWait()
        {
            Continue();
        }

        public override void OnEnter()
        {
            if (soundClip == null)
            {
                Continue();
                return;
            }

            var musicManager = LogaManager.Instance.SoundManager;

            musicManager.PlaySound(soundClip, volume);

            if (waitUntilFinished)
            {
                Invoke("DoWait", soundClip.length);
            }
            else
            {
                Continue();
            }
        }

        public override string GetSummary()
        {
            if (soundClip == null)
            {
                return "Error: No sound clip selected";
            }

            return soundClip.name;
        }

        public override Color GetButtonColour()
        {
            return new Color32(242, 209, 176, 255);
        }

    }
}
