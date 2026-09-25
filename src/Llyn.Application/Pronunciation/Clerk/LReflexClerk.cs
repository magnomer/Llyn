using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LReflexClerk
{
    private readonly LEntryVault _lReflexClerkEntries;
    private readonly LReflexVault _lReflexClerkReflexes;
    private readonly LReflexSource _lReflexClerkSource;
    private readonly LClock _lReflexClerkClock;
    private readonly LLanguageCache _lReflexClerkLanguages;
    private readonly LClaimClerk _lReflexClerkClaims;
    private readonly object _lReflexClerkGate;
    private readonly Action<LSubject, long> _lReflexClerkBulletin;
    private readonly SemaphoreSlim _lReflexClerkAdmission = new(2, 2);
    private readonly Dictionary<long, CancellationTokenSource> _lReflexClerkPending = [];
    private readonly HashSet<long> _lReflexClerkMissed = [];
    private readonly Dictionary<long, Dictionary<string, string>> _lReflexClerkMeaning = [];
    private DateTimeOffset _lReflexClerkStamp = DateTimeOffset.MinValue;

    public LReflexClerk(
        LRig rig, LLanguageCache languages, LClaimClerk claims, object gate, Action<LSubject, long> raise)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(gate);
        ArgumentNullException.ThrowIfNull(raise);
        _lReflexClerkEntries = rig.LRigEntries;
        _lReflexClerkReflexes = rig.LRigReflexes;
        _lReflexClerkSource = rig.LRigReflexSource;
        _lReflexClerkClock = rig.LRigClock;
        _lReflexClerkLanguages = languages;
        _lReflexClerkClaims = claims;
        _lReflexClerkGate = gate;
        _lReflexClerkBulletin = raise;
    }

    public static IReadOnlyList<long> LReflexAnchorToggle(IReadOnlyList<long> anchors, long fanqieId, bool anchored)
    {
        return LAnchor.LAnchorToggle(anchors, fanqieId, anchored);
    }

    public static bool LReflexAnchorMatch(IReadOnlyList<long> one, IReadOnlyList<long> other)
    {
        return LAnchor.LAnchorMatch(one, other);
    }

    public static IReadOnlyList<LAnchorRow> LReflexAnchorScan(
        IReadOnlyList<LFanqieRow> rows,
        IReadOnlyList<long> anchors,
        IReadOnlyList<LAnatomyTone> tones,
        string reflex,
        string tone)
    {
        return LAnchor.LAnchorRowScan(rows, anchors, LAnatomyTone.LAnatomyToneScan(tones, reflex, tone));
    }

    public static bool LReflexAnchorCheck(IReadOnlyList<LFanqieRow> rows, string headword)
    {
        return LAnchor.LAnchorCheck(rows, headword);
    }

    public static string LReflexAnchorFormat(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, string headword, string separator)
    {
        return LAnchor.LAnchorTextFormat(rows, anchors, headword, separator);
    }

    public IReadOnlyList<LReflexRule> LReflexRuleRead(string language)
    {
        return string.IsNullOrWhiteSpace(language)
            ? []
            : _lReflexClerkLanguages.LLanguageCacheRead(language).LLanguageReflexRules;
    }

    public void LReflexClerkSync(
        long entryId,
        string language,
        IReadOnlyList<LReflexDraft> drafts,
        List<LRevisionChange>? changes,
        Dictionary<long, long> identity)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(identity);

        LReflexVault reflexes = _lReflexClerkReflexes;
        IReadOnlyList<LReflex> stored = reflexes.LReflexRead(entryId);
        IReadOnlyList<LReflexDraft> written = _lReflexClerkLanguages.LLanguageAnatomyScan(
            language, LDraftClerkEquality.LReflexScan(drafts));
        IReadOnlyList<LReflex> current = LReflexClerkRow.LReflexRowRead(entryId, written);

        if (LReflexClerkRow.LReflexRowMatch(stored, current))
        {
            return;
        }

        foreach (LReflex row in current)
        {
            if (row.LReflexId > 0 && !stored.Any(kept => kept.LReflexId == row.LReflexId))
            {
                throw new LRefusal(LRefusal.LRefusalLink);
            }
        }

        IReadOnlyList<LReflex> saved = reflexes.LReflexSet(entryId, current);
        LReflexEpithetSave(entryId);
        for (int index = 0; index < saved.Count; index++)
        {
            LIdentity.LIdentityRecord(identity, written[index].LReflexDraftId, saved[index].LReflexId);
        }

        if (LReflexClerkRow.LReflexRowMatch(
                LReflexClerkRow.LReflexAnatomyClear(stored), LReflexClerkRow.LReflexAnatomyClear(current)))
        {
            return;
        }

        changes?.Add(new LRevisionChange(
            entryId,
            "reflex",
            current.Count == 0 ? "delete" : stored.Count == 0 ? "create" : "update",
            LReflexClerkRow.LReflexRowFormat(current)));
    }

    public static IReadOnlyList<LReflexDraft> LReflexClerkReset(IReadOnlyList<LReflexDraft> drafts)
    {
        ArgumentNullException.ThrowIfNull(drafts);

        List<LReflexDraft> renewed = new(drafts.Count);
        foreach (LReflexDraft draft in drafts)
        {
            renewed.Add(draft.LReflexDraftId > 0 ? draft with { LReflexDraftId = 0 } : draft);
        }

        return renewed;
    }

    public void LReflexEpithetSave(long entryId)
    {
        LEntry? entry = _lReflexClerkEntries.LEntryRead(entryId);
        if (entry is null)
        {
            return;
        }

        IReadOnlyList<LReflexRule> rules = LReflexRuleRead(entry.LEntryLanguage);
        string epithet = LReflexClerkEpithet.LReflexEpithetCheck(rules)
            ? LReflexClerkEpithet.LReflexEpithetFormat(rules, _lReflexClerkReflexes.LReflexRead(entryId))
            : string.Empty;
        _lReflexClerkEntries.LEntryEpithetSave(entryId, epithet);
    }

    public void LReflexClerkStart(long entryId)
    {
        LEntry? entry;
        CancellationTokenSource fetch;
        lock (_lReflexClerkGate)
        {
            if (_lReflexClerkMissed.Contains(entryId) || _lReflexClerkPending.ContainsKey(entryId))
            {
                return;
            }

            entry = _lReflexClerkEntries.LEntryRead(entryId);
            if (entry is null
                || LReflexRuleRead(entry.LEntryLanguage).Count == 0
                || _lReflexClerkReflexes.LReflexRead(entryId).Count > 0)
            {
                return;
            }

            fetch = new CancellationTokenSource();
            _lReflexClerkPending[entryId] = fetch;
        }

        _ = LReflexClerkRun(entry, fetch);
    }

    public void LReflexClerkRebuild(long entryId)
    {
        List<long> cleared;
        lock (_lReflexClerkGate)
        {
            if (_lReflexClerkPending.ContainsKey(entryId))
            {
                return;
            }

            LEntry? entry = _lReflexClerkEntries.LEntryRead(entryId);
            if (entry is null || LReflexRuleRead(entry.LEntryLanguage).Count == 0)
            {
                return;
            }

            _lReflexClerkMissed.Remove(entryId);
            LReflexMeaningRecord(entryId);
            _lReflexClerkReflexes.LReflexSet(entryId, []);
            LReflexEpithetSave(entryId);
            cleared = LReflexClerkPropagate(entryId, [], true);
        }

        LReflexClerkStart(entryId);
        _lReflexClerkBulletin(LSubject.LSubjectReflex, entryId);
        foreach (long draft in cleared)
        {
            _lReflexClerkBulletin(LSubject.LSubjectDraft, draft);
        }
    }

    public bool LReflexClerkCheck(long entryId)
    {
        lock (_lReflexClerkGate)
        {
            return _lReflexClerkPending.ContainsKey(entryId);
        }
    }

    public void LReflexClerkClear()
    {
        lock (_lReflexClerkGate)
        {
            foreach (CancellationTokenSource held in _lReflexClerkPending.Values)
            {
                held.Cancel();
                held.Dispose();
            }

            _lReflexClerkPending.Clear();
            _lReflexClerkMissed.Clear();
            _lReflexClerkMeaning.Clear();
        }
    }

    private async Task<(IReadOnlyList<LReflexDraft> LReflexFound, bool LReflexReached)> LReflexClerkScan(
        string headword, string language, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(headword);

        IReadOnlyList<LReflexRule> rules;
        lock (_lReflexClerkGate)
        {
            rules = LReflexRuleRead(language);
        }

        IReadOnlyList<string> characters = LGlyph.LGlyphScan(headword);
        bool reached = false;
        List<IReadOnlyList<LReflexDraft>> rounds = [];
        foreach (LReflexRule rule in rules)
        {
            foreach (string character in characters)
            {
                TimeSpan left = _lReflexClerkStamp - _lReflexClerkClock.LClockRead();
                if (left > TimeSpan.Zero)
                {
                    await Task.Delay(left, cancellation).ConfigureAwait(false);
                }

                (IReadOnlyList<LReflexDraft> found, bool answered) = await _lReflexClerkSource
                    .LReflexSourceFind(rule, character, cancellation)
                    .ConfigureAwait(false);
                DateTimeOffset next = _lReflexClerkClock.LClockRead() + TimeSpan.FromSeconds(rule.LReflexRuleInterval);
                if (next > _lReflexClerkStamp)
                {
                    _lReflexClerkStamp = next;
                }

                reached |= answered;
                rounds.Add(found);
            }
        }

        IReadOnlyList<LReflexDraft> merged = LReflexClerkRow.LReflexRoundResolve(rounds);
        List<LReflexDraft> spelled = new(merged.Count);
        foreach (LReflexDraft row in merged)
        {
            spelled.Add(_lReflexClerkLanguages.LLanguageAnatomyResolve(
                language, _lReflexClerkLanguages.LLanguageRespellingResolve(row)));
        }

        return (spelled, reached);
    }

    private async Task LReflexClerkRun(LEntry entry, CancellationTokenSource fetch)
    {
        bool raised = false;
        bool admitted = false;
        List<long> drafts = [];
        try
        {
            await _lReflexClerkAdmission.WaitAsync(fetch.Token).ConfigureAwait(false);
            admitted = true;
            (IReadOnlyList<LReflexDraft> found, bool reached) = await LReflexClerkScan(
                entry.LEntryHeadword, entry.LEntryLanguage, fetch.Token).ConfigureAwait(false);

            lock (_lReflexClerkGate)
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
                        _lReflexClerkMissed.Add(entry.LEntryId);
                    }

                    return;
                }

                LEntry? current = _lReflexClerkEntries.LEntryRead(entry.LEntryId);
                LReflexVault reflexes = _lReflexClerkReflexes;
                if (current is null
                    || !string.Equals(current.LEntryHeadword, entry.LEntryHeadword, StringComparison.Ordinal)
                    || !string.Equals(current.LEntryLanguage, entry.LEntryLanguage, StringComparison.Ordinal)
                    || reflexes.LReflexRead(entry.LEntryId).Count > 0)
                {
                    return;
                }

                IReadOnlyList<LReflex> saved = reflexes.LReflexSet(
                    entry.LEntryId,
                    LReflexMeaningRestore(
                        entry.LEntryId, LReflexClerkRow.LReflexRowRead(entry.LEntryId, found)));
                LReflexEpithetSave(entry.LEntryId);
                drafts = LReflexClerkPropagate(entry.LEntryId, saved);
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
                _lReflexClerkAdmission.Release();
            }

            lock (_lReflexClerkGate)
            {
                if (_lReflexClerkPending.TryGetValue(entry.LEntryId, out CancellationTokenSource? held)
                    && held == fetch)
                {
                    _lReflexClerkPending.Remove(entry.LEntryId);
                    fetch.Dispose();
                }
            }
        }

        if (raised)
        {
            _lReflexClerkBulletin(LSubject.LSubjectReflex, entry.LEntryId);
            foreach (long draft in drafts)
            {
                _lReflexClerkBulletin(LSubject.LSubjectDraft, draft);
            }
        }
    }

    private void LReflexMeaningRecord(long entryId)
    {
        Dictionary<string, string> held = [];
        foreach (LReflex row in _lReflexClerkReflexes.LReflexRead(entryId))
        {
            if (row.LReflexOwned && row.LReflexMeaning.Length > 0)
            {
                held[LMeaningKeyRead(row)] = row.LReflexMeaning;
            }
        }

        if (held.Count > 0)
        {
            _lReflexClerkMeaning[entryId] = held;
        }
        else
        {
            _lReflexClerkMeaning.Remove(entryId);
        }
    }

    private IReadOnlyList<LReflex> LReflexMeaningRestore(long entryId, IReadOnlyList<LReflex> rows)
    {
        if (!_lReflexClerkMeaning.Remove(entryId, out Dictionary<string, string>? held))
        {
            return rows;
        }

        List<LReflex> filled = new(rows.Count);
        foreach (LReflex row in rows)
        {
            filled.Add(held.TryGetValue(LMeaningKeyRead(row), out string? meaning)
                ? row with { LReflexMeaning = meaning, LReflexOwned = true }
                : row);
        }

        return filled;
    }

    private static string LMeaningKeyRead(LReflex row)
    {
        return string.Join(
            '\u001F', row.LReflexLanguage, row.LReflexRegion, row.LReflexKind, row.LReflexText);
    }

    private List<long> LReflexClerkPropagate(long entryId, IReadOnlyList<LReflex> saved, bool sweep = false)
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
        foreach (long id in _lReflexClerkClaims.LClaimClerkHeld)
        {
            LDraft? draft = _lReflexClerkClaims.LDraftRead(id);
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

            _lReflexClerkClaims.LDraftSave(
                draft with { LDraftContent = draft.LDraftContent with { LEntryDraftReflexes = rows } });
            filled.Add(id);
        }

        return filled;
    }
}
