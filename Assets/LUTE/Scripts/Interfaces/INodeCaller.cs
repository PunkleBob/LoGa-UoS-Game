/// Interface for indicating that the class holds a reference to and may call a node
public interface INodeCaller
{
    bool MayCallNode(Node node);
}
