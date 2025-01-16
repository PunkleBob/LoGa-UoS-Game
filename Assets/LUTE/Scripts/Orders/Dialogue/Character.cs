using UnityEngine;
using System.Collections.Generic;
using System.Globalization;

//Character class is used to identify characters in the scene that can be used in dialogues
[ExecuteInEditMode]
public class Character : MonoBehaviour, IComparer<Character>
{
    [SerializeField] protected string characterName;
    [SerializeField] protected Color nameColour = Color.white;
    [SerializeField] protected AudioClip characterSound;
    [SerializeField] protected List<Sprite> characterPortraits;
    [SerializeField] protected FacingDirection facingDirection;

    protected PortraitState portraitState = new PortraitState();
    protected static List<Character> activeCharacters = new List<Character>();

    protected virtual void OnEnable()
    {
        if (!activeCharacters.Contains(this))
        {
            activeCharacters.Add(this);
            activeCharacters.Sort(this);
        }
    }

    protected virtual void OnDisable()
    {
        activeCharacters.Remove(this);
    }

    public static List<Character> ActiveCharacters { get { return activeCharacters; } }

    public virtual string CharacterName { get { return characterName; } }
    public virtual Color NameColour { get { return nameColour; } }
    public virtual AudioClip SoundEffect { get { return characterSound; } }
    public virtual List<Sprite> Portraits { get { return characterPortraits; } }
    public virtual FacingDirection FacingDirection { get { return facingDirection; } }
    public virtual Sprite ProfileSprite { get; set; }
    public virtual PortraitState State { get { return portraitState; } }
    public string GetObjectName() { return gameObject.name; }

    /// Returns true if the character name starts with the specified string. Case insensitive.
    public virtual bool NameStartsWith(string matchString)
    {
#if NETFX_CORE
            return name.StartsWith(matchString, StringComparison.CurrentCultureIgnoreCase)
                || nameText.StartsWith(matchString, StringComparison.CurrentCultureIgnoreCase);
#else
        return name.StartsWith(matchString, true, System.Globalization.CultureInfo.CurrentCulture)
            || characterName.StartsWith(matchString, true, System.Globalization.CultureInfo.CurrentCulture);
#endif
    }

    /// Returns true if the character name is a complete match to the specified string. Case insensitive.
    public virtual bool NameMatch(string matchString)
    {
        return string.Compare(name, matchString, true, CultureInfo.CurrentCulture) == 0
            || string.Compare(characterName, matchString, true, CultureInfo.CurrentCulture) == 0;
    }

    public int Compare(Character x, Character y)
    {
        if (x == y)
            return 0;
        if (y == null)
            return 1;
        if (x == null)
            return -1;

        return x.name.CompareTo(y.name);
    }

    /// Looks for a portrait by name on a character
    public virtual Sprite GetPortrait(string portraitString)
    {
        if (string.IsNullOrEmpty(portraitString))
        {
            return null;
        }

        for (int i = 0; i < characterPortraits.Count; i++)
        {
            if (characterPortraits[i] != null && string.Compare(characterPortraits[i].name, portraitString, true) == 0)
            {
                return characterPortraits[i];
            }
        }
        return null;
    }

    #region ILocalizable implementation

    public virtual string GetStandardText()
    {
        return characterName;
    }

    public virtual void SetStandardText(string standardText)
    {
        characterName = standardText;
    }

    public virtual string GetStringId()
    {
        // String id for character names is CHARACTER.<Character Name>
        return "CHARACTER." + characterName;
    }

    #endregion

    protected virtual void OnValidate()
    {
        if (characterPortraits != null && characterPortraits.Count > 1)
        {
            characterPortraits.Sort(Portrait.PortraitCompareTo);
        }
    }
}
