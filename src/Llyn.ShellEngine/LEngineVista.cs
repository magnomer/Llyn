using System;
using System.Collections.Generic;
using System.Threading;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private long _lEngineVistaCount;

    public LVista LEngineVistaStart(string tab, LCatalogOrder fallback, bool blank = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tab);

        LCatalogOrder order = fallback;
        LCatalogFilter filter = LCatalogFilter.LCatalogFilterEmpty;
        lock (_lEngineGate)
        {
            foreach (LLayout record in _lEngineSettings.LSettingsLayout ?? [])
            {
                if (!string.Equals(record.LLayoutTab, tab, StringComparison.Ordinal))
                {
                    continue;
                }

                order = record.LLayoutOrder ?? fallback;
                filter = record.LLayoutFilter ?? LCatalogFilter.LCatalogFilterEmpty;
                break;
            }
        }

        return new LVista(this, Interlocked.Increment(ref _lEngineVistaCount), tab, order, filter, blank);
    }

    public IReadOnlyList<LVistaRow> LEngineEntryFind(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        if (vista.LVistaBlank && string.IsNullOrWhiteSpace(vista.LVistaQuery))
        {
            return [];
        }

        lock (_lEngineGate)
        {
            IReadOnlyList<LEntry> entries = LEngineEntryFind(vista.LVistaQuery, vista.LVistaOrder, vista.LVistaFilter);
            return LEngineVistaBuild(entries, vista.LVistaChosen);
        }
    }

    private IReadOnlyList<LVistaRow> LEngineVistaBuild(IReadOnlyList<LEntry> entries, long? chosen)
    {
        string[] names = LEngineTwinRead(entries);
        IReadOnlyDictionary<long, string> epithets = LEngineEpithetScan(entries);

        List<LVistaRow> rows = new(entries.Count);
        for (int index = 0; index < entries.Count; index++)
        {
            LEntry entry = entries[index];
            rows.Add(new LVistaRow(
                entry.LEntryId,
                entry.LEntryHeadword,
                entry.LEntryLanguage,
                epithets.GetValueOrDefault(entry.LEntryId),
                names[index],
                chosen == entry.LEntryId));
        }

        return rows;
    }

    private static string[] LEngineTwinRead(IReadOnlyList<LEntry> entries)
    {
        string[] names = new string[entries.Count];
        int[] places = new int[entries.Count];
        for (int index = 0; index < places.Length; index++)
        {
            places[index] = index;
        }

        LTwin.LTwinNameApply(
            places,
            place => entries[place].LEntryHeadword,
            (place, name) => names[place] = name,
            place => entries[place].LEntryId);

        return names;
    }

    private IReadOnlyDictionary<long, string> LEngineEpithetScan(IReadOnlyList<LEntry> entries)
    {
        if (!_lEngineSettings.LSettingsEpithet || entries.Count == 0)
        {
            return new Dictionary<long, string>();
        }

        long[] ids = new long[entries.Count];
        for (int index = 0; index < ids.Length; index++)
        {
            ids[index] = entries[index].LEntryId;
        }

        return new LEntryArchive(_lEngineDatabase).LEntryEpithetScan(ids);
    }
}
