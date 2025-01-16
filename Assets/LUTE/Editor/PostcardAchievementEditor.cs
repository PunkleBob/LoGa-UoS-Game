using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    [CustomPropertyDrawer(typeof(PostcardAchievement))]
    public class PostcardAchievementEditor : PropertyDrawer
    {
        private const string ExcludedPropertyName = "targetNode";
        private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            string propertyPath = property.propertyPath;
            if (!foldoutStates.ContainsKey(propertyPath))
            {
                foldoutStates[propertyPath] = true;
            }

            Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            foldoutStates[propertyPath] = EditorGUI.Foldout(foldoutRect, foldoutStates[propertyPath], label, true);

            if (foldoutStates[propertyPath])
            {
                EditorGUI.indentLevel++;

                SerializedProperty iterator = property.Copy();
                bool enterChildren = true;
                float yOffset = EditorGUIUtility.singleLineHeight;

                while (iterator.NextVisible(enterChildren))
                {
                    enterChildren = false;
                    bool triggerNode = property.FindPropertyRelative("triggerNode").boolValue;
                    // If we are triggering a node we will show a dropdown rather than forcing the user to type the node name
                    if (ShouldSkipProperty(iterator))
                    {
                        if (triggerNode)
                        {
                            // Handle the triggerNode property
                            var engine = UnityEngine.Object.FindObjectOfType<BasicFlowEngine>();
                            if (engine != null)
                            {
                                var nodes = engine.GetComponents<Node>();
                                string[] nodeNames = new string[nodes.Length];
                                for (int i = 0; i < nodes.Length; i++)
                                {
                                    nodeNames[i] = nodes[i]._NodeName;
                                }
                                int index = 0;
                                for (int i = 0; i < nodes.Length; i++)
                                {
                                    if (nodes[i]._NodeName == property.FindPropertyRelative("targetNode").stringValue)
                                    {
                                        index = i;
                                        break;
                                    }
                                }
                                Rect propertyRect = new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight);
                                index = EditorGUI.Popup(propertyRect, "Trigger Node", index, nodeNames);
                                property.FindPropertyRelative("targetNode").stringValue = nodes[index]._NodeName;
                                yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                            }
                            else
                            {
                                // If there is no engine, we will just show the text field
                                Rect propertyRect = new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight);
                                EditorGUI.PropertyField(propertyRect, iterator, true);
                                yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                            }
                        }
                        continue;
                    }
                    float propertyHeight = EditorGUI.GetPropertyHeight(iterator, true);
                    Rect newPropertyRect = new Rect(position.x, position.y + yOffset, position.width, propertyHeight);
                    EditorGUI.PropertyField(newPropertyRect, iterator, true);
                    yOffset += propertyHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        private bool ShouldSkipProperty(SerializedProperty property)
        {
            return property.name == ExcludedPropertyName;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            string propertyPath = property.propertyPath;
            if (!foldoutStates.ContainsKey(propertyPath))
            {
                foldoutStates[propertyPath] = true;
            }

            if (!foldoutStates[propertyPath])
                return EditorGUIUtility.singleLineHeight;

            float totalHeight = EditorGUIUtility.singleLineHeight; // Height for the foldout
            SerializedProperty iterator = property.Copy();
            bool hasNext = iterator.NextVisible(true);
            while (hasNext)
            {
                totalHeight += EditorGUI.GetPropertyHeight(iterator, GUIContent.none, true) + EditorGUIUtility.standardVerticalSpacing;
                hasNext = iterator.NextVisible(false);
            }
            return totalHeight;
        }
    }
}