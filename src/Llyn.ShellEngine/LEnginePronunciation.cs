using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LPronunciation LEnginePronunciationCreate(LPronunciation pronunciation)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(pronunciation);
            LPronunciation created = _lEnginePronunciations.LPronunciationCreate(pronunciation);
            LEngineUpdatedSet(created.LPronunciationEntryId);
            return created;
        }
    }

    internal IReadOnlyList<LPronunciation> LEnginePronunciationRead(long entryId)
    {
        lock (_lEngineGate)
        {
            return _lEnginePronunciations.LPronunciationRead(entryId);
        }
    }

    public IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            LPronunciationVault pronunciations = _lEnginePronunciations;

            List<LCatalogPronunciation> rows = [];
            foreach (LEntry entry in _lEngineEntries.LEntryFind(query))
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

            IReadOnlyList<LVistaRow> built = LEngineVistaBuild(entries, vista.LVistaChosen);
            List<LCatalogPronunciation> rows = new(found.Count);
            for (int index = 0; index < found.Count; index++)
            {
                rows.Add(found[index] with
                {
                    LCatalogPronunciationName = built[index].LVistaRowName,
                    LCatalogPronunciationEpithet = built[index].LVistaRowEpithet,
                    LCatalogPronunciationChosen = built[index].LVistaRowChosen,
                });
            }

            return rows;
        }
    }

    internal void LEnginePronunciationUpdate(LPronunciation pronunciation)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(pronunciation);
            _lEnginePronunciations.LPronunciationUpdate(pronunciation);
            LEngineUpdatedSet(pronunciation.LPronunciationEntryId);
        }
    }

    internal void LEnginePronunciationDelete(long id)
    {
        lock (_lEngineGate)
        {
            LPronunciationVault pronunciations = _lEnginePronunciations;
            long? entryId = pronunciations.LPronunciationHolderRead(id);
            pronunciations.LPronunciationDelete(id);
            if (entryId is long held)
            {
                LEngineUpdatedSet(held);
            }
        }
    }

    internal void LEngineAudioSave(long pronunciationId, string file, string? source)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(file);

            LPronunciationVault pronunciations = _lEnginePronunciations;
            pronunciations.LPronunciationAudioSave(pronunciationId, LEngineRecordingFormat(file), source);
            if (pronunciations.LPronunciationHolderRead(pronunciationId) is long held)
            {
                LEngineUpdatedSet(held);
            }
        }
    }

    internal LPronunciationAudio? LEngineAudioRead(long pronunciationId)
    {
        lock (_lEngineGate)
        {
            LPronunciationAudio? audio =
                _lEnginePronunciations.LPronunciationAudioRead(pronunciationId);
            return audio is null
                ? null
                : audio with { LPronunciationAudioFile = LEngineRecordingResolve(audio.LPronunciationAudioFile) };
        }
    }

    internal void LEngineNoteSave(LNote note)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(note);
            _lEngineNotes.LNoteSave(
                note with { LNoteText = LMarkdown.LMarkdownNormalize(note.LNoteText) });
            LEngineUpdatedSet(note.LNoteEntryId);
        }
    }

    internal LNote? LEngineNoteRead(long entryId)
    {
        lock (_lEngineGate)
        {
            return _lEngineNotes.LNoteRead(entryId);
        }
    }

    internal void LEngineNoteDelete(long entryId)
    {
        lock (_lEngineGate)
        {
            _lEngineNotes.LNoteDelete(entryId);
            LEngineUpdatedSet(entryId);
        }
    }
}
