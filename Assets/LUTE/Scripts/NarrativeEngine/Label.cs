using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to hold labels/comments used on the graph/flow window.
/// </summary>
///
public class Label : Annotation
{
    [SerializeField] protected Rect labelRect = new Rect(0, 0, 125, 40);
    [SerializeField] protected string labelText = "Label";
    [SerializeField] protected GUIStyle labelStyle = new GUIStyle();

    public virtual Rect LabelRect
    {
        get { return labelRect; }
        set { labelRect = value; }
    }
    public virtual string LabelText
    {
        get { return labelText; }
        set { labelText = value; }
    }
    public virtual GUIStyle LabelStyle
    {
        get { return labelStyle; }
        set { labelStyle = value; }
    }
}
