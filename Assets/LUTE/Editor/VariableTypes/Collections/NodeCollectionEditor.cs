using UnityEngine;
using UnityEditor;
using System.Collections.Specialized;

[CustomEditor(typeof(NodeCollection))]

public class NodeCollectionEditor : OrderEditor
{
    public static readonly GUIContent None = new GUIContent("<None>");

    public static readonly GUIContent[] emptyList = new GUIContent[]
    {
            None,
    };
    protected SerializedProperty collectionProp;

    public override void OnEnable()
    {
        base.OnEnable();

        collectionProp = serializedObject.FindProperty("collection");

    }
    public override void OnInspectorGUI()
    {
        DrawOrderGUI();
    }

    public override void DrawOrderGUI()
    {
        serializedObject.Update();

        NodeCollection t = target as NodeCollection;

        collectionProp.arraySize = EditorGUILayout.IntField("Size", collectionProp.arraySize);

        var flowChart = t.GetEngine();
        if (flowChart != null)
        {
            for (int i = 0; i < collectionProp.arraySize; i++)
            {
                NodeEditor.NodeField(collectionProp.GetArrayElementAtIndex(i),
                                       new GUIContent("Target Block", "Block in this group"),
                                       new GUIContent("<None>"),
                                       flowChart);
                bool remove = GUILayout.Toggle(false, "Remove");
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
