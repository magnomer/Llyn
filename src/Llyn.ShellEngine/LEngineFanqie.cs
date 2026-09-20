using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LFanqieBook> LEngineBookRead(string language)
    {
        lock (_lEngineGate)
        {
            return _lEngineFanqieClerk.LFanqieBookRead(language);
        }
    }

    public bool LEngineBookCheck(string language)
    {
        return LEngineBookRead(language).Count > 0;
    }

    public string? LEngineBookFind()
    {
        foreach (string language in LEngineLanguageRead())
        {
            if (LEngineBookCheck(language))
            {
                return language;
            }
        }

        return null;
    }

    public LHypothesis? LEngineHypothesisRead(string language)
    {
        lock (_lEngineGate)
        {
            return _lEngineFanqieClerk.LHypothesisRead(language);
        }
    }

    public IReadOnlyList<LFanqieRow> LEngineFanqieRead(long entryId)
    {
        return _lEngineFanqieClerk.LFanqieClerkRead(entryId);
    }

    public IReadOnlyList<LFanqieGroup> LEngineFanqieDivide(long entryId)
    {
        return _lEngineFanqieClerk.LFanqieClerkDivide(entryId);
    }

    public void LEngineFanqieStart(long entryId)
    {
        _lEngineFanqieClerk.LFanqieClerkStart(entryId);
    }

    public void LEngineFanqieRebuild(long entryId)
    {
        _lEngineFanqieClerk.LFanqieClerkRebuild(entryId);
    }

    public bool LEngineFanqieCheck(long entryId)
    {
        return _lEngineFanqieClerk.LFanqieClerkCheck(entryId);
    }

    internal Task<IReadOnlyList<LFanqieRow>> LEngineFanqieFind(
        string character, string language, CancellationToken cancellation)
    {
        return _lEngineFanqieClerk.LFanqieClerkFind(character, language, cancellation);
    }

    internal IReadOnlyList<LDiwei> LEngineDiweiRead(string language, string kind)
    {
        lock (_lEngineGate)
        {
            return _lEngineDiweiClerk.LDiweiClerkRead(language, kind);
        }
    }

    public LDiwei? LEngineDiweiRead(long? id)
    {
        lock (_lEngineGate)
        {
            return _lEngineDiweiClerk.LDiweiClerkRead(id);
        }
    }

    public LDiweiPage LEngineDiweiResolve(long? id, Func<string, string?> localize)
    {
        ArgumentNullException.ThrowIfNull(localize);

        lock (_lEngineGate)
        {
            LDiwei? diwei = _lEngineDiweiClerk.LDiweiClerkRead(id);
            if (diwei is null)
            {
                return LDiweiPage.LDiweiPageBlank;
            }

            return _lEngineDiweiClerk.LDiweiPageRead(
                diwei,
                LEngineRespellingCheck(diwei.LDiweiLanguage),
                _lEngineSettings.LSettingsTally,
                localize);
        }
    }

    public LDiwei? LEngineDiweiFind(string language, string kind, string key)
    {
        lock (_lEngineGate)
        {
            return _lEngineDiweiClerk.LDiweiClerkFind(language, kind, key);
        }
    }

    public IReadOnlyList<LDiwei> LEngineDiweiFind(LVista vista, string language, string kind)
    {
        ArgumentNullException.ThrowIfNull(vista);

        string wanted = vista.LVistaQuery.Trim();
        IEnumerable<LDiwei> kept = LEngineDiweiRead(language, kind).Where(row =>
            wanted.Length == 0 || row.LDiweiKey.Contains(wanted, StringComparison.OrdinalIgnoreCase));
        IReadOnlyList<LDiwei> sorted = vista.LVistaOrder switch
        {
            LCatalogOrder.LCatalogOrderReverse => [.. kept
                .OrderByDescending(row => row.LDiweiKey, StringComparer.Ordinal)],
            LCatalogOrder.LCatalogOrderUsage => [.. kept
                .OrderByDescending(row => row.LDiweiCount)
                .ThenBy(row => row.LDiweiKey, StringComparer.Ordinal)],
            _ => [.. kept.OrderBy(row => row.LDiweiKey, StringComparer.Ordinal)],
        };
        if (vista.LVistaChosen is long chosen && LDiwei.LDiweiFind(sorted, chosen) is null)
        {
            vista.LVistaSelect(null);
        }

        return [.. sorted.Select(row => row with { LDiweiChosen = vista.LVistaMatch(row.LDiweiId) })];
    }

    public IReadOnlyList<LFanqieRow> LEngineFanqieRead(LDiwei diwei)
    {
        lock (_lEngineGate)
        {
            return _lEngineDiweiClerk.LDiweiFanqieRead(diwei);
        }
    }

    internal IReadOnlyList<long> LEngineDiweiScan(string language, IReadOnlyList<long> diweiIds)
    {
        lock (_lEngineGate)
        {
            return _lEngineDiweiClerk.LDiweiClerkScan(language, diweiIds);
        }
    }

    public IReadOnlyList<LVistaRow> LEngineXiaoyunFind(
        string language, IReadOnlyList<long> diweiIds, string query, LVista? vista = null)
    {
        lock (_lEngineGate)
        {
            IReadOnlyList<LEntry> entries = _lEngineDiweiClerk.LDiweiEntryScan(language, diweiIds, query);
            return entries.Count == 0 ? [] : LEngineVistaBuild(entries, vista?.LVistaChosen);
        }
    }

    public IReadOnlyList<LVistaRow> LEngineXiaoyunFind(string language, LVista onset, LVista rime, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(onset);
        ArgumentNullException.ThrowIfNull(rime);
        ArgumentNullException.ThrowIfNull(vista);

        List<long> wanted = [];
        if (onset.LVistaChosen is long initial)
        {
            wanted.Add(initial);
        }

        if (rime.LVistaChosen is long final)
        {
            wanted.Add(final);
        }

        if (wanted.Count == 0)
        {
            return [];
        }

        return LEngineXiaoyunFind(language, wanted, vista.LVistaQuery.Trim(), vista);
    }

    internal void LEngineDiweiRebuild()
    {
        lock (_lEngineGate)
        {
            _lEngineFanqieClerk.LDiweiApply();
        }
    }

    public IReadOnlyList<LTally> LEngineTallyRead(LDiwei diwei)
    {
        lock (_lEngineGate)
        {
            return _lEngineDiweiClerk.LTallyRead(diwei);
        }
    }
}
