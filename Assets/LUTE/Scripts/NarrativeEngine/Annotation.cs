using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for labels/comments used on the graph/flow window.
/// </summary>
///
[ExecuteInEditMode]
[RequireComponent(typeof(BasicFlowEngine))]
[AddComponentMenu("")]
public class Annotation : MonoBehaviour
{
    [SerializeField] protected Color tint = Color.white;
    [SerializeField] protected bool useCustomTint = false;
    [SerializeField] protected int itemId = 0;

    public virtual Color Tint
    {
        get { return tint; }
        set { tint = value; }
    }
    public virtual bool UseCustomTint
    {
        get { return useCustomTint; }
        set { useCustomTint = value; }
    }
    public virtual int ItemId
    {
        get { return itemId; }
        set { itemId = value; }
    }
}
