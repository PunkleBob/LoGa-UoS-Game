using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// Directions that character portraits can face
public enum FacingDirection
{
    /// <summary> Unknown direction </summary>
    None,
    /// <summary> Facing left. </summary>
    Left,
    /// <summary> Facing right. </summary>
    Right
}

/// Types of display operations supported by portraits.
public enum DisplayType
{
    /// <summary> Do nothing. </summary>
    None,
    /// <summary> Show the portrait. </summary>
    Show,
    /// <summary> Hide the portrait. </summary>
    Hide,
    /// <summary> Replace the existing portrait. </summary>
    Replace,
    /// <summary> Move portrait to the front. </summary>
    MoveToFront
}

/// Represents the current state of a character portrait on the stage.
public class PortraitState
{
    public bool onScreen;
    public bool dimmed;
    public DisplayType display;
    public RectTransform position, holder;
    public FacingDirection facing;
    public Image portraitImage;
    public Sprite portrait { get { return portraitImage != null ? portraitImage.sprite : null; } }
    public List<Image> allPortraits = new List<Image>();

    public void SetPortraitImageBySprite(Sprite portrait)
    {
        portraitImage = allPortraits.Find(x => x.sprite == portrait);
    }
}

public class Portrait : MonoBehaviour
{
    static public int PortraitCompareTo(Sprite x, Sprite y)
    {
        if (x == y)
            return 0;
        if (y == null)
            return -1;
        if (x == null)
            return 1;

        return x.name.CompareTo(y.name);
    }
}
