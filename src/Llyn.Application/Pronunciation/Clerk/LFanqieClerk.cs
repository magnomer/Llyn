using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LFanqieClerk
{
    private const string LFanqieClerkTone = "Display.FanqieTone";

    private readonly LEntryVault _lFanqieClerkEntries;
    private readonly LFanqieVault _lFanqieClerkFanqie;
    private readonly LShengfuVault _lFanqieClerkShengfu;
    private readonly LDiweiVault _lFanqieClerkDiwei;
    private readonly LFanqieSource _lFanqieClerkSource;
    private readonly LLanguageVault _lFanqieClerkPacks;
    private readonly LClock _lFanqieClerkClock;
    private readonly LLanguageCache _lFanqieClerkLanguages;
    private readonly object _lFanqieClerkGate;
    private readonly Action<LSubject, long> _lFanqieClerkBulletin;
    private readonly SemaphoreSlim _lFanqieClerkAdmission = new(1, 1);
    private readonly Dictionary<string, CancellationTokenSource> _lFanqieClerkPending = new(StringComparer.Ordinal);
    private readonly HashSet<string> _lFanqieClerkMissed = new(StringComparer.Ordinal);
    private DateTimeOffset _lFanqieClerkStamp = DateTimeOffset.MinValue;

    public LFanqieClerk(LRig rig, LLanguageCache languages, object gate, Action<LSubject, long> raise)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(gate);
        ArgumentNullException.ThrowIfNull(raise);
        _lFanqieClerkEntries = rig.LRigEntries;
        _lFanqieClerkFanqie = rig.LRigFanqie;
        _lFanqieClerkShengfu = rig.LRigShengfu;
        _lFanqieClerkDiwei = rig.LRigDiwei;
        _lFanqieClerkSource = rig.LRigFanqieSource;
        _lFanqieClerkPacks = rig.LRigLanguages;
        _lFanqieClerkClock = rig.LRigClock;
        _lFanqieClerkLanguages = languages;
        _lFanqieClerkGate = gate;
        _lFanqieClerkBulletin = raise;
    }

    public IReadOnlyList<LFanqieBook> LFanqieBookRead(string language)
    {
        return string.IsNullOrWhiteSpace(language)
            ? []
            : _lFanqieClerkLanguages.LLanguageCacheRead(language).LLanguageFanqieBooks;
    }

    public LHypothesis? LHypothesisRead(string language)
    {
        return string.IsNullOrWhiteSpace(language)
            ? null
            : _lFanqieClerkLanguages.LLanguageCacheRead(language).LLanguageHypothesis;
    }

    public IReadOnlyList<LFanqieRow> LFanqieClerkRead(long entryId)
    {
        LEntry? entry;
        lock (_lFanqieClerkGate)
        {
            entry = _lFanqieClerkEntries.LEntryRead(entryId);
        }

        if (entry is null || LFanqieBookRead(entry.LEntryLanguage).Count == 0)
        {
            return [];
        }

        string pattern = LLocalization.LLocalizationTextFind(LFanqieClerkTone) ?? string.Empty;
        List<LFanqieRow> rows = [];
        foreach (string character in LGlyph.LGlyphScan(entry.LEntryHeadword))
        {
            IReadOnlyList<LFanqieRow> stored;
            lock (_lFanqieClerkGate)
            {
                stored = _lFanqieClerkFanqie.LFanqieRead(entry.LEntryLanguage, character);
            }

            foreach (LFanqieRow row in stored)
            {
                rows.Add(row.LFanqieRowFormat(pattern));
            }
        }

        return rows;
    }

    public IReadOnlyList<LFanqieGroup> LFanqieClerkDivide(long entryId)
    {
        IReadOnlyList<LFanqieRow> rows = LFanqieClerkRead(entryId);
        if (rows.Count == 0)
        {
            return [];
        }

        LEntry? entry;
        lock (_lFanqieClerkGate)
        {
            entry = _lFanqieClerkEntries.LEntryRead(entryId);
        }

        string language = entry?.LEntryLanguage ?? string.Empty;
        IReadOnlyList<LShengfu> shengfu = LShengfuStoredScan(language, LFanqieRow.LFanqieCharacterScan(rows));
        return LFanqieGroup.LFanqieGroupScan(
            rows, LFanqieBookRead(language), shengfu, LShengfuSeparatorRead(language));
    }

    public string LFanqieClerkFormat(long entryId, string headword)
    {
        return LFanqieGroup.LFanqieReadingFormat(LFanqieClerkDivide(entryId), headword);
    }

    private string LShengfuSeparatorRead(string language)
    {
        return string.IsNullOrWhiteSpace(language)
            ? string.Empty
            : _lFanqieClerkLanguages.LLanguageCacheRead(language).LLanguageShengfu?.LShengfuRuleSeparator
                ?? string.Empty;
    }

    private IReadOnlyList<LShengfu> LShengfuStoredScan(string language, IReadOnlyList<string> characters)
    {
        if (language.Length == 0)
        {
            return [];
        }

        lock (_lFanqieClerkGate)
        {
            return _lFanqieClerkShengfu.LShengfuScan(language, characters);
        }
    }

    public void LFanqieClerkStart(long entryId)
    {
        LEntry? entry;
        lock (_lFanqieClerkGate)
        {
            entry = _lFanqieClerkEntries.LEntryRead(entryId);
        }

        if (entry is null || LFanqieBookRead(entry.LEntryLanguage).Count == 0)
        {
            return;
        }

        foreach (string character in LGlyph.LGlyphScan(entry.LEntryHeadword))
        {
            bool stored;
            lock (_lFanqieClerkGate)
            {
                stored = _lFanqieClerkFanqie.LFanqieRead(entry.LEntryLanguage, character).Count > 0;
            }

            if (!stored)
            {
                LFanqieClerkStart(entryId, entry.LEntryLanguage, character);
            }
        }
    }

    public void LFanqieClerkRebuild(long entryId)
    {
        LEntry? entry;
        lock (_lFanqieClerkGate)
        {
            entry = _lFanqieClerkEntries.LEntryRead(entryId);
        }

        if (entry is null || LFanqieBookRead(entry.LEntryLanguage).Count == 0)
        {
            return;
        }

        foreach (string character in LGlyph.LGlyphScan(entry.LEntryHeadword))
        {
            lock (_lFanqieClerkGate)
            {
                _lFanqieClerkMissed.Remove(LFanqieKeyFormat(entry.LEntryLanguage, character));
            }

            LFanqieClerkStart(entryId, entry.LEntryLanguage, character);
        }
    }

    public void LFanqieClerkSet(long entryId, long fanqieId, int rank)
    {
        lock (_lFanqieClerkGate)
        {
            _lFanqieClerkFanqie.LFanqieRepresentativeSet(fanqieId, rank);
        }

        _lFanqieClerkBulletin(LSubject.LSubjectFanqie, entryId);
    }

    public bool LFanqieClerkCheck(long entryId)
    {
        LEntry? entry;
        lock (_lFanqieClerkGate)
        {
            entry = _lFanqieClerkEntries.LEntryRead(entryId);
            if (entry is null)
            {
                return false;
            }

            foreach (string character in LGlyph.LGlyphScan(entry.LEntryHeadword))
            {
                if (_lFanqieClerkPending.ContainsKey(LFanqieKeyFormat(entry.LEntryLanguage, character)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void LFanqieClerkClear()
    {
        lock (_lFanqieClerkGate)
        {
            foreach (CancellationTokenSource held in _lFanqieClerkPending.Values)
            {
                held.Cancel();
                held.Dispose();
            }

            _lFanqieClerkPending.Clear();
            _lFanqieClerkMissed.Clear();
        }
    }

    public void LDiweiApply()
    {
        foreach (string language in _lFanqieClerkPacks.LLanguageScan())
        {
            if (LFanqieBookRead(language).Count > 0)
            {
                _lFanqieClerkDiwei.LDiweiRebuild(language, LHypothesisRead(language));
            }
        }
    }

    private async Task<(IReadOnlyList<LFanqieRow> LFanqieFound, bool LFanqieReached)> LFanqieClerkScan(
        string character, string language, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(character);

        IReadOnlyList<LFanqieBook> books;
        lock (_lFanqieClerkGate)
        {
            books = LFanqieBookRead(language);
        }

        bool reached = false;
        List<LFanqieRow> rows = [];
        foreach (LFanqieBook book in books)
        {
            TimeSpan left = _lFanqieClerkStamp - _lFanqieClerkClock.LClockRead();
            if (left > TimeSpan.Zero)
            {
                await Task.Delay(left, cancellation).ConfigureAwait(false);
            }

            (IReadOnlyList<LFanqieRow> found, bool answered) = await _lFanqieClerkSource
                .LFanqieSourceFind(book, character, cancellation)
                .ConfigureAwait(false);
            DateTimeOffset next = _lFanqieClerkClock.LClockRead() + TimeSpan.FromSeconds(book.LFanqieBookInterval);
            if (next > _lFanqieClerkStamp)
            {
                _lFanqieClerkStamp = next;
            }

            reached |= answered;
            rows.AddRange(found);
        }

        return (rows, reached);
    }

    private static string LFanqieKeyFormat(string language, string character)
    {
        return language + '\n' + character;
    }

    private void LFanqieClerkStart(long entryId, string language, string character)
    {
        string key = LFanqieKeyFormat(language, character);
        CancellationTokenSource fetch;
        lock (_lFanqieClerkGate)
        {
            if (_lFanqieClerkMissed.Contains(key) || _lFanqieClerkPending.ContainsKey(key))
            {
                return;
            }

            fetch = new CancellationTokenSource();
            _lFanqieClerkPending[key] = fetch;
        }

        _ = LFanqieClerkRun(entryId, language, character, key, fetch);
    }

    private async Task LFanqieClerkRun(
        long entryId, string language, string character, string key, CancellationTokenSource fetch)
    {
        bool raised = false;
        bool admitted = false;
        try
        {
            await _lFanqieClerkAdmission.WaitAsync(fetch.Token).ConfigureAwait(false);
            admitted = true;
            (IReadOnlyList<LFanqieRow> found, bool reached) =
                await LFanqieClerkScan(character, language, fetch.Token).ConfigureAwait(false);

            lock (_lFanqieClerkGate)
            {
                if (fetch.IsCancellationRequested)
                {
                    return;
                }

                raised = true;
                if (found.Count > 0)
                {
                    _lFanqieClerkFanqie.LFanqieSave(language, character, found);
                    _lFanqieClerkDiwei.LDiweiApply(language, character, LHypothesisRead(language));
                }
                else if (reached)
                {
                    _lFanqieClerkMissed.Add(key);
                }
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
                _lFanqieClerkAdmission.Release();
            }

            lock (_lFanqieClerkGate)
            {
                if (_lFanqieClerkPending.TryGetValue(key, out CancellationTokenSource? held) && held == fetch)
                {
                    _lFanqieClerkPending.Remove(key);
                    fetch.Dispose();
                }
            }
        }

        if (raised)
        {
            _lFanqieClerkBulletin(LSubject.LSubjectFanqie, entryId);
        }
    }
}
