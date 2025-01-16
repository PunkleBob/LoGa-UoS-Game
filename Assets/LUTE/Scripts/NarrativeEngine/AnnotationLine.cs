using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to hold lines used on the graph/flow window for annotations.
/// </summary>
///
public class AnnotationLine : Annotation
{
    [SerializeField] protected Rect start = new Rect(0, 0, 65, 65);
    [SerializeField] protected Rect end = new Rect(0, 0, 65, 65);
    [SerializeField] protected Color lineColor = Color.white;
    [SerializeField] protected float lineWidth = 1.0f;
    [SerializeField] protected bool startSelected = false;

    public virtual Rect Start
    {
        get { return start; }
        set { start = value; }
    }
    public virtual Rect End
    {
        get { return end; }
        set { end = value; }
    }
    public virtual Color LineColor
    {
        get { return lineColor; }
        set { lineColor = value; }
    }
    public virtual float LineWidth
    {
        get { return lineWidth; }
        set { lineWidth = value; }
    }
    public virtual bool StartSelected
    {
        get { return startSelected; }
        set { startSelected = value; }
    }
}
