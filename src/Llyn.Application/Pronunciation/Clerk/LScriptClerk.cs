using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LScriptClerk
{
    private readonly LEntryVault _lScriptClerkEntries;
    private readonly LScriptVault _lScriptClerkScripts;
    private readonly LScriptSource _lScriptClerkSource;
    private readonly LLanguageCache _lScriptClerkLanguages;
    private readonly object _lScriptClerkGate;
    private readonly Action<LSubject, long> _lScriptClerkBulletin;
    private readonly SemaphoreSlim _lScriptClerkAdmission = new(2, 2);
    private readonly Dictionary<string, CancellationTokenSource> _lScriptClerkPending = new(StringComparer.Ordinal);
    private readonly HashSet<string> _lScriptClerkMissed = new(StringComparer.Ordinal);

    public LScriptClerk(LRig rig, LLanguageCache languages, object gate, Action<LSubject, long> raise)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(gate);
        ArgumentNullException.ThrowIfNull(raise);
        _lScriptClerkEntries = rig.LRigEntries;
        _lScriptClerkScripts = rig.LRigScripts;
        _lScriptClerkSource = rig.LRigScriptSource;
        _lScriptClerkLanguages = languages;
        _lScriptClerkGate = gate;
        _lScriptClerkBulletin = raise;
    }

    public IReadOnlyList<LScriptStyle> LScriptStyleRead(string language)
    {
        return string.IsNullOrWhiteSpace(language)
            ? []
            : _lScriptClerkLanguages.LLanguageCacheRead(language).LLanguageScripts;
    }

    public IReadOnlyList<LScriptImage> LScriptClerkRead(long entryId)
    {
        LEntry? entry;
        lock (_lScriptClerkGate)
        {
            entry = _lScriptClerkEntries.LEntryRead(entryId);
        }

        if (entry is null || LScriptStyleRead(entry.LEntryLanguage).Count == 0)
        {
            return [];
        }

        List<LScriptImage> images = [];
        foreach (string character in LGlyph.LGlyphScan(entry.LEntryHeadword))
        {
            IReadOnlyList<LScriptImage> stored;
            lock (_lScriptClerkGate)
            {
                stored = _lScriptClerkScripts.LScriptRead(entry.LEntryLanguage, character);
            }

            images.AddRange(stored);
        }

        return images;
    }

    public void LScriptClerkStart(long entryId)
    {
        LEntry? entry;
        lock (_lScriptClerkGate)
        {
            entry = _lScriptClerkEntries.LEntryRead(entryId);
        }

        if (entry is null || LScriptStyleRead(entry.LEntryLanguage).Count == 0)
        {
            return;
        }

        foreach (string character in LGlyph.LGlyphScan(entry.LEntryHeadword))
        {
            bool stored;
            lock (_lScriptClerkGate)
            {
                stored = _lScriptClerkScripts.LScriptRead(entry.LEntryLanguage, character).Count > 0;
            }

            if (!stored)
            {
                LScriptClerkStart(entryId, entry.LEntryLanguage, character);
            }
        }
    }

    public void LScriptClerkRebuild(long entryId)
    {
        LEntry? entry;
        lock (_lScriptClerkGate)
        {
            entry = _lScriptClerkEntries.LEntryRead(entryId);
        }

        if (entry is null || LScriptStyleRead(entry.LEntryLanguage).Count == 0)
        {
            return;
        }

        foreach (string character in LGlyph.LGlyphScan(entry.LEntryHeadword))
        {
            lock (_lScriptClerkGate)
            {
                _lScriptClerkMissed.Remove(LScriptKeyFormat(entry.LEntryLanguage, character));
                _lScriptClerkScripts.LScriptSave(entry.LEntryLanguage, character, []);
            }

            LScriptClerkStart(entryId, entry.LEntryLanguage, character);
        }

        _lScriptClerkBulletin(LSubject.LSubjectScript, entryId);
    }

    public IReadOnlyList<LScriptGroup> LScriptClerkDivide(long entryId)
    {
        IReadOnlyList<LScriptImage> images = LScriptClerkRead(entryId);
        if (images.Count == 0)
        {
            return [];
        }

        LEntry? entry;
        lock (_lScriptClerkGate)
        {
            entry = _lScriptClerkEntries.LEntryRead(entryId);
        }

        return LScriptGroup.LScriptGroupScan(images, LScriptStyleRead(entry?.LEntryLanguage ?? string.Empty));
    }

    public bool LScriptClerkCheck(long entryId)
    {
        LEntry? entry;
        lock (_lScriptClerkGate)
        {
            entry = _lScriptClerkEntries.LEntryRead(entryId);
            if (entry is null)
            {
                return false;
            }

            foreach (string character in LGlyph.LGlyphScan(entry.LEntryHeadword))
            {
                if (_lScriptClerkPending.ContainsKey(LScriptKeyFormat(entry.LEntryLanguage, character)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public async Task<IReadOnlyList<LScriptImage>> LScriptClerkFind(
        string character, string language, CancellationToken cancellation)
    {
        (IReadOnlyList<LScriptImage> found, _) =
            await LScriptClerkScan(character, language, cancellation).ConfigureAwait(false);
        return found;
    }

    public void LScriptClerkClear()
    {
        lock (_lScriptClerkGate)
        {
            foreach (CancellationTokenSource held in _lScriptClerkPending.Values)
            {
                held.Cancel();
                held.Dispose();
            }

            _lScriptClerkPending.Clear();
            _lScriptClerkMissed.Clear();
        }
    }

    private async Task<(IReadOnlyList<LScriptImage> LScriptFound, bool LScriptReached)> LScriptClerkScan(
        string character, string language, CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(character);

        IReadOnlyList<LScriptStyle> styles;
        lock (_lScriptClerkGate)
        {
            styles = LScriptStyleRead(language);
        }

        bool reached = false;
        List<LScriptImage> images = [];
        foreach (LScriptStyle style in styles)
        {
            (IReadOnlyList<LScriptImage> found, bool answered) = await _lScriptClerkSource
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

    private void LScriptClerkStart(long entryId, string language, string character)
    {
        string key = LScriptKeyFormat(language, character);
        CancellationTokenSource fetch;
        lock (_lScriptClerkGate)
        {
            if (_lScriptClerkMissed.Contains(key) || _lScriptClerkPending.ContainsKey(key))
            {
                return;
            }

            fetch = new CancellationTokenSource();
            _lScriptClerkPending[key] = fetch;
        }

        _ = LScriptClerkRun(entryId, language, character, key, fetch);
    }

    private async Task LScriptClerkRun(
        long entryId, string language, string character, string key, CancellationTokenSource fetch)
    {
        bool raised = false;
        bool admitted = false;
        try
        {
            await _lScriptClerkAdmission.WaitAsync(fetch.Token).ConfigureAwait(false);
            admitted = true;
            (IReadOnlyList<LScriptImage> found, bool reached) =
                await LScriptClerkScan(character, language, fetch.Token).ConfigureAwait(false);

            lock (_lScriptClerkGate)
            {
                if (fetch.IsCancellationRequested)
                {
                    return;
                }

                raised = true;
                if (found.Count > 0)
                {
                    _lScriptClerkScripts.LScriptSave(language, character, found);
                }
                else if (reached)
                {
                    _lScriptClerkMissed.Add(key);
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
                _lScriptClerkAdmission.Release();
            }

            lock (_lScriptClerkGate)
            {
                if (_lScriptClerkPending.TryGetValue(key, out CancellationTokenSource? held) && held == fetch)
                {
                    _lScriptClerkPending.Remove(key);
                    fetch.Dispose();
                }
            }
        }

        if (raised)
        {
            _lScriptClerkBulletin(LSubject.LSubjectScript, entryId);
        }
    }
}
