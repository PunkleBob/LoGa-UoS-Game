using UnityEngine;

namespace LoGaCulture.LUTE
{
    /// <summary>
    /// Interface that allows text to be routed to a specific destination.
    /// </summary>
    public interface IWriterTextDestination
    {
        string Text { get; set; }

        void ForceRichText();
        void SetTextColour(Color textColor);
        void SetTextAlpha(float textAlpha);
        bool HasTextObject();
        bool SupportsRichText();
    }
}
