using TMPro;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    public class VariableTMProText : TextMeshProUGUI
    {
        [VariableReference(null)]
        [SerializeField] protected string stringVariable;

        protected virtual void Update()
        {
            SetText();
        }

        protected virtual void SetText()
        {
            if (stringVariable != null && !string.IsNullOrEmpty(stringVariable))
                text = stringVariable;
        }
    }
}