using UnityEngine;

[VariableInfo("", "Float")]
[AddComponentMenu("")]
[System.Serializable]
public class FloatVariable : BaseVariable<float>
{
    public override bool SupportsArithmetic(SetOperator setOperator)
    {
        return true;
    }

    public override bool SupportsComparison()
    {
        return true;
    }

    public override void Apply(SetOperator setOperator, float value)
    {
        switch (setOperator)
        {
            case SetOperator.Negate:
                Value = Value * -1;
                break;
            case SetOperator.Add:
                Value += value;
                break;
            case SetOperator.Subtract:
                Value -= value;
                break;
            case SetOperator.Multiply:
                Value *= value;
                break;
            case SetOperator.Divide:
                Value /= value;
                break;
            default:
                base.Apply(setOperator, value);
                break;
        }
    }

    public override bool Evaluate(ComparisonOperator comparisonOperator, float value)
    {
        float lhs = Value;
        float rhs = value;

        bool condition = false;

        switch (comparisonOperator)
        {
            case ComparisonOperator.LessThan:
                condition = lhs < rhs;
                break;
            case ComparisonOperator.GreaterThan:
                condition = lhs > rhs;
                break;
            case ComparisonOperator.LessThanOrEquals:
                condition = lhs <= rhs;
                break;
            case ComparisonOperator.GreaterThanOrEquals:
                condition = lhs >= rhs;
                break;
            default:
                condition = base.Evaluate(comparisonOperator, value);
                break;
        }

        return condition;
    }

    /// <summary>
    /// Container for an float variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct FloatData
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(FloatVariable))]
        public FloatVariable floatRef;

        [SerializeField]
        public float floatVal;

        public FloatData(float v)
        {
            floatVal = v;
            floatRef = null;
        }

        public static implicit operator float(FloatData floatData)
        {
            return floatData.Value;
        }

        public float Value
        {
            get { return (floatRef == null) ? floatVal : floatRef.Value; }
            set { if (floatRef == null) { floatVal = value; } else { floatRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (floatRef == null)
            {
                return floatVal.ToString();
            }
            else
            {
                return floatRef.Key;
            }
        }
    }
}
