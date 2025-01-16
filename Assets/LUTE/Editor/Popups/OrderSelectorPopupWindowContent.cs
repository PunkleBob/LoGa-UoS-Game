using LoGaCulture.LUTE;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class OrderSelectorPopupWindowContent : BasePopupWindowContent
{
    static List<Type> _orderTypes;
    static List<Type> OrderTypes
    {
        get
        {
            if (_orderTypes == null || _orderTypes.Count == 0)
                CacheOrderTypes();

            return _orderTypes;
        }
    }

    static void CacheOrderTypes()
    {
        _orderTypes = EditorExtensions.FindDerivedTypes(typeof(Order)).Where(t => !t.IsAbstract).ToList();
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        CacheOrderTypes();
    }

    static Node curNode;
    public static ConditionalEventHandler curHandler;

    static protected List<KeyValuePair<Type, OrderInfoAttribute>> filteredAttributes;

    public OrderSelectorPopupWindowContent(string handlerName, int width, int height) : base(handlerName, width, height)
    {
    }

    protected override void SelectByOrigIndex(int index)
    {
        var orderType = (index >= 0 && index < OrderTypes.Count) ? OrderTypes[index] : null;
        AddOrderCallBack(orderType);
    }

    public static void SelectByType(Type orderType)
    {
        AddOrderCallBack(orderType);
    }

    protected override void PrepareAllItems()
    {
        filteredAttributes = GetFilteredSupportedOrders(curNode.GetEngine());
        foreach (var item in filteredAttributes)
        {
            var newFilteredItem = new FilteredListItem(OrderTypes.IndexOf(item.Key), (item.Value.Category.Length > 0 ? item.Value.Category + CATEGORY_CHAR : "") + item.Value.OrderName, item.Value.HelpText);
            allItems.Add(newFilteredItem);
        }
    }

    static public void ShowOrderMenu(Rect pos, string currentHandlerName, Node node, int width, int height)
    {
        curNode = node;

        var win = new OrderSelectorPopupWindowContent(currentHandlerName,
            width, (int)(height - EditorGUIUtility.singleLineHeight * 3));
        PopupWindow.Show(pos, win);
    }

    protected static List<KeyValuePair<Type, OrderInfoAttribute>> GetFilteredSupportedOrders(BasicFlowEngine engine)
    {
        List<KeyValuePair<Type, OrderInfoAttribute>> filteredAttributes = NodeEditor.GetFilteredOrderInfoAttribute(OrderTypes);

        filteredAttributes.Sort(NodeEditor.CompareOrderAttributes);
        filteredAttributes = filteredAttributes.Where(t => engine.IsOrderSupported(t.Value)).ToList();

        return filteredAttributes;
    }

    //Used by GenericMenu Delegate
    static protected void AddOrderCallback(object obj)
    {
        Type order = obj as Type;
        if (order != null)
        {
            AddOrderCallback(order);
        }
    }

    public static void AddOrderCallBack(Type orderType, Node customNode = null)
    {
        var node = curNode;

        if (customNode != null)
        {
            node = customNode;
        }

        if (node == null || orderType == null)
        {
            return;
        }

        var engine = node.GetEngine();

        // Use index of last selected order in list, or end of list if nothing selected.
        int index = -1;
        foreach (var order in engine.SelectedOrders)
        {
            if (order.OrderIndex + 1 > index)
            {
                index = order.OrderIndex + 1;
            }
        }
        if (index == -1)
            index = node.OrderList.Count;

        var newOrder = Undo.AddComponent(node.gameObject, orderType) as Order;
        node.GetEngine().AddSelectedOrder(newOrder);
        newOrder.ParentNode = node;
        newOrder.ItemId = engine.NextItemId();

        newOrder.OnOrderAdded(node);

        //record undo action here

        if (index < node.OrderList.Count - 1)
        {
            node.OrderList.Insert(index, newOrder);
        }
        else
        {
            node.OrderList.Add(newOrder);
        }

        // Because this is an async call, we need to force prefab instances to record changes
        PrefabUtility.RecordPrefabInstancePropertyModifications(node);

        engine.ClearSelectedOrders();
        engine.AddSelectedOrder(newOrder);
    }

    public static void AddOrderCallBack(Type orderType, EventHandler handler)
    {
        var newHandler = curHandler;

        if (newHandler == null || orderType == null)
        {
            return;
        }

        var engine = newHandler.ParentNode.GetEngine();

        // Use index of last selected order in list, or end of list if nothing selected.
        int index = -1;
        foreach (var order in engine.SelectedOrders)
        {
            if (order.OrderIndex + 1 > index)
            {
                index = order.OrderIndex + 1;
            }
        }
        if (index == -1)
            index = newHandler.Conditions.Count;

        var newOrder = Undo.AddComponent(newHandler.gameObject, orderType) as Order;
        engine.AddSelectedOrder(newOrder);
        newOrder.ItemId = engine.NextItemId();

        //record undo action here

        if (index < newHandler.Conditions.Count - 1)
        {
            newHandler.Conditions.Insert(index, newOrder);
        }
        else
        {
            newHandler.Conditions.Add(newOrder);
        }

        // Because this is an async call, we need to force prefab instances to record changes
        PrefabUtility.RecordPrefabInstancePropertyModifications(newHandler);

        engine.ClearSelectedOrders();
        engine.AddSelectedOrder(newOrder);
    }
}