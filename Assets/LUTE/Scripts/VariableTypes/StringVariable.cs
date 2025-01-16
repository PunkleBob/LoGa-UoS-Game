using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[VariableInfo("", "String")]
[AddComponentMenu("")]
[System.Serializable]
public class StringVariable : BaseVariable<string>
{
}

/// <summary>
/// Container for a string variable reference or constant value.
/// Appears as a single line property in the inspector.
/// </summary>

[System.Serializable]
public struct StringData
{
    [SerializeField]
    [VariableProperty("<Value>", typeof(StringVariable))]
    public StringVariable stringRef;

    [SerializeField]
    public string stringVal;

    public StringData(string v)
    {
        stringVal = v;
        stringRef = null;
    }

    public static implicit operator string(StringData stringData)
    {
        return stringData.Value;
    }

    public string Value
    {
        get
        {
            if (stringVal == null) stringVal = "";
            return (stringRef == null) ? stringVal : stringRef.Value;
        }
        set { if (stringRef == null) { stringVal = value; } else { stringRef.Value = value; } }
    }

    public string GetDescription()
    {
        if (stringRef == null)
        {
            return stringVal != null ? stringVal : string.Empty;
        }
        else
        {
            return stringRef.Key;
        }
    }
}
