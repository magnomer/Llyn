using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private static readonly TimeSpan LEngineBandPatience = TimeSpan.FromSeconds(1);

    private IReadOnlyList<LSource> LEngineFrequencyLoad(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return [];
        }

        if (_lEngineFrequencySources.TryGetValue(language, out IReadOnlyList<LSource>? sources))
        {
            return sources;
        }

        LLanguage pack = LEngineLanguageLoad(language);
        sources = LSourceFactory.LSourceFactoryCreate(pack.LLanguageFrequencies, _lEngineClient);
        _lEngineFrequencySources[language] = sources;
        return sources;
    }

    public async Task<LFrequency?> LEngineFrequencyFind(string word, string language, CancellationToken cancellation)
    {
        (LFrequency? found, _) = await LEngineFrequencyScan(word, language, cancellation).ConfigureAwait(false);
        return found;
    }

    private async Task<(LFrequency? LFrequencyFound, bool LFrequencyReached)> LEngineFrequencyScan(
        string word, string language, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(word);

        IReadOnlyList<LSource> sources;
        lock (_lEngineGate)
        {
            sources = LEngineFrequencyLoad(language);
        }

        bool reached = false;
        LReceiverFrequency receiver = new();
        foreach (LSource source in sources)
        {
            IReadOnlyList<LCandidate> found =
                await new LLookup([source], [], [], true)
                    .LSeekerStart(word, receiver, cancellation)
                    .ConfigureAwait(false);

            foreach (LCandidate candidate in found)
            {
                reached |= candidate.LCandidateReached;
                if (!string.IsNullOrEmpty(candidate.LCandidatePhonetic))
                {
                    string raw = candidate.LCandidatePhonetic;
                    return (new LFrequency(candidate.LCandidateSource, raw, LEngineBandResolve(language, raw)), true);
                }
            }
        }

        return (null, reached);
    }

    internal string? LEngineBandResolve(string language, string raw)
    {
        ArgumentNullException.ThrowIfNull(raw);

        if (string.IsNullOrWhiteSpace(language))
        {
            return null;
        }

        bool numeric = double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out double figure);
        foreach (LBand band in LEngineLanguageLoad(language).LLanguageBands)
        {
            if (band.LBandLimit is double limit && numeric && figure <= limit)
            {
                return band.LBandName;
            }

            if (band.LBandPattern is string pattern && LEngineBandMatch(raw, pattern))
            {
                return band.LBandName;
            }
        }

        return null;
    }

    private static bool LEngineBandMatch(string raw, string pattern)
    {
        try
        {
            return Regex.IsMatch(raw, pattern, RegexOptions.CultureInvariant, LEngineBandPatience);
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    public LFrequency? LEngineFrequencyRead(long entryId)
    {
        LEntry? entry;
        bool missed;
        lock (_lEngineGate)
        {
            entry = new LEntryArchive(_lEngineDatabase).LEntryRead(entryId);
            missed = _lEngineFrequencyMissed.Contains(entryId);
        }

        if (entry is null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(entry.LEntryFrequency))
        {
            if (!missed)
            {
                LEngineFrequencyStart(entryId);
            }

            return null;
        }

        LFrequency parsed = LFrequency.LFrequencyParse(entry.LEntryFrequency);
        return parsed with { LFrequencyBand = LEngineBandResolve(entry.LEntryLanguage, parsed.LFrequencyRaw) };
    }

    public void LEngineFrequencyStart(long entryId)
    {
        LEntry? entry;
        CancellationTokenSource fetch;
        lock (_lEngineGate)
        {
            _lEngineFrequencyMissed.Remove(entryId);
            LEngineFrequencyCancel(entryId);
            if (!_lEngineSettings.LSettingsFrequency)
            {
                return;
            }

            entry = new LEntryArchive(_lEngineDatabase).LEntryRead(entryId);
            if (entry is null || LEngineFrequencyLoad(entry.LEntryLanguage).Count == 0)
            {
                return;
            }

            fetch = new CancellationTokenSource();
            _lEngineFrequencyPending[entryId] = fetch;
        }

        _ = LEngineFrequencyRun(entry, fetch);
    }

    private void LEngineFrequencyCancel(long entryId)
    {
        if (_lEngineFrequencyPending.Remove(entryId, out CancellationTokenSource? held))
        {
            held.Cancel();
            held.Dispose();
        }
    }

    private void LEngineFrequencyClear()
    {
        foreach (CancellationTokenSource held in _lEngineFrequencyPending.Values)
        {
            held.Cancel();
            held.Dispose();
        }

        _lEngineFrequencyPending.Clear();
        _lEngineFrequencyMissed.Clear();
    }

    private async Task LEngineFrequencyRun(LEntry entry, CancellationTokenSource fetch)
    {
        bool raised = false;
        try
        {
            (LFrequency? found, bool reached) = await LEngineFrequencyScan(
                entry.LEntryHeadword, entry.LEntryLanguage, fetch.Token).ConfigureAwait(false);

            lock (_lEngineGate)
            {
                if (fetch.IsCancellationRequested || !_lEngineSettings.LSettingsFrequency)
                {
                    return;
                }

                if (found is null)
                {
                    if (reached)
                    {
                        _lEngineFrequencyMissed.Add(entry.LEntryId);
                    }

                    return;
                }

                LEntryArchive entries = new(_lEngineDatabase);
                LEntry? current = entries.LEntryRead(entry.LEntryId);
                if (current is null
                    || !string.Equals(current.LEntryHeadword, entry.LEntryHeadword, StringComparison.Ordinal)
                    || !string.Equals(current.LEntryLanguage, entry.LEntryLanguage, StringComparison.Ordinal))
                {
                    return;
                }

                entries.LEntryFrequencySet(entry.LEntryId, found.LFrequencyFormat());
                raised = true;
            }
        }
        catch (Exception)
        {
        }
        finally
        {
            lock (_lEngineGate)
            {
                if (_lEngineFrequencyPending.TryGetValue(entry.LEntryId, out CancellationTokenSource? held) && held == fetch)
                {
                    _lEngineFrequencyPending.Remove(entry.LEntryId);
                    fetch.Dispose();
                }
            }
        }

        if (raised)
        {
            LEngineBulletinRaise(LSubject.LSubjectFrequency, entry.LEntryId);
        }
    }

    private sealed class LReceiverFrequency : LReceiver
    {
        public void LReceiverSourceStart(string source, int order)
        {
        }

        public void LReceiverCandidateAdd(LCandidate candidate)
        {
        }

        public void LReceiverLookupFinish()
        {
        }
    }
}
