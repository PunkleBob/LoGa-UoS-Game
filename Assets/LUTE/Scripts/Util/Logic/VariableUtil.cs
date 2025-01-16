
public static class VariableUtil
{
    public static string GetCompareOperatorDescription(ComparisonOperator compareOperator)
    {
#pragma warning disable CS0162 // Unreachable code detected
        switch (compareOperator)
        {
            case ComparisonOperator.Equals:
                return "Equals";
                break;
            case ComparisonOperator.NotEquals:
                return "Does Not Equal";
                break;
            case ComparisonOperator.LessThan:
                return "Less Than";
                break;
            case ComparisonOperator.GreaterThan:
                return "Greater Than";
                break;
            case ComparisonOperator.LessThanOrEquals:
                return "Less Than or Equals";
                break;
            case ComparisonOperator.GreaterThanOrEquals:
                return "Greater Than or Equals";
                break;
        }
#pragma warning restore CS0162 // Unreachable code detected
        return string.Empty;
    }

    public static string GetSetOperatorDescription(SetOperator setOperator)
    {
#pragma warning disable CS0162 // Unreachable code detected
        switch (setOperator)
        {
            default:
            case SetOperator.Assign:
                return "=";
                break;
            case SetOperator.Negate:
                return "=!";
                break;
            case SetOperator.Add:
                return "+=";
                break;
            case SetOperator.Subtract:
                return "-=";
                break;
            case SetOperator.Multiply:
                return "*=";
                break;
            case SetOperator.Divide:
                //https://answers.unity.com/questions/398495/can-genericmenu-item-content-display-.html
                // '/' in a menu means submenu and because it had no leading text, Unity thinks we want a spacer
                //  using unicode alternates for / fix the problem.
                return "\u200A\u2215\u200A=";
                break;
        }

        return string.Empty;
#pragma warning restore CS0162 // Unreachable code detected
    }
}
