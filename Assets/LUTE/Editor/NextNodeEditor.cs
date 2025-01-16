using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NextNode))]
public class NextNodeEditor : OrderEditor
{
    protected SerializedProperty targetEngineProp;
    protected SerializedProperty targetNodeProp;
    protected SerializedProperty startIndexProp;
    protected SerializedProperty callModeProp;

    public override void OnEnable()
    {
        base.OnEnable();

        targetEngineProp = serializedObject.FindProperty("targetEngine");
        targetNodeProp = serializedObject.FindProperty("targetNode");
        startIndexProp = serializedObject.FindProperty("startIndex");
        callModeProp = serializedObject.FindProperty("callMode");
    }

    public override void DrawOrderGUI()
    {
        serializedObject.Update();

        NextNode t = target as NextNode;

        BasicFlowEngine engine = null;
        if (targetEngineProp.objectReferenceValue == null)
        {
            engine = (BasicFlowEngine)t.GetEngine();
        }
        else
        {
            engine = targetEngineProp.objectReferenceValue as BasicFlowEngine;
        }

        EditorGUILayout.PropertyField(targetEngineProp);

        if (engine != null)
        {
            NodeEditor.NodeField(targetNodeProp,
                                   new GUIContent("Target Node", "Next Node"),
                                   new GUIContent("<None>"),
                                   engine);

            EditorGUILayout.PropertyField(startIndexProp);
        }

        EditorGUILayout.PropertyField(callModeProp);

        serializedObject.ApplyModifiedProperties();
    }
}
