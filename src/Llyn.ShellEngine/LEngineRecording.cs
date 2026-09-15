using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private string LEngineRecordingFormat(string path)
    {
        string relative = Path.GetRelativePath(_lEngineWorkspace, path);
        return Path.IsPathRooted(relative) || relative.StartsWith("..", StringComparison.Ordinal)
            ? path
            : relative;
    }

    public Task LEnginePronunciationFind(
        long session,
        string word,
        string language,
        LReceiver receiver,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(receiver);

        IReadOnlyList<LCandidate>? held;
        IReadOnlyList<LSource> sources;
        LLanguage pack;
        lock (_lEngineGate)
        {
            held = _lEngineTrove.LTroveCandidateRead(session, word, language);
            sources = held is null ? LEngineLookupRead(language) : [];
            pack = LEngineLanguageLoad(language);
            if (_lEngineSettings.LSettingsRespelled && pack.LLanguageRespellings.Count > 0)
            {
                receiver = new LReceiverRespelling(receiver, pack.LLanguageRespellings);
            }
        }

        return held is null
            ? LEngineCandidateScan(session, word, language, sources, pack, receiver, cancellation)
            : LEngineCandidatePublish(held, receiver);
    }

    public Task LEngineRecordingFind(
        long session,
        string word,
        string language,
        long target,
        LListener listener,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(listener);

        IReadOnlyList<LRecording>? held;
        IReadOnlyList<LSource> sources;
        LLanguage pack;
        string variety;
        lock (_lEngineGate)
        {
            variety = LEngineVarietyResolve(session, target);
            held = _lEngineTrove.LTroveRecordingRead(session, word, language);
            sources = held is null ? LEngineHarvestRead(language) : [];
            pack = LEngineLanguageLoad(language);
        }

        return held is null
            ? LEngineRecordingScan(session, word, language, variety, sources, pack, listener, cancellation)
            : LEngineRecordingPublish(LHarvest.LHarvestRecordingScan(held, variety), listener);
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

    private async Task LEngineCandidateScan(
        long session,
        string word,
        string language,
        IReadOnlyList<LSource> sources,
        LLanguage pack,
        LReceiver receiver,
        CancellationToken cancellation)
    {
        IReadOnlyList<LCandidate> found =
            await new LLookup(sources, pack.LLanguageVarieties, pack.LLanguageCleanups)
                .LSeekerStart(word, receiver, cancellation)
                .ConfigureAwait(false);

        lock (_lEngineGate)
        {
            _lEngineTrove.LTroveCandidateSave(session, word, language, found);
        }
    }

    private async Task LEngineRecordingScan(
        long session,
        string word,
        string language,
        string variety,
        IReadOnlyList<LSource> sources,
        LLanguage pack,
        LListener listener,
        CancellationToken cancellation)
    {
        IReadOnlyList<LRecording> found =
            await new LHarvest(sources, pack.LLanguageVarieties)
                .LHarvestStart(word, variety, listener, cancellation)
                .ConfigureAwait(false);

        lock (_lEngineGate)
        {
            _lEngineTrove.LTroveRecordingSave(session, word, language, found);
        }
    }

    private static Task LEngineCandidatePublish(IReadOnlyList<LCandidate> held, LReceiver receiver)
    {
        foreach (LCandidate candidate in held)
        {
            receiver.LReceiverCandidateAdd(candidate);
        }

        receiver.LReceiverLookupFinish();
        return Task.CompletedTask;
    }

    private static Task LEngineRecordingPublish(IReadOnlyList<LRecording> held, LListener listener)
    {
        foreach (LRecording recording in held)
        {
            listener.LListenerRecordingAdd(recording);
        }

        listener.LListenerFinish();
        return Task.CompletedTask;
    }

    public Task<string> LEngineRecordingSave(
        LRecording recording, string word, string language, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(recording);

        string workspace;
        lock (_lEngineGate)
        {
            workspace = _lEngineWorkspace;
        }

        return LWorkspace.LWorkspaceRecordingSave(recording, word, language, workspace, _lEngineClient, cancellation);
    }

    public Task<string> LEngineRecordingPrepare(LRecording recording, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(recording);

        string workspace;
        lock (_lEngineGate)
        {
            workspace = _lEngineWorkspace;
        }

        return LWorkspace.LWorkspaceRecordingPrepare(recording, workspace, _lEngineClient, cancellation);
    }

    private IReadOnlyList<LSource> LEngineLookupRead(string language)
    {
        if (!_lEngineLookupSources.TryGetValue(language, out IReadOnlyList<LSource>? sources))
        {
            LLanguage pack = LEngineLanguageLoad(language);
            sources = LSourceFactory.LSourceFactoryCreate(pack.LLanguageLookupSources, _lEngineClient);
            _lEngineLookupSources[language] = sources;
        }

        return sources;
    }

    private IReadOnlyList<LSource> LEngineHarvestRead(string language)
    {
        if (!_lEngineHarvestSources.TryGetValue(language, out IReadOnlyList<LSource>? sources))
        {
            LLanguage pack = LEngineLanguageLoad(language);
            sources = LSourceFactory.LSourceFactoryCreate(pack.LLanguageHarvestSources, _lEngineClient);
            _lEngineHarvestSources[language] = sources;
        }

        return sources;
    }
}
