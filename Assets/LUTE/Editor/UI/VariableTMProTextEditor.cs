using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    [CustomEditor(typeof(VariableTMProText), true)]
    public class VariableTMProTextEditor : TMP_EditorPanelUI
    {
        public SerializedProperty varNameProp;

        protected override void OnEnable()
        {
            base.OnEnable();

            varNameProp = serializedObject.FindProperty("stringVariable");
        }

        protected override void DrawExtraSettings()
        {
            base.DrawExtraSettings();

            if (Foldout.extraSettings)
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(varNameProp, new GUIContent("String Variable", "The name of the string variable to display instead of text."));
                if (EditorGUI.EndChangeCheck())
                {
                    m_HavePropertiesChanged = true;
                    m_CheckPaddingRequiredProp.boolValue = true;
                }
            }
        }
    }
}
