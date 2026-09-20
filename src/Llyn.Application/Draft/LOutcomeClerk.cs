using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LOutcomeClerk
{
    private readonly LVault _lOutcomeClerkVault;
    private readonly LLanguageCache _lOutcomeClerkLanguages;
    private readonly LDraftClerk _lOutcomeClerkDrafts;
    private readonly LClaimClerk _lOutcomeClerkClaims;
    private readonly LCourtClerk _lOutcomeClerkCourts;
    private readonly LEntryClerk _lOutcomeClerkEntries;
    private readonly LLacunaClerk _lOutcomeClerkLacunae;
    private readonly LFrequencyClerk _lOutcomeClerkFrequencies;

    public LOutcomeClerk(
        LRig rig,
        LLanguageCache languages,
        LDraftClerk drafts,
        LClaimClerk claims,
        LCourtClerk courts,
        LEntryClerk entries,
        LLacunaClerk lacunae,
        LFrequencyClerk frequencies)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(courts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(lacunae);
        ArgumentNullException.ThrowIfNull(frequencies);
        _lOutcomeClerkVault = rig.LRigVault;
        _lOutcomeClerkLanguages = languages;
        _lOutcomeClerkDrafts = drafts;
        _lOutcomeClerkClaims = claims;
        _lOutcomeClerkCourts = courts;
        _lOutcomeClerkEntries = entries;
        _lOutcomeClerkLacunae = lacunae;
        _lOutcomeClerkFrequencies = frequencies;
    }

    public LEntry LOutcomeEntrySave(LEntryDraft draft, Dictionary<long, long> identity)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(identity);

        draft = _lOutcomeClerkLanguages.LLanguageRespellingUpdate(_lOutcomeClerkDrafts.LDraftClerkNormalize(draft) with
        {
            LEntryDraftHeadword = draft.LEntryDraftHeadword.Trim(),
        });

        LEntry entry = _lOutcomeClerkEntries.LEntryClerkSave(draft, identity);
        _lOutcomeClerkFrequencies.LFrequencyClerkStart(entry.LEntryId);
        return entry;
    }

    public LEntry LOutcomeEntryUpdate(long id, LEntryDraft draft, Dictionary<long, long> identity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(identity);

        LEntry? stored = _lOutcomeClerkEntries.LEntryClerkRead(id);
        _lOutcomeClerkLacunae.LLacunaClerkCancel(id);
        LEntry updated = _lOutcomeClerkEntries.LEntryClerkUpdate(id, draft, identity);

        bool renamed = stored is null
            || !string.Equals(stored.LEntryHeadword, updated.LEntryHeadword, StringComparison.Ordinal)
            || !string.Equals(stored.LEntryLanguage, updated.LEntryLanguage, StringComparison.Ordinal);
        if (renamed)
        {
            _lOutcomeClerkFrequencies.LFrequencyClerkStart(id);
        }

        return updated;
    }

    public LOutcome LOutcomeClerkCommit(long id, List<long> raised)
    {
        ArgumentOutOfRangeException.ThrowIfZero(id);
        ArgumentNullException.ThrowIfNull(raised);

        Dictionary<long, LDraft> loaded = [];
        Dictionary<long, LOutcome> settled = [];
        List<LCourt> deferred = [];
        List<long> finished = [];
        List<Action> written = [];

        LOutcome outcome;
        using (LVaultSession session = _lOutcomeClerkVault.LVaultSessionStart())
        {
            outcome = LOutcomeClerkCommit(id, true, loaded, settled, deferred, finished, written);
            _lOutcomeClerkCourts.LCourtClerkApply(loaded, settled, deferred);
            session.LVaultSessionCommit();
        }

        foreach (Action step in written)
        {
            step();
        }

        foreach (long done in finished)
        {
            _lOutcomeClerkClaims.LClaimClerkFinish(done);
        }

        foreach (LOutcome made in settled.Values)
        {
            raised.Add(made.LOutcomeEntry.LEntryId);
        }

        return outcome;
    }

    private LOutcome LOutcomeClerkCommit(
        long id,
        bool held,
        Dictionary<long, LDraft> loaded,
        Dictionary<long, LOutcome> settled,
        List<LCourt> deferred,
        List<long> finished,
        List<Action> written)
    {
        LDraft draft = _lOutcomeClerkClaims.LDraftLoad(id);
        loaded[id] = draft;
        Dictionary<long, long> identity = [];

        foreach (LCourt link in _lOutcomeClerkCourts.LCourtClerkScan(id))
        {
            if (loaded.ContainsKey(link.LCourtTargetId))
            {
                if (settled.TryGetValue(link.LCourtTargetId, out LOutcome? made))
                {
                    LIdentity.LIdentityRecord(identity, link.LCourtTargetId, made.LOutcomeEntry.LEntryId);
                }
                else
                {
                    deferred.Add(link);
                }

                continue;
            }

            if (_lOutcomeClerkClaims.LDraftRead(link.LCourtTargetId) is null)
            {
                continue;
            }

            LOutcome target = LOutcomeClerkCommit(
                link.LCourtTargetId,
                _lOutcomeClerkClaims.LClaimClerkCheck(link.LCourtTargetId),
                loaded,
                settled,
                deferred,
                finished,
                written);
            LIdentity.LIdentityRecord(identity, link.LCourtTargetId, target.LOutcomeEntry.LEntryId);
        }

        LEntryDraft sending = _lOutcomeClerkCourts.LTranslationSettle(draft.LDraftContent, identity);

        LEntry entry = draft.LDraftEntryId <= 0
            || _lOutcomeClerkEntries.LEntryClerkLoad(draft.LDraftEntryId) is null
            ? LOutcomeEntrySave(sending, identity)
            : LOutcomeEntryUpdate(draft.LDraftEntryId, sending, identity);

        LOutcome outcome = new(entry, identity);
        settled[id] = outcome;

        LDraft saved = draft with { LDraftEntryId = entry.LEntryId };
        if (_lOutcomeClerkEntries.LEntryClerkLoad(entry.LEntryId) is LEntryDraft stored)
        {
            saved = saved with { LDraftContent = stored };
        }

        long entryId = entry.LEntryId;
        written.Add(() => _lOutcomeClerkClaims.LDraftSave(saved));
        written.Add(() => _lOutcomeClerkCourts.LCourtClerkSettle(id, entryId));

        if (held)
        {
            finished.Add(id);
        }

        return outcome;
    }
}
