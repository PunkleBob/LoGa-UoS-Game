using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    [SerializeField] protected bool autoSelectFirstButton = false;
    [SerializeField] protected TextMeshProUGUI textDisplay; //this should be on a new button class

    protected Button[] cachedButtons;
    protected Slider[] cachedOptionSliders; //ensure that any of these are never equal to the timer slider
    protected TextMeshProUGUI[] cachedTextDisplays;
    protected Toggle[] cachedOptionToggles;
    protected StateManager stateManager;


    public static Popup ActivePopupWindow { get; set; }

    private List<Order> menuOrders = new List<Order>();
    private int nextOptionIndex;

    public virtual Button[] CachedButtons { get { return cachedButtons; } }
    /// A cached slider object used for the timer in the menu
    public virtual Slider[] CachedOptionSliders { get { return cachedOptionSliders; } }
    public virtual TextMeshProUGUI[] CachedTextDisplays { get { return cachedTextDisplays; } }
    public virtual Toggle[] CachedOptionToggles { get { return cachedOptionToggles; } }

    protected virtual void Awake()
    {
        stateManager = LogaManager.Instance.StateManager;

        // Get all text displays where the parent is not a button or slider 
        cachedTextDisplays = GetComponentsInChildren<TextMeshProUGUI>().Where(textDisplay => textDisplay.GetComponentInParent<Button>() == null
        && textDisplay.GetComponentInParent<Slider>() == null).ToArray();

        // Get all sliders except for the timeout slider
        cachedOptionSliders = GetComponentsInChildren<Slider>();

        cachedButtons = GetComponentsInChildren<Button>();

        Toggle[] optionToggles = GetComponentsInChildren<Toggle>();
        cachedOptionToggles = optionToggles;

        if (Application.isPlaying)
        {
            // Don't auto disable buttons in the editor
            Clear();
        }

        CheckEventSystem();
    }


    // There must be an Event System in the scene for menu systems to work
    // This method will automatically instantiate one if none exists
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

    protected virtual TextMeshProUGUI GetTextDisplay(Transform buttonParent)
    {
        textDisplay = null;
        textDisplay = buttonParent.GetComponentInChildren<TextMeshProUGUI>();
        if (textDisplay == null)
        {
            Debug.LogWarning("No TextMeshProUGUI found in children of " + buttonParent.name);
            return null;
        }
        return textDisplay;
    }

    public static Popup GetPopupWindow()
    {
        if (ActivePopupWindow == null)
        {
            var pw = GameObject.FindObjectOfType<Popup>();
            if (pw != null)
            {
                ActivePopupWindow = pw;
            }

            if (ActivePopupWindow == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/PopupMenu");
                if (prefab != null)
                {
                    GameObject go = Instantiate(prefab) as GameObject;
                    go.SetActive(false);
                    go.name = "PopupWindow";
                    ActivePopupWindow = go.GetComponent<Popup>();
                }
            }
        }
        return ActivePopupWindow;
    }

    public virtual void OpenClose()
    {
        if (IsOpen)
        {
            stateManager.ChangeState(StateManager.State.Game);
            gameObject.SetActive(false);
        }
        else
        {
            stateManager.ChangeState(StateManager.State.Pause);
            gameObject.SetActive(true);
        }
    }

    public virtual void SetOrders(List<Order> orders)
    {
        menuOrders = orders;
    }

    //iterates through the orders and creates the menu items
    public virtual void CreateMenuGUI()
    {
        if (menuOrders == null || menuOrders.Count == 0)
        {
            Debug.LogWarning("No orders found to create menu items");
            return;
        }

        // Filter orders based on certain types (should be exposed in the inspector)
        List<Order> filteredOrders = menuOrders.FindAll(order => order is MenuText || order is Choice || order is OptionSlider || order is BooleanOptionCustom || order is MenuChoice);

        // Create menu items for the filtered orders as only these can be displayed in the menu (this should be exposed in the inspector)
        foreach (Order order in filteredOrders)
        {
            switch (order)
            {
                case MenuText menuText:
                    menuText.SetMenuText(this);
                    break;
                case Choice choice:
                    choice.SetMenuChoice(this);
                    break;
                case MenuChoice menuChoice:
                    menuChoice.SetMenuChoice(this);
                    break;
                case OptionSlider optionSlider:
                    optionSlider.SetSliderOptions(this);
                    break;
                case BooleanOptionCustom booleanOptionCustom:
                    booleanOptionCustom.SetMenuChoice(this);
                    break;
            }
        }
    }

    /// Adds the option to the list of displayed options. Calls a Node when selected
    /// Will cause the Menu to become visible if it is not already visible
    /// <returns><c>true</c>, if the option was added successfully.</returns>
    public virtual bool AddOption(string text, bool interactable, bool hideOption, Node targetNode, bool closeMenuOnSelect, MMFeedbacks feedback = null, AudioClip buttonSound = null, bool saveSettings = false)
    {
        var node = targetNode;
        UnityEngine.Events.UnityAction action = delegate
        {
            EventSystem.current.SetSelectedGameObject(null);
            StopAllCoroutines();
            // Stop timeout
            // Clear();
            if (node != null)
            {
                var engine = node.GetEngine();
                // Use a coroutine to call the node on the next frame
                // Have to use th engine gameobject as this menu is now inactive
                engine.StartCoroutine(CallNode(node));
                if (closeMenuOnSelect)
                {
                    OpenClose();
                }
            }
            if (buttonSound != null)
            {
                LogaManager.Instance.SoundManager.PlaySound(buttonSound, -1);
            }
            feedback?.PlayFeedbacks();

            if (saveSettings)
            {
                string saveDesc = System.DateTime.UtcNow.ToString("HH:mm dd MMMM, yyyy");

                var saveManager = LogaManager.Instance.SaveManager;
                saveManager.AddSavePoint("OptionSettingsSave", saveDesc, false);
            }
        };

        return AddOption(text, interactable, hideOption, action);
    }

    private bool AddOption(string text, bool interactable, bool hideOption, UnityEngine.Events.UnityAction action)
    {
        if (nextOptionIndex >= CachedButtons.Length)
        {
            Debug.LogWarning("Unable to add menu item, not enough buttons!");
            return false;
        }

        var button = cachedButtons[nextOptionIndex];
        button.transform.SetSiblingIndex(nextOptionIndex);

        if (textDisplay == null)
        {
            var textDisplay = GetTextDisplay(button.transform);
        }
        textDisplay.text = text;
        textDisplay = null;

        //move forward for next call
        nextOptionIndex++;

        //don't need to set anything on it
        if (hideOption)
            return true;

        button.gameObject.SetActive(true);
        button.interactable = interactable;
        if (interactable && autoSelectFirstButton && !cachedButtons.Select(x => x.gameObject).Contains(EventSystem.current.currentSelectedGameObject))
        {
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        }

        //could add some funky text stuff here (such as animations or whatever)

        button.onClick.AddListener(action);

        return true;
    }

    /// Adds the option slider to the list of displayed options. sets a specific value related to the slider
    /// Will cause the Menu to become visible if it is not already visible
    public virtual bool AddOptionSlider(bool interactable, float targetFloat, bool hideOption, UnityEngine.Events.UnityAction<float> action, string sliderLabel = null)
    {
        if (nextOptionIndex >= CachedOptionSliders.Length)
        {
            Debug.LogWarning("Unable to add menu item, not enough sliders!");
            return false;
        }

        var slider = cachedOptionSliders[nextOptionIndex];
        slider.transform.SetSiblingIndex(nextOptionIndex);

        if (textDisplay == null)
        {
            var textDisplay = GetTextDisplay(slider.transform);
        }
        textDisplay.text = sliderLabel;
        textDisplay = null;
        nextOptionIndex++;

        if (hideOption)
            return true;

        slider.gameObject.SetActive(true);
        slider.interactable = interactable;

        if (interactable && autoSelectFirstButton && !cachedOptionSliders.Select(x => x.gameObject).Contains(EventSystem.current.currentSelectedGameObject))
        {
            EventSystem.current.SetSelectedGameObject(slider.gameObject);
        }

        // First set the slider value to the target value (if the target value is not default)
        slider.value = targetFloat;
        // If slider value changes then set the target value to the slider value
        slider.onValueChanged.AddListener((value) =>
        {
            action.Invoke(value);
        });

        return true;
    }

    public virtual bool AddMenuText(string text, bool hideOption)
    {
        if (nextOptionIndex >= CachedTextDisplays.Length)
        {
            Debug.LogWarning("Unable to add menu item, not enough text displays!");
            return false;
        }

        var textDisplay = cachedTextDisplays[nextOptionIndex];
        textDisplay.text = text;
        textDisplay.transform.SetSiblingIndex(nextOptionIndex);
        nextOptionIndex++;


        if (hideOption)
            return true;

        textDisplay.gameObject.SetActive(true);

        return true;
    }

    /// Adds the option toggle to the list of displayed options. sets a specific boolean related to the toggle
    /// Will cause the Menu to become visible if it is not already visible
    public virtual bool AddOptionToggle(bool interactable, bool targetBoolean, bool hideOption, UnityEngine.Events.UnityAction<bool> action, string toggleLabel = null)
    {
        if (nextOptionIndex >= CachedOptionToggles.Length)
        {
            Debug.LogWarning("Unable to add menu item, not enough toggles!");
            return false;
        }

        var toggle = cachedOptionToggles[nextOptionIndex];
        var toggleTextDisplay = toggle.GetComponentInChildren<Text>();
        if (toggleTextDisplay != null)
        {
            if (toggleLabel != null)
            {
                toggleTextDisplay.text = toggleLabel;
            }
            else
                toggleTextDisplay.text = targetBoolean.ToString();
        }
        toggleTextDisplay = null;
        nextOptionIndex++;

        if (hideOption)
            return true;

        toggle.gameObject.SetActive(true);
        toggle.interactable = interactable;

        if (interactable && autoSelectFirstButton && !cachedOptionToggles.Select(x => x.gameObject).Contains(EventSystem.current.currentSelectedGameObject))
        {
            EventSystem.current.SetSelectedGameObject(toggle.gameObject);
        }

        // First set the slider value to the target value (if the target value is not default)
        toggle.isOn = targetBoolean;
        // If slider value changes then set the target value to the slider value
        toggle.onValueChanged.AddListener((value) =>
        {
            action.Invoke(value);
        });

        return true;
    }

    protected IEnumerator CallNode(Node node)
    {
        yield return new WaitForEndOfFrame();
        node.StartExecution();
    }

    //clear all menu options
    public virtual void Clear()
    {
        StopAllCoroutines();

        nextOptionIndex = 0;

        var optionButtons = CachedButtons;
        for (int i = 0; i < optionButtons.Length; i++)
        {
            var button = optionButtons[i];
            button.onClick.RemoveAllListeners();
        }

        for (int i = 0; i < optionButtons.Length; i++)
        {
            var button = optionButtons[i];
            if (button != null)
            {
                button.transform.SetSiblingIndex(i);
                button.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < CachedOptionSliders.Length; i++)
        {
            CachedOptionSliders[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < CachedOptionToggles.Length; i++)
        {
            CachedOptionToggles[i].gameObject.SetActive(false);
        }

        if (CachedTextDisplays != null && CachedTextDisplays.Length > 0)
            for (int i = 0; i < CachedTextDisplays.Length; i++)
            {
                CachedTextDisplays[i].gameObject.SetActive(false);
            }
    }

    public virtual bool IsOpen { get { return gameObject.activeSelf; } }
}
