using UnityEngine;

namespace LoGaCulture.LUTE
{
    public class VariableImage : UnityEngine.UI.Image
    {
        [VariableReference]
        [SerializeField] protected Sprite spriteVariable;
        void Update()
        {
            SetImage();
        }

        protected virtual void SetImage()
        {
            if (spriteVariable != null)
                sprite = spriteVariable;
        }
    }
}
