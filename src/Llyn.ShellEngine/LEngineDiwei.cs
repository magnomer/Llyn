using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal IReadOnlyList<LDiwei> LEngineDiweiRead(string language, string kind)
    {
        lock (_lEngineGate)
        {
            return _lEngineDiweiVault.LDiweiRead(language, kind);
        }
    }

    public LDiwei? LEngineDiweiRead(long? id)
    {
        if (id is not long wanted)
        {
            return null;
        }

        lock (_lEngineGate)
        {
            return _lEngineDiweiVault.LDiweiRead(wanted);
        }
    }

    public LDiweiPage LEngineDiweiResolve(long? id, Func<string, string?> localize)
    {
        ArgumentNullException.ThrowIfNull(localize);

        LDiwei? diwei = LEngineDiweiRead(id);
        if (diwei is null)
        {
            return LDiweiPage.LDiweiPageBlank;
        }

        bool switched = LEngineRespellingCheck(diwei.LDiweiLanguage);
        return new LDiweiPage(
            diwei.LDiweiLanguage,
            diwei.LDiweiKey,
            diwei.LDiweiFinal,
            LDiweiSection.LDiweiSectionScan(
                diwei.LDiweiKind,
                LEngineFanqieRead(diwei),
                LEngineHypothesisRead(diwei.LDiweiLanguage),
                LEngineTallyRead(diwei),
                switched,
                switched && LEngineSettingsRead().LSettingsTally,
                localize));
    }

    public LDiwei? LEngineDiweiFind(string language, string kind, string key)
    {
        lock (_lEngineGate)
        {
            return _lEngineDiweiVault.LDiweiFind(language, kind, key);
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
        ArgumentNullException.ThrowIfNull(diwei);

        lock (_lEngineGate)
        {
            return _lEngineDiweiVault.LDiweiFanqieRead(diwei.LDiweiId);
        }
    }

    internal IReadOnlyList<long> LEngineDiweiScan(string language, IReadOnlyList<long> diweiIds)
    {
        lock (_lEngineGate)
        {
            return _lEngineDiweiVault.LDiweiEntryScan(language, diweiIds);
        }
    }

    public IReadOnlyList<LVistaRow> LEngineXiaoyunFind(string language, IReadOnlyList<long> diweiIds, string query, LVista? vista = null)
    {
        ArgumentNullException.ThrowIfNull(query);

        lock (_lEngineGate)
        {
            IReadOnlyList<long> ids = _lEngineDiweiVault.LDiweiEntryScan(language, diweiIds);
            if (ids.Count == 0)
            {
                return [];
            }

            IReadOnlyList<LEntry> entries = _lEngineEntries.LEntryScan(ids, query);
            return LEngineVistaBuild(entries, vista?.LVistaChosen);
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
            LEngineDiweiApply();
        }
    }

    private void LEngineDiweiApply()
    {
        foreach (string language in _lEngineLanguageVault.LLanguageScan())
        {
            if (LEngineBookRead(language).Count > 0)
            {
                _lEngineDiweiVault.LDiweiRebuild(language, LEngineHypothesisRead(language));
            }
        }
    }
}
