using TMPro;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    // Helper class that displays text information using canvas objects to help fade in and out
    public class TextInfoPanel : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] protected TextMeshProUGUI titleText;
        [SerializeField] protected TextMeshProUGUI bodyText;
        [SerializeField] protected UnityEngine.UI.Button defaultButton;
        [SerializeField] protected CanvasGroup panelGroup; // For fading in and out

        protected static bool infoPanelActive = false;

        public static TextInfoPanel ActiveInfoPanel;

        public static TextInfoPanel GetInfoPanel()
        {
            if (ActiveInfoPanel == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/DefaultTextInfoPanel");
                if (prefab != null)
                {
                    GameObject go = Instantiate(prefab);
                    go.SetActive(false);
                    go.name = "DefaultTextInfoPanel";
                    ActiveInfoPanel = go.GetComponent<TextInfoPanel>();
                }
            }
            return ActiveInfoPanel;
        }
        protected virtual void Awake()
        {
            ActiveInfoPanel = this;
        }

        protected virtual void OnDestroy()
        {
            ActiveInfoPanel = null;
        }

        protected virtual void Start()
        {
            if (!infoPanelActive && panelGroup != null)
            {
                panelGroup.alpha = 0.0f;
                panelGroup.interactable = false;
                panelGroup.blocksRaycasts = false;
            }
        }

        public virtual void SetActive(bool state) => gameObject.SetActive(state);

        public virtual void SetInfo(string text, UnityEngine.Events.UnityEvent onPress, TextAlignmentOptions alignment = TextAlignmentOptions.Left)
        {
            if (text == null)
            {
                return;
            }

            if (bodyText == null || defaultButton == null)
            {
                return;
            }

            bodyText.text = text;
            bodyText.alignment = alignment;

            if (onPress != null)
            {
                defaultButton.onClick.RemoveAllListeners();
                defaultButton.onClick.AddListener(() => onPress.Invoke());
            }
        }

        protected LTDescr fadeTween;

        public virtual void ToggleMenu()
        {
            if (fadeTween != null)
            {
                LeanTween.cancel(fadeTween.id, true);
                fadeTween = null;
            }

            if (infoPanelActive)
            {
                //Fade menu out
                LeanTween.value(panelGroup.gameObject, panelGroup.alpha, 0f, 0.4f)
        .setEase(LeanTweenType.easeOutQuint)
        .setOnUpdate((t) =>
        {
            panelGroup.alpha = t;
        }).setOnComplete(() =>
        {
            panelGroup.alpha = 0f;
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
            SetActive(false);
        });
            }
            else
            {
                SetActive(true);
                //Fade menu in
                LeanTween.value(panelGroup.gameObject, panelGroup.alpha, 1f, 0.4f)
        .setEase(LeanTweenType.easeOutQuint)
        .setOnUpdate((t) =>
        {
            panelGroup.alpha = t;
        }).setOnComplete(() =>
        {
            panelGroup.alpha = 1f;
            panelGroup.interactable = true;
            panelGroup.blocksRaycasts = true;
        });
            }
            infoPanelActive = !infoPanelActive;
        }

    }
}
