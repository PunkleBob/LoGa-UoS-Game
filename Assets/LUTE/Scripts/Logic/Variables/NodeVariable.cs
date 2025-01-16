using UnityEngine;

[VariableInfo("Nodes", "Node")]
[AddComponentMenu("")]
[System.Serializable]
public class NodeVariable : BaseVariable<Node>
{
    public override bool Evaluate(ComparisonOperator comparisonOperator, Node value)
    {
        bool condition = false;
        switch (comparisonOperator)
        {
            case ComparisonOperator.Equals:
                if (Value.NodeComplete)
                    condition = true;
                break;
            case ComparisonOperator.NotEquals:
                if (!Value.NodeComplete)
                    condition = false;
                break;
            default:
                condition = base.Evaluate(comparisonOperator, value);
                break;
        }

        return condition;
    }

    public override void Apply(SetOperator setOperator, Node value)
    {
        switch (setOperator)
        {
            default:
                base.Apply(setOperator, value);
                break;
        }
    }
}

/// Container for a node variable reference or constant value.
[System.Serializable]
public struct NodeData
{
    [SerializeField]
    [VariableProperty("<Value>", typeof(NodeVariable))]
    public NodeVariable nodeRef;

    [SerializeField]
    public Node nodeVal;

    public NodeData(Node n)
    {
        nodeVal = n;
        nodeRef = null;
    }

    public static implicit operator Node(NodeData nodeData)
    {
        return nodeData.Value;
    }

    public Node Value
    {
        get { return (nodeRef == null) ? nodeVal : nodeRef.Value; }
        set { if (nodeRef == null) { nodeVal = value; } else { nodeRef.Value = value; } }
    }

    public string GetDescription()
    {
        if (nodeRef == null)
        {
            return "Complete";
        }
        else
        {
            return nodeRef.Key;
        }
    }
}