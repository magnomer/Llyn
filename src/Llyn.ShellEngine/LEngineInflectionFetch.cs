using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private IReadOnlyList<LSource> LEngineInflectionLoad(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return [];
        }

        if (_lEngineInflectionSources.TryGetValue(language, out IReadOnlyList<LSource>? sources))
        {
            return sources;
        }

        LLanguage pack = LEngineLanguageLoad(language);
        sources = LSourceFactory.LSourceFactoryCreate(pack.LLanguageMorphologies, _lEngineClient);
        _lEngineInflectionSources[language] = sources;
        return sources;
    }

    private async Task<(IReadOnlyDictionary<string, string> LInflectionFound, bool LInflectionReached)> LEngineInflectionScan(
        string word, string language, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(word);

        IReadOnlyList<LSource> sources;
        lock (_lEngineGate)
        {
            sources = LEngineInflectionLoad(language);
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

    public bool LEngineInflectionCheck(long entryId)
    {
        lock (_lEngineGate)
        {
            return _lEngineInflectionPending.ContainsKey(entryId);
        }
    }

    public void LEngineInflectionStart(long entryId)
    {
        LEntry? entry;
        CancellationTokenSource fetch;
        lock (_lEngineGate)
        {
            if (!_lEngineSettings.LSettingsMorphology
                || _lEngineInflectionPending.ContainsKey(entryId)
                || _lEngineInflectionLost.Contains(entryId)
                || LEngineDraftFind(entryId) is not null)
            {
                return;
            }

            entry = new LEntryArchive(_lEngineDatabase).LEntryRead(entryId);
            if (entry is null || LEngineInflectionLoad(entry.LEntryLanguage).Count == 0)
            {
                return;
            }

            bool wanted = false;
            foreach (LParadigmSlot slot in LEngineParadigmRead(entry))
            {
                wanted |= slot.LParadigmSlotState == LState.LStateUnspecified;
            }

            if (!wanted)
            {
                return;
            }

            fetch = new CancellationTokenSource();
            _lEngineInflectionPending[entryId] = fetch;
        }

        _ = LEngineInflectionRun(entry, fetch);
    }

    private void LEngineInflectionReset(long entryId)
    {
        if (_lEngineInflectionPending.Remove(entryId, out CancellationTokenSource? held))
        {
            held.Cancel();
            held.Dispose();
        }

        _lEngineInflectionMissed.Remove(entryId);
        _lEngineInflectionLost.Remove(entryId);
    }

    private void LEngineInflectionClear()
    {
        foreach (CancellationTokenSource held in _lEngineInflectionPending.Values)
        {
            held.Cancel();
            held.Dispose();
        }

        _lEngineInflectionPending.Clear();
        _lEngineInflectionMissed.Clear();
        _lEngineInflectionLost.Clear();
    }

    private LDraft? LEngineDraftFind(long entryId)
    {
        foreach (long held in _lEngineDraftHeld)
        {
            LDraft? draft = LDraftArchive.LDraftArchiveRead(_lEngineWorkspace, held);
            if (draft is not null && draft.LDraftEntryId == entryId)
            {
                return draft;
            }
        }

        return null;
    }

    private void LEngineInflectionApply(LEntry entry, IReadOnlyDictionary<string, string> found)
    {
        List<LInflection> appended = [];
        HashSet<long> missed = [];
        foreach (LParadigmSlot slot in LEngineParadigmRead(entry))
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
            new LInflectionArchive(_lEngineDatabase).LInflectionAppend(entry.LEntryId, appended);
        }

        if (missed.Count > 0)
        {
            _lEngineInflectionMissed[entry.LEntryId] = missed;
        }
        else
        {
            _lEngineInflectionMissed.Remove(entry.LEntryId);
        }
    }

    private async Task LEngineInflectionRun(LEntry entry, CancellationTokenSource fetch)
    {
        bool finished = false;
        try
        {
            (IReadOnlyDictionary<string, string> found, bool reached) = await LEngineInflectionScan(
                entry.LEntryHeadword, entry.LEntryLanguage, fetch.Token).ConfigureAwait(false);

            lock (_lEngineGate)
            {
                if (fetch.IsCancellationRequested || !_lEngineSettings.LSettingsMorphology)
                {
                    return;
                }

                LEntry? current = new LEntryArchive(_lEngineDatabase).LEntryRead(entry.LEntryId);
                if (current is null
                    || !string.Equals(current.LEntryHeadword, entry.LEntryHeadword, StringComparison.Ordinal)
                    || !string.Equals(current.LEntryLanguage, entry.LEntryLanguage, StringComparison.Ordinal)
                    || LEngineDraftFind(entry.LEntryId) is not null)
                {
                    return;
                }

                if (reached)
                {
                    LEngineInflectionApply(current, found);
                }
                else
                {
                    _lEngineInflectionLost.Add(entry.LEntryId);
                }

                finished = true;
            }
        }
        catch (Exception)
        {
        }
        finally
        {
            lock (_lEngineGate)
            {
                if (_lEngineInflectionPending.TryGetValue(entry.LEntryId, out CancellationTokenSource? held) && held == fetch)
                {
                    _lEngineInflectionPending.Remove(entry.LEntryId);
                    fetch.Dispose();
                }
            }
        }

        if (finished)
        {
            LEngineBulletinRaise(LSubject.LSubjectInflection, entry.LEntryId);
        }
    }
}
