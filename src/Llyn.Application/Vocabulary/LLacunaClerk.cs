using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LLacunaClerk
{
    private readonly LEntryVault _lLacunaClerkEntries;
    private readonly LInflectionVault _lLacunaClerkInflections;
    private readonly LLacunaVault _lLacunaClerkLacunae;
    private readonly LSourceFactory _lLacunaClerkFactory;
    private readonly LAuditVault _lLacunaClerkAudit;
    private readonly LLanguageCache _lLacunaClerkLanguages;
    private readonly LParadigmClerk _lLacunaClerkParadigms;
    private readonly LClaimClerk _lLacunaClerkClaims;
    private readonly object _lLacunaClerkGate;
    private readonly Func<LSettings> _lLacunaClerkSettings;
    private readonly Action<LSubject, long> _lLacunaClerkBulletin;
    private readonly Dictionary<string, IReadOnlyList<LSource>> _lLacunaClerkSources = new(StringComparer.Ordinal);
    private readonly Dictionary<long, CancellationTokenSource> _lLacunaClerkPending = [];
    private readonly HashSet<long> _lLacunaClerkMissed = [];

    public LLacunaClerk(
        LRig rig,
        LLanguageCache languages,
        LParadigmClerk paradigms,
        LClaimClerk claims,
        object gate,
        Func<LSettings> settings,
        Action<LSubject, long> raise)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(paradigms);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(gate);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(raise);
        _lLacunaClerkEntries = rig.LRigEntries;
        _lLacunaClerkInflections = rig.LRigLexicon.LRigLexiconInflections;
        _lLacunaClerkLacunae = rig.LRigLexicon.LRigLexiconLacunae;
        _lLacunaClerkFactory = rig.LRigSource.LRigSourceFactory;
        _lLacunaClerkAudit = rig.LRigAudit;
        _lLacunaClerkLanguages = languages;
        _lLacunaClerkParadigms = paradigms;
        _lLacunaClerkClaims = claims;
        _lLacunaClerkGate = gate;
        _lLacunaClerkSettings = settings;
        _lLacunaClerkBulletin = raise;
    }

    public bool LLacunaClerkCheck(long entryId)
    {
        lock (_lLacunaClerkGate)
        {
            return _lLacunaClerkPending.ContainsKey(entryId);
        }
    }

    public void LLacunaClerkStart(long entryId)
    {
        LEntry? entry;
        CancellationTokenSource? fetch = null;
        bool stale = false;
        lock (_lLacunaClerkGate)
        {
            if (_lLacunaClerkPending.ContainsKey(entryId) || LDraftFind(entryId) is not null)
            {
                return;
            }

            entry = _lLacunaClerkEntries.LEntryRead(entryId);
            if (entry is null)
            {
                return;
            }

            IReadOnlyList<LParadigmSlot> slots = _lLacunaClerkParadigms.LParadigmClerkRead(entry);
            if (!string.IsNullOrWhiteSpace(entry.LEntryLanguage)
                && _lLacunaClerkLanguages.LLanguageCacheRead(entry.LEntryLanguage).LLanguageInflection is { } book)
            {
                stale = slots.Any(slot =>
                    slot.LParadigmSlotInflection is { } row
                    && !string.Equals(row.LInflectionStamp, book.LInflectionBookStamp, StringComparison.Ordinal));
                if (stale)
                {
                    _lLacunaClerkParadigms.LParadigmClerkUpdate(entry);
                }
            }

            if (_lLacunaClerkSettings().LSettingsMorphology
                && !_lLacunaClerkMissed.Contains(entryId)
                && LMorphologySourceRead(entry.LEntryLanguage).Count > 0
                && slots.Any(static slot => slot.LParadigmSlotState == LState.LStateUnspecified))
            {
                fetch = new CancellationTokenSource();
                _lLacunaClerkPending[entryId] = fetch;
            }
        }

        if (stale)
        {
            _lLacunaClerkBulletin(LSubject.LSubjectInflection, entryId);
        }

        if (fetch is not null)
        {
            _ = LLacunaClerkRun(entry, fetch);
        }
    }

    public void LLacunaClerkCancel(long entryId)
    {
        lock (_lLacunaClerkGate)
        {
            if (_lLacunaClerkPending.Remove(entryId, out CancellationTokenSource? held))
            {
                held.Cancel();
                held.Dispose();
            }

            _lLacunaClerkMissed.Remove(entryId);
            _lLacunaClerkLacunae.LLacunaDelete(entryId);
        }
    }

    public void LLacunaClerkClear()
    {
        lock (_lLacunaClerkGate)
        {
            foreach (CancellationTokenSource held in _lLacunaClerkPending.Values)
            {
                held.Cancel();
                held.Dispose();
            }

            _lLacunaClerkPending.Clear();
            _lLacunaClerkMissed.Clear();
        }
    }

    private LDraft? LDraftFind(long entryId)
    {
        LEntryDraft? stored = null;
        foreach (long held in _lLacunaClerkClaims.LClaimClerkHeld)
        {
            LDraft? draft = _lLacunaClerkClaims.LDraftRead(held);
            if (draft is null || draft.LDraftEntryId != entryId)
            {
                continue;
            }

            stored ??= _lLacunaClerkEntries.LEntryLoad(entryId);
            LEntryDraft content = draft.LDraftContent;
            if (stored is null
                || !string.Equals(
                    stored.LEntryDraftHeadword.Trim(), content.LEntryDraftHeadword.Trim(), StringComparison.Ordinal)
                || !string.Equals(stored.LEntryDraftLanguage, content.LEntryDraftLanguage, StringComparison.Ordinal)
                || !stored.LEntryDraftSpeeches.SequenceEqual(content.LEntryDraftSpeeches)
                || !LInflectionClerk.LInflectionClerkMatch(
                    stored.LEntryDraftInflections, content.LEntryDraftInflections))
            {
                return draft;
            }
        }

        return null;
    }

    private List<long> LDraftPropagate(long entryId)
    {
        IReadOnlyList<LInflection> stored = _lLacunaClerkInflections.LInflectionRead(entryId);
        List<long> filled = [];
        foreach (long held in _lLacunaClerkClaims.LClaimClerkHeld.ToList())
        {
            LDraft? draft = _lLacunaClerkClaims.LDraftRead(held);
            if (draft is null || draft.LDraftEntryId != entryId)
            {
                continue;
            }

            _lLacunaClerkClaims.LDraftSave(
                draft with { LDraftContent = draft.LDraftContent with { LEntryDraftInflections = stored } });
            filled.Add(held);
        }

        return filled;
    }

    private IReadOnlyList<LSource> LMorphologySourceRead(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return [];
        }

        if (_lLacunaClerkSources.TryGetValue(language, out IReadOnlyList<LSource>? sources))
        {
            return sources;
        }

        LLanguage pack = _lLacunaClerkLanguages.LLanguageCacheRead(language);
        sources = _lLacunaClerkFactory.LSourceFactoryCreate(pack.LLanguageMorphologies);
        _lLacunaClerkSources[language] = sources;
        return sources;
    }

    private async Task<(IReadOnlyDictionary<string, string> LLacunaFound, bool LLacunaReached)> LLacunaClerkScan(
        string word, string language, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(word);

        IReadOnlyList<LSource> sources;
        lock (_lLacunaClerkGate)
        {
            sources = LMorphologySourceRead(language);
        }

        bool reached = false;
        Dictionary<string, string> found = new(StringComparer.Ordinal);
        foreach (LSource source in sources)
        {
            LAnswer answer = await source.LSourceFind(word, cancellation).ConfigureAwait(false);
            reached |= answer.LAnswerReached;
            foreach (LReading reading in answer.LAnswerReadings)
            {
                if (!string.IsNullOrEmpty(reading.LReadingPhonetic))
                {
                    found.TryAdd(reading.LReadingVariety, reading.LReadingPhonetic);
                }
            }
        }

        return (found, reached);
    }

    private IReadOnlyList<long> LLacunaClerkApply(LEntry entry, IReadOnlyDictionary<string, string> found)
    {
        List<LInflection> appended = [];
        List<LLacuna> lacunae = [];
        foreach (LParadigmSlot slot in _lLacunaClerkParadigms.LParadigmClerkRead(entry))
        {
            if (slot.LParadigmSlotState == LState.LStateSpecified)
            {
                continue;
            }

            if (found.TryGetValue(slot.LParadigmSlotKey, out string? form))
            {
                appended.Add(new LInflection(
                    0,
                    entry.LEntryId,
                    0,
                    form,
                    null,
                    slot.LParadigmSlotSpeech.LSpeechValueId,
                    [.. slot.LParadigmSlotMorphologies.Select(static morphology => morphology.LMorphologyId)]));
            }
            else
            {
                long first = slot.LParadigmSlotMorphology.LMorphologyId;
                lacunae.Add(slot.LParadigmSlotMorphologies.Count == 1
                    ? new LLacuna(first)
                    : new LLacuna(first, slot.LParadigmSlotKey));
            }
        }

        if (appended.Count > 0)
        {
            _lLacunaClerkInflections.LInflectionAppend(entry.LEntryId, appended);
            _lLacunaClerkParadigms.LParadigmClerkUpdate(entry);
        }

        _lLacunaClerkLacunae.LLacunaSave(entry.LEntryId, lacunae);
        return appended.Count > 0 ? LDraftPropagate(entry.LEntryId) : [];
    }

    private async Task LLacunaClerkRun(LEntry entry, CancellationTokenSource fetch)
    {
        bool finished = false;
        IReadOnlyList<long> filled = [];
        try
        {
            (IReadOnlyDictionary<string, string> found, bool reached) = await LLacunaClerkScan(
                entry.LEntryHeadword, entry.LEntryLanguage, fetch.Token).ConfigureAwait(false);

            lock (_lLacunaClerkGate)
            {
                if (fetch.IsCancellationRequested || !_lLacunaClerkSettings().LSettingsMorphology)
                {
                    return;
                }

                LEntry? current = _lLacunaClerkEntries.LEntryRead(entry.LEntryId);
                if (current is null
                    || !string.Equals(current.LEntryHeadword, entry.LEntryHeadword, StringComparison.Ordinal)
                    || !string.Equals(current.LEntryLanguage, entry.LEntryLanguage, StringComparison.Ordinal)
                    || LDraftFind(entry.LEntryId) is not null)
                {
                    return;
                }

                if (reached)
                {
                    filled = LLacunaClerkApply(current, found);
                }
                else
                {
                    _lLacunaClerkMissed.Add(entry.LEntryId);
                }

                finished = true;
            }
        }
        catch (OperationCanceledException) when (fetch.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            _lLacunaClerkAudit.LAuditRecord(exception);
        }
        finally
        {
            lock (_lLacunaClerkGate)
            {
                if (_lLacunaClerkPending.TryGetValue(entry.LEntryId, out CancellationTokenSource? held)
                    && held == fetch)
                {
                    _lLacunaClerkPending.Remove(entry.LEntryId);
                    fetch.Dispose();
                }
            }
        }

        if (finished)
        {
            _lLacunaClerkBulletin(LSubject.LSubjectInflection, entry.LEntryId);
            foreach (long draft in filled)
            {
                _lLacunaClerkBulletin(LSubject.LSubjectDraft, draft);
            }
        }
    }
}
