using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[CustomEditor(typeof(ChoiceTimer))]
public class ChoiceTimerEditor : OrderEditor
{
    protected SerializedProperty durationProp;
    protected SerializedProperty targetNodeProp;
    protected SerializedProperty randomTargetProp;
    protected SerializedProperty randomTargetNodesProp;

    public override void OnEnable()
    {
        base.OnEnable();

        durationProp = serializedObject.FindProperty("_duration");
        targetNodeProp = serializedObject.FindProperty("targetNode");
        randomTargetProp = serializedObject.FindProperty("randomTarget");
        randomTargetNodesProp = serializedObject.FindProperty("randomTargetNodes");
    }

    public override void DrawOrderGUI()
    {
        ChoiceTimer t = target as ChoiceTimer;
        var engine = (BasicFlowEngine)t.GetEngine();
        if (engine == null)
        {
            return;
        }

        serializedObject.Update();

        EditorGUILayout.PropertyField(durationProp);

        NodeEditor.NodeField(targetNodeProp,
                             new GUIContent("Target Node", "Node to call when timer expires"),
                             new GUIContent("<None>"),
                             engine);

        EditorGUILayout.PropertyField(randomTargetProp);

        if (randomTargetProp.boolValue)
        {
            // Display the dropdown for the random target nodes
            EditorGUILayout.PropertyField(randomTargetNodesProp);
            if (randomTargetNodesProp.isExpanded)
            {
                if (randomTargetNodesProp.arraySize == 0)
                {
                    randomTargetNodesProp.arraySize++;
                }

                EditorGUI.indentLevel++;
                for (int i = 0; i < randomTargetNodesProp.arraySize; i++)
                {
                    var nodeProp = randomTargetNodesProp.GetArrayElementAtIndex(i);

                    NodeEditor.NodeField(nodeProp,
                                         new GUIContent("Target Node", "Node to call when timer expires"),
                                         new GUIContent("<None>"),
                                         engine);
                }
                EditorGUI.indentLevel--;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
