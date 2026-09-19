using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal IReadOnlyList<LDiwei> LEngineDiweiRead(string language, string kind)
    {
        lock (_lEngineGate)
        {
            return new LDiweiArchive(_lEngineDatabase).LDiweiRead(language, kind);
        }
    }

    public LDiwei? LEngineDiweiFind(string language, string kind, string key)
    {
        lock (_lEngineGate)
        {
            return new LDiweiArchive(_lEngineDatabase).LDiweiFind(language, kind, key);
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
            return new LDiweiArchive(_lEngineDatabase).LDiweiFanqieRead(diwei.LDiweiId);
        }
    }

    internal IReadOnlyList<long> LEngineDiweiScan(string language, IReadOnlyList<long> diweiIds)
    {
        lock (_lEngineGate)
        {
            return new LDiweiArchive(_lEngineDatabase).LDiweiEntryScan(language, diweiIds);
        }
    }

    public IReadOnlyList<LVistaRow> LEngineXiaoyunFind(string language, IReadOnlyList<long> diweiIds, string query, LVista? vista = null)
    {
        ArgumentNullException.ThrowIfNull(query);

        lock (_lEngineGate)
        {
            IReadOnlyList<long> ids = new LDiweiArchive(_lEngineDatabase).LDiweiEntryScan(language, diweiIds);
            if (ids.Count == 0)
            {
                return [];
            }

            IReadOnlyList<LEntry> entries = new LEntryArchive(_lEngineDatabase).LEntryScan(ids, query);
            return LEngineVistaBuild(entries, vista?.LVistaChosen);
        }
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
        foreach (string language in LLanguageLoader.LLanguageLoaderScan())
        {
            if (LEngineBookRead(language).Count > 0)
            {
                new LDiweiArchive(_lEngineDatabase).LDiweiRebuild(language, LEngineHypothesisRead(language));
            }
        }
    }
}
