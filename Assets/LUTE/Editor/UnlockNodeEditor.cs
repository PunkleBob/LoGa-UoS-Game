using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

[CustomEditor(typeof(UnlockNode))]
public class UnlockNodeEditor : OrderEditor
{
    protected SerializedProperty targetNodeProp;
    protected SerializedProperty groupCountProp;
    protected If newIfStatement;
    protected Node targetNode;
    protected Variable thisVar;

    public override void OnEnable()
    {
        base.OnEnable();

        targetNodeProp = serializedObject.FindProperty("targetNode");
        groupCountProp = serializedObject.FindProperty("groupCount");
    }

    public override void OnInspectorGUI()
    {
        DrawOrderGUI();
    }

    public override void DrawOrderGUI()
    {
        UnlockNode unlockNode = (UnlockNode)target;

        var engine = unlockNode.GetEngine();
        if (engine == null)
        {
            return;
        }

        NodeEditor.NodeField(targetNodeProp,
                       new GUIContent("Unlocks Node", "Node to unlock when this option is selected"),
                       new GUIContent("<None>"),
                       engine);

        if (unlockNode.ParentNode is Group)
        {
            EditorGUILayout.PropertyField(groupCountProp);
            serializedObject.ApplyModifiedProperties();
        }

        if (targetNodeProp.objectReferenceValue != null)
        {
            //Only if the variable does not exist we will add this (this logic happens on the engine side)
            if (unlockNode.ParentNode is not Group && !unlockNode.ParentNode.IsGrouped)
                thisVar = engine.AddVariable(typeof(NodeVariable), unlockNode.ParentNode._NodeName, unlockNode.ParentNode) as NodeVariable;
            else
            {
                if (unlockNode.ParentNode.IsGrouped)
                    return;
                thisVar = engine.GetVariable(unlockNode.ParentNode._NodeName) as NodeCollectionVariable;
            }            //If it does not we will add it
            //We also must ensure that this statement is placed before any other orders in the target node
            targetNode = (Node)targetNodeProp.objectReferenceValue;
            if (targetNode.OrderList.Count == 0)
            {
                OrderSelectorPopupWindowContent.AddOrderCallBack(typeof(If), targetNode);
                newIfStatement = (If)targetNode.OrderList[0];
                newIfStatement.conditions[0].AnyVariable.variable = thisVar;
            }
            else
            {
                //Go through all the orders in the target node to ensure that an if statement with this variable does not already exist
                foreach (Order order in targetNode.OrderList)
                {
                    if (order.GetType() == typeof(If))
                    {
                        If ifStatement = (If)order;
                        foreach (ConditionExpression condition in ifStatement.conditions)
                        {
                            if (condition.AnyVariable.variable != null && condition.AnyVariable.variable == thisVar)
                            {
                                newIfStatement = ifStatement;
                                targetNode.OrderList.Remove(newIfStatement);
                                targetNode.OrderList.Insert(0, newIfStatement);
                                if (unlockNode.ParentNode is Group)
                                {
                                    NodeCollectionData nodeCollectionData = new NodeCollectionData
                                    {
                                        total = groupCountProp.intValue
                                    };
                                    newIfStatement.conditions[0].AnyVariable.data.nodeCollectionData = nodeCollectionData;
                                }
                                return;
                            }
                        }
                    }
                }
                OrderSelectorPopupWindowContent.AddOrderCallBack(typeof(If), targetNode);
                newIfStatement = (If)targetNode.OrderList[targetNode.OrderList.Count - 1];
                newIfStatement.conditions[0].AnyVariable.variable = thisVar;
            }
        }
        if (newIfStatement != null)
        {
            //move the new if order up to first position in target order list
            targetNode.OrderList.Remove(newIfStatement);
            targetNode.OrderList.Insert(0, newIfStatement);
            if (unlockNode.ParentNode is Group)
            {
                NodeCollectionData nodeCollectionData = new NodeCollectionData
                {
                    total = groupCountProp.intValue
                };
                newIfStatement.conditions[0].AnyVariable.data.nodeCollectionData = nodeCollectionData;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
