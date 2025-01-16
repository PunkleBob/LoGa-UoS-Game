using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VariableCondition), true)]
public class VariableConditionEditor : OrderEditor
{
    public static bool showHelpText;
    public static void DrawTagHelpLabel()
    {
        string helpText = "";
        helpText += "-------- CUSTOM IFs --------";
        helpText += "\n\n";
        helpText += "For a location variable, the if statement evaluates true or false based on whether the device or mouse pointer falls within the specified radius of the location";
        helpText += "\n\n";
        helpText += "With a node collection variable, the if statement returns true when the specified integer, operator, and node collection count conditions are met";
        helpText += "\n\n";
        float pixelHeight = EditorStyles.miniLabel.CalcHeight(new GUIContent(helpText), EditorGUIUtility.currentViewWidth);
        EditorGUILayout.SelectableLabel(helpText, GUI.skin.GetStyle("HelpBox"), GUILayout.MinHeight(pixelHeight + 50));
    }
    public static readonly GUIContent None = new GUIContent("<None>");
    public static readonly GUIContent[] emptyList = new GUIContent[] { None };

    private static readonly GUIContent[] compareListAll = new GUIContent[]
    {
        new GUIContent(VariableUtil.GetCompareOperatorDescription(ComparisonOperator.Equals)),
        new GUIContent(VariableUtil.GetCompareOperatorDescription(ComparisonOperator.NotEquals)),
        new GUIContent(VariableUtil.GetCompareOperatorDescription(ComparisonOperator.LessThan)),
        new GUIContent(VariableUtil.GetCompareOperatorDescription(ComparisonOperator.GreaterThan)),
        new GUIContent(VariableUtil.GetCompareOperatorDescription(ComparisonOperator.LessThanOrEquals)),
        new GUIContent(VariableUtil.GetCompareOperatorDescription(ComparisonOperator.GreaterThanOrEquals)),
    };

    private static readonly GUIContent[] compareListEqualOnly = new GUIContent[]
    {
        new GUIContent(VariableUtil.GetCompareOperatorDescription(ComparisonOperator.Equals)),
        new GUIContent(VariableUtil.GetCompareOperatorDescription(ComparisonOperator.NotEquals)),
    };

    protected SerializedProperty conditions;

    public override void OnEnable()
    {
        base.OnEnable();

        conditions = serializedObject.FindProperty("conditions");
    }

    public override void DrawOrderGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("anyOrAllCondition"));

        conditions.arraySize = EditorGUILayout.IntField("Size", conditions.arraySize);
        GUILayout.Label("Conditions", EditorStyles.boldLabel);

        VariableCondition t = target as VariableCondition;

        var engine = (BasicFlowEngine)t.GetEngine();
        if (engine == null)
        {
            return;
        }

        EditorGUI.indentLevel++;
        for (int i = 0; i < conditions.arraySize; i++)
        {
            var conditionAnyVar = conditions.GetArrayElementAtIndex(i).FindPropertyRelative("anyVariable");
            var conditionCompare = conditions.GetArrayElementAtIndex(i).FindPropertyRelative("compareOperator");

            EditorGUILayout.PropertyField(conditionAnyVar, new GUIContent("Variable"), true);

            // Get selected variable
            Variable selectedVariable = conditionAnyVar.FindPropertyRelative("variable").objectReferenceValue as Variable;

            if (selectedVariable == null)
                continue;

            GUIContent[] operatorsList = emptyList;
            operatorsList = selectedVariable.SupportsComparison() ? compareListAll : compareListEqualOnly;

            // Get previously selected operator
            int selectedIndex = conditionCompare.enumValueIndex;
            if (selectedIndex < 0 || selectedIndex >= operatorsList.Length)
            {
                // Default to first index if the operator is not found in the available operators list
                // This can occur when changing between variable types
                selectedIndex = 0;
            }

            selectedIndex = EditorGUILayout.Popup(
                new GUIContent("Compare", "The comparison operator to use when comparing values"),
                selectedIndex,
                operatorsList);

            conditionCompare.enumValueIndex = selectedIndex;

            EditorGUILayout.Separator();

            if(selectedVariable.GetType() == typeof(DiceVariable))
            {
                var diceVar = selectedVariable as DiceVariable;
                diceVar.SetRollAgain(EditorGUILayout.Toggle("Roll Again", diceVar.GetRollAgain()));
                diceVar.SetModifier(EditorGUILayout.IntField("Modifier", diceVar.GetModifier()));

                //Get dice var data to ensure the value to compare is not greater than sides or less than 1
                var diceData = conditionAnyVar.FindPropertyRelative("data.diceData");
                var diceVal = diceData.FindPropertyRelative("diceVal");
                if(diceVal.intValue > engine.SidesOfDie)
                {
                    diceVal.intValue = engine.SidesOfDie;
                }
                else if(diceVal.intValue < 1)
                {
                    diceVal.intValue = 1;
                }
            }
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(new GUIContent("Custom If Help", "View available custom if info"), new GUIStyle(EditorStyles.miniButton)))
        {
            showHelpText = !showHelpText;
        }
        EditorGUILayout.EndHorizontal();


        if (showHelpText)
        {
            DrawTagHelpLabel();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
