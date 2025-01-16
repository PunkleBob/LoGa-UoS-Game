using UnityEngine;

/// Implement this interface to be notified about events relating to writing text (text writer)
public interface IWriterListener
{
    /// <summary>
    /// Called on user input event (e.g. click, key press)
    /// </summary>
    void OnInput();

    void OnStart(AudioClip audioClip);

    //called when the text writer writes a new character glyph
    void OnGlyph();

    /// Called when the Writer has paused writing text (e.g. on a {wi} tag).
    void OnPause();

    /// Called when the Writer has resumed writing text.
    void OnResume();

    /// Called when the Writer has finished.
    /// <param name="stopAudio">Controls whether audio should be stopped when writing ends.</param>
    void OnEnd(bool stopAudio);

    /// <summary>
    /// Called when the Writer has no more Words remaining, but may have waits or other tokens still pending.
    /// Will not be called if there is NO Words for the writer to process in the first place. e.g. Audio only says
    /// do not trigger this.
    /// 
    /// Note that the writer does not know what may happen after it's job is done. If a following Say does
    /// not clear the existing, you'll get what looks like AllWordsWritten and then more words written.
    /// </summary>
    void OnAllWordsWritten();

    /// <summary>
    /// Called when voiceover should start.
    /// </summary>
    void OnVoiceover(AudioClip voiceOverClip);
}
