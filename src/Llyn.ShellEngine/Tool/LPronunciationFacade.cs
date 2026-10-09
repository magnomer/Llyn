using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LPronunciationFacade : LPronunciationPort
{
    private readonly LEngineHearth _lPronunciationFacadeHearth;
    private readonly LDraftFacade _lPronunciationFacadeDraft;
    private readonly LVistaRowFacade _lPronunciationFacadeRow;
    private readonly object _lPronunciationFacadeGate;

    internal LPronunciationFacade(LEngineHearth hearth, LDraftFacade draft, LVistaRowFacade row)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(row);
        _lPronunciationFacadeHearth = hearth;
        _lPronunciationFacadeDraft = draft;
        _lPronunciationFacadeRow = row;
        _lPronunciationFacadeGate = _lPronunciationFacadeHearth.LEngineGate;
    }

    private LEngineStaff LPronunciationFacadeStaff => _lPronunciationFacadeHearth.LEngineStaffHeld;

    public IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(string query, LCatalogOrder order)
    {
        lock (_lPronunciationFacadeGate)
        {
            return LPronunciationFacadeStaff.LEngineStaffLanguage.LLanguageStaffPronunciation
                .LPronunciationClerkFind(query, order);
        }
    }

    public IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(
        string query,
        LCatalogOrder order,
        LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return LPronunciationClerk.LPronunciationClerkFind(LEnginePronunciationFind(query, order), filter);
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

            IReadOnlyList<LVistaRow> built = _lPronunciationFacadeRow.LEngineVistaBuild(
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
        LReceiverRelay receiver,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(receiver);

        IReadOnlyList<LCandidate>? held;
        Task<IReadOnlyList<LCandidate>>? scan = null;
        lock (_lPronunciationFacadeGate)
        {
            held = _lPronunciationFacadeHearth.LEngineTrove.LTroveCandidateRead(session, word, language);
            if (held is null)
            {
                scan = LPronunciationFacadeStaff.LEngineStaffLanguage.LLanguageStaffTranscription
                    .LTranscriptionClerkFind(word, language, receiver, cancellation);
            }
        }

        return scan is null
            ? LPronunciationFacadeStaff.LEngineStaffLanguage.LLanguageStaffTranscription
                .LTranscriptionClerkPublish(held!, language, receiver)
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
                _lPronunciationFacadeHearth.LEngineTrove.LTroveCandidateSave(session, word, language, found);
            }
            else
            {
                _lPronunciationFacadeHearth.LEngineTrove.LTroveTranscriptionSave(
                    session, word, language, scheme, found);
            }
        }
    }

    internal Task LEngineRecordingFind(
        long session,
        string word,
        string language,
        long target,
        LListenerRelay listener,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(listener);

        IReadOnlyList<LRecording>? held;
        string variety;
        Task<IReadOnlyList<LRecording>>? scan = null;
        lock (_lPronunciationFacadeGate)
        {
            variety = LEngineVarietyResolve(session, target);
            held = _lPronunciationFacadeHearth.LEngineTrove.LTroveRecordingRead(session, word, language);
            if (held is null)
            {
                scan = LPronunciationFacadeStaff.LEngineStaffLanguage.LLanguageStaffRecording.LRecordingClerkFind(
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
            _lPronunciationFacadeHearth.LEngineTrove.LTroveRecordingSave(session, word, language, found);
        }
    }

    private string LEngineVarietyResolve(long session, long target)
    {
        if (session == 0)
        {
            return string.Empty;
        }

        IReadOnlyList<LPronunciationDraft>? rows =
            _lPronunciationFacadeDraft.LEngineDraftRead(session)?.LDraftContent.LEntryDraftPronunciations;
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

    internal Task<string?> LEngineRecordingSave(
        LRecording recording, string word, string language, CancellationToken cancellation)
    {
        LRecordingClerk recordings;
        lock (_lPronunciationFacadeGate)
        {
            recordings = LPronunciationFacadeStaff.LEngineStaffLanguage.LLanguageStaffRecording;
        }

        return recordings.LRecordingClerkSave(recording, word, language, cancellation);
    }

    public Task<string?> LEngineRecordingPrepare(LRecording recording, CancellationToken cancellation)
    {
        LRecordingClerk recordings;
        lock (_lPronunciationFacadeGate)
        {
            recordings = LPronunciationFacadeStaff.LEngineStaffLanguage.LLanguageStaffRecording;
        }

        return recordings.LRecordingClerkPrepare(recording, cancellation);
    }

    public void LEngineRecordingSweep()
    {
        lock (_lPronunciationFacadeGate)
        {
            LPronunciationFacadeStaff.LEngineStaffLanguage.LLanguageStaffRecording.LRecordingClerkSweep();
        }
    }

    public bool LEngineRecordingExist(string? file)
    {
        lock (_lPronunciationFacadeGate)
        {
            return LPronunciationFacadeStaff.LEngineStaffLanguage.LLanguageStaffRecording.LRecordingClerkExist(file);
        }
    }

    public int LEngineRecordingPlay(string? file, double volume)
    {
        lock (_lPronunciationFacadeGate)
        {
            return LPronunciationFacadeStaff.LEngineStaffLanguage.LLanguageStaffRecording
                .LRecordingClerkPlay(file, volume);
        }
    }

    public int LEngineRecordingPlay(LEntryDraft draft, double volume)
    {
        ArgumentNullException.ThrowIfNull(draft);

        return LEngineRecordingPlay(draft.LEntryDraftAudio, volume);
    }

    public (bool, bool) LEnginePlaybackRead(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        bool recorded = LEngineRecordingExist(draft.LEntryDraftAudio);
        return (recorded, recorded || draft.LEntryDraftAccents.Any(static spoken =>
            spoken.LPronunciationDraftNotated && spoken.LPronunciationDraftAudio.Length > 0));
    }

    public (string?, bool) LEngineAudioRead(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        string? audio = LEngineRecordingExist(draft.LEntryDraftAudio) ? draft.LEntryDraftAudio : null;
        return (audio, audio is not null || draft.LEntryDraftAccents.Any(static spoken =>
            spoken.LPronunciationDraftAudio.Length > 0));
    }

    public Uri? LEngineAudioResolve(string? file)
    {
        return file is not null && LEngineRecordingExist(file) ? new Uri(file) : null;
    }

    public void LEngineRecordingStop(int ticket)
    {
        LPronunciationFacadeStaff.LEngineStaffLanguage.LLanguageStaffRecording.LRecordingClerkStop(ticket);
    }

    public void LEngineVolumeSet(double volume)
    {
        LPronunciationFacadeStaff.LEngineStaffLanguage.LLanguageStaffRecording.LRecordingClerkAdjust(volume);
    }

    public IReadOnlyList<string> LEngineSchemeRead(string language)
    {
        lock (_lPronunciationFacadeGate)
        {
            return LPronunciationFacadeStaff.LEngineStaffLanguage.LLanguageStaffTranscription.LSchemeRead(language);
        }
    }

    internal Task LEngineTranscriptionFind(
        long session,
        string word,
        string language,
        string scheme,
        LReceiverRelay receiver,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(receiver);
        ArgumentException.ThrowIfNullOrWhiteSpace(scheme);

        IReadOnlyList<LCandidate>? held;
        Task<IReadOnlyList<LCandidate>>? scan = null;
        lock (_lPronunciationFacadeGate)
        {
            held = _lPronunciationFacadeHearth.LEngineTrove.LTroveTranscriptionRead(session, word, language, scheme);
            if (held is null)
            {
                scan = LPronunciationFacadeStaff.LEngineStaffLanguage.LLanguageStaffTranscription
                    .LTranscriptionClerkFind(word, language, scheme, receiver, cancellation);
            }
        }

        return scan is null
            ? LTranscriptionClerk.LTranscriptionClerkPublish(held!, receiver)
            : LEngineTroveSave(scan, session, word, language, scheme);
    }

    public LArticulation LEngineConsonantRead()
    {
        return LPronunciationClerk.LPronunciationConsonantRead();
    }

    public LArticulation LEngineVowelRead()
    {
        return LPronunciationClerk.LPronunciationVowelRead();
    }

    public LFrequencyGauge? LEngineFrequencyResolve(long entryId, string once)
    {
        return LPronunciationFacadeStaff.LEngineStaffLanguage.LLanguageStaffFrequency
            .LFrequencyClerkResolve(entryId, once);
    }

    internal void LEngineFrequencyStart(long entryId)
    {
        LPronunciationFacadeStaff.LEngineStaffLanguage.LLanguageStaffFrequency.LFrequencyClerkStart(entryId);
    }
}
