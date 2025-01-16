using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EventHandler), true)]
public class EventHandlerEditor : Editor
{
    protected virtual void DrawProperties()
    {
        EditorGUI.indentLevel++;
        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;
        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;

            if (iterator.name == "m_Script")
            {
                continue;
            }

            EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
        }

        EditorGUI.indentLevel--;
    }

    protected virtual void DrawHelpBox()
    {
        EventHandler t = target as EventHandler;
        EventHandlerInfoAttribute info = EventHandlerEditor.GetEventHandlerInfo(t.GetType());

        if (info != null && info.HelpText.Length > 0)
        {
            EditorGUILayout.HelpBox(info.HelpText, MessageType.Info);
        }
    }

    public static EventHandlerInfoAttribute GetEventHandlerInfo(System.Type eventHandlerType)
    {
        object[] attributes = eventHandlerType.GetCustomAttributes(typeof(EventHandlerInfoAttribute), false);
        foreach (var obj in attributes)
        {
            EventHandlerInfoAttribute eventHandlerInfoAttr = obj as EventHandlerInfoAttribute;
            if (eventHandlerInfoAttr != null)
            {
                return eventHandlerInfoAttr;
            }
        }

        return null;
    }

    public virtual void DrawInspectorGUI()
    {
        serializedObject.Update();
        DrawProperties();
        DrawHelpBox();
        serializedObject.ApplyModifiedProperties();
    }
}
