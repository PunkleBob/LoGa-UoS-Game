using UnityEngine;

namespace LoGaCulture.LUTE
{
    [OrderInfo("Flow",
             "Stop All Nodes",
             "Stop executing all the Nodes that are contained on the engine.")]
    [AddComponentMenu("")]
    public class StopAllNodes : Order
    {
        [SerializeField] protected BasicFlowEngine engine;
        public override void OnEnter()
        {
            if (engine == null)
            {
                engine = FindObjectOfType<BasicFlowEngine>();
            }
            if (engine == null)
            {
                Continue();
            }

            engine.StopAllNodes();
            Continue();
        }

        public override string GetSummary()
        {
            string engineName = "";
            if (engine != null)
            {
                engineName = engine.name;
            }
            else
                engineName = FindObjectOfType<BasicFlowEngine>().name;
            return "Stops all nodes on engine: " + engineName;
        }
    }
}
