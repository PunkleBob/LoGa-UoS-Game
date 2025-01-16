namespace LoGaCulture.LUTE
{
    public class WriterSignals
    {
        /// <summary>
        /// WriterState signal. Sent when the writer changes state.
        /// </summary>
        public static event WriterStateHandler OnWriterState;
        public delegate void WriterStateHandler(TextWriter writer, WriterState writerState);
        public static void DoWriterState(TextWriter writer, WriterState writerState)
        {
            if (OnWriterState != null) OnWriterState(writer, writerState);
        }
        /// <summary>
        /// TextTagToken signal. Sent for each unique token when writing text.
        /// </summary>
        public static event TextTagTokenHandler OnTextTagToken;
        public delegate void TextTagTokenHandler(TextWriter writer, TextTagToken token, int index, int maxIndex);
        public static void DoTextTagToken(TextWriter writer, TextTagToken token, int index, int maxIndex) { if (OnTextTagToken != null) OnTextTagToken(writer, token, index, maxIndex); }

        /// <summary>
        /// WriterInput signal. Sent when the writer receives player input.
        /// </summary>
        public static event WriterInputHandler OnWriterInput;
        public delegate void WriterInputHandler(TextWriter writer);
        public static void DoWriterInput(TextWriter writer) { if (OnWriterInput != null) OnWriterInput(writer); }

        /// <summary>
        /// WriterGlyph signal. Sent when the writer writes out a glyph.
        /// </summary>
        public delegate void WriterGlyphHandler(TextWriter writer);
        public static event WriterGlyphHandler OnWriterGlyph;
        public static void DoWriterGlyph(TextWriter writer) { if (OnWriterGlyph != null) OnWriterGlyph(writer); }
    }
}
