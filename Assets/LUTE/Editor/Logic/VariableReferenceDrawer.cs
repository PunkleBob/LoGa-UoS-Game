using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(VariableReference))]
public class VariableReferenceDrawer : PropertyDrawer
{
    public BasicFlowEngine lastEngine;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var l = EditorGUI.BeginProperty(position, label, property);
        var startPos = position;
        position = EditorGUI.PrefixLabel(position, l);
        position.height = EditorGUIUtility.singleLineHeight;
        var variable = property.FindPropertyRelative("variable");

        Variable v = variable.objectReferenceValue as Variable;

        if (variable.objectReferenceValue != null && lastEngine == null)
        {
            if (v != null)
            {
                lastEngine = v.GetComponent<BasicFlowEngine>();
            }
        }

        lastEngine = EditorGUI.ObjectField(position, lastEngine, typeof(BasicFlowEngine), true) as BasicFlowEngine;
        position.y += EditorGUIUtility.singleLineHeight;

        if (lastEngine != null)
        {
            var thisPos = startPos;
            thisPos.y = position.y;
            var prefixLabel = new GUIContent(v != null ? v.GetType().Name : "No Variable Selected");
            EditorGUI.indentLevel++;
            VariableEditor.VariableField(variable, prefixLabel, lastEngine, "<None>", null, (s, t, u) => EditorGUI.Popup(thisPos, s, t, u));
            EditorGUI.indentLevel--;
        }
        else
        {
            EditorGUI.LabelField(position, new GUIContent("No engine selected"));
        }

        variable.serializedObject.ApplyModifiedProperties();
        property.serializedObject.ApplyModifiedProperties();
        EditorGUI.EndProperty();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2;
    }
}
