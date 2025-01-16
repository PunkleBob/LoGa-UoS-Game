using System.Collections;
using UnityEngine;

[VariableInfo("", "Dice")]
[AddComponentMenu("")]
[System.Serializable]
public class DiceVariable : BaseVariable<int>
{
    [Tooltip("Whether to roll the dice again when checking the condition (this could be zero if no dice roll has commenced!)")]
    [SerializeField] protected bool rollAgain = true;
    [Tooltip("The modifier to apply to the dice roll")]
    [SerializeField] protected int modifier;
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
            default:
                base.Apply(setOperator, value);
                break;
        }
    }

    public override bool Evaluate(ComparisonOperator comparisonOperator, int value)
    {
        if (rollAgain)
        {
            RollDice();
        }

        float leftSide = Value;
        float rightSide = value;

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

    public int RollDice()
    {
        int diceSides = GetEngine().SidesOfDie;
        Value = Random.Range(1, diceSides) + modifier;
        return Value;
    }

    public void AddToModifier(int value)
    {
        modifier += value;
    }

    public void SubtractFromModifier(int value)
    {
        modifier -= value;
    }

    public void SetModifier(int value)
    {
        modifier = value;
    }

    public int GetModifier()
    {
        return modifier;
    }

    public void SetRollAgain(bool value)
    {
        rollAgain = value;
    }

    public bool GetRollAgain()
    {
        return rollAgain;
    }
}

// Container for dice variables ref
[System.Serializable]
public struct DiceData
{
    [SerializeField]
    [VariableProperty("<Value>", typeof(DiceVariable))]
    public DiceVariable diceRef;
    [SerializeField]
    public int diceVal;

    public DiceData(int v)
    {
        diceVal = v;
        diceRef = null;
    }

    public static implicit operator int(DiceData diceData)
    {
        return diceData.Value;
    }

    public int Value
    {
        get { return (diceRef == null) ? diceVal : diceRef.Value; }
        set { if (diceRef == null) { diceVal = value; } else { diceRef.Value = value; } }
    }

    public string GetDescription()
    {
        if (diceRef == null)
        {
            return diceVal.ToString();
        }
        else
        {
            return diceRef.Key;
        }
    }
}
