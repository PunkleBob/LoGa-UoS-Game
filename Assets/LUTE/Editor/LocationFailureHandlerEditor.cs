using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    [CustomEditor(typeof(LocationFailureHandler))]
    public class LocationFailureHandlerEditor : Editor
    {
        private ReorderableList failureMethodsList;
        private SerializedProperty failureMethodsProp;
        private string[] availableMethods;

        private Dictionary<int, ReorderableList> prioritizedMethodsLists = new Dictionary<int, ReorderableList>();

        private GUIStyle headerStyle;

        private void OnEnable()
        {
            failureMethodsProp = serializedObject.FindProperty("failureMethods");
            failureMethodsList = new ReorderableList(serializedObject, failureMethodsProp, true, true, true, true);

            failureMethodsList.drawElementCallback = DrawFailureMethodElement;
            failureMethodsList.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, "Failure Methods");
            failureMethodsList.elementHeightCallback = GetFailureMethodHeight;

            availableMethods = LocationFailureHandler.GetAvailableMethods();

            Color desiredColor = new Color32(255, 229, 217, 200);
            Texture2D backgroundTexture = new Texture2D(1, 1);
            backgroundTexture.SetPixel(0, 0, desiredColor);
            backgroundTexture.Apply();

            headerStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 12,
                normal = new GUIStyleState
                {
                    background = backgroundTexture,
                    textColor = Color.black
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            failureMethodsList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            // Draw an auto location button that will setup all locations to handle failure in a simple way
            LocationFailureHandler t = (LocationFailureHandler)target;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (t != null)
            {
                if (GUILayout.Button(new GUIContent("Auto Setup Locations", "Sets up all locations to handle failure")))
                {
                    t.SetupLocations();
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawFailureMethodElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = failureMethodsProp.GetArrayElementAtIndex(index);
            var foldoutProp = element.FindPropertyRelative("foldout");
            var queriedLocationProp = element.FindPropertyRelative("queriedLocation");
            var updateLocationTextProp = element.FindPropertyRelative("updateLocationText");
            var backupLocationsProp = element.FindPropertyRelative("backupLocations");
            var prioritizedMethodsProp = element.FindPropertyRelative("priorityMethods");
            var backupNodeProp = element.FindPropertyRelative("backupNode");
            var startIndexProp = element.FindPropertyRelative("startIndex");
            var continuousIncreaseProp = element.FindPropertyRelative("allowContinuousIncrease");
            var radiusIncreaseSizeProp = element.FindPropertyRelative("radiusIncreaseSize");

            // Draw foldout
            LocationVariable queriedName = queriedLocationProp.objectReferenceValue as LocationVariable;
            string name = queriedName != null ? queriedName.Key : "None";
            foldoutProp.boolValue = EditorGUI.Foldout(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), foldoutProp.boolValue, $"Failure Method: <{name}>");
            rect.y += EditorGUIUtility.singleLineHeight;

            if (foldoutProp.boolValue)
            {
                // Indent the content
                EditorGUI.indentLevel++;
                rect.x += 10;
                rect.width -= 10;

                // Draw the queried location and backup locations as a dropdown based on locations derived from the engine
                MonoBehaviour monoBev = (MonoBehaviour)target;
                var engine = monoBev.GetComponent<BasicFlowEngine>();

                if (engine == null)
                {
                    engine = FindObjectOfType<BasicFlowEngine>();
                    if (engine == null)
                    {
                        EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 2), "No BasicFlowEngine found in the scene. Please add one to the scene or create a new one.", MessageType.Error);
                        rect.y += EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
                        return;
                    }
                }
                var locations = engine.GetComponents<LocationVariable>();
                var locationKeys = locations.Select(x => x.Key).ToArray();

                // Find the index of the currently selected location
                int locationVarIndex = Array.FindIndex(locations, x => x == (queriedLocationProp.objectReferenceValue as LocationVariable));
                if (locationVarIndex == -1)
                {
                    locationVarIndex = 0; // Default to first item if not found
                    queriedLocationProp.objectReferenceValue = locations[0];
                }
                // Location Settings Header
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Location Settings", headerStyle);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // Create the dropdown for queried location
                EditorGUI.BeginChangeCheck();
                locationVarIndex = EditorGUI.Popup(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    queriedLocationProp.displayName,
                    locationVarIndex,
                    locationKeys
                );

                // Update the property if the selection changed
                if (EditorGUI.EndChangeCheck() && locations.Length > 0)
                {
                    queriedLocationProp.objectReferenceValue = locations[locationVarIndex];
                }

                // Move the rect down for the next control
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;


                // Backup Location Settings Header
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Backup Location Settings", headerStyle);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // Update Location Text
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), updateLocationTextProp);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // Create backup locations list excluding the queried location
                var backupLocations = locations.Where(x => x != queriedLocationProp.objectReferenceValue).ToArray();
                var backupLocationKeys = backupLocations.Select(x => x.Key).ToArray();

                // Backup Locations Dropdown
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Backup Locations");
                rect.y += EditorGUIUtility.singleLineHeight;

                for (int i = 0; i < backupLocationsProp.arraySize; i++)
                {
                    var backupLocationProp = backupLocationsProp.GetArrayElementAtIndex(i);
                    int backupLocationIndex = Array.FindIndex(backupLocations, x => x == (backupLocationProp.objectReferenceValue as LocationVariable));
                    if (backupLocationIndex == -1) backupLocationIndex = 0;

                    EditorGUI.BeginChangeCheck();
                    backupLocationIndex = EditorGUI.Popup(
                        new Rect(rect.x, rect.y, rect.width - 20, EditorGUIUtility.singleLineHeight),
                        backupLocationIndex,
                        backupLocationKeys
                    );

                    if (EditorGUI.EndChangeCheck() && backupLocations.Length > 0)
                    {
                        backupLocationProp.objectReferenceValue = backupLocations[backupLocationIndex];
                    }

                    if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y, 20, EditorGUIUtility.singleLineHeight), "-"))
                    {
                        backupLocationsProp.DeleteArrayElementAtIndex(i);
                        break;
                    }

                    rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                // Add button for new backup location
                if (GUI.Button(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Add Backup Location"))
                {
                    backupLocationsProp.InsertArrayElementAtIndex(backupLocationsProp.arraySize);
                }

                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // Backup Node Settings Header
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Backup Node Settings", headerStyle);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // Backup Node Properties
                rect.height = EditorGUIUtility.singleLineHeight;
                Rect labelRect = EditorGUI.PrefixLabel(rect, new GUIContent("Target Node", "The node to jump to if the location is inaccessible"));
                Rect popupRect = new Rect(labelRect.x, labelRect.y, rect.width - labelRect.width, labelRect.height);

                NodeEditor.NodeField(
                    popupRect,
                    backupNodeProp,
                    GUIContent.none,
                    new GUIContent("<None>"),
                    engine
                );
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), startIndexProp);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // Radius Settings Header
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Radius Settings", headerStyle);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // Draw radius increase properties
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), continuousIncreaseProp);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), radiusIncreaseSizeProp);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // Draw the prioritized methods list
                if (!prioritizedMethodsLists.TryGetValue(index, out ReorderableList prioritizedMethodsList))
                {
                    prioritizedMethodsList = new ReorderableList(prioritizedMethodsProp.serializedObject, prioritizedMethodsProp, true, true, true, true);
                    prioritizedMethodsList.drawHeaderCallback = (Rect r) => EditorGUI.LabelField(r, "Prioritised Methods");

                    prioritizedMethodsList.drawElementCallback = (Rect r, int i, bool active, bool focused) =>
                    {
                        var methodProp = prioritizedMethodsProp.GetArrayElementAtIndex(i);
                        EditorGUI.PropertyField(new Rect(r.x, r.y, r.width, EditorGUIUtility.singleLineHeight), methodProp, GUIContent.none);
                    };

                    prioritizedMethodsList.onAddDropdownCallback = (Rect buttonRect, ReorderableList list) =>
                    {
                        var menu = new GenericMenu();
                        foreach (var method in availableMethods)
                        {
                            if (!prioritizedMethodsProp.arrayContains(method))
                            {
                                menu.AddItem(new GUIContent(method), false, () =>
                                {
                                    prioritizedMethodsProp.InsertArrayElementAtIndex(prioritizedMethodsProp.arraySize);
                                    prioritizedMethodsProp.GetArrayElementAtIndex(prioritizedMethodsProp.arraySize - 1).stringValue = method;
                                    prioritizedMethodsProp.serializedObject.ApplyModifiedProperties();
                                });
                            }
                        }
                        menu.ShowAsContext();
                    };

                    prioritizedMethodsLists[index] = prioritizedMethodsList;
                }

                prioritizedMethodsList.DoList(new Rect(rect.x, rect.y, rect.width, prioritizedMethodsList.GetHeight()));

                // Reset indent
                EditorGUI.indentLevel--;
            }
        }

        private float GetFailureMethodHeight(int index)
        {
            var element = failureMethodsProp.GetArrayElementAtIndex(index);
            var foldoutProp = element.FindPropertyRelative("foldout");
            var backupLocationsProp = element.FindPropertyRelative("backupLocations");

            float height = EditorGUIUtility.singleLineHeight; // Foldout height

            if (foldoutProp.boolValue)
            {
                height += EditorGUIUtility.standardVerticalSpacing; // Space after foldout
                height += EditorGUIUtility.singleLineHeight * 4; // Location Settings, Backup Location Settings, Update Location Text, Backup Locations label
                height += EditorGUIUtility.standardVerticalSpacing * 4;

                // Backup Locations
                height += EditorGUIUtility.singleLineHeight * backupLocationsProp.arraySize;
                height += EditorGUIUtility.standardVerticalSpacing * backupLocationsProp.arraySize;
                height += EditorGUIUtility.singleLineHeight; // Add Backup Location button

                // Backup Node Settings
                height += EditorGUIUtility.singleLineHeight * 4; // Header, Target Node, Start Index, Radius Settings header
                height += EditorGUIUtility.standardVerticalSpacing * 4;

                // Radius Settings
                height += EditorGUIUtility.singleLineHeight * 2; // Continuous Increase, Radius Increase Size
                height += EditorGUIUtility.standardVerticalSpacing * 2;

                // Prioritized Methods List
                if (prioritizedMethodsLists.TryGetValue(index, out ReorderableList prioritizedMethodsList))
                {
                    height += prioritizedMethodsList.GetHeight() + 15;
                }
                else
                {
                    height += EditorGUIUtility.singleLineHeight * 2; // Fallback height if list not created yet
                }

                height += EditorGUIUtility.standardVerticalSpacing * 2; // Extra space at the bottom
            }

            return height;
        }
    }

    public static class SerializedPropertyExtensions
    {
        public static bool arrayContains(this SerializedProperty property, string value)
        {
            for (int i = 0; i < property.arraySize; i++)
            {
                if (property.GetArrayElementAtIndex(i).stringValue == value)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
