using System;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    [AttributeUsage(AttributeTargets.Field)]
    public class LUTECustomPropAttribute : PropertyAttribute
    {
        public string Namespace { get; private set; }
        public LUTECustomPropAttribute(string namespaceName = "LoGaCulture.LUTE")
        {
            Namespace = namespaceName;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class VariableReferenceAttribute : PropertyAttribute
    {
        public string Namespace { get; }

        public VariableReferenceAttribute(string namespaceName = "LoGaCulture.LUTE")
        {
            Namespace = namespaceName;
        }
    }
}
