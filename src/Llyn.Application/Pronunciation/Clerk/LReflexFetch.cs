using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LReflexFetch
{
    private readonly LReflexClerk _lReflexFetchClerk;
    private readonly LEntryVault _lReflexFetchEntries;
    private readonly LReflexVault _lReflexFetchReflexes;
    private readonly LReflexSource _lReflexFetchSource;
    private readonly LClock _lReflexFetchClock;
    private readonly LLanguageCache _lReflexFetchLanguages;
    private readonly LClaimClerk _lReflexFetchClaims;
    private readonly object _lReflexFetchGate;
    private readonly Action<LSubject, long> _lReflexFetchBulletin;
    private readonly SemaphoreSlim _lReflexFetchAdmission = new(2, 2);
    private readonly Dictionary<long, CancellationTokenSource> _lReflexFetchPending = [];
    private readonly HashSet<long> _lReflexFetchMissed = [];
    private readonly LReflexGloss _lReflexFetchMeaning = new();
    private DateTimeOffset _lReflexFetchStamp = DateTimeOffset.MinValue;

    public LReflexFetch(
        LRig rig, LReflexClerk clerk, LLanguageCache languages, LClaimClerk claims, object gate,
        Action<LSubject, long> raise)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(clerk);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(gate);
        ArgumentNullException.ThrowIfNull(raise);
        _lReflexFetchClerk = clerk;
        _lReflexFetchEntries = rig.LRigEntries;
        _lReflexFetchReflexes = rig.LRigReflexes;
        _lReflexFetchSource = rig.LRigReflexSource;
        _lReflexFetchClock = rig.LRigClock;
        _lReflexFetchLanguages = languages;
        _lReflexFetchClaims = claims;
        _lReflexFetchGate = gate;
        _lReflexFetchBulletin = raise;
    }

    public void LReflexFetchStart(long entryId)
    {
        LEntry? entry;
        CancellationTokenSource fetch;
        lock (_lReflexFetchGate)
        {
            if (_lReflexFetchMissed.Contains(entryId) || _lReflexFetchPending.ContainsKey(entryId))
            {
                return;
            }

            entry = _lReflexFetchEntries.LEntryRead(entryId);
            if (entry is null
                || _lReflexFetchClerk.LReflexRuleRead(entry.LEntryLanguage).Count == 0
                || _lReflexFetchReflexes.LReflexRead(entryId).Count > 0)
            {
                return;
            }

            fetch = new CancellationTokenSource();
            _lReflexFetchPending[entryId] = fetch;
        }

        _ = LReflexFetchRun(entry, fetch);
    }

    public void LReflexFetchRebuild(long entryId)
    {
        List<long> cleared;
        lock (_lReflexFetchGate)
        {
            if (_lReflexFetchPending.ContainsKey(entryId))
            {
                return;
            }

            LEntry? entry = _lReflexFetchEntries.LEntryRead(entryId);
            if (entry is null || _lReflexFetchClerk.LReflexRuleRead(entry.LEntryLanguage).Count == 0)
            {
                return;
            }

            _lReflexFetchMissed.Remove(entryId);
            _lReflexFetchMeaning.LReflexGlossRecord(entryId, _lReflexFetchReflexes.LReflexRead(entryId));
            _lReflexFetchReflexes.LReflexSet(entryId, []);
            _lReflexFetchClerk.LReflexEpithetSave(entryId);
            cleared = LReflexFetchPropagate(entryId, [], true);
        }

        LReflexFetchStart(entryId);
        _lReflexFetchBulletin(LSubject.LSubjectReflex, entryId);
        foreach (long draft in cleared)
        {
            _lReflexFetchBulletin(LSubject.LSubjectDraft, draft);
        }
    }

    public bool LReflexFetchCheck(long entryId)
    {
        lock (_lReflexFetchGate)
        {
            return _lReflexFetchPending.ContainsKey(entryId);
        }
    }

    public void LReflexFetchClear()
    {
        lock (_lReflexFetchGate)
        {
            foreach (CancellationTokenSource held in _lReflexFetchPending.Values)
            {
                held.Cancel();
                held.Dispose();
            }

            _lReflexFetchPending.Clear();
            _lReflexFetchMissed.Clear();
            _lReflexFetchMeaning.LReflexGlossClear();
        }
    }

    private async Task<(IReadOnlyList<LReflexDraft> LReflexFound, bool LReflexReached)> LReflexFetchScan(
        string headword, string language, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(headword);

        IReadOnlyList<LReflexRule> rules;
        lock (_lReflexFetchGate)
        {
            rules = _lReflexFetchClerk.LReflexRuleRead(language);
        }

        IReadOnlyList<string> characters = LGlyph.LGlyphScan(headword);
        bool reached = false;
        List<IReadOnlyList<LReflexDraft>> rounds = [];
        foreach (LReflexRule rule in rules)
        {
            foreach (string character in characters)
            {
                TimeSpan left = _lReflexFetchStamp - _lReflexFetchClock.LClockRead();
                if (left > TimeSpan.Zero)
                {
                    await _lReflexFetchClock.LClockPause(left, cancellation).ConfigureAwait(false);
                }

                (IReadOnlyList<LReflexDraft> found, bool answered) = await _lReflexFetchSource
                    .LReflexSourceFind(rule, character, cancellation)
                    .ConfigureAwait(false);
                DateTimeOffset next = _lReflexFetchClock.LClockRead() + TimeSpan.FromSeconds(rule.LReflexRuleInterval);
                if (next > _lReflexFetchStamp)
                {
                    _lReflexFetchStamp = next;
                }

                reached |= answered;
                rounds.Add(found);
            }
        }

        IReadOnlyList<LReflexDraft> merged = LReflexClerkRow.LReflexRoundResolve(rounds);
        List<LReflexDraft> spelled = new(merged.Count);
        foreach (LReflexDraft row in merged)
        {
            spelled.Add(_lReflexFetchLanguages.LLanguageAnatomyResolve(
                language, _lReflexFetchLanguages.LLanguageRespellingResolve(row)));
        }

        return (spelled, reached);
    }

    private async Task LReflexFetchRun(LEntry entry, CancellationTokenSource fetch)
    {
        bool raised = false;
        bool admitted = false;
        List<long> drafts = [];
        try
        {
            await _lReflexFetchAdmission.WaitAsync(fetch.Token).ConfigureAwait(false);
            admitted = true;
            (IReadOnlyList<LReflexDraft> found, bool reached) = await LReflexFetchScan(
                entry.LEntryHeadword, entry.LEntryLanguage, fetch.Token).ConfigureAwait(false);

            lock (_lReflexFetchGate)
            {
                if (fetch.IsCancellationRequested)
                {
                    return;
                }

                raised = true;
                if (found.Count == 0)
                {
                    if (reached)
                    {
                        _lReflexFetchMissed.Add(entry.LEntryId);
                    }

                    return;
                }

                LEntry? current = _lReflexFetchEntries.LEntryRead(entry.LEntryId);
                LReflexVault reflexes = _lReflexFetchReflexes;
                if (current is null
                    || !string.Equals(current.LEntryHeadword, entry.LEntryHeadword, StringComparison.Ordinal)
                    || !string.Equals(current.LEntryLanguage, entry.LEntryLanguage, StringComparison.Ordinal)
                    || reflexes.LReflexRead(entry.LEntryId).Count > 0)
                {
                    return;
                }

                IReadOnlyList<LReflex> saved = reflexes.LReflexSet(
                    entry.LEntryId,
                    _lReflexFetchMeaning.LReflexGlossRestore(
                        entry.LEntryId, LReflexClerkRow.LReflexRowRead(entry.LEntryId, found)));
                _lReflexFetchClerk.LReflexEpithetSave(entry.LEntryId);
                drafts = LReflexFetchPropagate(entry.LEntryId, saved);
            }
        }
        catch (Exception)
        {
            raised = !fetch.IsCancellationRequested;
        }
        finally
        {
            if (admitted)
            {
                _lReflexFetchAdmission.Release();
            }

            lock (_lReflexFetchGate)
            {
                if (_lReflexFetchPending.TryGetValue(entry.LEntryId, out CancellationTokenSource? held)
                    && held == fetch)
                {
                    _lReflexFetchPending.Remove(entry.LEntryId);
                    fetch.Dispose();
                }
            }
        }

        if (raised)
        {
            _lReflexFetchBulletin(LSubject.LSubjectReflex, entry.LEntryId);
            foreach (long draft in drafts)
            {
                _lReflexFetchBulletin(LSubject.LSubjectDraft, draft);
            }
        }
    }

    private List<long> LReflexFetchPropagate(long entryId, IReadOnlyList<LReflex> saved, bool sweep = false)
    {
        List<LReflexDraft> rows = new(saved.Count);
        foreach (LReflex reflex in saved)
        {
            rows.Add(new LReflexDraft(
                reflex.LReflexLanguage,
                reflex.LReflexKind,
                reflex.LReflexText,
                reflex.LReflexMain,
                reflex.LReflexId,
                reflex.LReflexRomanization,
                reflex.LReflexMeaning,
                reflex.LReflexOwned,
                reflex.LReflexNote,
                reflex.LReflexRespelling,
                reflex.LReflexRegion,
                reflex.LReflexAnatomy,
                reflex.LReflexAnchors));
        }

        List<long> filled = [];
        foreach (long id in _lReflexFetchClaims.LClaimClerkHeld)
        {
            LDraft? draft = _lReflexFetchClaims.LDraftRead(id);
            if (draft is null
                || draft.LDraftEntryId != entryId
                || draft.LDraftExample is not null
                || draft.LDraftSituation is not null
                || draft.LDraftReference is not null
                || draft.LDraftAuthorHeld is not null
                || (!sweep && LDraftClerkEquality.LReflexScan(draft.LDraftContent.LEntryDraftReflexes).Count > 0))
            {
                continue;
            }

            _lReflexFetchClaims.LDraftSave(
                draft with { LDraftContent = draft.LDraftContent with { LEntryDraftReflexes = rows } });
            filled.Add(id);
        }

        return filled;
    }
}
