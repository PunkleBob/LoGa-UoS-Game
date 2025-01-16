using System.Collections;
using UnityEngine;

[VariableInfo("", "Integer")]
[AddComponentMenu("")]
[System.Serializable]
public class IntegerVariable : BaseVariable<int>
{
    public override bool SupportsArithmetic(SetOperator setOperator)
    {
        return true;
    }
    public override bool SupportsComparison()
    {
        return true;
    }

    public override void Apply(SetOperator setOperator, int value)
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
            // case SetOperator.Modulo: // TODO: Implement modulo
            //     Value %= value;
            //     break;
            default:
                base.Apply(setOperator, value);
                break;
        }
    }

    public override bool Evaluate(ComparisonOperator comparisonOperator, int value)
    {
        int leftSide = Value;
        int rightSide = value;

        bool condition;
        switch (comparisonOperator)
        {
            case ComparisonOperator.LessThan:
                condition = leftSide < rightSide;
                break;
            case ComparisonOperator.LessThanOrEquals:
                condition = leftSide <= rightSide;
                break;
            case ComparisonOperator.GreaterThan:
                condition = leftSide > rightSide;
                break;
            case ComparisonOperator.GreaterThanOrEquals:
                condition = leftSide >= rightSide;
                break;
            default:
                condition = base.Evaluate(comparisonOperator, value);
                break;
        }

        return condition;
    }

}

// Container for integer variables ref
[System.Serializable]
public struct IntegerData
{
    [SerializeField]
    [VariableProperty("<Value>", typeof(IntegerVariable))]
    public IntegerVariable integerRef;
    [SerializeField]
    public int integerVal;

    public IntegerData(int v)
    {
        integerVal = v;
        integerRef = null;
    }

    public static implicit operator int(IntegerData integerData)
    {
        return integerData.Value;
    }

    public int Value
    {
        get { return (integerRef == null) ? integerVal : integerRef.Value; }
        set { if (integerRef == null) { integerVal = value; } else { integerRef.Value = value; } }
    }

    public string GetDescription()
    {
        if (integerRef == null)
        {
            return integerVal.ToString();
        }
        else
        {
            return integerRef.Key;
        }
    }
}
