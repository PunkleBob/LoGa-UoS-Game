using LoGaCulture.LUTE;
using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// Presents multiple choice buttons to the players - similar to a dialogue box but with buttons instead of text
public class MenuDialogue : MonoBehaviour
{
    [SerializeField] protected bool autoSelectFirstButton = false;
    [SerializeField] protected TextMeshProUGUI textDisplay; //this needs to be on a new button class
    [SerializeField] protected CanvasGroup canvasGroup;

    protected Button[] cachedButtons;
    protected Slider cachedSlider;
    protected Slider[] cachedOptionSliders; //ensure that any of these are never equal to the timer slider
    protected Toggle[] cachedOptionToggles;

    private int nextOptionIndex;

    /// Currently active Menu  used to display Menu options
    public static MenuDialogue ActiveMenuDialogue { get; set; }
    /// A cached list of button objects in the menu
    public virtual Button[] CachedButtons { get { return cachedButtons; } }
    /// A cached slider object used for the timer in the menu
    public virtual Slider CachedSlider { get { return cachedSlider; } }
    public virtual Slider[] CachedOptionSliders { get { return cachedOptionSliders; } }
    public virtual Toggle[] CachedOptionToggles { get { return cachedOptionToggles; } }

    private static bool usingSetDialogue = false;

    /// Sets the active state of the Menu gameobject.
    public virtual void SetActive(bool state)
    {
        gameObject.SetActive(state);
        if (state == true)
            GetCanvasGroup().alpha = 1;
    }

    //searches scene for menu then creates one if none found
    public static MenuDialogue GetMenuDialogue()
    {
        if (ActiveMenuDialogue == null)
        {
            // Use first Menu Dialog found in the scene (if any)
            var md = GameObject.FindObjectOfType<MenuDialogue>();
            if (md != null)
            {
                ActiveMenuDialogue = md;
            }

            if (ActiveMenuDialogue == null)
            {
                // Auto spawn a menu object from the prefab (ensure you have one in a Resources folder)
                GameObject prefab = Resources.Load<GameObject>("Prefabs/MenuDialogue");
                if (prefab != null)
                {
                    GameObject go = Instantiate(prefab) as GameObject;
                    go.SetActive(false);
                    go.name = "MenuDialogue";
                    ActiveMenuDialogue = go.GetComponent<MenuDialogue>();
                }
            }
        }

        return ActiveMenuDialogue;
    }

    public static void SetMenuDialogue(MenuDialogue menuDialogue)
    {
        ActiveMenuDialogue = menuDialogue;
        usingSetDialogue = true;
    }

    protected virtual void Awake()
    {
        Button[] optionButtons = GetComponentsInChildren<Button>();
        cachedButtons = optionButtons;

        Slider timeoutSlider = GetComponentInChildren<Slider>(); //ensure that your timeout slider is top of slider pile in inspector
        cachedSlider = timeoutSlider;

        // Get all sliders except for the timeout slider
        cachedOptionSliders = GetComponentsInChildren<Slider>().Where(slider => slider != timeoutSlider).ToArray();

        Toggle[] optionToggles = GetComponentsInChildren<Toggle>();
        cachedOptionToggles = optionToggles;

        if (Application.isPlaying)
        {
            // Don't auto disable buttons in the editor
            Clear();
        }
        GetCanvasGroup().alpha = 0;

        CheckEventSystem();
    }

    // There must be an Event System in the scene for dialogues systems to work
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
    protected virtual void OnEnable()
    {
        // The canvas sometimes fails to update if the menu  is enabled in the first game frame
        // thus we need to force the canvas update when the object is enabled (silly unity)
        Canvas.ForceUpdateCanvases();
    }

