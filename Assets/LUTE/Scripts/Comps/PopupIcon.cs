using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopupIcon : MonoBehaviour
{
    public static PopupIcon ActivePopupIcon { get; set; }

    private Popup popupWindow;
    private Button[] cachedButtons;
    private int nextOptionIndex;

    public virtual Button[] CachedButtons { get { return cachedButtons; } }

    private void Awake()
    {
        Button[] optionButtons = GetComponentsInChildren<Button>();
        cachedButtons = optionButtons;

        foreach (Button button in cachedButtons)
        {
            button.gameObject.SetActive(false);
        }

        if (cachedButtons.Length <= 0)
        {
            Debug.LogError("PopupIcon requires a Button component on a child object");
            return;
        }

        CheckEventSystem();
    }

    protected virtual void CheckEventSystem()
    {
        EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            // Auto spawn an Event System from the prefab - ensure you have one in a Resources folder
            GameObject prefab = Resources.Load<GameObject>("Prefabs/EventSystem");
            if (prefab != null)
            {
                GameObject go = Instantiate(prefab) as GameObject;
                go.name = "EventSystem";
            }
        }
    }

    public static PopupIcon GetPopupIcon()
    {
        if (ActivePopupIcon == null)
        {
            var pi = GameObject.FindObjectOfType<PopupIcon>();
            if (pi != null)
            {
                ActivePopupIcon = pi;
            }

            if (ActivePopupIcon == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/HamburgerMenuButton");
                if (prefab != null)
                {
                    GameObject go = Instantiate(prefab) as GameObject;
                    go.SetActive(false);
                    go.name = "PopupIcon";
                    ActivePopupIcon = go.GetComponent<PopupIcon>();
                }
            }
        }
        return ActivePopupIcon;
    }

    public bool SetIcon(Sprite icon)
    {
        if (nextOptionIndex >= CachedButtons.Length)
        {
            Debug.LogWarning("Unable to add popup option, not enough buttons!");
            return false;
        }
        ActivePopupIcon.CachedButtons[nextOptionIndex].image.sprite = icon;
        return true;
    }

    public void SetPopupWindow(Popup popupWindow)
    {
        this.popupWindow = popupWindow;
    }

    private void OnClick()
    {
        if (popupWindow != null)
        {
            popupWindow.OpenClose();
        }
    }

    public bool SetAction(UnityAction onClick)
    {
        if (nextOptionIndex >= CachedButtons.Length)
        {
            Debug.LogWarning("Unable to add popup option, not enough buttons!");
            return false;
        }
        ActivePopupIcon.CachedButtons[nextOptionIndex].onClick.AddListener(() => { onClick.Invoke(); });
        ActivePopupIcon.CachedButtons[nextOptionIndex].gameObject.SetActive(true);
        return true;
    }

    public void MoveToNextOption()
    {
        nextOptionIndex++;
    }

    public virtual void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }
}
