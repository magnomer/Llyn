using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LVistaRowFacade
{
    private readonly LEngineHearth _lVistaRowHearth;

    internal LVistaRowFacade(LEngineHearth hearth)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        _lVistaRowHearth = hearth;
    }

    internal IReadOnlyList<LVistaRow> LEngineVistaBuild(IReadOnlyList<LEntry> entries, long? chosen)
    {
        string[] names = LEngineTwinRead(entries);
        IReadOnlyDictionary<long, string> epithets;
        lock (_lVistaRowHearth.LEngineGate)
        {
            epithets = LEngineEpithetScan(entries);
        }

        List<LVistaRow> rows = new(entries.Count);
        for (int index = 0; index < entries.Count; index++)
        {
            LEntry entry = entries[index];
            rows.Add(new LVistaRow(
                entry.LEntryId,
                entry.LEntryHeadword,
                entry.LEntryLanguage,
                epithets.GetValueOrDefault(entry.LEntryId, string.Empty),
                names[index],
                chosen == entry.LEntryId));
        }

        return rows;
    }

    private static string[] LEngineTwinRead(IReadOnlyList<LEntry> entries)
    {
        return LEntryClerkTwin.LTwinRead(entries);
    }

    private IReadOnlyDictionary<long, string> LEngineEpithetScan(IReadOnlyList<LEntry> entries)
    {
        if (!_lVistaRowHearth.LEngineSettingsHeld.LSettingsEpithet || entries.Count == 0)
        {
            return new Dictionary<long, string>();
        }

        long[] ids = new long[entries.Count];
        for (int index = 0; index < ids.Length; index++)
        {
            ids[index] = entries[index].LEntryId;
        }

        return _lVistaRowHearth.LEngineStaffHeld.LEngineStaffEntry.LEntryStaffQuery.LEntryEpithetScan(ids);
    }
}
