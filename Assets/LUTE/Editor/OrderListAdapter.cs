using LoGaCulture.LUTE;
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


public class OrderListAdapter
{
    public void DrawOrderList()
    {
        if (summaryStyle == null)
        {
            summaryStyle = new GUIStyle();
            summaryStyle.fontSize = 10;
            summaryStyle.padding.top += 5;
            summaryStyle.richText = true;
            summaryStyle.wordWrap = false;
            summaryStyle.clipping = TextClipping.Clip;
        }

        if (orderLabelStyle == null)
        {
            orderLabelStyle = new GUIStyle(GUI.skin.label);
            orderLabelStyle.fontStyle = FontStyle.Bold;
            orderLabelStyle.normal.background = LogaEditorResources.OrderBackground;
            orderLabelStyle.normal.textColor = Color.black;
            int borderSize = 5;
            orderLabelStyle.border.top = borderSize;
            orderLabelStyle.border.bottom = borderSize;
            orderLabelStyle.border.left = borderSize;
            orderLabelStyle.border.right = borderSize;
            orderLabelStyle.alignment = TextAnchor.MiddleLeft;
            orderLabelStyle.richText = true;
            orderLabelStyle.fontSize = 11;
            orderLabelStyle.padding.top -= 1;
            orderLabelStyle.alignment = TextAnchor.MiddleLeft;
        }

        // If collapsed, show an info box and skip drawing the list
        if (isCollapsed)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("List is collapsed. ", EditorStyles.helpBox, GUILayout.ExpandWidth(true));

            // Draw the toggle button inside the help box
            if (GUILayout.Button("►", GUILayout.Width(30))) // Small button
            {
                isCollapsed = false; // Expand the list
            }
            EditorGUILayout.EndHorizontal();

            return;
        }

