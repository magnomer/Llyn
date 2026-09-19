using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private readonly SemaphoreSlim _lEngineScriptGate = new(2, 2);
    private readonly Dictionary<string, CancellationTokenSource> _lEngineScriptPending = new(StringComparer.Ordinal);
    private readonly HashSet<string> _lEngineScriptMissed = new(StringComparer.Ordinal);

    public IReadOnlyList<LScriptStyle> LEngineStyleRead(string language)
    {
        return string.IsNullOrWhiteSpace(language) ? [] : LEngineLanguageLoad(language).LLanguageScripts;
    }

    public IReadOnlyList<LScriptImage> LEngineScriptRead(long entryId)
    {
        LEntry? entry;
        lock (_lEngineGate)
        {
            entry = _lEngineEntries.LEntryRead(entryId);
        }

        if (entry is null || LEngineStyleRead(entry.LEntryLanguage).Count == 0)
        {
            return [];
        }

        List<LScriptImage> images = [];
        foreach (string character in LGlyph.LGlyphScan(entry.LEntryHeadword))
        {
            IReadOnlyList<LScriptImage> stored;
            lock (_lEngineGate)
            {
                stored = _lEngineScripts.LScriptRead(entry.LEntryLanguage, character);
            }

            images.AddRange(stored);
        }

        return images;
    }

    public void LEngineScriptStart(long entryId)
    {
        LEntry? entry;
        lock (_lEngineGate)
        {
            entry = _lEngineEntries.LEntryRead(entryId);
        }

        if (entry is null || LEngineStyleRead(entry.LEntryLanguage).Count == 0)
        {
            return;
        }

        foreach (string character in LGlyph.LGlyphScan(entry.LEntryHeadword))
        {
            bool stored;
            lock (_lEngineGate)
            {
                stored = _lEngineScripts.LScriptRead(entry.LEntryLanguage, character).Count > 0;
            }

            if (!stored)
            {
                LEngineScriptStart(entryId, entry.LEntryLanguage, character);
            }
        }
    }

    public IReadOnlyList<LScriptGroup> LEngineScriptDivide(long entryId)
    {
        IReadOnlyList<LScriptImage> images = LEngineScriptRead(entryId);
        if (images.Count == 0)
        {
            return [];
        }

        LEntry? entry;
        lock (_lEngineGate)
        {
            entry = _lEngineEntries.LEntryRead(entryId);
        }

        return LScriptGroup.LScriptGroupScan(images, LEngineStyleRead(entry?.LEntryLanguage ?? string.Empty));
    }

    public bool LEngineScriptCheck(long entryId)
    {
        LEntry? entry;
        lock (_lEngineGate)
        {
            entry = _lEngineEntries.LEntryRead(entryId);
            if (entry is null)
            {
                return false;
            }

            foreach (string character in LGlyph.LGlyphScan(entry.LEntryHeadword))
            {
                if (_lEngineScriptPending.ContainsKey(LScriptKeyFormat(entry.LEntryLanguage, character)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    internal async Task<IReadOnlyList<LScriptImage>> LEngineScriptFind(
        string character, string language, CancellationToken cancellation)
    {
        (IReadOnlyList<LScriptImage> found, _) =
            await LEngineScriptScan(character, language, cancellation).ConfigureAwait(false);
        return found;
    }

    private async Task<(IReadOnlyList<LScriptImage> LScriptFound, bool LScriptReached)> LEngineScriptScan(
        string character, string language, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(character);

        IReadOnlyList<LScriptStyle> styles;
        lock (_lEngineGate)
        {
            styles = LEngineStyleRead(language);
        }

        bool reached = false;
        List<LScriptImage> images = [];
        foreach (LScriptStyle style in styles)
        {
            (IReadOnlyList<LScriptImage> found, bool answered) = await _lEngineScriptSource
                .LScriptSourceFind(style, character, cancellation)
                .ConfigureAwait(false);
            reached |= answered;
            images.AddRange(found);
        }

        return (images, reached);
    }

    private static string LScriptKeyFormat(string language, string character)
    {
        return language + '\n' + character;
    }

    private void LEngineScriptStart(long entryId, string language, string character)
    {
        string key = LScriptKeyFormat(language, character);
        CancellationTokenSource fetch;
        lock (_lEngineGate)
        {
            if (_lEngineScriptMissed.Contains(key) || _lEngineScriptPending.ContainsKey(key))
            {
                return;
            }

            fetch = new CancellationTokenSource();
            _lEngineScriptPending[key] = fetch;
        }

        _ = LEngineScriptRun(entryId, language, character, key, fetch);
    }

    private void LEngineScriptClear()
    {
        foreach (CancellationTokenSource held in _lEngineScriptPending.Values)
        {
            held.Cancel();
            held.Dispose();
        }

        _lEngineScriptPending.Clear();
        _lEngineScriptMissed.Clear();
    }

    private async Task LEngineScriptRun(
        long entryId, string language, string character, string key, CancellationTokenSource fetch)
    {
        bool raised = false;
        bool admitted = false;
        try
        {
            await _lEngineScriptGate.WaitAsync(fetch.Token).ConfigureAwait(false);
            admitted = true;
            (IReadOnlyList<LScriptImage> found, bool reached) =
                await LEngineScriptScan(character, language, fetch.Token).ConfigureAwait(false);

            lock (_lEngineGate)
            {
                if (fetch.IsCancellationRequested)
                {
                    return;
                }

                raised = true;
                if (found.Count > 0)
                {
                    _lEngineScripts.LScriptSave(language, character, found);
                }
                else if (reached)
                {
                    _lEngineScriptMissed.Add(key);
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
                _lEngineScriptGate.Release();
            }

            lock (_lEngineGate)
            {
                if (_lEngineScriptPending.TryGetValue(key, out CancellationTokenSource? held) && held == fetch)
                {
                    _lEngineScriptPending.Remove(key);
                    fetch.Dispose();
                }
            }
        }

        if (raised)
        {
            LEngineBulletinRaise(LSubject.LSubjectScript, entryId);
        }
    }
}
