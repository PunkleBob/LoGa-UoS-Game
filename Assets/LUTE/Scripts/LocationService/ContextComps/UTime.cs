using System;
using UnityEditor;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    [System.Serializable]
    public class UTime : ISerializationCallbackReceiver
    {
        [HideInInspector] public TimeSpan time;

        [HideInInspector][SerializeField] private string _timeString;

        public static implicit operator TimeSpan(UTime ut)
        {
            return ut.time;
        }

        public static implicit operator UTime(TimeSpan ts)
        {
            return new UTime() { time = ts };
        }

        public void OnAfterDeserialize()
        {
            TimeSpan.TryParse(_timeString, out time);
        }

        public void OnBeforeSerialize()
        {
            _timeString = time.ToString();
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(UTime))]
    public class UTimeDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            Rect amountRect = new Rect(position.x, position.y, position.width, position.height);

            // Draw the time field - pass GUIContent.none to ensure it's without labels
            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("_timeString"), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
#endif

}
