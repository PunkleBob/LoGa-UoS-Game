using UnityEngine;

namespace LoGaCulture.LUTE
{
    [AddComponentMenu("")]
    public abstract class BaseVariableProperty : Order
    {
        public enum GetSet
        {
            Get,
            Set,
        }

        public GetSet getOrSet = GetSet.Get;
    }
}
