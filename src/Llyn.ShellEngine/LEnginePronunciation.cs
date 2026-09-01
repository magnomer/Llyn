using System;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

/// <summary>
/// The pronunciation half of the engine as stored data — the row an Entry keeps, its syllables and
/// representations, and the audio file hanging from it. This is the stored side; finding a
/// pronunciation or a recording on the web is the lookup side, and the two meet only where a chosen
/// recording is downloaded and then saved here.
/// <para>
/// An Entry keeps at most one pronunciation, so it is read by the Entry it belongs to and there is
/// nothing to order. The audio row hangs off the pronunciation rather than the Entry, which is why a
/// recording saved with no typed IPA still needs a pronunciation to hang from.
/// </para>
/// <para>
/// A file is stored workspace-relative and handed out full, the same way a loaded draft's audio is:
/// the shell plays a path and never has to know the workspace folder, and an entry opened from a moved
/// workspace still resolves to a file that is there. A path outside the workspace has no relative form
/// and is kept as it stands.
/// </para>
/// </summary>
public sealed partial class LEngine
{
    /// <summary>
    /// Creates <paramref name="pronunciation"/> with its syllables and representations as its ordered
    /// child rows and returns it with its assigned id.
    /// </summary>
    public LPronunciation LEnginePronunciationCreate(LPronunciation pronunciation)
    {
        ArgumentNullException.ThrowIfNull(pronunciation);
        return new LPronunciationArchive(_lEngineDatabase).LPronunciationCreate(pronunciation);
    }

    /// <summary>
    /// Reads the pronunciation the Entry identified by <paramref name="entryId"/> keeps, with its
    /// syllables and representations, or <c>null</c> when it keeps none.
    /// </summary>
    public LPronunciation? LEnginePronunciationRead(string entryId)
    {
        return new LPronunciationArchive(_lEngineDatabase).LPronunciationRead(entryId);
    }

    /// <summary>
    /// Rewrites the pronunciation <paramref name="pronunciation"/> identifies, its syllables and its
    /// representations. The audio hanging from it is untouched: its id does not change, so the
    /// recording stays attached across an edit of the IPA.
    /// </summary>
    public void LEnginePronunciationUpdate(LPronunciation pronunciation)
    {
        ArgumentNullException.ThrowIfNull(pronunciation);
        new LPronunciationArchive(_lEngineDatabase).LPronunciationUpdate(pronunciation);
    }

    /// <summary>
    /// Deletes the pronunciation identified by <paramref name="id"/> with its syllables,
    /// representations and audio row. The file on disk is not removed — the workspace owns it, and
    /// another entry may have been given the same recording.
    /// </summary>
    public void LEnginePronunciationDelete(string id)
    {
        new LPronunciationArchive(_lEngineDatabase).LPronunciationDelete(id);
    }

    /// <summary>
    /// Saves <paramref name="file"/> as the audio of the pronunciation identified by
    /// <paramref name="pronunciationId"/>, replacing whatever it played before, and records the
    /// <paramref name="source"/> the recording came from. The path is stored relative to the workspace
    /// when it lies inside it, so a moved workspace keeps its audio.
    /// </summary>
    public void LEngineAudioSave(string pronunciationId, string file, string? source)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(file);

        new LPronunciationArchive(_lEngineDatabase).LPronunciationAudioSave(
            pronunciationId, LEngineRecordingFormat(file), source);
    }

    /// <summary>
    /// Reads the audio of the pronunciation identified by <paramref name="pronunciationId"/> with its
    /// file resolved to a full path in the workspace in use now, or <c>null</c> when it has none.
    /// </summary>
    public LPronunciationAudio? LEngineAudioRead(string pronunciationId)
    {
        LPronunciationAudio? audio =
            new LPronunciationArchive(_lEngineDatabase).LPronunciationAudioRead(pronunciationId);
        return audio is null
            ? null
            : audio with { LPronunciationAudioFile = LEngineRecordingResolve(audio.LPronunciationAudioFile) };
    }

    /// <summary>
    /// Saves <paramref name="note"/> as the note of its Entry, replacing the one there — an Entry keeps
    /// at most one note, keyed by the Entry itself, so a save is a create or a rewrite and the caller
    /// need not know which.
    /// </summary>
    public void LEngineNoteSave(LNote note)
    {
        ArgumentNullException.ThrowIfNull(note);
        new LNoteArchive(_lEngineDatabase).LNoteSave(note);
    }

    /// <summary>
    /// Reads the note the Entry identified by <paramref name="entryId"/> keeps, or <c>null</c> when it
    /// keeps none.
    /// </summary>
    public LNote? LEngineNoteRead(string entryId)
    {
        return new LNoteArchive(_lEngineDatabase).LNoteRead(entryId);
    }

    /// <summary>Deletes the note of the Entry identified by <paramref name="entryId"/>, if it has one.</summary>
    public void LEngineNoteDelete(string entryId)
    {
        new LNoteArchive(_lEngineDatabase).LNoteDelete(entryId);
    }
}
