using UnityEngine;

[VariableInfo("", "Boolean")]
[AddComponentMenu("")]
[System.Serializable]
public class BooleanVariable : BaseVariable<bool>
{
    public override bool SupportsArithmetic(SetOperator setOperator)
    {
        return setOperator == SetOperator.Negate || base.SupportsArithmetic(setOperator);
    }

    public override void Apply(SetOperator setOperator, bool value)
    {
        switch (setOperator)
        {
            case SetOperator.Negate:
                Value = !value;
                break;
            default:
                base.Apply(setOperator, value);
                break;
        }
    }

    /// <summary>
    /// Container for a Bool variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct BooleanData
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(BooleanVariable))]
        public BooleanVariable booleanRef;

        [SerializeField]
        public bool booleanVal;

        public BooleanData(bool v)
        {
            booleanRef = null;
            booleanVal = v;
        }

        public static implicit operator bool(BooleanData booleanData)
        {
            return booleanData.Value;
        }

        public bool Value
        { 
            get { return (booleanRef == null) ? booleanVal : booleanRef.Value; }
            set { if (booleanRef == null) { booleanVal = value; } else { booleanRef.Value = value; } }
        }

        public string GetDescription()
        {
            return (booleanRef == null) ? booleanVal.ToString() : booleanRef.key;
        }
    }
}
