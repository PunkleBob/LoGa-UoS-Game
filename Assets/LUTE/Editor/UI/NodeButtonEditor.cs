using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    [CustomEditor(typeof(NodeButton), true)]
    public class NodeButtonEditor : ButtonEditor
    {
        public SerializedProperty nodeProp;
        public SerializedProperty engineProp;

        protected override void OnEnable()
        {
            base.OnEnable();

            nodeProp = serializedObject.FindProperty("targetNode");
            engineProp = serializedObject.FindProperty("targetEngine");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(engineProp);

            var engine = engineProp.objectReferenceValue as BasicFlowEngine;

            if (engine == null)
            {
                engine = FindObjectOfType<BasicFlowEngine>();
            }

            serializedObject.Update();

            if (engine == null)
            {
                EditorGUILayout.HelpBox("No BasicFlowEngine found in scene", MessageType.Error);
                return;
            }

            NodeEditor.NodeField(nodeProp,
                                 new GUIContent("Target Node", "Node to execute once this button has been pressed."),
                                 new GUIContent("<None>"),
            engine, null);

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
            EditorGUILayout.Space();
        }
    }
}
