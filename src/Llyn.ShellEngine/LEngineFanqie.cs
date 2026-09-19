using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private const string LEngineFanqieTone = "Display.FanqieTone";

    private readonly SemaphoreSlim _lEngineFanqieGate = new(1, 1);
    private readonly Dictionary<string, CancellationTokenSource> _lEngineFanqiePending = new(StringComparer.Ordinal);
    private readonly HashSet<string> _lEngineFanqieMissed = new(StringComparer.Ordinal);
    private DateTime _lEngineFanqieStamp = DateTime.MinValue;

    public IReadOnlyList<LFanqieBook> LEngineBookRead(string language)
    {
        return string.IsNullOrWhiteSpace(language) ? [] : LEngineLanguageLoad(language).LLanguageFanqieBooks;
    }

    public bool LEngineBookCheck(string language)
    {
        return LEngineBookRead(language).Count > 0;
    }

    public LHypothesis? LEngineHypothesisRead(string language)
    {
        return string.IsNullOrWhiteSpace(language) ? null : LEngineLanguageLoad(language).LLanguageHypothesis;
    }

    public IReadOnlyList<LFanqieRow> LEngineFanqieRead(long entryId)
    {
        LEntry? entry;
        lock (_lEngineGate)
        {
            entry = new LEntryArchive(_lEngineDatabase).LEntryRead(entryId);
        }

        if (entry is null || LEngineBookRead(entry.LEntryLanguage).Count == 0)
        {
            return [];
        }

        string pattern = LLocalization.LLocalizationTextFind(LEngineFanqieTone) ?? string.Empty;
        List<LFanqieRow> rows = [];
        foreach (string character in LGlyph.LGlyphScan(entry.LEntryHeadword))
        {
            IReadOnlyList<LFanqieRow> stored;
            lock (_lEngineGate)
            {
                stored = new LFanqieArchive(_lEngineDatabase).LFanqieRead(entry.LEntryLanguage, character);
            }

            foreach (LFanqieRow row in stored)
            {
                rows.Add(row.LFanqieRowFormat(pattern));
            }
        }

        return rows;
    }

    public IReadOnlyList<LFanqieGroup> LEngineFanqieDivide(long entryId)
    {
        IReadOnlyList<LFanqieRow> rows = LEngineFanqieRead(entryId);
        if (rows.Count == 0)
        {
            return [];
        }

        LEntry? entry;
        lock (_lEngineGate)
        {
            entry = new LEntryArchive(_lEngineDatabase).LEntryRead(entryId);
        }

        return LFanqieGroup.LFanqieGroupScan(rows, LEngineBookRead(entry?.LEntryLanguage ?? string.Empty));
    }

    public void LEngineFanqieStart(long entryId)
    {
        LEntry? entry;
        lock (_lEngineGate)
        {
            entry = new LEntryArchive(_lEngineDatabase).LEntryRead(entryId);
        }

        if (entry is null || LEngineBookRead(entry.LEntryLanguage).Count == 0)
        {
            return;
        }

        foreach (string character in LGlyph.LGlyphScan(entry.LEntryHeadword))
        {
            bool stored;
            lock (_lEngineGate)
            {
                stored = new LFanqieArchive(_lEngineDatabase).LFanqieRead(entry.LEntryLanguage, character).Count > 0;
            }

            if (!stored)
            {
                LEngineFanqieStart(entryId, entry.LEntryLanguage, character);
            }
        }
    }

    public void LEngineFanqieRebuild(long entryId)
    {
        LEntry? entry;
        lock (_lEngineGate)
        {
            entry = new LEntryArchive(_lEngineDatabase).LEntryRead(entryId);
        }

        if (entry is null || LEngineBookRead(entry.LEntryLanguage).Count == 0)
        {
            return;
        }

        foreach (string character in LGlyph.LGlyphScan(entry.LEntryHeadword))
        {
            lock (_lEngineGate)
            {
                _lEngineFanqieMissed.Remove(LFanqieKeyFormat(entry.LEntryLanguage, character));
            }

            LEngineFanqieStart(entryId, entry.LEntryLanguage, character);
        }
    }

    public bool LEngineFanqieCheck(long entryId)
    {
        LEntry? entry;
        lock (_lEngineGate)
        {
            entry = new LEntryArchive(_lEngineDatabase).LEntryRead(entryId);
            if (entry is null)
            {
                return false;
            }

            foreach (string character in LGlyph.LGlyphScan(entry.LEntryHeadword))
            {
                if (_lEngineFanqiePending.ContainsKey(LFanqieKeyFormat(entry.LEntryLanguage, character)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    internal async Task<IReadOnlyList<LFanqieRow>> LEngineFanqieFind(
        string character, string language, CancellationToken cancellation)
    {
        (IReadOnlyList<LFanqieRow> found, _) =
            await LEngineFanqieScan(character, language, cancellation).ConfigureAwait(false);
        return found;
    }

    private async Task<(IReadOnlyList<LFanqieRow> LFanqieFound, bool LFanqieReached)> LEngineFanqieScan(
        string character, string language, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(character);

        IReadOnlyList<LFanqieBook> books;
        lock (_lEngineGate)
        {
            books = LEngineBookRead(language);
        }

        bool reached = false;
        List<LFanqieRow> rows = [];
        foreach (LFanqieBook book in books)
        {
            TimeSpan left = _lEngineFanqieStamp - DateTime.UtcNow;
            if (left > TimeSpan.Zero)
            {
                await Task.Delay(left, cancellation).ConfigureAwait(false);
            }

            (IReadOnlyList<LFanqieRow> found, bool answered) = await LFanqieSource
                .LFanqieSourceFind(_lEngineClient, book, character, cancellation)
                .ConfigureAwait(false);
            DateTime next = DateTime.UtcNow + TimeSpan.FromSeconds(book.LFanqieBookInterval);
            if (next > _lEngineFanqieStamp)
            {
                _lEngineFanqieStamp = next;
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

    private void LEngineFanqieStart(long entryId, string language, string character)
    {
        string key = LFanqieKeyFormat(language, character);
        CancellationTokenSource fetch;
        lock (_lEngineGate)
        {
            if (_lEngineFanqieMissed.Contains(key) || _lEngineFanqiePending.ContainsKey(key))
            {
                return;
            }

            fetch = new CancellationTokenSource();
            _lEngineFanqiePending[key] = fetch;
        }

        _ = LEngineFanqieRun(entryId, language, character, key, fetch);
    }

    private void LEngineFanqieClear()
    {
        foreach (CancellationTokenSource held in _lEngineFanqiePending.Values)
        {
            held.Cancel();
            held.Dispose();
        }

        _lEngineFanqiePending.Clear();
        _lEngineFanqieMissed.Clear();
    }

    private async Task LEngineFanqieRun(
        long entryId, string language, string character, string key, CancellationTokenSource fetch)
    {
        bool raised = false;
        bool admitted = false;
        try
        {
            await _lEngineFanqieGate.WaitAsync(fetch.Token).ConfigureAwait(false);
            admitted = true;
            (IReadOnlyList<LFanqieRow> found, bool reached) =
                await LEngineFanqieScan(character, language, fetch.Token).ConfigureAwait(false);

            lock (_lEngineGate)
            {
                if (fetch.IsCancellationRequested)
                {
                    return;
                }

                raised = true;
                if (found.Count > 0)
                {
                    new LFanqieArchive(_lEngineDatabase).LFanqieSave(language, character, found);
                    new LDiweiArchive(_lEngineDatabase).LDiweiApply(
                        language, character, LEngineHypothesisRead(language));
                }
                else if (reached)
                {
                    _lEngineFanqieMissed.Add(key);
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
                _lEngineFanqieGate.Release();
            }

            lock (_lEngineGate)
            {
                if (_lEngineFanqiePending.TryGetValue(key, out CancellationTokenSource? held) && held == fetch)
                {
                    _lEngineFanqiePending.Remove(key);
                    fetch.Dispose();
                }
            }
        }

        if (raised)
        {
            LEngineBulletinRaise(LSubject.LSubjectFanqie, entryId);
        }
    }
}
