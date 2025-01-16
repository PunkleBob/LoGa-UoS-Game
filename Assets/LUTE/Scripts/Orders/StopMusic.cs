using UnityEngine;

namespace LoGaCulture.LUTE
{
    [OrderInfo("Audio",
                "Stop Music",
                "Stops game music/sound. Game music will continue playing across scene loads")]
    [AddComponentMenu("")]
    public class StopMusic : Order
    {
        [SerializeField] protected float fadeDuration = 1;
        public override void OnEnter()
        {
            var soundManager = LogaManager.Instance.SoundManager;

            soundManager.StopMusic(fadeDuration);

            Continue();
        }

        public override string GetSummary()
        {

            return "Stopping Game Music or Sound that is currently playing";
        }

        public override Color GetButtonColour()
        {
            return new Color32(224, 160, 250, 255);
        }
    }
}
