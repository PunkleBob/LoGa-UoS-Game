using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    [CustomEditor(typeof(ConditionalEventHandler))]
    public class ConditionalEventHandlerEditor : EventHandlerEditor
    {
        public static List<Action> actionList = new List<Action>();

        protected Texture2D addIcon;
        protected Texture2D removeIcon;
        protected Texture2D duplicateIcon;

        private OrderListAdapter orderListAdapter;
        private SerializedProperty fireMode;
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
            removeIcon = LogaEditorResources.Remove;
            duplicateIcon = LogaEditorResources.Duplicate;

            fireMode = serializedObject.FindProperty("fireMode");
            orderListProp = serializedObject.FindProperty("conditions");
            orderListAdapter = new OrderListAdapter(null, orderListProp, null, target as ConditionalEventHandler);

            var h = target as ConditionalEventHandler;

            OrderSelectorPopupWindowContent.curHandler = h;
        }

        protected override void DrawProperties()
        {
            var handler = target as ConditionalEventHandler;

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

            EditorGUILayout.PropertyField(fireMode, new GUIContent("Fire Mode", "When to check the conditions. Start happens once, update continously checks."));

            DrawHandlerToolBar();

            handler.UpdateIndentLevels();

            orderListAdapter.DrawOrderList();

            for (int i = orderListProp.arraySize - 1; i >= 0; --i)
            {
                SerializedProperty orderProperty = orderListProp.GetArrayElementAtIndex(i);
                if (orderProperty.objectReferenceValue == null)
                {
                    orderListProp.DeleteArrayElementAtIndex(i);
                }
            }
        }

        protected void DrawHandlerToolBar()
        {
            var handler = target as ConditionalEventHandler;

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            //using false to prevent forcing a longer row than will fit on smallest inspector -- for now we only want add button
            var pos = EditorGUILayout.GetControlRect(false, 0, EditorStyles.objectField);
            if (pos.x != 0)
            {
                lastCMDpopupPos = pos;
                lastCMDpopupPos.x += EditorGUIUtility.labelWidth;
                lastCMDpopupPos.y += EditorGUIUtility.singleLineHeight * 2;
            }

            if (GUILayout.Button(addIcon))
            {
                OrderSelectorPopupWindowContent.AddOrderCallBack(typeof(If), handler);
            }
            if (GUILayout.Button(duplicateIcon))
            {
                CopyOrder();
                PasteOrder();
            }
            if (GUILayout.Button(removeIcon))
            {
                DeleteCondition();
            }

            GUILayout.EndHorizontal();
        }

        protected void CopyOrder()
        {
            var handler = target as ConditionalEventHandler;
            var engine = handler.ParentNode.GetEngine();

            if (engine == null || target == null)
            {
                return;
            }

            OrderCopyBuffer orderCopyBuffer = OrderCopyBuffer.GetInstance();
            orderCopyBuffer.Clear();

            //go through all orders to deterimine which ones need copying
            foreach (Order order in handler.Conditions)
            {
                if (engine.SelectedOrders.Contains(order))
                {
                    var type = order.GetType();
                    Order newOrder = Undo.AddComponent(orderCopyBuffer.gameObject, type) as Order;
                    var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                    foreach (var field in fields)
                    {
                        //copy all public fields
                        bool copy = field.IsPublic;
                        //copy non public fields that have the SerializeField attribute
                        var attributes = field.GetCustomAttributes(typeof(SerializeField), true);
                        if (attributes.Length > 0)
                        {
                            copy = true;
                        }

                        if (copy)
                        {
                            field.SetValue(newOrder, field.GetValue(order));
                        }
                    }
                }
            }
        }

        protected void PasteOrder()
        {
            var handler = target as ConditionalEventHandler;
            var engine = handler.ParentNode.GetEngine();

            if (engine == null || target == null)
            {
                return;
            }

            //get the index to paste the orders at
            OrderCopyBuffer orderCopyBuffer = OrderCopyBuffer.GetInstance();

            //if there is no selected order, paste at the end of the list
            int pasteIndex = handler.Conditions.Count;
            //if there is a selected order, paste after the last selected order
            if (engine.SelectedOrders.Count > 0)
            {
                for (int i = 0; i < handler.Conditions.Count; i++)
                {
                    Order order = handler.Conditions[i];

                    foreach (Order selectedOrder in engine.SelectedOrders)
                    {
                        if (order == selectedOrder)
                        {
                            pasteIndex = i + 1;
                        }
                    }
                }
            }

            //go through all orders in the copy buffer and paste them
            foreach (Order order in orderCopyBuffer.GetOrders())
            {
                // Using the Editor copy / paste functionality instead of reflection
                // because this does a deep copy
                if (ComponentUtility.CopyComponent(order))
                {
                    // Paste the component as a new component
                    if (ComponentUtility.PasteComponentAsNew(engine.gameObject))
                    {
                        // Get the newly pasted component
                        Order[] orders = engine.GetComponents<Order>();
                        // Get the last pasted component
                        Order pastedOrder = orders.Last<Order>();
                        // Set the item id to a new unique value
                        if (pastedOrder != null)
                        {
                            pastedOrder.ItemId = engine.NextItemId();
                            handler.Conditions.Insert(pasteIndex++, pastedOrder as If);
                        }
                    }

                    // This stops the user pasting the order manually into another game object
                    ComponentUtility.CopyComponent(engine.transform);
                }
            }

            // Because this is an async call, we need to force prefab instances to record changes
            PrefabUtility.RecordPrefabInstancePropertyModifications(handler);

            Repaint();
        }

        protected void DeleteCondition()
        {
            var handler = target as ConditionalEventHandler;
            var engine = handler.ParentNode.GetEngine();

            if (engine == null || target == null)
            {
                return;
            }

            //go through all orders to determine which ones need deleting
            int lastSelectedIndex = 0;
            for (int i = handler.Conditions.Count - 1; i >= 0; i--)
            {
                Order order = handler.Conditions[i];
                foreach (Order selectedOrder in engine.SelectedOrders)
                {
                    if (order == selectedOrder)
                    {
                        //remove the order from the list - important to do this to ensure undo works
                        Undo.DestroyObjectImmediate(order);

                        Undo.RecordObject(handler.ParentNode, "DeleteOrder");
                        handler.Conditions.RemoveAt(i);

                        lastSelectedIndex = i;

                        break;
                    }
                }
            }
        }
    }
}
