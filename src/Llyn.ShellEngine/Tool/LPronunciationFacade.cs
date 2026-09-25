using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LPronunciationFacade
{
    private readonly LEngine _lPronunciationFacadeEngine;
    private readonly object _lPronunciationFacadeGate;

    public LPronunciationFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lPronunciationFacadeEngine = engine;
        _lPronunciationFacadeGate = engine.LEngineGate;
    }

    private LEngineStaff LPronunciationFacadeStaff => _lPronunciationFacadeEngine.LEngineStaffHeld;

    public IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(string query, LCatalogOrder order)
    {
        lock (_lPronunciationFacadeGate)
        {
            return LPronunciationFacadeStaff.LEngineStaffPronunciation.LPronunciationClerkFind(query, order);
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

        lock (_lPronunciationFacadeGate)
        {
            IReadOnlyList<LCatalogPronunciation> found = LEnginePronunciationFind(
                vista.LVistaQuery, vista.LVistaOrder, vista.LVistaFilter);
            List<LEntry> entries = new(found.Count);
            foreach (LCatalogPronunciation row in found)
            {
                entries.Add(row.LCatalogPronunciationEntry);
            }

            IReadOnlyList<LVistaRow> built = _lPronunciationFacadeEngine.LEngineVista.LEngineVistaBuild(
                entries, vista.LVistaChosen);
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
        lock (_lPronunciationFacadeGate)
        {
            held = _lPronunciationFacadeEngine.LEngineTrove.LTroveCandidateRead(session, word, language);
            if (held is null)
            {
                scan = LPronunciationFacadeStaff.LEngineStaffTranscription.LTranscriptionClerkFind(
                    word, language, sink, cancellation);
            }
        }

        return scan is null
            ? LPronunciationFacadeStaff.LEngineStaffTranscription.LTranscriptionClerkPublish(held!, language, sink)
            : LEngineTroveSave(scan, session, word, language, null);
    }

    private async Task LEngineTroveSave(
        Task<IReadOnlyList<LCandidate>> scan, long session, string word, string language, string? scheme)
    {
        IReadOnlyList<LCandidate> found = await scan.ConfigureAwait(false);

        lock (_lPronunciationFacadeGate)
        {
            if (scheme is null)
            {
                _lPronunciationFacadeEngine.LEngineTrove.LTroveCandidateSave(session, word, language, found);
            }
            else
            {
                _lPronunciationFacadeEngine.LEngineTrove.LTroveTranscriptionSave(
                    session, word, language, scheme, found);
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
        lock (_lPronunciationFacadeGate)
        {
            variety = LEngineVarietyResolve(session, target);
            held = _lPronunciationFacadeEngine.LEngineTrove.LTroveRecordingRead(session, word, language);
            if (held is null)
            {
                scan = LPronunciationFacadeStaff.LEngineStaffRecording.LRecordingClerkFind(
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

        lock (_lPronunciationFacadeGate)
        {
            _lPronunciationFacadeEngine.LEngineTrove.LTroveRecordingSave(session, word, language, found);
        }
    }

    private string LEngineVarietyResolve(long session, long target)
    {
        if (session == 0)
        {
            return string.Empty;
        }

        IReadOnlyList<LPronunciationDraft>? rows =
            _lPronunciationFacadeEngine.LEngineDraft.LEngineDraftRead(session)?.LDraftContent.LEntryDraftPronunciations;
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
        lock (_lPronunciationFacadeGate)
        {
            recordings = LPronunciationFacadeStaff.LEngineStaffRecording;
        }

        return recordings.LRecordingClerkSave(recording, word, language, cancellation);
    }

    public Task<string> LEngineRecordingPrepare(LRecording recording, CancellationToken cancellation)
    {
        LRecordingClerk recordings;
        lock (_lPronunciationFacadeGate)
        {
            recordings = LPronunciationFacadeStaff.LEngineStaffRecording;
        }

        return recordings.LRecordingClerkPrepare(recording, cancellation);
    }

    public void LEngineRecordingSweep()
    {
        lock (_lPronunciationFacadeGate)
        {
            LPronunciationFacadeStaff.LEngineStaffRecording.LRecordingClerkSweep();
        }
    }

    public bool LEngineRecordingExist(string? file)
    {
        lock (_lPronunciationFacadeGate)
        {
            return LPronunciationFacadeStaff.LEngineStaffRecording.LRecordingClerkExist(file);
        }
    }

    public int LEngineRecordingPlay(string? file, double volume)
    {
        lock (_lPronunciationFacadeGate)
        {
            return LPronunciationFacadeStaff.LEngineStaffRecording.LRecordingClerkPlay(file, volume);
        }
    }

    public void LEngineRecordingStop(int ticket)
    {
        LPronunciationFacadeStaff.LEngineStaffRecording.LRecordingClerkStop(ticket);
    }

    public void LEngineVolumeSet(double volume)
    {
        LPronunciationFacadeStaff.LEngineStaffRecording.LRecordingClerkAdjust(volume);
    }

    public IReadOnlyList<string> LEngineSchemeRead(string language)
    {
        lock (_lPronunciationFacadeGate)
        {
            return LPronunciationFacadeStaff.LEngineStaffTranscription.LSchemeRead(language);
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
        lock (_lPronunciationFacadeGate)
        {
            held = _lPronunciationFacadeEngine.LEngineTrove.LTroveTranscriptionRead(session, word, language, scheme);
            if (held is null)
            {
                scan = LPronunciationFacadeStaff.LEngineStaffTranscription.LTranscriptionClerkFind(
                    word, language, scheme, sink, cancellation);
            }
        }

        return scan is null
            ? LTranscriptionClerk.LTranscriptionClerkPublish(held!, sink)
            : LEngineTroveSave(scan, session, word, language, scheme);
    }

    public IReadOnlyList<LFrequency> LEngineFrequencyRead(long entryId)
    {
        return LPronunciationFacadeStaff.LEngineStaffFrequency.LFrequencyClerkRead(entryId);
    }

    internal void LEngineFrequencyStart(long entryId)
    {
        LPronunciationFacadeStaff.LEngineStaffFrequency.LFrequencyClerkStart(entryId);
    }
}
