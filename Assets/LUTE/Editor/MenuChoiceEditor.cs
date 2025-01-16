using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MenuChoice))]
public class MenuChoiceEditor : OrderEditor
{
    protected SerializedProperty textProp;
    protected SerializedProperty descriptionProp;
    protected SerializedProperty targetNodeProp;
    protected SerializedProperty hideIfVisitedProp;
    protected SerializedProperty interactableProp;
    protected SerializedProperty setMenuDialogProp;
    protected SerializedProperty hideThisOptionProp;
    protected SerializedProperty closeMenuOnSelectProp;
    protected SerializedProperty buttonFeedbackProp;
    protected SerializedProperty buttonSoundProp;
    protected SerializedProperty saveSettingsProp;

    public override void OnEnable()
    {
        base.OnEnable();

        textProp = serializedObject.FindProperty("text");
        descriptionProp = serializedObject.FindProperty("description");
        targetNodeProp = serializedObject.FindProperty("targetNode");
        hideIfVisitedProp = serializedObject.FindProperty("hideIfVisited");
        interactableProp = serializedObject.FindProperty("interactable");
        setMenuDialogProp = serializedObject.FindProperty("setMenuDialogue");
        hideThisOptionProp = serializedObject.FindProperty("hideThisOption");
        closeMenuOnSelectProp = serializedObject.FindProperty("closeMenuOnSelect");
        buttonFeedbackProp = serializedObject.FindProperty("buttonFeedback");
        buttonSoundProp = serializedObject.FindProperty("buttonSound");
        saveSettingsProp = serializedObject.FindProperty("saveSettings");
    }

    public override void DrawOrderGUI()
    {
        MenuChoice t = target as MenuChoice;
        var engine = (BasicFlowEngine)t.GetEngine();
        if (engine == null)
        {
            return;
        }

        serializedObject.Update();

        EditorGUILayout.PropertyField(textProp);

        EditorGUILayout.PropertyField(descriptionProp);

        EditorGUILayout.BeginHorizontal();
        NodeEditor.NodeField(targetNodeProp,
                               new GUIContent("Target Node", "Node to call when option is selected"),
                               new GUIContent("<None>"),
                               engine);
        const int popupWidth = 17;
        if (targetNodeProp.objectReferenceValue == null && GUILayout.Button("+", GUILayout.MaxWidth(popupWidth)))
        {
            engine.SelectedNode = t.ParentNode;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(hideIfVisitedProp);
        EditorGUILayout.PropertyField(interactableProp);
        EditorGUILayout.PropertyField(setMenuDialogProp);
        EditorGUILayout.PropertyField(hideThisOptionProp);
        EditorGUILayout.PropertyField(closeMenuOnSelectProp);
        EditorGUILayout.PropertyField(buttonFeedbackProp);
        EditorGUILayout.PropertyField(buttonSoundProp);
        EditorGUILayout.PropertyField(saveSettingsProp);

        serializedObject.ApplyModifiedProperties();
    }
}

