using MoreMountains.Tools;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Achievement))]
public class AchievementEditor : OrderEditor
{
    protected SerializedProperty achievementProp;
    protected SerializedProperty progressProp;
    protected SerializedProperty amountProp;
    protected SerializedProperty feedbackProp;
    protected SerializedProperty targetEngineProp;
    protected SerializedProperty nodeProp;
    protected SerializedProperty startIndexProp;
    protected SerializedProperty callModeProp;

    protected List<MMAchievement> Achievements;

    protected int achievementIndex = 0;

    public override void OnEnable()
    {
        base.OnEnable();
        achievementProp = serializedObject.FindProperty("achievementID");
        progressProp = serializedObject.FindProperty("progress");
        amountProp = serializedObject.FindProperty("amount");
        feedbackProp = serializedObject.FindProperty("achievementFeedback");
        targetEngineProp = serializedObject.FindProperty("targetEngine");
        nodeProp = serializedObject.FindProperty("triggerNode");
        startIndexProp = serializedObject.FindProperty("startIndex");
        callModeProp = serializedObject.FindProperty("callMode");
    }

    public override void OnInspectorGUI()
    {
        DrawOrderGUI();
    }

    public override void DrawOrderGUI()
    {
        Achievement t = target as Achievement;
        BasicFlowEngine engine = null;
        if (targetEngineProp.objectReferenceValue == null)
        {
            engine = (BasicFlowEngine)t.GetEngine();
        }
        else
        {
            engine = targetEngineProp.objectReferenceValue as BasicFlowEngine;
        }

        if (engine == null)
            return;

        var achievements = engine.GetComponentInChildren<AchievementRules>()?.AchievementList.Achievements;

        if (achievements == null)
        {
            return;
        }

        if (achievements.Count == 0)
        {
            return;
        }

        for (int i = 0; i < achievements.Count; i++)
        {
            if (achievements[i].AchievementID == achievementProp.stringValue)
            {
                achievementIndex = i;
            }
        }

        achievementIndex = EditorGUILayout.Popup("Achievement", achievementIndex, achievements.Select(x => x.AchievementID).ToArray());
        achievementProp.stringValue = achievements[achievementIndex].AchievementID;

        EditorGUILayout.PropertyField(progressProp);
        EditorGUILayout.PropertyField(amountProp);
        EditorGUILayout.PropertyField(feedbackProp);
        EditorGUILayout.PropertyField(targetEngineProp);
        if (engine != null)
        {
            NodeEditor.NodeField(nodeProp,
                                   new GUIContent("Trigger Node", "Node to call when the achievement has completed"),
                                   new GUIContent("<None>"),
                                   engine);
            EditorGUILayout.PropertyField(startIndexProp);
            EditorGUILayout.PropertyField(callModeProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
