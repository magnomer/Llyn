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

    private readonly SemaphoreSlim _lEngineFrequencyGate = new(4, 4);

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

    internal async Task<IReadOnlyList<LFrequency>> LEngineFrequencyFind(
        string word, string language, CancellationToken cancellation)
    {
        (IReadOnlyList<LFrequency> found, _) = await LEngineFrequencyScan(word, language, cancellation)
            .ConfigureAwait(false);
        return found;
    }

    private async Task<(IReadOnlyList<LFrequency> LFrequencyFound, bool LFrequencyReached)> LEngineFrequencyScan(
        string word, string language, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(word);

        IReadOnlyList<LSource> sources;
        lock (_lEngineGate)
        {
            sources = LEngineFrequencyLoad(language);
        }

        bool reached = false;
        List<LFrequency> rows = [];
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
                    rows.Add(LEngineFrequencyResolve(
                        language, new LFrequency(candidate.LCandidateSource, candidate.LCandidatePhonetic, null)));
                    break;
                }
            }
        }

        return (rows, reached || rows.Count > 0);
    }

    private LFrequency LEngineFrequencyResolve(string language, LFrequency row)
    {
        LSourceSpec? spec = LEngineSpecFind(language, row.LFrequencySource);
        if (spec is null)
        {
            return row;
        }

        bool numeric = double.TryParse(
            row.LFrequencyRaw, NumberStyles.Float, CultureInfo.InvariantCulture, out double figure);
        return row with
        {
            LFrequencyBand = LEngineBandResolve(spec, row.LFrequencyRaw),
            LFrequencyOnce = numeric ? LFrequency.LFrequencyOnceResolve(spec, figure) : null,
            LFrequencyUnit = numeric ? spec.LSourceSpecUnit : null,
        };
    }

    private LSourceSpec? LEngineSpecFind(string language, string source)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return null;
        }

        foreach (LSourceSpec spec in LEngineLanguageLoad(language).LLanguageFrequencies)
        {
            if (string.Equals(spec.LSourceSpecName, source, StringComparison.Ordinal))
            {
                return spec;
            }
        }

        return null;
    }

    internal string? LEngineBandResolve(string language, string source, string raw)
    {
        ArgumentNullException.ThrowIfNull(raw);

        LSourceSpec? spec = LEngineSpecFind(language, source);
        return spec is null ? null : LEngineBandResolve(spec, raw);
    }

    private static string? LEngineBandResolve(LSourceSpec spec, string raw)
    {
        if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out double figure)
            && LFrequency.LFrequencyBandResolve(spec, figure) is string graded)
        {
            return graded;
        }

        foreach (LBand band in spec.LSourceSpecBands)
        {
            if (LEngineBandMatch(raw, band.LBandPattern))
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

    public IReadOnlyList<LFrequency> LEngineFrequencyRead(long entryId)
    {
        LEntry? entry;
        IReadOnlyList<LFrequency> stored;
        bool missed;
        lock (_lEngineGate)
        {
            entry = new LEntryArchive(_lEngineDatabase).LEntryRead(entryId);
            stored = entry is null ? [] : new LFrequencyArchive(_lEngineDatabase).LFrequencyRead(entryId);
            missed = _lEngineFrequencyMissed.Contains(entryId);
        }

        if (entry is null)
        {
            return [];
        }

        if (stored.Count == 0)
        {
            if (!missed)
            {
                LEngineFrequencyStart(entryId);
            }

            return [];
        }

        List<LFrequency> rows = new(stored.Count);
        foreach (LFrequency row in stored)
        {
            LFrequency resolved = LEngineFrequencyResolve(entry.LEntryLanguage, row);
            if (!string.Equals(row.LFrequencyBand, resolved.LFrequencyBand, StringComparison.Ordinal))
            {
                lock (_lEngineGate)
                {
                    new LFrequencyArchive(_lEngineDatabase)
                        .LFrequencyBandSet(entryId, row.LFrequencySource, resolved.LFrequencyBand);
                }
            }

            rows.Add(resolved);
        }

        return rows;
    }

    internal void LEngineFrequencyStart(long entryId)
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
        bool admitted = false;
        try
        {
            await _lEngineFrequencyGate.WaitAsync(fetch.Token).ConfigureAwait(false);
            admitted = true;
            (IReadOnlyList<LFrequency> found, bool reached) = await LEngineFrequencyScan(
                entry.LEntryHeadword, entry.LEntryLanguage, fetch.Token).ConfigureAwait(false);

            lock (_lEngineGate)
            {
                if (fetch.IsCancellationRequested || !_lEngineSettings.LSettingsFrequency)
                {
                    return;
                }

                if (found.Count == 0)
                {
                    if (reached)
                    {
                        _lEngineFrequencyMissed.Add(entry.LEntryId);
                    }

                    return;
                }

                LEntry? current = new LEntryArchive(_lEngineDatabase).LEntryRead(entry.LEntryId);
                if (current is null
                    || !string.Equals(current.LEntryHeadword, entry.LEntryHeadword, StringComparison.Ordinal)
                    || !string.Equals(current.LEntryLanguage, entry.LEntryLanguage, StringComparison.Ordinal))
                {
                    return;
                }

                new LFrequencyArchive(_lEngineDatabase).LFrequencySet(entry.LEntryId, found);
                raised = true;
            }
        }
        catch (Exception)
        {
        }
        finally
        {
            if (admitted)
            {
                _lEngineFrequencyGate.Release();
            }

            lock (_lEngineGate)
            {
                if (_lEngineFrequencyPending.TryGetValue(entry.LEntryId, out CancellationTokenSource? held)
                    && held == fetch)
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
