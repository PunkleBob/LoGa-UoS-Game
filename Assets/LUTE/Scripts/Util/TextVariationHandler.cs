using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class TextVariationHandler
{
    public class Section
    {
        public enum VaryType
        {
            Sequence,
            Cycle,
            Once,
            Random,
            Conditional
        }

        public Node parentNode = null;

        public VaryType type = VaryType.Sequence;
        public string entire = string.Empty;
        public List<string> elements = new List<string>();

        // Conditional properties
        public string variableName = string.Empty;
        public ComparisonOperator operatorType = ComparisonOperator.Equals;
        public string compareValue = string.Empty;
        public string trueResult = string.Empty;
        public string falseResult = string.Empty;
        public object compareObj = null;

        public string Select(ref int index)
        {
            if (type == VaryType.Conditional)
            {
                // Remove the operator prefix from variable name before finding it
                string cleanVarName = variableName.TrimStart('=', '!', '>', '<').TrimEnd('=', '<', '>', '!');

                BasicFlowEngine engine = null;

                if (parentNode != null)
                {
                    engine = parentNode.GetEngine();
                }
                else
                {
                    engine = GameObject.FindObjectsOfType<BasicFlowEngine>().ToList().Where(x => !x.gameObject.name.Contains("GlobalVariablesEngine")).FirstOrDefault();
                }

                Debug.Log(cleanVarName);

                var variable = engine?.GetVariable(cleanVarName);

                if (variable == null)
                {
                    return string.Empty;
                }

                // You can do this here or you could do this at variable level where we parse the string value to the correct type
                // Either method requires custom handling
                switch (variable.GetType())
                {
                    case Type t when t == typeof(StringVariable):
                        compareObj = compareValue;
                        break;
                    case Type t when t == typeof(IntegerVariable):
                        if (int.TryParse(compareValue, out int intResult))
                        {
                            compareObj = intResult;
                        }
                        break;
                    case Type t when t == typeof(FloatVariable):
                        if (float.TryParse(compareValue, out float floatResult))
                        {
                            compareObj = floatResult;
                        }
                        break;
                    case Type t when t == typeof(BooleanVariable):
                        if (bool.TryParse(compareValue, out bool boolResult))
                        {
                            compareObj = boolResult;
                        }
                        break;
                    default:
                        compareObj = compareValue;
                        break;
                }

                return variable != null && compareObj != null && variable.Evaluate(operatorType, compareObj)
                    ? trueResult
                    : falseResult;
            }

            switch (type)
            {
                case VaryType.Sequence:
                    index = Mathf.Min(index, elements.Count - 1);
                    break;
                case VaryType.Cycle:
                    index = index % elements.Count;
                    break;
                case VaryType.Once:
                    index = Mathf.Min(index, elements.Count);
                    break;
                case VaryType.Random:
                    index = UnityEngine.Random.Range(0, elements.Count);
                    break;
            }

            if (index >= 0 && index < elements.Count)
            {
                return elements[index];
            }
            return string.Empty;
        }
    }

    static Dictionary<int, int> hashedSections = new Dictionary<int, int>();

    public static void ClearHistory()
    {
        hashedSections.Clear();
    }

    public static bool TokeniseVarySections(string input, List<Section> varyingSections)
    {
        varyingSections.Clear();
        int currentDepth = 0;
        int curStartIndex = 0;
        int curPipeIndex = 0;
        Section curSection = null;

        for (int i = 0; i < input.Length; i++)
        {
            if (i < input.Length - 2 && input[i] == '[' && input[i + 1] == 'i' && input[i + 3] == '$')
            {
                // Handle conditional statement
                if (ParseConditional(input, i, out Section conditionalSection))
                {
                    varyingSections.Add(conditionalSection);
                    i += conditionalSection.entire.Length - 1; // Skip to end of conditional
                    continue;
                }
            }

            switch (input[i])
            {
                case '[':
                    if (currentDepth == 0)
                    {
                        curSection = new Section();
                        varyingSections.Add(curSection);

                        // Determine type and skip control char if needed
                        if (i + 1 < input.Length)
                        {
                            var typedIndicatingChar = input[i + 1];
                            switch (typedIndicatingChar)
                            {
                                case '~':
                                    curSection.type = Section.VaryType.Random;
                                    curPipeIndex = i + 2;
                                    break;
                                case '&':
                                    curSection.type = Section.VaryType.Cycle;
                                    curPipeIndex = i + 2;
                                    break;
                                case '!':
                                    curSection.type = Section.VaryType.Once;
                                    curPipeIndex = i + 2;
                                    break;
                                default:
                                    curPipeIndex = i + 1;
                                    break;
                            }
                        }

                        // Mark start
                        curStartIndex = i;
                    }
                    currentDepth++;
                    break;
                case ']':
                    if (currentDepth == 1)
                    {
                        // Extract, including the ]
                        curSection.entire = input.Substring(curStartIndex, i - curStartIndex + 1);

                        // Add the last element
                        if (curPipeIndex < i)
                        {
                            curSection.elements.Add(input.Substring(curPipeIndex, i - curPipeIndex));
                        }
                    }
                    currentDepth--;
                    break;
                case '|':
                    if (currentDepth == 1)
                    {
                        // Split
                        curSection.elements.Add(input.Substring(curPipeIndex, i - curPipeIndex));

                        // Over the | on the next one
                        curPipeIndex = i + 1;
                    }
                    break;
            }
        }

        if (varyingSections.Count == 1)
        {
            if (varyingSections[0].type == Section.VaryType.Sequence && varyingSections[0].elements.Count == 0)
            {
                return false;
            }
        }
        return varyingSections.Count > 0;
    }

    private static bool ParseConditional(string input, int startIndex, out Section section)
    {
        section = new Section { type = Section.VaryType.Conditional };

        // Regular expression to match the new syntax
        var regex = new Regex(@"\[if\$([^=!<>]+)(!=|==|>=|<=|>|<|=)([^\]]+)\?([^:]+):([^\]]+)\]");

        var match = regex.Match(input, startIndex);

        if (!match.Success)
            return false;

        section.variableName = match.Groups[1].Value.Trim();
        string fullOperator = match.Groups[2].ToString(); // Use ToString() to get full match
        section.operatorType = ParseOperator(fullOperator);
        section.compareValue = match.Groups[3].Value.Trim();
        section.trueResult = match.Groups[4].Value.Trim();
        section.falseResult = match.Groups[5].Value.Trim();

        section.entire = match.Value;

        return true;
    }

    private static ComparisonOperator ParseOperator(string opString)
    {
        switch (opString)
        {
            case "=": return ComparisonOperator.Equals;
            case "!=": return ComparisonOperator.NotEquals;
            case ">=": return ComparisonOperator.GreaterThanOrEquals;
            case "<=": return ComparisonOperator.LessThanOrEquals;
            case ">": return ComparisonOperator.GreaterThan;
            case "<": return ComparisonOperator.LessThan;
            default: throw new ArgumentException("Invalid operator: " + opString);
        }
    }

    public static string SelectVariations(string input, Node parentNode = null, int parentHash = 0)
    {
        List<Section> sections = new List<Section>();
        bool foundSections = TokeniseVarySections(input, sections);

        if (!foundSections)
        {
            return input;
        }

        StringBuilder sb = new StringBuilder();
        sb.Length = 0;
        sb.Append(input);

        for (int i = 0; i < sections.Count; i++)
        {
            var curSection = sections[i];

            if (parentNode != null)
            {
                curSection.parentNode = parentNode;
            }

            string selected = string.Empty;

            if (curSection.type == Section.VaryType.Conditional)
            {
                int dummyIndex = 0;
                selected = curSection.Select(ref dummyIndex);
            }
            else
            {
                int index = -1;

                int hash = input.GetHashCode();
                hash ^= (hash << 13);
                int curSecHash = curSection.entire.GetHashCode();
                curSecHash ^= (curSecHash >> 17);
                int key = hash ^ curSecHash ^ parentHash;

                if (hashedSections.TryGetValue(key, out int foundVal))
                {
                    index = foundVal;
                }

                index++;

                selected = curSection.Select(ref index);
                hashedSections[key] = index;
            }

            selected = SelectVariations(selected, null, parentHash);
            sb.Replace(curSection.entire, selected);
        }
        return sb.ToString();
    }
}