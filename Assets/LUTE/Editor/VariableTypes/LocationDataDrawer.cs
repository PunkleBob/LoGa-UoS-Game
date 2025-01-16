using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LocationData))]
public class LocationDataDrawer : VariableDataDrawer<LocationVariable>
{
    private List<LocationVariable> m_locationVariables;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (m_locationVariables == null)
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
            m_locationVariables = engine.GetComponents<LocationVariable>().ToList();
        }

        var locationProperty = property.FindPropertyRelative("locationRef");

        ShowLocationList(locationProperty,
            new GUIContent("Location", "The related location variable."),
            new GUIContent("<None>"),
            m_locationVariables);

        EditorGUI.EndProperty();
    }

    private void ShowLocationList(SerializedProperty property, GUIContent label, GUIContent nullLabel, List<LocationVariable> entries)
    {
        if (property == null)
        {
            return;
        }

        List<GUIContent> objectNames = new List<GUIContent>();

        LocationVariable selectedObject = property.objectReferenceValue as LocationVariable;

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

        LocationVariable result;

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
