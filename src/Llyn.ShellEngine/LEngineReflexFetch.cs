using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private readonly SemaphoreSlim _lEngineReflexGate = new(2, 2);
    private readonly Dictionary<long, CancellationTokenSource> _lEngineReflexPending = [];
    private readonly HashSet<long> _lEngineReflexMissed = [];
    private DateTime _lEngineReflexStamp = DateTime.MinValue;

    public void LEngineReflexStart(long entryId)
    {
        LEntry? entry;
        CancellationTokenSource fetch;
        lock (_lEngineGate)
        {
            if (_lEngineReflexMissed.Contains(entryId) || _lEngineReflexPending.ContainsKey(entryId))
            {
                return;
            }

            entry = _lEngineEntries.LEntryRead(entryId);
            if (entry is null
                || LEngineReflexRead(entry.LEntryLanguage).Count == 0
                || new LReflexArchive(_lEngineDatabase).LReflexRead(entryId).Count > 0)
            {
                return;
            }

            fetch = new CancellationTokenSource();
            _lEngineReflexPending[entryId] = fetch;
        }

        _ = LEngineReflexRun(entry, fetch);
    }

    public void LEngineReflexRebuild(long entryId)
    {
        List<long> cleared;
        lock (_lEngineGate)
        {
            if (_lEngineReflexPending.ContainsKey(entryId))
            {
                return;
            }

            LEntry? entry = _lEngineEntries.LEntryRead(entryId);
            if (entry is null || LEngineReflexRead(entry.LEntryLanguage).Count == 0)
            {
                return;
            }

            _lEngineReflexMissed.Remove(entryId);
            new LReflexArchive(_lEngineDatabase).LReflexSet(entryId, []);
            LEngineEpithetUpdate(entryId);
            cleared = LEngineReflexPropagate(entryId, [], true);
        }

        LEngineReflexStart(entryId);
        LEngineBulletinRaise(LSubject.LSubjectReflex, entryId);
        foreach (long draft in cleared)
        {
            LEngineBulletinRaise(LSubject.LSubjectDraft, draft);
        }
    }

    public bool LEngineReflexCheck(long entryId)
    {
        lock (_lEngineGate)
        {
            return _lEngineReflexPending.ContainsKey(entryId);
        }
    }

    internal async Task<IReadOnlyList<LReflexDraft>> LEngineReflexFind(
        string headword, string language, CancellationToken cancellation)
    {
        (IReadOnlyList<LReflexDraft> found, _) =
            await LEngineReflexScan(headword, language, cancellation).ConfigureAwait(false);
        return found;
    }

    private async Task<(IReadOnlyList<LReflexDraft> LReflexFound, bool LReflexReached)> LEngineReflexScan(
        string headword, string language, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(headword);

        IReadOnlyList<LReflexRule> rules;
        lock (_lEngineGate)
        {
            rules = LEngineReflexRead(language);
        }

        IReadOnlyList<string> characters = LGlyph.LGlyphScan(headword);
        bool reached = false;
        List<IReadOnlyList<LReflexDraft>> rounds = [];
        foreach (LReflexRule rule in rules)
        {
            foreach (string character in characters)
            {
                TimeSpan left = _lEngineReflexStamp - DateTime.UtcNow;
                if (left > TimeSpan.Zero)
                {
                    await Task.Delay(left, cancellation).ConfigureAwait(false);
                }

                (IReadOnlyList<LReflexDraft> found, bool answered) = await LReflexSource
                    .LReflexSourceFind(_lEngineClient, rule, character, cancellation)
                    .ConfigureAwait(false);
                DateTime next = DateTime.UtcNow + TimeSpan.FromSeconds(rule.LReflexRuleInterval);
                if (next > _lEngineReflexStamp)
                {
                    _lEngineReflexStamp = next;
                }

                reached |= answered;
                rounds.Add(found);
            }
        }

        IReadOnlyList<LReflexDraft> merged = LEngineReflexResolve(rounds);
        List<LReflexDraft> spelled = new(merged.Count);
        foreach (LReflexDraft row in merged)
        {
            spelled.Add(LEngineAnatomyResolve(language, LEngineRespellingResolve(row)));
        }

        return (spelled, reached);
    }

    private static IReadOnlyList<LReflexDraft> LEngineReflexResolve(IReadOnlyList<IReadOnlyList<LReflexDraft>> rounds)
    {
        List<LReflexDraft> merged = [];
        List<int> stamps = [];
        for (int round = 0; round < rounds.Count; round++)
        {
            foreach (LReflexDraft row in rounds[round])
            {
                int place = merged.FindIndex(held =>
                    string.Equals(held.LReflexDraftLanguage, row.LReflexDraftLanguage, StringComparison.Ordinal)
                    && string.Equals(held.LReflexDraftRegion, row.LReflexDraftRegion, StringComparison.Ordinal)
                    && string.Equals(held.LReflexDraftKind, row.LReflexDraftKind, StringComparison.Ordinal)
                    && string.Equals(held.LReflexDraftNote, row.LReflexDraftNote, StringComparison.Ordinal));
                if (place >= 0 && stamps[place] != round)
                {
                    LReflexDraft held = merged[place];
                    merged[place] = held with
                    {
                        LReflexDraftText = held.LReflexDraftText + ' ' + row.LReflexDraftText,
                        LReflexDraftMain = held.LReflexDraftMain || row.LReflexDraftMain,
                    };
                    stamps[place] = round;
                    continue;
                }

                merged.Add(row);
                stamps.Add(round);
            }
        }

        return merged;
    }

    private void LEngineReflexClear()
    {
        foreach (CancellationTokenSource held in _lEngineReflexPending.Values)
        {
            held.Cancel();
            held.Dispose();
        }

        _lEngineReflexPending.Clear();
        _lEngineReflexMissed.Clear();
    }

    private async Task LEngineReflexRun(LEntry entry, CancellationTokenSource fetch)
    {
        bool raised = false;
        bool admitted = false;
        List<long> drafts = [];
        try
        {
            await _lEngineReflexGate.WaitAsync(fetch.Token).ConfigureAwait(false);
            admitted = true;
            (IReadOnlyList<LReflexDraft> found, bool reached) = await LEngineReflexScan(
                entry.LEntryHeadword, entry.LEntryLanguage, fetch.Token).ConfigureAwait(false);

            lock (_lEngineGate)
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
                        _lEngineReflexMissed.Add(entry.LEntryId);
                    }

                    return;
                }

                LEntry? current = _lEngineEntries.LEntryRead(entry.LEntryId);
                LReflexArchive reflexes = new(_lEngineDatabase);
                if (current is null
                    || !string.Equals(current.LEntryHeadword, entry.LEntryHeadword, StringComparison.Ordinal)
                    || !string.Equals(current.LEntryLanguage, entry.LEntryLanguage, StringComparison.Ordinal)
                    || reflexes.LReflexRead(entry.LEntryId).Count > 0)
                {
                    return;
                }

                IReadOnlyList<LReflex> saved = reflexes.LReflexSet(
                    entry.LEntryId, LEngineReflexRead(entry.LEntryId, found));
                LEngineEpithetUpdate(entry.LEntryId);
                drafts = LEngineReflexPropagate(entry.LEntryId, saved);
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
                _lEngineReflexGate.Release();
            }

            lock (_lEngineGate)
            {
                if (_lEngineReflexPending.TryGetValue(entry.LEntryId, out CancellationTokenSource? held)
                    && held == fetch)
                {
                    _lEngineReflexPending.Remove(entry.LEntryId);
                    fetch.Dispose();
                }
            }
        }

        if (raised)
        {
            LEngineBulletinRaise(LSubject.LSubjectReflex, entry.LEntryId);
            foreach (long draft in drafts)
            {
                LEngineBulletinRaise(LSubject.LSubjectDraft, draft);
            }
        }
    }

    private List<long> LEngineReflexPropagate(long entryId, IReadOnlyList<LReflex> saved, bool sweep = false)
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
                reflex.LReflexNote,
                reflex.LReflexRespelling,
                reflex.LReflexRegion,
                reflex.LReflexRemark,
                reflex.LReflexAnatomy,
                reflex.LReflexAnchors));
        }

        List<long> filled = [];
        foreach (long id in _lEngineDraftHeld)
        {
            LDraft? draft = _lEngineDrafts.LDraftRead(id);
            if (draft is null
                || draft.LDraftEntryId != entryId
                || draft.LDraftExample is not null
                || draft.LDraftSituation is not null
                || draft.LDraftReference is not null
                || draft.LDraftAuthorHeld is not null
                || (!sweep && LEngineReflexScan(draft.LDraftContent.LEntryDraftReflexes).Count > 0))
            {
                continue;
            }

            _lEngineDrafts.LDraftSave(
                draft with { LDraftContent = draft.LDraftContent with { LEntryDraftReflexes = rows } });
            filled.Add(id);
        }

        return filled;
    }
}
