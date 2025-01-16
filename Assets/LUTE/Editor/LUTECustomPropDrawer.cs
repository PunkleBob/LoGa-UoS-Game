using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    [CustomPropertyDrawer(typeof(LUTECustomPropAttribute))]
    public class LUTECustomPropDrawer : PropertyDrawer
    {

    }

    [CustomPropertyDrawer(typeof(VariableReferenceAttribute))]
    public class VariableReferenceDrawer : PropertyDrawer
    {
        private VariableScriptGenerator generator = new VariableScriptGenerator();

        // Static dictionary to maintain references across play mode
        private static Dictionary<Type, List<Variable>> cachedVariables = new Dictionary<Type, List<Variable>>();

        private static void RefreshVariableCache()
        {
            cachedVariables.Clear();
            var engines = Resources.FindObjectsOfTypeAll<BasicFlowEngine>();

            foreach (var engine in engines)
            {
                var variables = engine.GetComponentsInChildren<Variable>(true);
                foreach (var variable in variables)
                {
                    Type variableType = variable.GetType();
                    if (!cachedVariables.ContainsKey(variableType))
                    {
                        cachedVariables[variableType] = new List<Variable>();
                    }
                    cachedVariables[variableType].Add(variable);
                }
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var variableRefAttribute = (VariableReferenceAttribute)attribute;

            // Get original type
            Type originalType = GetTypeFromManagedReference(property);

            if (originalType == null)
            {
                EditorGUI.LabelField(position, "Error: Could not determine type");
                EditorGUI.EndProperty();
                return;
            }

            // Construct variable type name
            string typeName = originalType.Name;

            string fullTypeName = string.IsNullOrEmpty(variableRefAttribute.Namespace)
                ? typeName + "Variable"
                : $"{variableRefAttribute.Namespace}.{typeName}Variable";

            // Find the variable type
            Type variableType = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.FullName == fullTypeName);

            if (variableType == null)
            {
                try
                {
                    generator.TargetType = originalType;
                    generator.Generate();
                    EditorUtility.DisplayProgressBar("Generating " + originalType, "Importing Scripts", 0);
                    AssetDatabase.Refresh();
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                    //throw e;
                }
                generator = new VariableScriptGenerator();
                EditorUtility.ClearProgressBar();

                //EditorGUI.LabelField(position, $"Error: Type {fullTypeName} not found");
                //EditorGUI.EndProperty();
                //return;
            }

            // Ensure cache is populated for runtime use
            if (variableType != null)
            {
                if (!cachedVariables.ContainsKey(variableType) || cachedVariables[variableType].Count == 0)
                {
                    RefreshVariableCache();
                }

                // Get variables of the specific type
                var variables = cachedVariables.ContainsKey(variableType)
                    ? cachedVariables[variableType].ToArray()
                    : new Variable[0];


                // Prepare names for popup
                string[] variableNames = variables
                    .Select(v => v.Key)
                    .Prepend("<None>")
                    .ToArray();

                int currentIndex = 0;

                PropertyReference propRef = null;
                UnityEngine.Object targetObj = property.serializedObject.targetObject;
                GameObject relatedGameObject = null;

                if (targetObj is MonoBehaviour monoBehaviour)
                {
                    relatedGameObject = monoBehaviour.gameObject;
                }
                else
                {
                    Debug.LogError("Target object is not a MonoBehaviour, attribute only supports basic objects.");
                    return;
                }

                foreach (Variable v in variables)
                {
                    propRef = v.FindPropertyReference(relatedGameObject, property.name);
                    if (propRef != null)
                    {
                        currentIndex = Array.IndexOf(variables, v) + 1;
                        break;
                    }
                }

                int previousIndex = currentIndex;
                int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, variableNames);

                if (selectedIndex != previousIndex)
                {
                    Variable v = selectedIndex > 0 ? variables[selectedIndex - 1] : null;
                    Variable previousVariable = previousIndex > 0 ? variables[previousIndex - 1] : null;

                    if (v != null)
                    {
                        if (previousVariable != null)
                        {
                            previousVariable.RemovePropertyReference(relatedGameObject, property.name);
                        }

                        v.AddPropertyReference(relatedGameObject, property.name);
                    }
                }

                EditorGUI.EndProperty();
            }
        }

        private Type GetTypeFromManagedReference(SerializedProperty property)
        {
            var targetObject = property.serializedObject.targetObject;
            var field = targetObject.GetType().GetField(property.name,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.FlattenHierarchy);
            return field?.FieldType;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
    }
}