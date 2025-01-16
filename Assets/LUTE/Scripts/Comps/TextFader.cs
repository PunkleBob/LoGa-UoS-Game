using TMPro;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    [RequireComponent(typeof(TMPro.TextMeshProUGUI))]
    public class TextFader : MonoBehaviour
    {
        protected float fadeDuration;
        protected float fadeTimer;
        protected Color startColor;
        protected Color endColor;
        protected Vector2 slideOffset;
        protected Vector3 endPosition;

        protected TMPro.TextMeshProUGUI tmText;
        protected System.Action onFadeComplete;

        protected virtual void Start()
        {
            tmText = GetComponent<TMPro.TextMeshProUGUI>();
        }

        protected virtual void Update()
        {
            fadeTimer += Time.deltaTime;
            if (fadeTimer > fadeDuration)
            {
                // Snap to final values
                tmText.color = endColor;
                if (slideOffset.magnitude > 0)
                {
                    transform.position = endPosition;
                }

                // Remove this component when transition is complete
                Destroy(this);

                if (onFadeComplete != null)
                {
                    onFadeComplete();
                }
            }
            else
            {
                float t = Mathf.SmoothStep(0, 1, fadeTimer / fadeDuration);
                tmText.color = Color.Lerp(startColor, endColor, t);
                if (slideOffset.magnitude > 0)
                {
                    Vector3 startPosition = endPosition;
                    startPosition.x += slideOffset.x;
                    startPosition.y += slideOffset.y;
                    transform.position = Vector3.Lerp(startPosition, endPosition, t);
                }
            }
        }

        public static void FadeText(TextMeshProUGUI tmText, Color targetColor, float duration, Vector2 slideOffset, System.Action onComplete = null)
        {
            if (tmText == null)
            {
                return;
            }

            // Fade child tm renderers
            if (tmText != null)
            {
                var tmTexts = tmText.gameObject.GetComponentsInChildren<TextMeshProUGUI>();
                for (int i = 0; i < tmTexts.Length; i++)
                {
                    var tm = tmTexts[i];
                    if (tm == tmText)
                    {
                        continue;
                    }
                    FadeText(tm, targetColor, duration, slideOffset);
                }
            }

            // Destroy any existing fader component
            TextFader oldTextFader = null;

            if (tmText != null)
            {
                oldTextFader = tmText.GetComponent<TextFader>();
            }
            if (oldTextFader != null)
            {
                Destroy(oldTextFader);
            }

            // Early out if duration is zero
            if (Mathf.Approximately(duration, 0f))
            {
                if (tmText != null)
                {
                    tmText.color = targetColor;
                }
                if (onComplete != null)
                {
                    onComplete();
                }
                return;
            }

            // Set up color transition to be applied during update
            TextFader textFader = tmText.gameObject.AddComponent<TextFader>();

            textFader.fadeDuration = duration;
            textFader.startColor = tmText.color;
            textFader.endColor = targetColor;
            textFader.endPosition = tmText.transform.position;
            textFader.slideOffset = slideOffset;
            textFader.onFadeComplete = onComplete;
        }
    }
}
