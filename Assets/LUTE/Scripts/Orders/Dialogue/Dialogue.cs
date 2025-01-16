using LoGaCulture.LUTE;
using UnityEngine;

[OrderInfo("Narrative",
             "Dialogue",
             "Writes text into a Dialogue Box - box can be customised.")]
[AddComponentMenu("")]
public class Dialogue : Order
{
    [Tooltip("Character that is speaking")]
    [SerializeField] protected Character character;
    [Tooltip("Portrait that represents speaking character")]
    [SerializeField] protected Sprite characterPortrait;
    [Tooltip("Character name to display in the box")]
    [SerializeField] protected string characterName;
    [Tooltip("Colour of the character name text ")]
    [SerializeField] protected Color32 characterNameColour;
    [TextArea(5, 10)]
    [SerializeField] protected string storyText = "";
    [Tooltip("Type the text at this speed")]
    [SerializeField] protected float typingSpeed = 40f;
    [Tooltip("Type this text in the previous Dialogue box")]
    [SerializeField] protected bool extendPrevious = false;
    [Tooltip("Voiceover audio to play when writing the text")]
    [SerializeField] protected AudioClip voiceOverClip;
    [Tooltip("Always show this text when the order is executed multiple times")]
    [SerializeField] protected bool showAlways = true;
    [Tooltip("Number of times to show this text when the order is executed multiple times")]
    [SerializeField] protected int showCount = 1;
    [Tooltip("Fade out the box when writing has finished and not waiting for input")]
    [SerializeField] protected bool fadeWhenDone = false;
    [Tooltip("Allow players to skip to end of text")]
    [SerializeField] protected bool allowLineSkip = true;
    [Tooltip("Players can progress on click rather than automatically showing")]
    [SerializeField] protected bool waitForClick = true;
    [Tooltip("How long the next text will wait before showing")]
    [SerializeField] protected float timeToWait = 1f;
    [Tooltip("Stop playing voiceover when text finishes writing")]
    [SerializeField] protected bool stopVoiceover = true;
    [Tooltip("Wait for the Voice Over to complete before continuing")]
    [SerializeField] protected bool waitForVO = false;
    [Tooltip("Sets the active dialogue box with a reference to a box object in the scene. All story text will now display using this box.")]
    [SerializeField] protected DialogueBox setDialogueBox;
    [Tooltip("Show the continue button after the text has finished writing")]
    [SerializeField] protected bool showButton = true;

    protected int executionCount;

    public virtual Character _Character { get { return character; } }
    public virtual Sprite Portrait { get { return characterPortrait; } set { characterPortrait = value; } }
    public virtual bool ExtendPrevious { get { return extendPrevious; } }

    public override void OnEnter()
    {
        if (!showAlways && executionCount >= showCount)
        {
            Continue();
            return;
        }

        executionCount++;

        // To implement when using conversation order/feature
        //if (character != null && character.SetDialogueBox != null)
        //{
        //    DialogueBox.ActiveDialogueBox = character.SetDialogueBox;
        //}

        if (setDialogueBox != null)
        {
            DialogueBox.ActiveDialogueBox = setDialogueBox;
        }

        var dialogueBox = DialogueBox.GetDialogueBox();
        if (dialogueBox == null)
        {
            Continue();
            return;
        }

        var engine = GetEngine();

        dialogueBox.SetActive(true);

        dialogueBox.SetCharacter(character);
        dialogueBox.SetCharacterImage(characterPortrait);

        string displayText = storyText;

        var activeCustomTags = CustomTag.activeCustomTags;
        for (int i = 0; i < activeCustomTags.Count; i++)
        {
            var ct = activeCustomTags[i];
            displayText = displayText.Replace(ct.TagStartSymbol, ct.ReplaceTagStartWith);
            // If the tag has an end symbol and a replacement for it, we replace it
            if (ct.TagEndSymbol != "" && ct.ReplaceTagEndWith != "")
            {
                displayText = displayText.Replace(ct.TagEndSymbol, ct.ReplaceTagEndWith);
            }
        }

        string subbedText = engine.SubstituteVariables(displayText);

        dialogueBox.StartDialogue(subbedText, !extendPrevious, waitForClick, fadeWhenDone, stopVoiceover, waitForVO, voiceOverClip, delegate
        {
            Continue();
        }, typingSpeed, showButton, ParentNode);
    }

    public override string GetSummary()
    {
        string namePrefix = "";
        if (character != null)
        {
            namePrefix = character.CharacterName + ": ";
        }
        if (extendPrevious)
        {
            namePrefix = "Extended: ";
        }
        return namePrefix + "\"" + storyText + "\"";
    }

    public override Color GetButtonColour()
    {
        return new Color32(224, 159, 22, 255);
    }

    public override void OnReset()
    {
        executionCount = 0;
    }

    public override void OnStopExecuting()
    {
        var dialogueBox = DialogueBox.GetDialogueBox();
        if (dialogueBox == null)
        {
            return;
        }

        dialogueBox.Stop();
    }

    #region ILocalizable implementation

    public virtual string GetStandardText()
    {
        return storyText;
    }

    public virtual void SetStandardText(string standardText)
    {
        storyText = standardText;
    }

    public virtual string GetStringId()
    {
        // String id for Say commands is SAY.<Localization Id>.<Command id>.[Character Name]
        string stringId = "SAY." + GetEngineLocalizationId() + "." + itemId + ".";
        if (character != null)
        {
            stringId += character.CharacterName;
        }

        return stringId;
    }

    #endregion
}