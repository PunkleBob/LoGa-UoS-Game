using UnityEngine;
using UnityEngine.Serialization;

namespace LoGaCulture.LUTE
{
    [OrderInfo("UI",
             "Fade TM Text",
             "Fades a text component to a target colour over a period of time")]
    [AddComponentMenu("")]
    public class FadeText : Order
    {
        [Tooltip("TMPro Text object to be faded")]
        [SerializeField] protected TMPro.TextMeshProUGUI TMText;
        [Tooltip("Unity Text object to be faded")]
        [SerializeField] protected UnityEngine.UI.Text unityText;
        [Tooltip("Length of time to perform the fade")]
        [SerializeField] protected float _duration = 1f;
        [Tooltip("Target color to fade to. To only fade transparency level, set the color to white and set the alpha to required transparency.")]
        [SerializeField] protected Color32 _targetColor = Color.white;
        [Tooltip("Wait until the fade has finished before executing the next order")]
        [SerializeField] protected bool waitUntilFinished = true;

        public override void OnEnter()
        {
            if (TMText == null && unityText == null)
            {
                Continue();
                return;
            }

            //Custom class to handle sprite fading to avoid bloating this order out and to allow for reuse (and avoid putting update in here!)
            TextFader.FadeText(TMText, _targetColor, _duration, Vector2.zero, delegate
            {
                if (waitUntilFinished)
                {
                    Continue();
                }
            });

            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        public override string GetSummary()
        {
            if (TMText == null && unityText == null)
            {
                return "Error: No text component selected";
            }

            return (TMText != null ? TMText.name : unityText.name) + " to " + _targetColor.ToString();
        }

        #region Backwards compatibility

        [HideInInspector][FormerlySerializedAs("duration")] public float durationOLD;
        [HideInInspector][FormerlySerializedAs("targetColor")] public Color targetColorOLD;

        protected virtual void OnEnable()
        {
            if (durationOLD != default(float))
            {
                _duration = durationOLD;
                durationOLD = default(float);
            }
            if (targetColorOLD != default(Color))
            {
                _targetColor = targetColorOLD;
                targetColorOLD = default(Color);
            }
        }

        #endregion
    }
}
