using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LRecordingClerk
{
    private readonly LRecordingVault _lRecordingClerkRecordings;
    private readonly LPronunciationVault _lRecordingClerkPronunciations;
    private readonly LSourceFactory _lRecordingClerkFactory;
    private readonly LLanguageCache _lRecordingClerkLanguages;
    private readonly LTrailClerk _lRecordingClerkTrail;
    private readonly LClaimClerk _lRecordingClerkClaims;
    private readonly Dictionary<string, IReadOnlyList<LSource>> _lRecordingClerkSources = new(StringComparer.Ordinal);

    public LRecordingClerk(LRig rig, LLanguageCache languages, LTrailClerk trail, LClaimClerk claims)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(trail);
        ArgumentNullException.ThrowIfNull(claims);
        _lRecordingClerkRecordings = rig.LRigRecordings;
        _lRecordingClerkPronunciations = rig.LRigPronunciations;
        _lRecordingClerkFactory = rig.LRigSources;
        _lRecordingClerkLanguages = languages;
        _lRecordingClerkTrail = trail;
        _lRecordingClerkClaims = claims;
    }

    public Task<IReadOnlyList<LRecording>> LRecordingClerkFind(
        string word, string language, string variety, LListener listener, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(listener);

        LLanguage pack = _lRecordingClerkLanguages.LLanguageCacheRead(language);
        return new LHarvest(LHarvestSourceRead(language, pack), pack.LLanguageVarieties)
            .LHarvestStart(word, variety, listener, cancellation);
    }

    public static Task LRecordingClerkPublish(IReadOnlyList<LRecording> held, string variety, LListener listener)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(listener);

        foreach (LRecording recording in LHarvest.LHarvestRecordingScan(held, variety))
        {
            listener.LListenerRecordingAdd(recording);
        }

        listener.LListenerFinish();
        return Task.CompletedTask;
    }

    public Task<string> LRecordingClerkSave(
        LRecording recording, string word, string language, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(recording);
        return _lRecordingClerkRecordings.LRecordingSave(recording, word, language, cancellation);
    }

    public Task<string> LRecordingClerkPrepare(LRecording recording, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(recording);
        return _lRecordingClerkRecordings.LRecordingPrepare(recording, cancellation);
    }

    public bool LRecordingClerkExist(string? file)
    {
        return _lRecordingClerkTrail.LRecordingExist(file);
    }

    public LEntryDraft LRecordingClerkResolve(LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        List<LPronunciationDraft> resolved = new(draft.LEntryDraftPronunciations.Count);
        foreach (LPronunciationDraft spoken in draft.LEntryDraftPronunciations)
        {
            resolved.Add(spoken.LPronunciationDraftAudio.Length == 0
                ? spoken
                : spoken with
                {
                    LPronunciationDraftAudio = _lRecordingClerkTrail.LRecordingResolve(spoken.LPronunciationDraftAudio),
                });
        }

        return draft with { LEntryDraftPronunciations = resolved };
    }

    public void LRecordingClerkSweep()
    {
        HashSet<string> kept = new(StringComparer.OrdinalIgnoreCase);
        foreach (string file in _lRecordingClerkPronunciations.LPronunciationAudioScan())
        {
            LRecordingPlace(kept, file);
        }

        foreach (LDraft draft in _lRecordingClerkClaims.LDraftScan())
        {
            foreach (LPronunciationDraft spoken in draft.LDraftContent.LEntryDraftPronunciations)
            {
                LRecordingPlace(kept, spoken.LPronunciationDraftAudio);
            }
        }

        _lRecordingClerkRecordings.LRecordingSweep(kept);
    }

    private void LRecordingPlace(HashSet<string> kept, string file)
    {
        if (file.Length == 0)
        {
            return;
        }

        if (_lRecordingClerkTrail.LTrailFileResolve(file) is { IsFile: true } resolved)
        {
            kept.Add(_lRecordingClerkTrail.LTrailPathResolve(resolved.LocalPath) ?? resolved.LocalPath);
        }
    }

    private IReadOnlyList<LSource> LHarvestSourceRead(string language, LLanguage pack)
    {
        if (!_lRecordingClerkSources.TryGetValue(language, out IReadOnlyList<LSource>? sources))
        {
            sources = _lRecordingClerkFactory.LSourceFactoryCreate(pack.LLanguageHarvestSources);
            _lRecordingClerkSources[language] = sources;
        }

        return sources;
    }
}