    protected virtual CanvasGroup GetCanvasGroup()
    {
        if (canvasGroup != null)
        {
            return canvasGroup;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        return canvasGroup;
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

    protected virtual IEnumerator WaitForTimeout(float timeoutDuration, Node node)
    {
        float elapsedTime = 0;

        Slider timeoutSlider = CachedSlider;

        while (elapsedTime < timeoutDuration)
        {
            if (timeoutSlider != null)
            {
                float t = 1f - elapsedTime / timeoutDuration;
                timeoutSlider.value = t;
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        Clear();
        gameObject.SetActive(false);

        HideDialogue();

        if (node != null)
        {
            node.StartExecution();
        }
    }

    protected IEnumerator CallNode(Node node)
    {
        yield return new WaitForEndOfFrame();
        node.StartExecution();
    }

    //clear all menu options
    public virtual void Clear(bool closeMenu = true)
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

        Slider timeoutSlider = CachedSlider;
        if (timeoutSlider != null)
        {
            timeoutSlider.gameObject.SetActive(false);
        }

        for (int i = 0; i < CachedOptionSliders.Length; i++)
        {
            CachedOptionSliders[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < CachedOptionToggles.Length; i++)
        {
            CachedOptionToggles[i].gameObject.SetActive(false);
        }

        canvasGroup.alpha = 0;

        if (usingSetDialogue && closeMenu)
        {
            usingSetDialogue = false;

            ActiveMenuDialogue = null;
            SetActive(false);
        }
    }

    public virtual void HideDialogue()
    {
        var dialogueBox = DialogueBox.GetDialogueBox();
        if (dialogueBox != null)
        {
            dialogueBox.FadeWhenDone = true;
        }
    }

    /// Adds the option to the list of displayed options. Calls a Node when selected
    /// Will cause the Menu to become visible if it is not already visible
    /// <returns><c>true</c>, if the option was added successfully.</returns>
    public virtual bool AddOption(string text, bool interactable, bool hideOption, Node targetNode, bool hideMenu, MMFeedbacks feedback = null, bool justContinue = false, UnityAction continueAction = null, bool showNextChoice = false, int orderIndex = -1, Node parentNode = null, AudioClip buttonSound = null, bool saveSettings = false)
    {
        var node = targetNode;
        UnityEngine.Events.UnityAction action = delegate
        {
            EventSystem.current.SetSelectedGameObject(null);
            StopAllCoroutines();
            // Stop timeout
            if (hideMenu)
            {
                Clear(hideMenu);
                HideDialogue();
            }
            else
            {
                var dialogueBox = DialogueBox.GetDialogueBox();
                if (dialogueBox != null)
                {
                    dialogueBox.FadeWhenDone = false;
                }
            }

            if (justContinue)
            {
                if (parentNode != null && orderIndex >= 0)
                {
                    // We must use the order index provided to execute the order in the list after this choice
                    var nextOrder = parentNode.OrderList[orderIndex + 1];
                    if (nextOrder != null)
                    {
                        nextOrder.Execute();
                        parentNode.JumpToOrderIndex = orderIndex + 1;
                    }
                }
            }

            if (!justContinue)
            {
                if (node != null)
                {
                    var engine = node.GetEngine();
                    if (hideMenu)
                        GetCanvasGroup().alpha = 0;
                    //gameObject.SetActive(false);
                    // Use a coroutine to call the node on the next frame
                    // Have to use the engine gameobject as this menu is now inactive
                    engine.StartCoroutine(CallNode(node));
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

        TextAdapter textAdapter = new TextAdapter();
        textAdapter.InitFromGameObject(button.gameObject, true);
        if (textAdapter.HasTextObject())
        {
            text = TextVariationHandler.SelectVariations(text);

            textAdapter.Text = text;
        }

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

    /// Show a timer during which the player can select an option. Calls a Node when the time runs out
    public virtual void ShowTimer(float duration, Node targetNode)
    {
        if (cachedSlider != null)
        {
            cachedSlider.gameObject.SetActive(true);
            gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(WaitForTimeout(duration, targetNode));
        }
        else
        {
            Debug.LogWarning("Unable to show timer, no slider set!");
        }
    }

    //saves space and makes it easier to read
    public virtual bool IsActive()
    {
        return gameObject.activeInHierarchy;
    }

    //returns the number of options currently displayed
    public virtual int DisplayedOptionsCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < cachedButtons.Length; i++)
            {
                var button = cachedButtons[i];
                if (button.gameObject.activeSelf)
                {
                    count++;
                }
            }
            return count;
        }
    }

    // Shuffle the parent order of the cached buttons - allows for randomising button order; buttons are auto reordered when cleared
    public void Shuffle(System.Random r)
    {
        for (int i = 0; i < CachedButtons.Length; i++)
        {
            CachedButtons[i].transform.SetSiblingIndex(r.Next(CachedButtons.Length));
        }
    }
}