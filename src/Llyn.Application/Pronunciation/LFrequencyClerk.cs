using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LFrequencyClerk
{
    private static readonly TimeSpan LFrequencyClerkPatience = TimeSpan.FromSeconds(1);

    private readonly LEntryVault _lFrequencyClerkEntries;
    private readonly LFrequencyVault _lFrequencyClerkFrequencies;
    private readonly LSourceFactory _lFrequencyClerkFactory;
    private readonly LLanguageCache _lFrequencyClerkLanguages;
    private readonly object _lFrequencyClerkGate;
    private readonly Func<LSettings> _lFrequencyClerkSettings;
    private readonly Action<LSubject, long> _lFrequencyClerkBulletin;
    private readonly SemaphoreSlim _lFrequencyClerkAdmission = new(4, 4);
    private readonly Dictionary<string, IReadOnlyList<LSource>> _lFrequencyClerkSources = new(StringComparer.Ordinal);
    private readonly Dictionary<long, CancellationTokenSource> _lFrequencyClerkPending = [];
    private readonly HashSet<long> _lFrequencyClerkMissed = [];

    public LFrequencyClerk(
        LRig rig,
        LLanguageCache languages,
        object gate,
        Func<LSettings> settings,
        Action<LSubject, long> raise)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(gate);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(raise);
        _lFrequencyClerkEntries = rig.LRigEntries;
        _lFrequencyClerkFrequencies = rig.LRigFrequencies;
        _lFrequencyClerkFactory = rig.LRigSources;
        _lFrequencyClerkLanguages = languages;
        _lFrequencyClerkGate = gate;
        _lFrequencyClerkSettings = settings;
        _lFrequencyClerkBulletin = raise;
    }

    public async Task<IReadOnlyList<LFrequency>> LFrequencyClerkFind(
        string word, string language, CancellationToken cancellation)
    {
        (IReadOnlyList<LFrequency> found, _) = await LFrequencyClerkScan(word, language, cancellation)
            .ConfigureAwait(false);
        return found;
    }

    public string? LBandResolve(string language, string source, string raw)
    {
        ArgumentNullException.ThrowIfNull(raw);

        LSourceSpec? spec = LSourceSpecFind(language, source);
        return spec is null ? null : LBandResolve(spec, raw);
    }

    public IReadOnlyList<LFrequency> LFrequencyClerkRead(long entryId)
    {
        LEntry? entry;
        IReadOnlyList<LFrequency> stored;
        bool missed;
        lock (_lFrequencyClerkGate)
        {
            entry = _lFrequencyClerkEntries.LEntryRead(entryId);
            stored = entry is null ? [] : _lFrequencyClerkFrequencies.LFrequencyRead(entryId);
            missed = _lFrequencyClerkMissed.Contains(entryId);
        }

        if (entry is null)
        {
            return [];
        }

        if (stored.Count == 0)
        {
            if (!missed)
            {
                LFrequencyClerkStart(entryId);
            }

            return [];
        }

        List<LFrequency> rows = new(stored.Count);
        foreach (LFrequency row in stored)
        {
            LFrequency resolved = LFrequencyResolve(entry.LEntryLanguage, row);
            if (!string.Equals(row.LFrequencyBand, resolved.LFrequencyBand, StringComparison.Ordinal))
            {
                lock (_lFrequencyClerkGate)
                {
                    _lFrequencyClerkFrequencies
                        .LFrequencyBandSet(entryId, row.LFrequencySource, resolved.LFrequencyBand);
                }
            }

            rows.Add(resolved);
        }

        return rows;
    }

    public void LFrequencyClerkStart(long entryId)
    {
        LEntry? entry;
        CancellationTokenSource fetch;
        lock (_lFrequencyClerkGate)
        {
            _lFrequencyClerkMissed.Remove(entryId);
            LFrequencyClerkCancel(entryId);
            if (!_lFrequencyClerkSettings().LSettingsFrequency)
            {
                return;
            }

            entry = _lFrequencyClerkEntries.LEntryRead(entryId);
            if (entry is null || LFrequencySourceRead(entry.LEntryLanguage).Count == 0)
            {
                return;
            }

            fetch = new CancellationTokenSource();
            _lFrequencyClerkPending[entryId] = fetch;
        }

        _ = LFrequencyClerkRun(entry, fetch);
    }

    public void LFrequencyClerkClear()
    {
        lock (_lFrequencyClerkGate)
        {
            foreach (CancellationTokenSource held in _lFrequencyClerkPending.Values)
            {
                held.Cancel();
                held.Dispose();
            }

            _lFrequencyClerkPending.Clear();
            _lFrequencyClerkMissed.Clear();
        }
    }

    private void LFrequencyClerkCancel(long entryId)
    {
        if (_lFrequencyClerkPending.Remove(entryId, out CancellationTokenSource? held))
        {
            held.Cancel();
            held.Dispose();
        }
    }

    private IReadOnlyList<LSource> LFrequencySourceRead(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return [];
        }

        if (_lFrequencyClerkSources.TryGetValue(language, out IReadOnlyList<LSource>? sources))
        {
            return sources;
        }

        LLanguage pack = _lFrequencyClerkLanguages.LLanguageCacheRead(language);
        sources = _lFrequencyClerkFactory.LSourceFactoryCreate(pack.LLanguageFrequencies);
        _lFrequencyClerkSources[language] = sources;
        return sources;
    }

    private async Task<(IReadOnlyList<LFrequency> LFrequencyFound, bool LFrequencyReached)> LFrequencyClerkScan(
        string word, string language, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(word);

        IReadOnlyList<LSource> sources;
        lock (_lFrequencyClerkGate)
        {
            sources = LFrequencySourceRead(language);
        }

        bool reached = false;
        List<LFrequency> rows = [];
        LReceiverRelay receiver = new(static _ => { });
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
                    rows.Add(LFrequencyResolve(
                        language, new LFrequency(candidate.LCandidateSource, candidate.LCandidatePhonetic, null)));
                    break;
                }
            }
        }

        return (rows, reached || rows.Count > 0);
    }

    private LFrequency LFrequencyResolve(string language, LFrequency row)
    {
        LSourceSpec? spec = LSourceSpecFind(language, row.LFrequencySource);
        if (spec is null)
        {
            return row;
        }

        bool numeric = double.TryParse(
            row.LFrequencyRaw, NumberStyles.Float, CultureInfo.InvariantCulture, out double figure);
        return row with
        {
            LFrequencyBand = LBandResolve(spec, row.LFrequencyRaw),
            LFrequencyOnce = numeric ? LFrequency.LFrequencyOnceResolve(spec, figure) : null,
            LFrequencyUnit = numeric ? spec.LSourceSpecUnit : null,
        };
    }

    private LSourceSpec? LSourceSpecFind(string language, string source)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return null;
        }

        foreach (LSourceSpec spec in _lFrequencyClerkLanguages.LLanguageCacheRead(language).LLanguageFrequencies)
        {
            if (string.Equals(spec.LSourceSpecName, source, StringComparison.Ordinal))
            {
                return spec;
            }
        }

        return null;
    }

    private static string? LBandResolve(LSourceSpec spec, string raw)
    {
        if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out double figure)
            && LFrequency.LFrequencyBandResolve(spec, figure) is string graded)
        {
            return graded;
        }

        foreach (LBand band in spec.LSourceSpecBands)
        {
            if (LBandMatch(raw, band.LBandPattern))
            {
                return band.LBandName;
            }
        }

        return null;
    }

    private static bool LBandMatch(string raw, string pattern)
    {
        try
        {
            return Regex.IsMatch(raw, pattern, RegexOptions.CultureInvariant, LFrequencyClerkPatience);
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    private async Task LFrequencyClerkRun(LEntry entry, CancellationTokenSource fetch)
    {
        bool raised = false;
        bool admitted = false;
        try
        {
            await _lFrequencyClerkAdmission.WaitAsync(fetch.Token).ConfigureAwait(false);
            admitted = true;
            (IReadOnlyList<LFrequency> found, bool reached) = await LFrequencyClerkScan(
                entry.LEntryHeadword, entry.LEntryLanguage, fetch.Token).ConfigureAwait(false);

            lock (_lFrequencyClerkGate)
            {
                if (fetch.IsCancellationRequested || !_lFrequencyClerkSettings().LSettingsFrequency)
                {
                    return;
                }

                if (found.Count == 0)
                {
                    if (reached)
                    {
                        _lFrequencyClerkMissed.Add(entry.LEntryId);
                    }

                    return;
                }

                LEntry? current = _lFrequencyClerkEntries.LEntryRead(entry.LEntryId);
                if (current is null
                    || !string.Equals(current.LEntryHeadword, entry.LEntryHeadword, StringComparison.Ordinal)
                    || !string.Equals(current.LEntryLanguage, entry.LEntryLanguage, StringComparison.Ordinal))
                {
                    return;
                }

                _lFrequencyClerkFrequencies.LFrequencySet(entry.LEntryId, found);
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
                _lFrequencyClerkAdmission.Release();
            }

            lock (_lFrequencyClerkGate)
            {
                if (_lFrequencyClerkPending.TryGetValue(entry.LEntryId, out CancellationTokenSource? held)
                    && held == fetch)
                {
                    _lFrequencyClerkPending.Remove(entry.LEntryId);
                    fetch.Dispose();
                }
            }
        }

        if (raised)
        {
            _lFrequencyClerkBulletin(LSubject.LSubjectFrequency, entry.LEntryId);
        }
    }
}
