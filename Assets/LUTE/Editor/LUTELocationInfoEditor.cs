using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    //[CustomEditor(typeof(LUTELocationInfo))]
    public class LUTELocationInfoEditor : Editor
    {
        protected SerializedProperty nodeCompleteProp;
        protected SerializedProperty executeNodeProp;

        protected SerializedProperty nodeProp;
        public SerializedProperty engineProp;


        protected void OnEnable()
        {
            nodeCompleteProp = serializedObject.FindProperty("nodeComplete");
            executeNodeProp = serializedObject.FindProperty("executeNode");
            nodeProp = serializedObject.FindProperty("testNode");
            engineProp = serializedObject.FindProperty("targetEngine");
        }

        public override void OnInspectorGUI()
        {
            var locationInfo = target as LUTELocationInfo;

            base.OnInspectorGUI();
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(engineProp);

            var engine = FindObjectOfType<BasicFlowEngine>();

            serializedObject.Update();

            if (engine != null)
            {
                var nodes = engine.GetComponents<Node>();
                string[] nodeNames = new string[nodes.Length];
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodeNames[i] = nodes[i]._NodeName;
                }
                int nodeCompleteIndex = 0;
                int executeNodeIndex = 0;
                for (int i = 0; i < nodes.Length; i++)
                {
                    if (nodes[i]._NodeName == nodeCompleteProp.stringValue)
                    {
                        nodeCompleteIndex = i;
                        break;
                    }
                }
                for (int i = 0; i < nodes.Length; i++)
                {
                    if (nodes[i]._NodeName == executeNodeProp.stringValue)
                    {
                        executeNodeIndex = i;
                        break;
                    }
                }
                nodeCompleteIndex = EditorGUILayout.Popup(new GUIContent("Complete Node", "When this Node completes, the location marker gets set to complete"), nodeCompleteIndex, nodeNames);
                nodeCompleteProp.stringValue = nodes[nodeCompleteIndex]._NodeName;
                executeNodeIndex = EditorGUILayout.Popup(new GUIContent("Execute Node", "When the location marker is clicked, this Node gets executed"), executeNodeIndex, nodeNames);
                executeNodeProp.stringValue = nodes[executeNodeIndex]._NodeName;
            }

            if (GUILayout.Button(new GUIContent("Create Node from Location", "Creates a Node based on this Location.")))
            {
                GraphWindow graphWindow = GraphWindow.ShowWindow();

                if (graphWindow != null)
                {
                    if (engine != null)
                    {
                        var locVar = engine.GetComponents<LocationVariable>().FirstOrDefault(x => x.Value.infoID == locationInfo.infoID);
                        graphWindow.AddNodeFromLocation(locVar);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();


        }
        private void GetAllChildObjects(GameObject parent, List<GameObject> allObjects)
        {
            foreach (Transform child in parent.transform)
            {
                allObjects.Add(child.gameObject);
                GetAllChildObjects(child.gameObject, allObjects);
            }
        }
    }
}