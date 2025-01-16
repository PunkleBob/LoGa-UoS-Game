using LoGaCulture.LUTE;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Display story text in a visual novel style Dialogue box.
/// </summary>
/// 
public class DialogueBox : MonoBehaviour
{
    [Tooltip("Duration to fade dialogue in/out")]
    [SerializeField] protected float fadeDuration = 0.25f;

    [Tooltip("The continue button UI object")]
    [SerializeField] protected Button continueButton;

    [Tooltip("The text UI object")]
    [SerializeField] protected TextMeshProUGUI textDisplay;
    [Tooltip("TextAdapter will search for appropriate output on this GameObject if storyText is null")]
    [SerializeField] protected GameObject textDisplayGO;
    protected TextAdapter storyTextAdapter = new TextAdapter();
    public virtual string StoryText
    {
        get
        {
            return storyTextAdapter.Text;
        }
        set
        {
            storyTextAdapter.Text = value;
        }
    }

    [Tooltip("The name text UI object")]
    [SerializeField] protected TextMeshProUGUI nameText;
    [Tooltip("TextAdapter will search for appropriate output on this GameObject if nameText is null")]
    [SerializeField] protected GameObject nameTextGO;
    protected TextAdapter nameTextAdapter = new TextAdapter();
    public virtual string NameText
    {
        get
        {
            return nameTextAdapter.Text;
        }
        set
        {
            nameTextAdapter.Text = value;
        }
    }

    [Tooltip("Close any other open dialogue boxes when this one is active")]
    [SerializeField] protected bool closeOtherDialogues;

    [Tooltip("The character UI object")]
    [SerializeField] protected Image characterImage;

    [Tooltip("Adjust width of story text when Character Image is displayed (to avoid overlapping)")]
    [SerializeField] protected bool fitTextWithImage = true;

    protected TextWriter writer;
    protected AudioWriter audioWriter;
    protected CanvasGroup canvasGroup;

    protected bool fadeWhenDone = true;
    protected bool waitForClick = true;
    protected float targetAlpha = 0f;
    protected float fadeCoolDownTimer = 0f;
    protected bool showButton;

    // Cache active boxes to avoid expensive scene search
    protected static List<DialogueBox> activeDialogueBoxes = new List<DialogueBox>();

    protected Sprite currentCharacterImage;

    protected StringSubstituter stringSubstituter = new StringSubstituter();

    protected float startStoryTextWidth;
    protected float startStoryTextInset;

    protected static Character speakingCharacter;

    public virtual Image CharacterImage { get { return characterImage; } }

    public virtual RectTransform StoryTextRectTrans
    {
        get
        {
            return textDisplay != null ? textDisplay.rectTransform : textDisplayGO.GetComponent<RectTransform>();
        }
    }

    protected virtual void Awake()
    {
        if (!activeDialogueBoxes.Contains(this))
        {
            activeDialogueBoxes.Add(this);
        }

        nameTextAdapter.InitFromGameObject(nameText != null ? nameText.gameObject : nameTextGO);
        storyTextAdapter.InitFromGameObject(textDisplay != null ? textDisplay.gameObject : textDisplayGO);
    }

    protected virtual void OnDestroy()
    {
        activeDialogueBoxes.Remove(this);
    }

    protected virtual TextWriter GetWriter()
    {
        if (writer != null)
        {
            return writer;
        }

        writer = GetComponent<TextWriter>();
        if (writer == null)
        {
            writer = gameObject.AddComponent<TextWriter>();
        }

        return writer;
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

    public virtual AudioWriter GetWriterAudio()
    {
        if (audioWriter != null)
        {
            return audioWriter;
        }

        audioWriter = GetComponent<AudioWriter>();
        if (audioWriter == null)
        {
            audioWriter = gameObject.AddComponent<AudioWriter>();
        }
        return audioWriter;
    }

    protected virtual void Start()
    {
        // Dialogue always starts invisible, will be faded in when writing starts
        GetCanvasGroup().alpha = 0f;

        // Add a raycaster if none already exists so we can handle input
        GraphicRaycaster raycaster = GetComponent<GraphicRaycaster>();
        if (raycaster == null)
        {
            gameObject.AddComponent<GraphicRaycaster>();
        }

        if (currentCharacterImage == null)
        {
            // Character image is hidden by default.
            SetCharacterImage(null);
        }

        if (NameText == "")
        {
            SetCharacterName("", Color.white);
        }
        if (currentCharacterImage == null)
        {
            // Character image is hidden by default.
            SetCharacterImage(null);
        }
    }

    protected virtual void LateUpdate()
    {
        UpdateAlpha();

        if (continueButton != null && showButton)
        {
            continueButton.gameObject.SetActive(GetWriter().IsWaitingForInput);
        }
    }

    protected virtual void UpdateAlpha()
    {
        if (GetWriter().IsTyping)
        {
            targetAlpha = 1f;
            fadeCoolDownTimer = 0.1f;
        }
        else if (fadeWhenDone && Mathf.Approximately(fadeCoolDownTimer, 0f))
        {
            targetAlpha = 0f;
        }
        else
        {
            // Add a short delay before we start fading in case there's another text order in the next frame or two
            // This avoids a noticeable flicker between consecutive text orders
            fadeCoolDownTimer = Mathf.Max(0f, fadeCoolDownTimer - Time.deltaTime);
        }

        CanvasGroup canvasGroup = GetCanvasGroup();
        if (fadeDuration <= 0f)
        {
            canvasGroup.alpha = targetAlpha;
        }
        else
        {
            float delta = (1f / fadeDuration) * Time.deltaTime;
            float alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, delta);
            canvasGroup.alpha = alpha;

            if (alpha <= 0f)
            {
                // Deactivate object once invisible
                gameObject.SetActive(false);
            }
        }
    }

