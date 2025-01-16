using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    [CustomEditor(typeof(VariableImage))]
    public class VariableImageEditor : ImageEditor
    {
        public SerializedProperty spriteVariableProp;

        protected override void OnEnable()
        {
            base.OnEnable();

            spriteVariableProp = serializedObject.FindProperty("spriteVariable");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(spriteVariableProp, new GUIContent("Sprite Variable", "The sprite variable to display instead of sprite."));

            var m_sprite = serializedObject.FindProperty("m_Sprite");

            if (m_sprite != null)
            {
                m_sprite.objectReferenceValue = spriteVariableProp.objectReferenceValue;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
