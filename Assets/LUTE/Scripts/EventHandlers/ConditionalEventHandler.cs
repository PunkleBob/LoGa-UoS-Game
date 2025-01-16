using System.Collections.Generic;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    [EventHandlerInfo("Conditional",
                  "Condition List",
                  "Executes when the list of conditions has been met")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class ConditionalEventHandler : EventHandler
    {
        public enum FireMode
        {
            Start,
            Update
        }

        [Tooltip("When to check the conditions. Start happens once, update continously checks.")]
        [SerializeField] protected FireMode fireMode = FireMode.Start;
        [Tooltip("The list of conditions to check.")]
        [SerializeField] protected List<Order> conditions = new List<Order>();

        protected bool executionInfoSet = false;

        private bool isComplete;

        public virtual List<Order> Conditions
        {
            get { return conditions; }
        }

        protected virtual void Awake()
        {
            SetExecutionInfo();
        }
        protected virtual void SetExecutionInfo()
        {
            // Give each child order a reference back to its parent block and tell each order its index in the list
            int index = 0;
            for (int i = 0; i < conditions.Count; i++)
            {
                var order = conditions[i];
                if (order == null)
                {
                    continue;
                }
                order.ParentNode = this.ParentNode;
                order.OrderIndex = index++;
            }

            UpdateIndentLevels();

            executionInfoSet = true;
        }

        protected virtual void Update()
        {
            if (Application.isPlaying)
            {
                if (fireMode == FireMode.Update)
                    CheckConditions();
            }

            // The user can modify the order list order while playing in the editor,
            // so we keep the order indices updated every frame
            //There's no need to do this in player builds so we compile this bit out for those builds
#if UNITY_EDITOR
            int index = 0;
            for (int i = 0; i < conditions.Count; i++)
            {
                var order = conditions[i];
                if (order == null)// Null entry will be deleted automatically later
                {
                    continue;
                }
                order.OrderIndex = index++;
            }
#endif
        }

        public virtual void UpdateIndentLevels()
        {
            int indentLevel = 0;
            for (int i = 0; i < conditions.Count; i++)
            {
                var order = conditions[i];
                if (order == null)
                {
                    continue;
                }
                if (order.CloseNode())
                {
                    indentLevel--;
                }
                // Negative indent level is not permitted
                indentLevel = System.Math.Max(indentLevel, 0);
                order.IndentLevel = indentLevel;
                if (order.OpenNode())
                {
                    indentLevel++;
                }
            }
        }

        protected virtual void Start()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            if (fireMode == FireMode.Start)
                CheckConditions();
        }

        protected virtual void CheckConditions()
        {
            if (conditions.Count == 0)
            {
                ExecuteNode();
                return;
            }

            foreach (If condition in conditions)
            {
                if (!condition.EvaluateConditions())
                {
                    isComplete = false;
                    break;
                }
                else
                {
                    isComplete = true;
                }
            }
            if (isComplete)
            {
                ExecuteNode();
            }
        }

        public override string GetSummary()
        {
            return "This node will execute upon all conditions being met.";
        }

        protected void OnDestroy()
        {
            foreach (Order order in conditions)
            {
                if (order != null)
                {
                    if (Application.isPlaying)
                        Destroy(order.gameObject);
                    else
                        DestroyImmediate(order);
                }
            }
        }
    }
}