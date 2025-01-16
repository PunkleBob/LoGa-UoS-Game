using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    [CustomPropertyDrawer(typeof(UDateTimeData))]
    public class UDateTimeDataDrawer : VariableDataDrawer<UDateTimeVariable>
    {
        private List<UDateTimeVariable> vars;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (vars == null)
            {
                var engine = BasicFlowEngine.CachedEngines.FirstOrDefault();
                if (engine == null)
                {
                    engine = Object.FindObjectOfType<BasicFlowEngine>();
                    if (engine == null)
                    {
                        return;
                    }
                }
                vars = engine.GetComponents<UDateTimeVariable>().ToList();
            }

            var uDateTimeProperty = property.FindPropertyRelative("uDateTimeRef");

            ShowVariableList(uDateTimeProperty,
                new GUIContent("UDateTime", "The related UDateTime variable."),
                new GUIContent("<None>"),
                vars);
        }

        private void ShowVariableList(SerializedProperty property, GUIContent label, GUIContent nullLabel, List<UDateTimeVariable> entries)
        {
            if (property == null)
            {
                return;
            }

            List<GUIContent> objectNames = new List<GUIContent>();

            UDateTimeVariable selectedObject = property.objectReferenceValue as UDateTimeVariable;

            int selectedIndex = -1; // Invalid index

            // First option in list is <None>
            objectNames.Add(nullLabel);
            if (selectedObject == null)
            {
                selectedIndex = 0;
            }

            for (int i = 0; i < entries.Count; ++i)
            {
                objectNames.Add(new GUIContent(entries[i].Key));

                if (selectedObject == entries[i])
                {
                    selectedIndex = i + 1;
                }
            }

            UDateTimeVariable result;

            selectedIndex = EditorGUILayout.Popup(label, selectedIndex, objectNames.ToArray());

            if (selectedIndex == -1)
            {
                return;
            }
            else if (selectedIndex == 0)
            {
                result = null;
            }
            else
            {
                result = entries[selectedIndex - 1];
            }

            property.objectReferenceValue = result;
        }
    }
}