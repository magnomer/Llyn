using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LPronunciation LEnginePronunciationCreate(LPronunciation pronunciation)
    {
        lock (_lEngineGate)
        {
            return _lEnginePronunciationClerk.LPronunciationClerkCreate(pronunciation);
        }
    }

    internal IReadOnlyList<LPronunciation> LEnginePronunciationRead(long entryId)
    {
        lock (_lEngineGate)
        {
            return _lEnginePronunciationClerk.LPronunciationClerkRead(entryId);
        }
    }

    public IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            return _lEnginePronunciationClerk.LPronunciationClerkFind(query, order);
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
            _lEnginePronunciationClerk.LPronunciationClerkUpdate(pronunciation);
        }
    }

    internal void LEnginePronunciationDelete(long id)
    {
        lock (_lEngineGate)
        {
            _lEnginePronunciationClerk.LPronunciationClerkDelete(id);
        }
    }

    internal void LEngineAudioSave(long pronunciationId, string file, string? source)
    {
        lock (_lEngineGate)
        {
            _lEnginePronunciationClerk.LAudioSave(pronunciationId, file, source);
        }
    }

    internal LPronunciationAudio? LEngineAudioRead(long pronunciationId)
    {
        lock (_lEngineGate)
        {
            LPronunciationAudio? audio = _lEnginePronunciationClerk.LAudioRead(pronunciationId);
            if (audio is null)
            {
                return null;
            }

            string file = _lEngineRecordingClerk.LRecordingClerkResolve(audio.LPronunciationAudioFile);
            return audio with { LPronunciationAudioFile = file };
        }
    }

    internal void LEngineNoteSave(LNote note)
    {
        lock (_lEngineGate)
        {
            _lEnginePronunciationClerk.LNoteSave(note);
        }
    }

    internal LNote? LEngineNoteRead(long entryId)
    {
        lock (_lEngineGate)
        {
            return _lEnginePronunciationClerk.LNoteRead(entryId);
        }
    }

    internal void LEngineNoteDelete(long entryId)
    {
        lock (_lEngineGate)
        {
            _lEnginePronunciationClerk.LNoteDelete(entryId);
        }
    }

    public Task LEnginePronunciationFind(
        long session,
        string word,
        string language,
        Action<LLookupStep> sink,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(sink);

        IReadOnlyList<LCandidate>? held;
        Task<IReadOnlyList<LCandidate>>? scan = null;
        lock (_lEngineGate)
        {
            held = _lEngineTrove.LTroveCandidateRead(session, word, language);
            if (held is null)
            {
                scan = _lEngineTranscriptionClerk.LTranscriptionClerkFind(word, language, sink, cancellation);
            }
        }

        return scan is null
            ? _lEngineTranscriptionClerk.LTranscriptionClerkPublish(held!, language, sink)
            : LEngineTroveSave(scan, session, word, language, null);
    }

    private async Task LEngineTroveSave(
        Task<IReadOnlyList<LCandidate>> scan, long session, string word, string language, string? scheme)
    {
        IReadOnlyList<LCandidate> found = await scan.ConfigureAwait(false);

        lock (_lEngineGate)
        {
            if (scheme is null)
            {
                _lEngineTrove.LTroveCandidateSave(session, word, language, found);
            }
            else
            {
                _lEngineTrove.LTroveTranscriptionSave(session, word, language, scheme, found);
            }
        }
    }

    internal Task LEngineRecordingFind(
        long session,
        string word,
        string language,
        long target,
        Action<LHarvestStep> sink,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(sink);

        LListenerRelay listener = new(sink);
        IReadOnlyList<LRecording>? held;
        string variety;
        Task<IReadOnlyList<LRecording>>? scan = null;
        lock (_lEngineGate)
        {
            variety = LEngineVarietyResolve(session, target);
            held = _lEngineTrove.LTroveRecordingRead(session, word, language);
            if (held is null)
            {
                scan = _lEngineRecordingClerk.LRecordingClerkFind(word, language, variety, listener, cancellation);
            }
        }

        return scan is null
            ? LRecordingClerk.LRecordingClerkPublish(held!, variety, listener)
            : LEngineTroveSave(scan, session, word, language);
    }

    private async Task LEngineTroveSave(
        Task<IReadOnlyList<LRecording>> scan, long session, string word, string language)
    {
        IReadOnlyList<LRecording> found = await scan.ConfigureAwait(false);

        lock (_lEngineGate)
        {
            _lEngineTrove.LTroveRecordingSave(session, word, language, found);
        }
    }

    private string LEngineVarietyResolve(long session, long target)
    {
        if (session == 0)
        {
            return string.Empty;
        }

        IReadOnlyList<LPronunciationDraft>? rows =
            LEngineDraftRead(session)?.LDraftContent.LEntryDraftPronunciations;
        if (rows is null)
        {
            return string.Empty;
        }

        if (target == 0)
        {
            return rows.Count == 0 ? string.Empty : rows[0].LPronunciationDraftVariety.Trim();
        }

        foreach (LPronunciationDraft row in rows)
        {
            if (row.LPronunciationDraftId == target)
            {
                return row.LPronunciationDraftVariety.Trim();
            }
        }

        return string.Empty;
    }

    internal Task<string> LEngineRecordingSave(
        LRecording recording, string word, string language, CancellationToken cancellation)
    {
        LRecordingClerk recordings;
        lock (_lEngineGate)
        {
            recordings = _lEngineRecordingClerk;
        }

        return recordings.LRecordingClerkSave(recording, word, language, cancellation);
    }

    public Task<string> LEngineRecordingPrepare(LRecording recording, CancellationToken cancellation)
    {
        LRecordingClerk recordings;
        lock (_lEngineGate)
        {
            recordings = _lEngineRecordingClerk;
        }

        return recordings.LRecordingClerkPrepare(recording, cancellation);
    }

    public void LEngineRecordingSweep()
    {
        lock (_lEngineGate)
        {
            _lEngineRecordingClerk.LRecordingClerkSweep();
        }
    }

    public bool LEngineRecordingExist(string? file)
    {
        lock (_lEngineGate)
        {
            return _lEngineRecordingClerk.LRecordingClerkExist(file);
        }
    }

    public IReadOnlyList<string> LEngineSchemeRead(string language)
    {
        lock (_lEngineGate)
        {
            return _lEngineTranscriptionClerk.LSchemeRead(language);
        }
    }

    internal Task LEngineTranscriptionFind(
        long session,
        string word,
        string language,
        string scheme,
        Action<LLookupStep> sink,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(sink);
        ArgumentException.ThrowIfNullOrWhiteSpace(scheme);

        IReadOnlyList<LCandidate>? held;
        Task<IReadOnlyList<LCandidate>>? scan = null;
        lock (_lEngineGate)
        {
            held = _lEngineTrove.LTroveTranscriptionRead(session, word, language, scheme);
            if (held is null)
            {
                scan = _lEngineTranscriptionClerk.LTranscriptionClerkFind(
                    word, language, scheme, sink, cancellation);
            }
        }

        return scan is null
            ? LTranscriptionClerk.LTranscriptionClerkPublish(held!, sink)
            : LEngineTroveSave(scan, session, word, language, scheme);
    }

    internal IReadOnlyList<LTranscription> LEngineTranscriptionRead(long entryId)
    {
        lock (_lEngineGate)
        {
            return _lEngineTranscriptionClerk.LTranscriptionClerkRead(entryId);
        }
    }

    internal IReadOnlyList<LTranscription> LEngineTranscriptionSet(
        long entryId, IReadOnlyList<LTranscription> transcriptions)
    {
        lock (_lEngineGate)
        {
            return _lEngineTranscriptionClerk.LTranscriptionClerkSet(entryId, transcriptions);
        }
    }

    internal Task<IReadOnlyList<LFrequency>> LEngineFrequencyFind(
        string word, string language, CancellationToken cancellation)
    {
        return _lEngineFrequencyClerk.LFrequencyClerkFind(word, language, cancellation);
    }

    internal string? LEngineBandResolve(string language, string source, string raw)
    {
        lock (_lEngineGate)
        {
            return _lEngineFrequencyClerk.LBandResolve(language, source, raw);
        }
    }

    public IReadOnlyList<LFrequency> LEngineFrequencyRead(long entryId)
    {
        return _lEngineFrequencyClerk.LFrequencyClerkRead(entryId);
    }

    internal void LEngineFrequencyStart(long entryId)
    {
        _lEngineFrequencyClerk.LFrequencyClerkStart(entryId);
    }
}
