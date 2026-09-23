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
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffPronunciation.LPronunciationClerkCreate(pronunciation);
        }
    }

    internal IReadOnlyList<LPronunciation> LEnginePronunciationRead(long entryId)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffPronunciation.LPronunciationClerkRead(entryId);
        }
    }

    public IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(string query, LCatalogOrder order)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffPronunciation.LPronunciationClerkFind(query, order);
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

        lock (LEngineGate)
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
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffPronunciation.LPronunciationClerkUpdate(pronunciation);
        }
    }

    internal void LEnginePronunciationDelete(long id)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffPronunciation.LPronunciationClerkDelete(id);
        }
    }

    internal void LEngineAudioSave(long pronunciationId, string file, string? source)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffPronunciation.LAudioSave(pronunciationId, file, source);
        }
    }

    internal LPronunciationAudio? LEngineAudioRead(long pronunciationId)
    {
        lock (LEngineGate)
        {
            LPronunciationAudio? audio = _lEngineStaff.LEngineStaffPronunciation.LAudioRead(pronunciationId);
            if (audio is null)
            {
                return null;
            }

            string file = _lEngineStaff.LEngineStaffRecording.LRecordingClerkResolve(audio.LPronunciationAudioFile);
            return audio with { LPronunciationAudioFile = file };
        }
    }

    internal void LEngineNoteSave(LNote note)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffPronunciation.LNoteSave(note);
        }
    }

    internal LNote? LEngineNoteRead(long entryId)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffPronunciation.LNoteRead(entryId);
        }
    }

    internal void LEngineNoteDelete(long entryId)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffPronunciation.LNoteDelete(entryId);
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
        lock (LEngineGate)
        {
            held = LEngineTrove.LTroveCandidateRead(session, word, language);
            if (held is null)
            {
                scan = _lEngineStaff.LEngineStaffTranscription.LTranscriptionClerkFind(
                    word, language, sink, cancellation);
            }
        }

        return scan is null
            ? _lEngineStaff.LEngineStaffTranscription.LTranscriptionClerkPublish(held!, language, sink)
            : LEngineTroveSave(scan, session, word, language, null);
    }

    private async Task LEngineTroveSave(
        Task<IReadOnlyList<LCandidate>> scan, long session, string word, string language, string? scheme)
    {
        IReadOnlyList<LCandidate> found = await scan.ConfigureAwait(false);

        lock (LEngineGate)
        {
            if (scheme is null)
            {
                LEngineTrove.LTroveCandidateSave(session, word, language, found);
            }
            else
            {
                LEngineTrove.LTroveTranscriptionSave(session, word, language, scheme, found);
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
        lock (LEngineGate)
        {
            variety = LEngineVarietyResolve(session, target);
            held = LEngineTrove.LTroveRecordingRead(session, word, language);
            if (held is null)
            {
                scan = _lEngineStaff.LEngineStaffRecording.LRecordingClerkFind(
                    word, language, variety, listener, cancellation);
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

        lock (LEngineGate)
        {
            LEngineTrove.LTroveRecordingSave(session, word, language, found);
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
        lock (LEngineGate)
        {
            recordings = _lEngineStaff.LEngineStaffRecording;
        }

        return recordings.LRecordingClerkSave(recording, word, language, cancellation);
    }

    public Task<string> LEngineRecordingPrepare(LRecording recording, CancellationToken cancellation)
    {
        LRecordingClerk recordings;
        lock (LEngineGate)
        {
            recordings = _lEngineStaff.LEngineStaffRecording;
        }

        return recordings.LRecordingClerkPrepare(recording, cancellation);
    }

    public void LEngineRecordingSweep()
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffRecording.LRecordingClerkSweep();
        }
    }

    public bool LEngineRecordingExist(string? file)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffRecording.LRecordingClerkExist(file);
        }
    }

    public IReadOnlyList<string> LEngineSchemeRead(string language)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffTranscription.LSchemeRead(language);
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
        lock (LEngineGate)
        {
            held = LEngineTrove.LTroveTranscriptionRead(session, word, language, scheme);
            if (held is null)
            {
                scan = _lEngineStaff.LEngineStaffTranscription.LTranscriptionClerkFind(
                    word, language, scheme, sink, cancellation);
            }
        }

        return scan is null
            ? LTranscriptionClerk.LTranscriptionClerkPublish(held!, sink)
            : LEngineTroveSave(scan, session, word, language, scheme);
    }

    internal IReadOnlyList<LTranscription> LEngineTranscriptionRead(long entryId)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffTranscription.LTranscriptionClerkRead(entryId);
        }
    }

    internal IReadOnlyList<LTranscription> LEngineTranscriptionSet(
        long entryId, IReadOnlyList<LTranscription> transcriptions)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffTranscription.LTranscriptionClerkSet(entryId, transcriptions);
        }
    }

    internal Task<IReadOnlyList<LFrequency>> LEngineFrequencyFind(
        string word, string language, CancellationToken cancellation)
    {
        return _lEngineStaff.LEngineStaffFrequency.LFrequencyClerkFind(word, language, cancellation);
    }

    internal string? LEngineBandResolve(string language, string source, string raw)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffFrequency.LBandResolve(language, source, raw);
        }
    }

    public IReadOnlyList<LFrequency> LEngineFrequencyRead(long entryId)
    {
        return _lEngineStaff.LEngineStaffFrequency.LFrequencyClerkRead(entryId);
    }

    internal void LEngineFrequencyStart(long entryId)
    {
        _lEngineStaff.LEngineStaffFrequency.LFrequencyClerkStart(entryId);
    }
}
