
/// <summary>
/// A collection of nodes in which the editor script will allow the creation based on all nodes from the engine
/// </summary>
public class NodeCollection : GenericCollection<Node>
{
    public virtual BasicFlowEngine GetEngine()
    {
        var engine = GetComponent<BasicFlowEngine>();
        if (engine == null &&
            transform.parent != null)
        {
            engine = transform.parent.GetComponent<BasicFlowEngine>();
        }
        return engine;
    }
}
