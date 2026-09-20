using System;
using System.Collections.Generic;
using System.Globalization;
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
    private readonly LLanguageCache _lLacunaClerkLanguages;
    private readonly LParadigmClerk _lLacunaClerkParadigms;
    private readonly LClaimClerk _lLacunaClerkClaims;
    private readonly object _lLacunaClerkGate;
    private readonly Func<LSettings> _lLacunaClerkSettings;
    private readonly Action<LSubject, long> _lLacunaClerkBulletin;
    private readonly Dictionary<string, IReadOnlyList<LSource>> _lLacunaClerkSources = new(StringComparer.Ordinal);
    private readonly Dictionary<long, CancellationTokenSource> _lLacunaClerkPending = [];

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
        _lLacunaClerkInflections = rig.LRigInflections;
        _lLacunaClerkLacunae = rig.LRigLacunae;
        _lLacunaClerkFactory = rig.LRigSources;
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
        CancellationTokenSource fetch;
        lock (_lLacunaClerkGate)
        {
            if (!_lLacunaClerkSettings().LSettingsMorphology
                || _lLacunaClerkPending.ContainsKey(entryId)
                || LDraftFind(entryId) is not null)
            {
                return;
            }

            entry = _lLacunaClerkEntries.LEntryRead(entryId);
            if (entry is null || LMorphologySourceRead(entry.LEntryLanguage).Count == 0)
            {
                return;
            }

            foreach (LLacuna lacuna in _lLacunaClerkLacunae.LLacunaRead(entryId))
            {
                if (lacuna.LLacunaMorphologyId is null)
                {
                    return;
                }
            }

            bool wanted = false;
            foreach (LParadigmSlot slot in _lLacunaClerkParadigms.LParadigmClerkRead(entry))
            {
                wanted |= slot.LParadigmSlotState == LState.LStateUnspecified;
            }

            if (!wanted)
            {
                return;
            }

            fetch = new CancellationTokenSource();
            _lLacunaClerkPending[entryId] = fetch;
        }

        _ = LLacunaClerkRun(entry, fetch);
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
        }
    }

    private LDraft? LDraftFind(long entryId)
    {
        foreach (long held in _lLacunaClerkClaims.LClaimClerkHeld)
        {
            LDraft? draft = _lLacunaClerkClaims.LDraftRead(held);
            if (draft is not null && draft.LDraftEntryId == entryId)
            {
                return draft;
            }
        }

        return null;
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

    private void LLacunaClerkApply(LEntry entry, IReadOnlyDictionary<string, string> found)
    {
        List<LInflection> appended = [];
        List<long?> missed = [];
        foreach (LParadigmSlot slot in _lLacunaClerkParadigms.LParadigmClerkRead(entry))
        {
            if (slot.LParadigmSlotState == LState.LStateSpecified)
            {
                continue;
            }

            string code = slot.LParadigmSlotMorphology.LMorphologyCode.ToString(CultureInfo.InvariantCulture);
            if (found.TryGetValue(code, out string? form))
            {
                appended.Add(new LInflection(
                    0,
                    entry.LEntryId,
                    0,
                    form,
                    null,
                    slot.LParadigmSlotSpeech.LSpeechValueId,
                    [slot.LParadigmSlotMorphology.LMorphologyId]));
            }
            else
            {
                missed.Add(slot.LParadigmSlotMorphology.LMorphologyId);
            }
        }

        if (appended.Count > 0)
        {
            _lLacunaClerkInflections.LInflectionAppend(entry.LEntryId, appended);
            _lLacunaClerkParadigms.LParadigmClerkUpdate(entry);
        }

        _lLacunaClerkLacunae.LLacunaSave(entry.LEntryId, missed);
    }

    private async Task LLacunaClerkRun(LEntry entry, CancellationTokenSource fetch)
    {
        bool finished = false;
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
                    LLacunaClerkApply(current, found);
                }
                else
                {
                    LLacunaVault lacunae = _lLacunaClerkLacunae;
                    List<long?> kept = [];
                    foreach (LLacuna lacuna in lacunae.LLacunaRead(entry.LEntryId))
                    {
                        kept.Add(lacuna.LLacunaMorphologyId);
                    }

                    kept.Add(null);
                    lacunae.LLacunaSave(entry.LEntryId, kept);
                }

                finished = true;
            }
        }
        catch (Exception)
        {
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
        }
    }
}
