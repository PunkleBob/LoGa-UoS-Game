using LoGaCulture.LUTE;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Dialogue))]
public class DialogueEditor : OrderEditor
{
    public static bool showTagHelp;

    public static void DrawTagHelpLabel()
    {
        string tagText = TextTagParser.GetTagHelp();

        if (CustomTag.activeCustomTags.Count > 0)
        {
            List<Transform> activeCustomTagGroup = new List<Transform>();
            foreach (CustomTag ct in CustomTag.activeCustomTags)
            {
                if (ct.transform.parent != null)
                {
                    if (!activeCustomTagGroup.Contains(ct.transform.parent.transform))
                    {
                        activeCustomTagGroup.Add(ct.transform.parent.transform);
                    }
                }
                else
                {
                    activeCustomTagGroup.Add(ct.transform);
                }
            }
            foreach (Transform parent in activeCustomTagGroup)
            {
                string tagName = parent.name;
                string tagStartSymbol = "";
                string tagEndSymbol = "";
                CustomTag parentTag = parent.GetComponent<CustomTag>();
                if (parentTag != null)
                {
                    tagName = parentTag.name;
                    tagStartSymbol = parentTag.TagStartSymbol;
                    tagEndSymbol = parentTag.TagEndSymbol;
                }
                tagText += "\n\n\t" + tagStartSymbol + " " + tagName + " " + tagEndSymbol;
                foreach (Transform child in parent)
                {
                    tagName = child.name;
                    tagStartSymbol = "";
                    tagEndSymbol = "";
                    CustomTag childTag = child.GetComponent<CustomTag>();
                    if (childTag != null)
                    {
                        tagName = childTag.name;
                        tagStartSymbol = childTag.TagStartSymbol;
                        tagEndSymbol = childTag.TagEndSymbol;
                    }
                    tagText += "\n\t      " + tagStartSymbol + " " + tagName + " " + tagEndSymbol;
                }
            }
        }
        tagText += "\n";
        float pixelHeight = EditorStyles.miniLabel.CalcHeight(new GUIContent(tagText), EditorGUIUtility.currentViewWidth);
        EditorGUILayout.SelectableLabel(tagText, GUI.skin.GetStyle("HelpBox"), GUILayout.MinHeight(pixelHeight));
    }

    protected SerializedProperty characterProp;
    protected SerializedProperty typingSpeedProp;
    protected SerializedProperty portraitProp;
    protected SerializedProperty storyTextProp;
    protected SerializedProperty voiceOverClipProp;
    protected SerializedProperty showAlwaysProp;
    protected SerializedProperty showCountProp;
    protected SerializedProperty extendPreviousProp;
    protected SerializedProperty fadeWhenDoneProp;
    protected SerializedProperty waitForClickProp;
    protected SerializedProperty stopVoiceoverProp;
    protected SerializedProperty waitForVOProp;
    protected SerializedProperty setBoxProp;
    protected SerializedProperty showButtonProp;

    public override void OnEnable()
    {
        base.OnEnable();

        characterProp = serializedObject.FindProperty("character");
        portraitProp = serializedObject.FindProperty("characterPortrait");
        storyTextProp = serializedObject.FindProperty("storyText");
        voiceOverClipProp = serializedObject.FindProperty("voiceOverClip");
        showAlwaysProp = serializedObject.FindProperty("showAlways");
        showCountProp = serializedObject.FindProperty("showCount");
        extendPreviousProp = serializedObject.FindProperty("extendPrevious");
        fadeWhenDoneProp = serializedObject.FindProperty("fadeWhenDone");
        waitForClickProp = serializedObject.FindProperty("waitForClick");
        stopVoiceoverProp = serializedObject.FindProperty("stopVoiceover");
        waitForVOProp = serializedObject.FindProperty("waitForVO");
        typingSpeedProp = serializedObject.FindProperty("typingSpeed");
        setBoxProp = serializedObject.FindProperty("setDialogueBox");
        showButtonProp = serializedObject.FindProperty("showButton");
    }

    public override void DrawOrderGUI()
    {
        serializedObject.Update();

        bool showPortraits = false;
        OrderEditor.ObjectField<Character>(characterProp,
                                            new GUIContent("Character", "Character that is speaking"),
                                            new GUIContent("<None>"),
                                            Character.ActiveCharacters);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(" ");
        characterProp.objectReferenceValue = (Character)EditorGUILayout.ObjectField(characterProp.objectReferenceValue, typeof(Character), true);
        EditorGUILayout.EndHorizontal();

        Dialogue t = target as Dialogue;

        // Only show portrait selection if...
        if (t._Character != null &&              // Character is selected
            t._Character.Portraits != null &&    // Character has a portraits field
            t._Character.Portraits.Count > 0)   // Selected Character has at least 1 portrait
        {
            showPortraits = true;
        }

        if (showPortraits)
        {
            OrderEditor.ObjectField<Sprite>(portraitProp,
                                              new GUIContent("Portrait", "Portrait representing speaking character"),
                                              new GUIContent("<None>"),
                                              t._Character.Portraits);
        }
        else
        {
            if (!t.ExtendPrevious)
            {
                t.Portrait = null;
            }
        }

        EditorGUILayout.PropertyField(storyTextProp);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(extendPreviousProp);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(new GUIContent("Tag Help", "View available rich text tags"), new GUIStyle(EditorStyles.miniButton)))
        {
            showTagHelp = !showTagHelp;
        }
        EditorGUILayout.EndHorizontal();

        if (showTagHelp)
        {
            DrawTagHelpLabel();
        }

        EditorGUILayout.PropertyField(typingSpeedProp);

        EditorGUILayout.Separator();

        EditorGUILayout.PropertyField(voiceOverClipProp,
                                      new GUIContent("Voice Over Clip", "Voice over audio to play when the text is displayed"));
        EditorGUILayout.PropertyField(setBoxProp);

        EditorGUILayout.PropertyField(showAlwaysProp);

        if (showAlwaysProp.boolValue == false)
        {
            EditorGUILayout.PropertyField(showCountProp);
        }

        EditorGUILayout.PropertyField(fadeWhenDoneProp);
        EditorGUILayout.PropertyField(waitForClickProp);
        EditorGUILayout.PropertyField(showButtonProp);
        EditorGUILayout.PropertyField(stopVoiceoverProp);
        EditorGUILayout.PropertyField(waitForVOProp);

        if (showPortraits && t.Portrait != null)
        {
            Texture2D characterTexture = t.Portrait.texture;
            float aspect = (float)characterTexture.width / (float)characterTexture.height;
            Rect previewRect = GUILayoutUtility.GetAspectRect(aspect, GUILayout.Width(100), GUILayout.ExpandWidth(true));
            if (characterTexture != null)
            {
                GUI.DrawTexture(previewRect, characterTexture, ScaleMode.ScaleToFit, true, aspect);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}