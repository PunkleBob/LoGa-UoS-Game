/// <summary>
/// Use this class to define signals that can be used to communicate between nodes or be informed of events in node executions
/// </summary>
public static class NodeSignals
{
    public static event NodeStartHandler OnNodeStart;
    public delegate void NodeStartHandler(Node node);

    public static void NodeStart(Node node) { OnNodeStart?.Invoke(node); }

    public static event NodeEndHandler OnNodeEnd;
    public delegate void NodeEndHandler(Node node);

    public static void NodeEnd(Node node) { OnNodeEnd?.Invoke(node); }

    public static event OrderExecuteHandler OnOrderExecute;
    public delegate void OrderExecuteHandler(Node node, Order order, int orderIndex, int maxOrderIndex);

    public static void DoOrderExecute(Node node, Order order, int orderIndex, int maxOrderIndex) { OnOrderExecute?.Invoke(node, order, orderIndex, maxOrderIndex); }

}
