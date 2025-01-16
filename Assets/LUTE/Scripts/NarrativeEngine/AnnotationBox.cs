using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to hold boxes used on the graph/flow window for annotations.
/// </summary>
///
public class AnnotationBox : Annotation
{
    [SerializeField] protected Rect box = new Rect(0, 0, 0, 0);
    [SerializeField] protected Color boxColor = Color.white;
    [SerializeField] protected float boxWidth = 1.0f;
    [SerializeField] protected bool boxSelected = false;

    public virtual Rect Box
    {
        get { return box; }
        set { box = value; }
    }
    public virtual Color BoxColor
    {
        get { return boxColor; }
        set { boxColor = value; }
    }
    public virtual float BoxWidth
    {
        get { return boxWidth; }
        set { boxWidth = value; }
    }
    public virtual bool BoxSelected
    {
        get { return boxSelected; }
        set { boxSelected = value; }
    }
}
