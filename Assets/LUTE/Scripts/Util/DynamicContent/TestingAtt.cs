using UnityEngine;

namespace LoGaCulture.LUTE
{
    public class TestingAtt : MonoBehaviour
    {
        [VariableReference]
        public Sprite spriteTest;

        [VariableReference(null)]
        public string stringTest;

        public SpriteRenderer sr;

        protected virtual void Update()
        {
            if (sr != null && spriteTest != null)
            {
                sr.sprite = spriteTest;
            }

            if (!string.IsNullOrEmpty(stringTest))
                Debug.Log(stringTest);
        }
    }
}