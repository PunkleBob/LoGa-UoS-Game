using UnityEngine;
using UnityEngine.UI;

namespace LoGaCulture.LUTE
{
    public class NodeButton : Button
    {
        [SerializeField] protected Node targetNode;

        [SerializeField] private BasicFlowEngine targetEngine;

        protected override void Start()
        {
            if (targetEngine == null)
            {
                targetEngine = FindObjectOfType<BasicFlowEngine>();
            }
            onClick.AddListener(() =>
            {
                OnClick();
            });
        }

        public void OnClick(Node customNode = null)
        {
            if (customNode != null)
            {
                // Alllows for the button to be used to execute a different node than the one assigned in the inspector
                targetNode = customNode;
            }

            if (targetEngine == null)
            {
                return;
            }

            targetEngine.ExecuteNode(targetNode);
        }
    }
}
