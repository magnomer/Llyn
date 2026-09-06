using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LPronunciation LEnginePronunciationCreate(LPronunciation pronunciation)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(pronunciation);
            return new LPronunciationArchive(_lEngineDatabase).LPronunciationCreate(pronunciation);
        }
    }

    public LPronunciation? LEnginePronunciationRead(string entryId)
    {
        lock (_lEngineGate)
        {
            return new LPronunciationArchive(_lEngineDatabase).LPronunciationRead(entryId);
        }
    }

    public IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            LPronunciationArchive pronunciations = new(_lEngineDatabase);

            List<LCatalogPronunciation> rows = [];
            foreach (LEntry entry in new LEntryArchive(_lEngineDatabase).LEntryFind(query))
            {
                LPronunciation? pronunciation = pronunciations.LPronunciationRead(entry.LEntryId);
                rows.Add(LCatalogPronunciation.LCatalogPronunciationCreate(
                    entry,
                    pronunciation?.LPronunciationIpa));
            }

            return LCatalogPronunciation.LCatalogPronunciationSort(rows, order);
        }
    }

    public void LEnginePronunciationUpdate(LPronunciation pronunciation)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(pronunciation);
            new LPronunciationArchive(_lEngineDatabase).LPronunciationUpdate(pronunciation);
        }
    }

    public void LEnginePronunciationDelete(string id)
    {
        lock (_lEngineGate)
        {
            new LPronunciationArchive(_lEngineDatabase).LPronunciationDelete(id);
        }
    }

    public void LEngineAudioSave(string pronunciationId, string file, string? source)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(file);

            new LPronunciationArchive(_lEngineDatabase).LPronunciationAudioSave(
                pronunciationId, LEngineRecordingFormat(file), source);
        }
    }

    public LPronunciationAudio? LEngineAudioRead(string pronunciationId)
    {
        lock (_lEngineGate)
        {
            LPronunciationAudio? audio =
                new LPronunciationArchive(_lEngineDatabase).LPronunciationAudioRead(pronunciationId);
            return audio is null
                ? null
                : audio with { LPronunciationAudioFile = LEngineRecordingResolve(audio.LPronunciationAudioFile) };
        }
    }

    public void LEngineNoteSave(LNote note)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(note);
            new LNoteArchive(_lEngineDatabase).LNoteSave(note);
        }
    }

    public LNote? LEngineNoteRead(string entryId)
    {
        lock (_lEngineGate)
        {
            return new LNoteArchive(_lEngineDatabase).LNoteRead(entryId);
        }
    }

    public void LEngineNoteDelete(string entryId)
    {
        lock (_lEngineGate)
        {
            new LNoteArchive(_lEngineDatabase).LNoteDelete(entryId);
        }
    }
}