        if (node != null && node.OrderList.Count == 0 || group != null && group.OrderList.Count == 0 || handler != null && handler.Conditions.Count == 0)
        {
            EditorGUILayout.HelpBox(overrideDesc, MessageType.Info);
        }
        else
        {
            EditorGUI.indentLevel++;
            list.DoLayoutList();
            EditorGUI.indentLevel--;
        }
    }

    protected SerializedProperty _arrayProp;
    protected ReorderableList list;
    protected Node node;
    protected Group group;
    protected ConditionalEventHandler handler;
    protected GUIStyle summaryStyle, orderLabelStyle;
    protected string overrideName = "Orders";
    protected string overrideDesc = "Press the add button to add an order to the list...";

    private bool isCollapsed = false;

    public SerializedProperty this[int index]
    {
        get { return _arrayProp.GetArrayElementAtIndex(index); }
    }

    public SerializedProperty arrayProperty
    {
        get { return _arrayProp; }
    }

    public OrderListAdapter(Node _node, SerializedProperty arrayProp, Group _group, ConditionalEventHandler _handler = null)
    {
        if (arrayProp == null)
            throw new ArgumentNullException("Array property was null");
        if (!arrayProp.isArray)
            throw new InvalidOperationException("Specified serialised propery is not an array");
        this._arrayProp = arrayProp;
        this.node = _node;
        this.group = _group;
        this.handler = _handler;

        if (this.handler != null)
        {
            this.overrideName = "Conditions";
            this.overrideDesc = "Press the add button to add a condition to the list...";
        }

        list = new ReorderableList(arrayProp.serializedObject, arrayProp, true, true, false, false);
        list.drawHeaderCallback = DrawHeader;
        list.drawElementCallback = DrawItem;
        list.onSelectCallback = SelectChanged;
    }

    private void DrawItem(Rect rect, int index, bool isActive, bool isFocused)
    {
        // this simply draws the list item (not the actual content that will be edited) - if the inspector is closed then return
        if (rect.width < 0) return;

        if (isCollapsed)
        {
            return; // Don't draw elements when collapsed
        }

        Order order = this[index].objectReferenceValue as Order;

        if (order == null)
            return;

        OrderInfoAttribute orderInfo = OrderEditor.GetOrderInfo(order.GetType());
        if (orderInfo == null)
            return;

        var engine = (BasicFlowEngine)order.GetEngine();
        if (engine == null)
            return;

        bool isComment = order.GetType() == typeof(Comment);
        // bool isLabel = node.GetType() == typeof(Label);

        string summary = order.GetSummary();
        if (summary == null)
            summary = "";
        else
            summary = summary.Replace("\n", "").Replace("\r", "");

        if (summary.StartsWith("Error:"))
        {
            summary = "<color=red> " + summary + "</color>";
        }

        if (isComment)
        {
            summary = "<b> " + summary + "</b>";
        }
        else
        {
            summary = "<i>" + summary + "</i>";
        }

        bool orderIsSelected = false;
        foreach (Order selectedCOrder in engine.SelectedOrders)
        {
            if (selectedCOrder == order)
            {
                orderIsSelected = true;
                break;
            }
        }

        string orderName = orderInfo.OrderName;

        float indentSize = 20;
        for (int i = 0; i < order.IndentLevel; ++i)
        {
            Rect indentRect = rect;
            indentRect.x += i * indentSize;// - 21;
            indentRect.width = indentSize + 1;
            indentRect.y -= 2;
            indentRect.height += 5;
            GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            GUI.Box(indentRect, "", orderLabelStyle);
        }

        float orderNameWidth = Mathf.Max(orderLabelStyle.CalcSize(new GUIContent(orderName)).x, 90f);
        float indentWidth = order.IndentLevel * indentSize;

        Rect orderLabelRect = rect;
        orderLabelRect.x += indentWidth;// - 21;
        orderLabelRect.y -= 2;
        orderLabelRect.width -= (indentSize * order.IndentLevel);// - 22);
        orderLabelRect.height += 5;

        Rect clickRect = rect;

        // Select Order via left-click
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
        {
            if (engine.SelectedOrders.Contains(order) && Event.current.button == 0)
            {
                // Left-clicking on already selected order
                // Command-key or shift not pressed
                if (!EditorGUI.actionKey && !Event.current.shift)
                {
                    NodeEditor.actionList.Add(delegate
                    {
                        engine.SelectedOrders.Remove(order);
                        engine.ClearSelectedOrders();
                    });
                }

                // Command-key pressed
                if (EditorGUI.actionKey)
                {
                    NodeEditor.actionList.Add(delegate
                    {
                        engine.SelectedOrders.Remove(order);
                    });
                    Event.current.Use();
                }
            }
            else
            {
                bool shift = Event.current.shift;

                // Left-click and no command key
                if (!shift && !EditorGUI.actionKey && Event.current.button == 0)
                {
                    NodeEditor.actionList.Add(delegate
                    {
                        engine.ClearSelectedOrders();
                    });
                    Event.current.Use();
                    list.index = index;
                }

                NodeEditor.actionList.Add(delegate
                {
                    engine.AddSelectedOrder(order);
                });

                // Find first and last selected order
                int firstSelectedIndex = -1;
                int lastSelectedIndex = -1;
                if (engine.SelectedOrders.Count > 0)
                {
                    if (engine.SelectedNode != null)
                    {
                        for (int i = 0; i < engine.SelectedNode.OrderList.Count; i++)
                        {
                            Order orderInNode = engine.SelectedNode.OrderList[i];
                            foreach (Order selectedOrder in engine.SelectedOrders)
                            {
                                if (orderInNode == selectedOrder)
                                {
                                    lastSelectedIndex = i;
                                    break;
                                }
                            }
                        }
                        for (int i = engine.SelectedNode.OrderList.Count - 1; i >= 0; i--)
                        {
                            Order orderInNode = engine.SelectedNode.OrderList[i];
                            foreach (Order selectedOrder in engine.SelectedOrders)
                            {
                                if (orderInNode == selectedOrder)
                                {
                                    firstSelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (shift)
                {
                    int currentIndex = order.OrderIndex;
                    if (firstSelectedIndex == -1 || lastSelectedIndex == -1)
                    {
                        // No selected order so we select whole list
                        firstSelectedIndex = 0;
                        lastSelectedIndex = currentIndex;
                    }
                    else
                    {
                        if (currentIndex < firstSelectedIndex)
                        {
                            firstSelectedIndex = currentIndex;
                        }
                        if (currentIndex > lastSelectedIndex)
                        {
                            lastSelectedIndex = currentIndex;
                        }
                    }
                    for (int i = Math.Min(firstSelectedIndex, lastSelectedIndex); i < Math.Max(firstSelectedIndex, lastSelectedIndex); i++)
                    {
                        var selectedOrder = engine.SelectedNode.OrderList[i];
                        NodeEditor.actionList.Add(delegate
                        {
                            engine.AddSelectedOrder(selectedOrder);
                        });
                    }
                }
                Event.current.Use();
            }
            GUIUtility.keyboardControl = 0; // Fix for textarea not refeshing (change focus)
        }

        Color orderLabelColor = Color.white;
        if (engine.ColourOrders)
        {
            orderLabelColor = order.GetButtonColour();
        }

        if (orderIsSelected)
        {
            orderLabelColor = Color.green;
        }
        else if (!order.enabled)
        {
            orderLabelColor = Color.grey;
        }

        GUI.backgroundColor = orderLabelColor;

        if (isComment)
        {
            GUI.Label(orderLabelRect, "", orderLabelStyle);
        }
        else
        {
            string orderNameLabel;
            if (engine.ShowLineNumbers)
            {
                orderNameLabel = order.OrderIndex.ToString() + ": " + orderName;
            }
            else
            {
                orderNameLabel = orderName;
            }

            GUI.Label(orderLabelRect, orderNameLabel, orderLabelStyle);
        }

        //you then want to handle mouse clicks on the order in the list - adding it if lclick only then removing if ctrl + lclick
        //you can set your label colour of each order depending if it is selected or not

        if (order.ExecutingIconTimer > Time.realtimeSinceStartup)
        {
            Rect iconRect = new Rect(orderLabelRect);
            iconRect.x += iconRect.width - orderLabelRect.width - 20;
            iconRect.width = 20;
            iconRect.height = 20;

            Color storeColor = GUI.color;

            float alpha = (order.ExecutingIconTimer - Time.realtimeSinceStartup) / LogaConstants.ExecutingIconFadeTime;
            alpha = Mathf.Clamp01(alpha);

            GUI.color = new Color(1, 1, 1, alpha);
            GUI.Label(iconRect, LogaEditorResources.PlayBig, new GUIStyle());

            GUI.color = storeColor;
        }

        Rect summaryRect = new Rect(orderLabelRect);
        if (isComment)
        {
            summaryRect.x += 5;
        }
        else
        {
            summaryRect.x += orderNameWidth + 5;
            summaryRect.width -= orderNameWidth + 5;
        }

        GUI.Label(summaryRect, summary, summaryStyle);

        GUI.backgroundColor = Color.white;
    }

    private void DrawHeader(Rect rect)
    {
        if (rect.width < 0) return;

        // Adjust rects for the label and button
        Rect buttonRect = new Rect(rect.x + rect.width - 25, rect.y, 25, rect.height);

        EditorGUI.LabelField(rect, new GUIContent(overrideName));

        if (GUI.Button(buttonRect, isCollapsed ? "►" : "▼"))
        {
            isCollapsed = !isCollapsed; // Toggle the collapsed state
        }
    }

    private void SelectChanged(ReorderableList list)
    {
        Order order = this[list.index].objectReferenceValue as Order;
        var engine = (BasicFlowEngine)order.GetEngine();
        NodeEditor.actionList.Add(delegate
        {
            engine.ClearSelectedOrders();
            engine.AddSelectedOrder(order);
        });

        ConditionalEventHandlerEditor.actionList.Add(delegate
        {
            engine.ClearSelectedOrders();
            engine.AddSelectedOrder(order);
        });
    }
}
