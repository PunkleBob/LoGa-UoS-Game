using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
namespace LoGaCulture.LUTE
{
    /// <summary>
    /// Helper class for hiding too many ways to show text in a given context.
    /// Currently we use standard text and TMPRO text.
    /// </summary>
    public class TextAdapter : IWriterTextDestination
    {
        protected Text textUI;
#if UNITY_2018_1_OR_NEWER
        protected TMPro.TMP_Text tmpro;
#endif
        protected PropertyInfo textProperty;
        protected IWriterTextDestination writerTextDestination;

        public void InitFromGameObject(GameObject go, bool includeChildren = false)
        {
            if (go == null)
            {
                return;
            }

            if (!includeChildren)
            {
                textUI = go.GetComponent<Text>();
#if UNITY_2018_1_OR_NEWER
                tmpro = go.GetComponent<TMPro.TMP_Text>();
#endif
                writerTextDestination = go.GetComponent<IWriterTextDestination>();
            }
            else
            {
                textUI = go.GetComponentInChildren<Text>();
#if UNITY_2018_1_OR_NEWER
                tmpro = go.GetComponentInChildren<TMPro.TMP_Text>();
#endif
                writerTextDestination = go.GetComponentInChildren<IWriterTextDestination>();
            }

            // If you have more text components, find them here.
            // Only if the above have not been found.
        }

        public void ForceRichText()
        {
            if (textUI != null)
            {
                textUI.supportRichText = true;
            }
#if UNITY_2018_1_OR_NEWER
            if (tmpro != null)
            {
                tmpro.richText = true;
            }
#endif
            if (writerTextDestination != null)
            {
                writerTextDestination.ForceRichText();
            }
        }

        public void SetTextColour(Color textColour)
        {
            if (textUI != null)
            {
                textUI.color = textColour;
            }
#if UNITY_2018_1_OR_NEWER
            else if (tmpro != null)
            {
                tmpro.color = textColour;
            }
#endif
            else if (writerTextDestination != null)
            {
                writerTextDestination.SetTextColour(textColour);
            }
        }

        public void SetTextAlpha(float textAlpha)
        {
            if (textUI != null)
            {
                Color color = textUI.color;
                color.a = textAlpha;
                textUI.color = color;
            }
#if UNITY_2018_1_OR_NEWER
            else if (tmpro != null)
            {
                tmpro.alpha = textAlpha;
            }
#endif
            else if (writerTextDestination != null)
            {
                writerTextDestination.SetTextAlpha(textAlpha);
            }
        }

        public bool HasTextObject()
        {
            return textUI != null || writerTextDestination != null ||
#if UNITY_2018_1_OR_NEWER
                tmpro != null;
#endif
        }

        public bool SupportsRichText()
        {
            if (textUI != null)
            {
                return textUI.supportRichText;
            }
#if UNITY_2018_1_OR_NEWER
            if (tmpro != null)
            {
                return true;
            }
#endif
            if (writerTextDestination != null)
            {
                return writerTextDestination.SupportsRichText();
            }
            return false;
        }

        public bool SupportsHiddenCharacters()
        {
#if UNITY_2018_1_OR_NEWER
            if (tmpro != null)
            {
                return true;
            }
#endif
            return false;
        }

        public int RevealedCharacters
        {
            get
            {
#if UNITY_2018_1_OR_NEWER
                if (tmpro != null)
                {
                    return tmpro.maxVisibleCharacters;
                }
#endif
                return 0;
            }
            set
            {
#if UNITY_2018_1_OR_NEWER
                if (tmpro != null)
                {
                    tmpro.maxVisibleCharacters = value;
                }
#endif
            }
        }

        public char LastRevealedCharacter
        {
            get
            {
#if UNITY_2018_1_OR_NEWER
                if (tmpro != null && tmpro.textInfo != null && tmpro.textInfo.characterInfo != null)
                {
                    if (tmpro.maxVisibleCharacters < tmpro.textInfo.characterInfo.Length && tmpro.maxVisibleCharacters > 0)
                    {
                        return tmpro.textInfo.characterInfo[tmpro.maxVisibleCharacters - 1].character;
                    }
                }
#endif
                return (char)0;
            }
        }

        public int CharactersToReveal
        {
            get
            {
#if UNITY_2018_1_OR_NEWER
                if (tmpro != null)
                {
                    return tmpro.textInfo.characterCount;
                }
#endif
                return 0;
            }
        }

        public string Text
        {
            get
            {
                if (textUI != null)
                {
                    return textUI.text;
                }
#if UNITY_2018_1_OR_NEWER
                else if (tmpro != null)
                {
                    return tmpro.text;
                }
#endif
                return "";
            }
            set
            {
                if (textUI != null)
                {
                    textUI.text = value;
                }
#if UNITY_2018_1_OR_NEWER
                else if (tmpro != null)
                {
                    tmpro.text = value;
                    tmpro.ForceMeshUpdate();
                }
#endif
            }
        }
    }
}
