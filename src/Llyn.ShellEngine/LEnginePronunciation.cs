using System;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LPronunciation LEnginePronunciationCreate(LPronunciation pronunciation)
    {
        ArgumentNullException.ThrowIfNull(pronunciation);
        return new LPronunciationArchive(_lEngineDatabase).LPronunciationCreate(pronunciation);
    }

    public LPronunciation? LEnginePronunciationRead(string entryId)
    {
        return new LPronunciationArchive(_lEngineDatabase).LPronunciationRead(entryId);
    }

    public void LEnginePronunciationUpdate(LPronunciation pronunciation)
    {
        ArgumentNullException.ThrowIfNull(pronunciation);
        new LPronunciationArchive(_lEngineDatabase).LPronunciationUpdate(pronunciation);
    }

    public void LEnginePronunciationDelete(string id)
    {
        new LPronunciationArchive(_lEngineDatabase).LPronunciationDelete(id);
    }

    public void LEngineAudioSave(string pronunciationId, string file, string? source)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(file);

        new LPronunciationArchive(_lEngineDatabase).LPronunciationAudioSave(
            pronunciationId, LEngineRecordingFormat(file), source);
    }

    public LPronunciationAudio? LEngineAudioRead(string pronunciationId)
    {
        LPronunciationAudio? audio =
            new LPronunciationArchive(_lEngineDatabase).LPronunciationAudioRead(pronunciationId);
        return audio is null
            ? null
            : audio with { LPronunciationAudioFile = LEngineRecordingResolve(audio.LPronunciationAudioFile) };
    }

    public void LEngineNoteSave(LNote note)
    {
        ArgumentNullException.ThrowIfNull(note);
        new LNoteArchive(_lEngineDatabase).LNoteSave(note);
    }

    public LNote? LEngineNoteRead(string entryId)
    {
        return new LNoteArchive(_lEngineDatabase).LNoteRead(entryId);
    }

    public void LEngineNoteDelete(string entryId)
    {
        new LNoteArchive(_lEngineDatabase).LNoteDelete(entryId);
    }
}
