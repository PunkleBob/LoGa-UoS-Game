using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Group))]
public class GroupEditor : Editor
{
    public static List<Action> actionList = new List<Action>();
    protected Texture2D addIcon;
    protected Texture2D duplicateIcon;
    protected Texture2D removeIcon;


    private OrderListAdapter orderListAdapter;
    private SerializedProperty orderListProp;

    private Rect lastEventPopupPos, lastCMDpopupPos;

    protected virtual void OnEnable()
    {
        //this appears to happen when leaving playmode
        try
        {
            if (serializedObject == null)
                return;
        }
        catch (Exception)
        {
            return;
        }

        addIcon = LogaEditorResources.Add;
        duplicateIcon = LogaEditorResources.Duplicate;
        removeIcon = LogaEditorResources.Remove;

        orderListProp = serializedObject.FindProperty("orderList");
        orderListAdapter = new OrderListAdapter(target as Node, orderListProp, target as Group);
    }

    public virtual void DrawGroupGUI(BasicFlowEngine engine)
    {
        serializedObject.Update();
        var group = target as Group;

        // Execute any queued cut, copy, paste, etc. operations from the prevous GUI update
        // We need to defer applying these operations until the following update because
        // the ReorderableList control emits GUI errors if you clear the list in the same frame
        // as drawing the control (e.g. select all and then delete)
        if (Event.current.type == EventType.Layout)
        {
            foreach (Action action in actionList)
            {
                if (action != null)
                {
                    action();
                }
            }
            actionList.Clear();
        }

        //first thing to do is ensure that each command has a reference to its parent node
        foreach (var order in group.OrderList)
        {
            if (order == null) // Will be deleted from the list later on
            {
                continue;
            }
            //ensure that for all the orders on this group that they have a reference to each node in the group
            foreach (Node node in group.GroupedNodes)
            {
                order.ParentNode = node;
            }
        }

        EditorGUILayout.Space();

        // Draw the event handler popup
        DrawEventHandlerGUI(engine);

        EditorGUILayout.Space(15);

        //then we draw the order list using a custom script
        orderListAdapter.DrawOrderList();

        //last thing to do is delete any null orders from the list (if they have been deleted or renamed)
        for (int i = orderListProp.arraySize - 1; i >= 0; --i)
        {
            SerializedProperty orderProperty = orderListProp.GetArrayElementAtIndex(i);
            if (orderProperty.objectReferenceValue == null)
            {
                orderListProp.DeleteArrayElementAtIndex(i);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    public virtual void DrawGroupToolBar()
    {
        var group = target as Group;

        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        var pos = EditorGUILayout.GetControlRect(false, 0, EditorStyles.objectField);
        if (pos.x != 0)
        {
            lastCMDpopupPos = pos;
            lastCMDpopupPos.x += EditorGUIUtility.labelWidth;
            lastCMDpopupPos.y += EditorGUIUtility.singleLineHeight * 2;
        }
        // Add Button
        if (GUILayout.Button(addIcon))
        {
            //this may be less reliable for HDPI scaling but previous method using editor window height is now returning 
            //  null in 2019.2 suspect ongoing ui changes, so default to screen.height and then attempt to get the better result
            int h = Screen.height;
            if (EditorWindow.focusedWindow != null) h = (int)EditorWindow.focusedWindow.position.height;
            else if (EditorWindow.mouseOverWindow != null) h = (int)EditorWindow.mouseOverWindow.position.height;

            OrderSelectorPopupWindowContent.ShowOrderMenu(lastCMDpopupPos, "", target as Node,
                (int)(EditorGUIUtility.currentViewWidth),
                (int)(h - lastCMDpopupPos.y));
        }
        // if (GUILayout.Button(duplicateIcon))
        // {
        //     CopyOrder();
        //     PasteOrder();
        // }
        // if (GUILayout.Button(removeIcon))
        // {
        //     DeleteOrder();
        // }

        GUILayout.EndHorizontal();
    }

    protected virtual void DrawEventHandlerGUI(BasicFlowEngine engine)
    {
        //show all available event handlers in a popup
        Group group = target as Group;
        Type currentType = null;
        if (group._EventHandler != null)
        {
            currentType = group._EventHandler.GetType();
        }

        string currentName = "<None>";
        if (currentType != null)
        {
            EventHandlerInfoAttribute info = EventHandlerEditor.GetEventHandlerInfo(currentType);
            if (info != null)
            {
                currentName = info.EventHandlerName;
            }
        }

        var pos = EditorGUILayout.GetControlRect(true, 0, EditorStyles.objectField);
        if (pos.x != 0)
        {
            lastEventPopupPos = pos;
            lastEventPopupPos.x += EditorGUIUtility.labelWidth;
            lastEventPopupPos.y += EditorGUIUtility.singleLineHeight;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(new GUIContent("Activate By: "), EditorStyles.largeLabel);
        if (EditorGUILayout.DropdownButton(new GUIContent(currentName), FocusType.Passive))
        {
            EventSelectorPopupWindowContent.DoEventHandlerPopupGroup(lastEventPopupPos, currentName, group, (int)(EditorGUIUtility.currentViewWidth - lastEventPopupPos.x), 200);
        }
        EditorGUILayout.EndHorizontal();

        if (group._EventHandler != null)
        {
            EventHandlerEditor eventHandlerEditor = Editor.CreateEditor(group._EventHandler) as EventHandlerEditor;
            if (eventHandlerEditor != null)
            {
                EditorGUI.BeginChangeCheck();
                eventHandlerEditor.DrawInspectorGUI();

                if (EditorGUI.EndChangeCheck())
                {
                    //set stale node data here 
                }

                DestroyImmediate(eventHandlerEditor);
            }
        }
    }
}
