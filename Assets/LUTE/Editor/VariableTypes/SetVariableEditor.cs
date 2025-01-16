using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SetVariable))]
public class SetVariableEditor : OrderEditor
{
    protected SerializedProperty varProp;
    protected SerializedProperty setOperatorProp;

    public override void OnEnable()
    {
        base.OnEnable();

        varProp = serializedObject.FindProperty("variable");
        setOperatorProp = serializedObject.FindProperty("setOperator");
    }

    public override void DrawOrderGUI()
    {
        serializedObject.Update();

        SetVariable t = target as SetVariable;

        var engine = (BasicFlowEngine)t.GetEngine();
        if (engine == null)
        {
            return;
        }

        EditorGUILayout.PropertyField(varProp, true);

        Variable selectedVariable = varProp.FindPropertyRelative("variable").objectReferenceValue as Variable;
        List<GUIContent> operatorsList = new List<GUIContent>();
        if (selectedVariable != null)
        {
            if (selectedVariable.SupportsArithmetic(SetOperator.Assign))
                operatorsList.Add(new GUIContent(VariableUtil.GetSetOperatorDescription(SetOperator.Assign)));
            if (selectedVariable.SupportsArithmetic(SetOperator.Negate))
                operatorsList.Add(new GUIContent(VariableUtil.GetSetOperatorDescription(SetOperator.Negate)));
            if (selectedVariable.SupportsArithmetic(SetOperator.Add))
                operatorsList.Add(new GUIContent(VariableUtil.GetSetOperatorDescription(SetOperator.Add)));
            if (selectedVariable.SupportsArithmetic(SetOperator.Subtract))
                operatorsList.Add(new GUIContent(VariableUtil.GetSetOperatorDescription(SetOperator.Subtract)));
            if (selectedVariable.SupportsArithmetic(SetOperator.Multiply))
                operatorsList.Add(new GUIContent(VariableUtil.GetSetOperatorDescription(SetOperator.Multiply)));
            if (selectedVariable.SupportsArithmetic(SetOperator.Divide))
                operatorsList.Add(new GUIContent(VariableUtil.GetSetOperatorDescription(SetOperator.Divide)));
        }
        else
            operatorsList.Add(VariableConditionEditor.None);

        int selectedIndex = (int)t._SetOperator;
        if (selectedIndex < 0)
            selectedIndex = 0;

        selectedIndex = EditorGUILayout.Popup(new GUIContent("Operation", "The type of operation to perform on the variable"), selectedIndex, operatorsList.ToArray());

        if (selectedVariable != null)
            setOperatorProp.enumValueIndex = selectedIndex;

        serializedObject.ApplyModifiedProperties();
    }
}
