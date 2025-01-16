using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

//popup window for selecting an event handler
public class EventSelectorPopupWindowContent : BasePopupWindowContent
{
    static List<System.Type> _eventHandlerTypes;
    static List<System.Type> EventHandlerTypes
    {
        get
        {
            if (_eventHandlerTypes == null || _eventHandlerTypes.Count == 0)
                CacheEventHandlerTypes();

            return _eventHandlerTypes;
        }
    }
    static void CacheEventHandlerTypes()
    {
        _eventHandlerTypes = EditorExtensions.FindDerivedTypes(typeof(EventHandler)).Where(x => !x.IsAbstract).ToList();
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        CacheEventHandlerTypes();
    }

    protected class SetEventHandlerOperation
    {
        public Node node;
        public Type eventHandlerType;
    }

    protected Node node;
    protected Group group;
    public EventSelectorPopupWindowContent(string currentHandlerName, Node node, int width, int height, Group group)
        : base(currentHandlerName, width, height, true)
    {
        if (node != null)
            this.node = node;
        if (group != null)
            this.group = group;
    }

    protected override void PrepareAllItems()
    {
        int i = 0;
        foreach (Type type in EventHandlerTypes)
        {
            EventHandlerInfoAttribute info = EventHandlerEditor.GetEventHandlerInfo(type);
            if (info != null)
            {
                allItems.Add(new FilteredListItem(i, (info.Category.Length > 0 ? info.Category + CATEGORY_CHAR : "") + info.EventHandlerName, info.HelpText));
            }
            else
            {
                allItems.Add(new FilteredListItem(i, type.Name, info.HelpText));
            }
            i++;
        }
    }

    protected override void SelectByOrigIndex(int index)
    {
        SetEventHandlerOperation operation = new SetEventHandlerOperation();
        operation.node = node;
        operation.eventHandlerType = (index >= 0 && index < EventHandlerTypes.Count) ? EventHandlerTypes[index] : null;
        OnSelectEventHandler(operation);
    }

    public static void DoEventHandlerPopup(Rect pos, string handlerName, Node node, int width, int height)
    {
        EventSelectorPopupWindowContent content = new EventSelectorPopupWindowContent(handlerName, node, width, height, null);
        PopupWindow.Show(pos, content);
    }

    public static void DoEventHandlerPopupGroup(Rect pos, string handlerName, Group group, int width, int height)
    {
        EventSelectorPopupWindowContent content = new EventSelectorPopupWindowContent(handlerName, null, width, height, group);
        PopupWindow.Show(pos, content);
    }

    protected static void OnSelectEventHandler(object obj)
    {
        SetEventHandlerOperation operation = obj as SetEventHandlerOperation;
        Node node = operation.node;
        if (node.GetType() == typeof(Group))
        {
            node = node as Group;
        }
        Type selectedType = operation.eventHandlerType;

        if (node == null)
        {
            return;
        }

        if (node._EventHandler != null)
        {
            Undo.DestroyObjectImmediate(node._EventHandler);
        }

        if (selectedType != null)
        {
            EventHandler newHandler = Undo.AddComponent(node.gameObject, selectedType) as EventHandler;
            newHandler.ParentNode = node;
            node._EventHandler = newHandler;
        }

        //update stale node data here

        // Because this is an async call, we need to force prefab instances to record changes
        PrefabUtility.RecordPrefabInstancePropertyModifications(node);
    }
}