    protected virtual void ClearStoryText()
    {
        StoryText = "";
    }

    /// Current active Dialogue used to display Say text
    public static DialogueBox ActiveDialogueBox { get; set; }

    public Character SpeakingCharacter { get { return speakingCharacter; } }

    public static DialogueBox GetDialogueBox()
    {
        if (ActiveDialogueBox == null)
        {
            DialogueBox dialogueBox = null;
            if (activeDialogueBoxes.Count > 0)
            {
                dialogueBox = activeDialogueBoxes[0];
            }
            if (dialogueBox != null)
            {
                ActiveDialogueBox = dialogueBox;
            }
            if (ActiveDialogueBox == null)
            {
                // Create a new dialogue box
                GameObject prefab = Resources.Load<GameObject>("Prefabs/DialogueBox");
                if (prefab != null)
                {
                    GameObject go = Instantiate(prefab) as GameObject;
                    go.SetActive(false);
                    go.name = "DialogueBox";
                    ActiveDialogueBox = go.GetComponent<DialogueBox>(); ;
                }
            }
        }
        return ActiveDialogueBox;
    }

    /// <summary>
    /// Stops all active portrait tweens.
    /// </summary>
    /// TO DO: implement and add portrait controller
    //public static void StopPortraitTweens()
    //{
    //    // Stop all tweening portraits
    //    var activeCharacters = Character.ActiveCharacters;
    //    for (int i = 0; i < activeCharacters.Count; i++)
    //    {
    //        var c = activeCharacters[i];
    //        if (c.State.portraitImage != null)
    //        {
    //            if (LeanTween.isTweening(c.State.portraitImage.gameObject))
    //            {
    //                LeanTween.cancel(c.State.portraitImage.gameObject, true);
    //                //PortraitController.SetRectTransform(c.State.portraitImage.rectTransform, c.State.position);
    //                if (c.State.dimmed == true)
    //                {
    //                    c.State.portraitImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
    //                }
    //                else
    //                {
    //                    c.State.portraitImage.color = Color.white;
    //                }
    //            }
    //        }
    //    }
    //}

