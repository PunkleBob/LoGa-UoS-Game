using System;
using UnityEditor;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    [System.Serializable]
    public class UDate : ISerializationCallbackReceiver
    {
        [HideInInspector] public DateTime date;

        [HideInInspector][SerializeField] private string _dateString;

        public static implicit operator DateTime(UDate ud)
        {
            return ud.date;
        }

        public static implicit operator UDate(DateTime dt)
        {
            return new UDate() { date = dt };
        }

        public void OnAfterDeserialize()
        {
            // Parse the date string in the format dd/MM/yyyy
            DateTime.TryParseExact(_dateString, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out date);
        }

        public void OnBeforeSerialize()
        {
            // Serialize the date in the format dd/MM/yyyy
            _dateString = date.ToString("dd/MM/yyyy");
        }
    }



#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(UDate))]
    public class UDateDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Begin property to support prefab overrides
            EditorGUI.BeginProperty(position, label, property);

            // Draw the label (optional)
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Get the _dateString field to display in the inspector
            SerializedProperty dateProperty = property.FindPropertyRelative("_dateString");

            // Draw the date field in the inspector with a text field
            dateProperty.stringValue = EditorGUI.TextField(position, dateProperty.stringValue);

            // Restore previous indent level
            EditorGUI.indentLevel = indent;

            // End property
            EditorGUI.EndProperty();
        }
    }
#endif
}
