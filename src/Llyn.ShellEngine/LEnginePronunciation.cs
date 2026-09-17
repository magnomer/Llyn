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
            LPronunciation created = new LPronunciationArchive(_lEngineDatabase).LPronunciationCreate(pronunciation);
            LEngineUpdatedSet(created.LPronunciationEntryId);
            return created;
        }
    }

    public IReadOnlyList<LPronunciation> LEnginePronunciationRead(long entryId)
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
                IReadOnlyList<LPronunciation> spoken = pronunciations.LPronunciationRead(entry.LEntryId);
                rows.Add(LCatalogPronunciation.LCatalogPronunciationCreate(
                    entry,
                    spoken.Count == 0 ? null : spoken[0].LPronunciationIpa));
            }

            return LCatalogPronunciation.LCatalogPronunciationSort(rows, order);
        }
    }

    public IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(
        string query,
        LCatalogOrder order,
        LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return filter.LCatalogFilterApply(
            LEnginePronunciationFind(query, order), row => row.LCatalogPronunciationEntry.LEntryLanguage);
    }

    public IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        lock (_lEngineGate)
        {
            IReadOnlyList<LCatalogPronunciation> found = LEnginePronunciationFind(
                vista.LVistaQuery, vista.LVistaOrder, vista.LVistaFilter);
            List<LEntry> entries = new(found.Count);
            foreach (LCatalogPronunciation row in found)
            {
                entries.Add(row.LCatalogPronunciationEntry);
            }

            IReadOnlyList<LVistaRow> built = LEngineVistaBuild(entries, null);
            List<LCatalogPronunciation> rows = new(found.Count);
            for (int index = 0; index < found.Count; index++)
            {
                rows.Add(found[index] with
                {
                    LCatalogPronunciationName = built[index].LVistaRowName,
                    LCatalogPronunciationEpithet = built[index].LVistaRowEpithet,
                });
            }

            return rows;
        }
    }

    public void LEnginePronunciationUpdate(LPronunciation pronunciation)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(pronunciation);
            new LPronunciationArchive(_lEngineDatabase).LPronunciationUpdate(pronunciation);
            LEngineUpdatedSet(pronunciation.LPronunciationEntryId);
        }
    }

    public void LEnginePronunciationDelete(long id)
    {
        lock (_lEngineGate)
        {
            LPronunciationArchive pronunciations = new(_lEngineDatabase);
            long? entryId = pronunciations.LPronunciationHolderRead(id);
            pronunciations.LPronunciationDelete(id);
            if (entryId is long held)
            {
                LEngineUpdatedSet(held);
            }
        }
    }

    public void LEngineAudioSave(long pronunciationId, string file, string? source)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(file);

            LPronunciationArchive pronunciations = new(_lEngineDatabase);
            pronunciations.LPronunciationAudioSave(pronunciationId, LEngineRecordingFormat(file), source);
            if (pronunciations.LPronunciationHolderRead(pronunciationId) is long held)
            {
                LEngineUpdatedSet(held);
            }
        }
    }

    public LPronunciationAudio? LEngineAudioRead(long pronunciationId)
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
            new LNoteArchive(_lEngineDatabase).LNoteSave(
                note with { LNoteText = LMarkdown.LMarkdownNormalize(note.LNoteText) });
            LEngineUpdatedSet(note.LNoteEntryId);
        }
    }

    public LNote? LEngineNoteRead(long entryId)
    {
        lock (_lEngineGate)
        {
            return new LNoteArchive(_lEngineDatabase).LNoteRead(entryId);
        }
    }

    public void LEngineNoteDelete(long entryId)
    {
        lock (_lEngineGate)
        {
            new LNoteArchive(_lEngineDatabase).LNoteDelete(entryId);
            LEngineUpdatedSet(entryId);
        }
    }
}
