using System;
using System.Collections.Generic;
using System.Threading;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private long _lEngineVistaCount;

    private readonly Dictionary<string, LVista> _lEngineVistas = [];

    public LVista LEngineVistaStart(
        string tab, LSubject? subject, LCatalogOrder order, LCatalogFilter filter, bool editing, bool blank = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tab);
        ArgumentNullException.ThrowIfNull(filter);

        LVista vista = new(
            this, Interlocked.Increment(ref _lEngineVistaCount), tab, subject, order, filter, blank, editing);
        lock (_lEngineGate)
        {
            if (_lEngineVistas.TryGetValue(tab, out LVista? former))
            {
                LEngineObserverDetach(former);
            }

            _lEngineVistas[tab] = vista;
        }

        LEngineObserverAttach(vista);
        return vista;
    }

    public LVista? LEngineVistaRead(long id)
    {
        lock (_lEngineGate)
        {
            foreach (LVista vista in _lEngineVistas.Values)
            {
                if (vista.LVistaId == id)
                {
                    return vista;
                }
            }

            return null;
        }
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

    public IReadOnlyList<LVistaRow> LEngineEntryFind(LVista? parent, LVista? child)
    {
        if (parent is null || child is null)
        {
            return [];
        }

        lock (_lEngineGate)
        {
            long id = parent.LVistaChosen ?? 0;
            IReadOnlyList<LEntry> entries = parent.LVistaSubject switch
            {
                LSubject.LSubjectTag => LEngineEntryFind(new LTag(id, string.Empty)),
                LSubject.LSubjectRegister => LEngineEntryFind(new LRegister(id, LStateValue.LStateValueUnspecified)),
                LSubject.LSubjectExample => LEngineEntryFind(new LExample(id, string.Empty,
                    LStateValue.LStateValueUnspecified, LStateAnchor.LStateAnchorUnspecified)),
                LSubject.LSubjectSituation => LEngineEntryFind(new LSituation(id,
                    LStateValue.LStateValueUnspecified, LStateValue.LStateValueUnspecified, LStateValue.LStateValueUnspecified)),
                LSubject.LSubjectReference => LEngineEntryFind(LEngineReferenceBlank with { LReferenceId = id }),
                _ => [],
            };
            entries = LEntryClerk.LEntryClerkMatch(parent.LVistaFilter.LCatalogFilterApply(entries,
                entry => entry.LEntryLanguage), child.LVistaQuery);
            return LEngineVistaBuild(entries, child.LVistaChosen);
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

    private static string[] LEngineTwinRead<LEngineRow>(IReadOnlyList<LEngineRow> rows, Func<LEngineRow, string> name, Func<LEngineRow, long> id)
    {
        string[] names = new string[rows.Count];
        int[] places = new int[rows.Count];
        for (int index = 0; index < places.Length; index++)
        {
            places[index] = index;
        }

        LTwin.LTwinNameApply(places, place => name(rows[place]),
            (place, twinned) => names[place] = twinned, place => id(rows[place]));
        return names;
    }

    private static string LEngineNameRead(LStateValue value, string unknown, string fallback)
    {
        return value.LStateValueState == LState.LStateUnknown
            ? unknown
            : value.LStateValueShow() is { Length: > 0 } text ? text : fallback;
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

        return _lEngineEntries.LEntryEpithetScan(ids);
    }
}