    public virtual void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }

    // TO DO: set stage
    public virtual void SetCharacter(Character character)
    {
        if (character == null)
        {
            if (characterImage != null)
            {
                characterImage.gameObject.SetActive(false);
            }
            if (nameText.text != null)
            {
                nameText.text = "";
            }
            speakingCharacter = null;
        }
        else
        {
            string characterName = character.CharacterName;

            if (characterName == "")
            {
                // Use game object name as default
                characterName = character.GetObjectName();
            }

            SetCharacterName(characterName, character.NameColour);
        }
    }

    public virtual void SetCharacterImage(Sprite sprite)
    {
        if (characterImage == null)
            return;

        if (sprite != null)
        {
            characterImage.overrideSprite = sprite;
            characterImage.gameObject.SetActive(true);
            currentCharacterImage = sprite;
        }
        else
        {
            characterImage.gameObject.SetActive(false);

            if (startStoryTextWidth != 0)
            {
                StoryTextRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, startStoryTextInset, startStoryTextWidth);
            }
        }

        if (fitTextWithImage && StoryText != null && characterImage.gameObject.activeSelf)
        {
            if (Mathf.Approximately(startStoryTextWidth, 0f))
            {
                startStoryTextWidth = StoryTextRectTrans.rect.width;
                startStoryTextInset = StoryTextRectTrans.offsetMin.x;
            }

            if (StoryTextRectTrans.position.x < characterImage.rectTransform.position.x)
            {
                StoryTextRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,
                startStoryTextInset,
                startStoryTextWidth - characterImage.rectTransform.rect.width);
            }
            else
            {
                StoryTextRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right,
                startStoryTextInset,
                startStoryTextWidth - characterImage.rectTransform.rect.width);
            }
        }
    }

    public virtual void SetCharacterName(string name, Color colour)
    {
        if (NameText != null)
        {
            var subbedName = stringSubstituter.SubstituteStrings(name);
            NameText = subbedName;
            nameTextAdapter.SetTextColour(colour);
        }
    }

    /// <summary>
    /// Write a line of story text to the Say Dialog. Starts coroutine automatically.
    /// </summary>
    /// <param name="text">The text to display.</param>
    /// <param name="clearPrevious">Clear any previous text in the Say Dialog.</param>
    /// <param name="waitForInput">Wait for player input before continuing once text is written.</param>
    /// <param name="fadeWhenDone">Fade out the Say Dialog when writing and player input has finished.</param>
    /// <param name="stopVoiceover">Stop any existing voiceover audio before writing starts.</param>
    /// <param name="voiceOverClip">Voice over audio clip to play.</param>
    /// <param name="onComplete">Callback to execute when writing and player input have finished.</param>
    public virtual void StartDialogue(string text, bool clearPrevious, bool waitForInput, bool fadeWhenDone, bool stopVoiceover, bool waitForVO, AudioClip voiceOverClip, Action onComplete, float newSpeed = 40, bool showButton = true, Node parentNode = null)
    {
        this.showButton = showButton;
        StartCoroutine(DoDialogue(text, clearPrevious, waitForInput, fadeWhenDone, stopVoiceover, waitForVO, voiceOverClip, onComplete, newSpeed, parentNode));
    }

    /// <summary>
    /// Write a line of story text to the Dialogue box. Must be started as a coroutine.
    /// </summary>
    /// <param name="text">The text to display.</param>
    /// <param name="clearPrevious">Clear any previous text in the Say Dialog.</param>
    /// <param name="waitForInput">Wait for player input before continuing once text is written.</param>
    /// <param name="fadeWhenDone">Fade out the Say Dialog when writing and player input has finished.</param>
    /// <param name="stopVoiceover">Stop any existing voiceover audio before writing starts.</param>
    /// <param name="voiceOverClip">Voice over audio clip to play.</param>
    /// <param name="onComplete">Callback to execute when writing and player input have finished.</param>
    public virtual IEnumerator DoDialogue(string text, bool clearPrevious, bool waitForInput, bool fadeWhenDone, bool stopVoiceover, bool waitForVO, AudioClip voiceOverClip, Action onComplete, float newSpeed = 40, Node parentNode = null)
    {
        var writer = GetWriter();

        if (writer.IsTyping || writer.IsWaitingForInput)
        {
            writer.Stop();
            while (writer.IsTyping || writer.IsWaitingForInput)
            {
                yield return null;
            }
        }

        if (closeOtherDialogues)
        {
            for (int i = 0; i < activeDialogueBoxes.Count; i++)
            {
                var db = activeDialogueBoxes[i];
                if (db.gameObject != gameObject)
                {
                    db.SetActive(false);
                }
            }
        }
        gameObject.SetActive(true);

        this.fadeWhenDone = fadeWhenDone;

        AudioClip SFXClip = null;
        if (voiceOverClip != null)
        {
            AudioWriter wa = GetWriterAudio();
            wa.OnVoiceover(voiceOverClip);
        }
        else if (speakingCharacter != null)
        {
            SFXClip = speakingCharacter.SoundEffect;
        }

        writer.AttachedAudioWriter = audioWriter;

        yield return StartCoroutine(writer.Write(text, clearPrevious, waitForInput, stopVoiceover, waitForVO, SFXClip, onComplete, newSpeed, parentNode));
    }

    public virtual bool FadeWhenDone { get { return fadeWhenDone; } set { fadeWhenDone = value; } }

    /// Stop the dialogue while its writing text
    public virtual void Stop()
    {
        fadeWhenDone = true;
        GetWriter().Stop();
    }

    /// Stops writing text and clears text
    public virtual void Clear()
    {
        ClearStoryText();

        StopAllCoroutines();
    }
}
