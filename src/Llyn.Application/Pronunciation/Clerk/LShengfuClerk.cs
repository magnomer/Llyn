using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LShengfuClerk
{
    private readonly LEntryVault _lShengfuClerkEntries;
    private readonly LShengfuVault _lShengfuClerkShengfu;
    private readonly LStemVault _lShengfuClerkStems;
    private readonly LLanguageVault _lShengfuClerkPacks;
    private readonly LShengfuSource _lShengfuClerkSource;
    private readonly LClock _lShengfuClerkClock;
    private readonly LLanguageCache _lShengfuClerkLanguages;
    private readonly object _lShengfuClerkGate;
    private readonly Action<LSubject, long> _lShengfuClerkBulletin;
    private readonly SemaphoreSlim _lShengfuClerkAdmission = new(1, 1);
    private readonly Dictionary<string, CancellationTokenSource> _lShengfuClerkPending = new(StringComparer.Ordinal);
    private readonly HashSet<string> _lShengfuClerkMissed = new(StringComparer.Ordinal);
    private DateTimeOffset _lShengfuClerkStamp = DateTimeOffset.MinValue;

    public LShengfuClerk(LRig rig, LLanguageCache languages, object gate, Action<LSubject, long> raise)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(gate);
        ArgumentNullException.ThrowIfNull(raise);
        _lShengfuClerkEntries = rig.LRigEntries;
        _lShengfuClerkShengfu = rig.LRigShengfu;
        _lShengfuClerkStems = rig.LRigStems;
        _lShengfuClerkPacks = rig.LRigLanguages;
        _lShengfuClerkSource = rig.LRigShengfuSource;
        _lShengfuClerkClock = rig.LRigClock;
        _lShengfuClerkLanguages = languages;
        _lShengfuClerkGate = gate;
        _lShengfuClerkBulletin = raise;
    }

    public LShengfuRule? LShengfuRuleRead(string language)
    {
        return string.IsNullOrWhiteSpace(language)
            ? null
            : _lShengfuClerkLanguages.LLanguageCacheRead(language).LLanguageShengfu;
    }

    public IReadOnlyList<LShengfu> LShengfuClerkRead(long entryId)
    {
        LEntry? entry;
        lock (_lShengfuClerkGate)
        {
            entry = _lShengfuClerkEntries.LEntryRead(entryId);
        }

        if (entry is null || LShengfuRuleRead(entry.LEntryLanguage) is null)
        {
            return [];
        }

        lock (_lShengfuClerkGate)
        {
            return _lShengfuClerkShengfu.LShengfuScan(
                entry.LEntryLanguage, LGlyph.LGlyphScan(entry.LEntryHeadword));
        }
    }

    public void LShengfuClerkStart(long entryId)
    {
        LEntry? entry;
        lock (_lShengfuClerkGate)
        {
            entry = _lShengfuClerkEntries.LEntryRead(entryId);
        }

        if (entry is null || LShengfuRuleRead(entry.LEntryLanguage) is null)
        {
            return;
        }

        foreach (string character in LGlyph.LGlyphScan(entry.LEntryHeadword))
        {
            bool stored;
            lock (_lShengfuClerkGate)
            {
                stored = _lShengfuClerkShengfu.LShengfuRead(entry.LEntryLanguage, character) is not null;
            }

            if (!stored)
            {
                LShengfuClerkStart(entryId, entry.LEntryLanguage, character);
            }
        }
    }

    public void LShengfuClerkRebuild(long entryId)
    {
        LEntry? entry;
        lock (_lShengfuClerkGate)
        {
            entry = _lShengfuClerkEntries.LEntryRead(entryId);
        }

        if (entry is null || LShengfuRuleRead(entry.LEntryLanguage) is null)
        {
            return;
        }

        foreach (string character in LGlyph.LGlyphScan(entry.LEntryHeadword))
        {
            lock (_lShengfuClerkGate)
            {
                _lShengfuClerkMissed.Remove(LShengfuKeyFormat(entry.LEntryLanguage, character));
            }

            LShengfuClerkStart(entryId, entry.LEntryLanguage, character);
        }
    }

    public bool LShengfuClerkCheck(long entryId)
    {
        lock (_lShengfuClerkGate)
        {
            LEntry? entry = _lShengfuClerkEntries.LEntryRead(entryId);
            if (entry is null)
            {
                return false;
            }

            foreach (string character in LGlyph.LGlyphScan(entry.LEntryHeadword))
            {
                if (_lShengfuClerkPending.ContainsKey(LShengfuKeyFormat(entry.LEntryLanguage, character)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void LStemApply()
    {
        foreach (string language in _lShengfuClerkPacks.LLanguageScan())
        {
            if (LShengfuRuleRead(language) is LShengfuRule rule)
            {
                _lShengfuClerkStems.LStemRebuild(language, rule.LShengfuRuleSeparator);
            }
        }
    }

    public void LShengfuClerkClear()
    {
        lock (_lShengfuClerkGate)
        {
            foreach (CancellationTokenSource held in _lShengfuClerkPending.Values)
            {
                held.Cancel();
                held.Dispose();
            }

            _lShengfuClerkPending.Clear();
            _lShengfuClerkMissed.Clear();
        }
    }

    private string LShengfuSeparatorRead(string language)
    {
        return LShengfuRuleRead(language)?.LShengfuRuleSeparator ?? string.Empty;
    }

    private static string LShengfuKeyFormat(string language, string character)
    {
        return language + '\n' + character;
    }

    private void LShengfuClerkStart(long entryId, string language, string character)
    {
        string key = LShengfuKeyFormat(language, character);
        CancellationTokenSource fetch;
        lock (_lShengfuClerkGate)
        {
            if (_lShengfuClerkMissed.Contains(key) || _lShengfuClerkPending.ContainsKey(key))
            {
                return;
            }

            fetch = new CancellationTokenSource();
            _lShengfuClerkPending[key] = fetch;
        }

        _ = LShengfuClerkRun(entryId, language, character, key, fetch);
    }

    private async Task<(LShengfu? LShengfuFound, bool LShengfuReached)> LShengfuClerkScan(
        string character, string language, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(character);

        LShengfuRule? rule;
        lock (_lShengfuClerkGate)
        {
            rule = LShengfuRuleRead(language);
        }

        if (rule is null)
        {
            return (null, false);
        }

        TimeSpan left = _lShengfuClerkStamp - _lShengfuClerkClock.LClockRead();
        if (left > TimeSpan.Zero)
        {
            await Task.Delay(left, cancellation).ConfigureAwait(false);
        }

        (LShengfu? found, bool answered) = await _lShengfuClerkSource
            .LShengfuSourceFind(rule, character, cancellation)
            .ConfigureAwait(false);
        DateTimeOffset next = _lShengfuClerkClock.LClockRead() + TimeSpan.FromSeconds(rule.LShengfuRuleInterval);
        if (next > _lShengfuClerkStamp)
        {
            _lShengfuClerkStamp = next;
        }

        return (found, answered);
    }

    private async Task LShengfuClerkRun(
        long entryId, string language, string character, string key, CancellationTokenSource fetch)
    {
        bool raised = false;
        bool admitted = false;
        try
        {
            await _lShengfuClerkAdmission.WaitAsync(fetch.Token).ConfigureAwait(false);
            admitted = true;
            (LShengfu? found, bool reached) =
                await LShengfuClerkScan(character, language, fetch.Token).ConfigureAwait(false);

            lock (_lShengfuClerkGate)
            {
                if (fetch.IsCancellationRequested)
                {
                    return;
                }

                raised = true;
                if (found is not null)
                {
                    _lShengfuClerkShengfu.LShengfuSave(language, found);
                    _lShengfuClerkStems.LStemApply(
                        language,
                        found.LShengfuCharacter,
                        LStem.LStemKeyScan(found.LShengfuText, LShengfuSeparatorRead(language)));
                }
                else if (reached)
                {
                    _lShengfuClerkMissed.Add(key);
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
                _lShengfuClerkAdmission.Release();
            }

            lock (_lShengfuClerkGate)
            {
                if (_lShengfuClerkPending.TryGetValue(key, out CancellationTokenSource? held) && held == fetch)
                {
                    _lShengfuClerkPending.Remove(key);
                    fetch.Dispose();
                }
            }
        }

        if (raised)
        {
            _lShengfuClerkBulletin(LSubject.LSubjectFanqie, entryId);
        }
    }
}
