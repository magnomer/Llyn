using System;
using System.Collections.Generic;
using System.Linq;
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
        lock (LEngineGate)
        {
            if (_lEngineVistas.TryGetValue(tab, out LVista? former))
            {
                LEngineObserverDetach(former.LVistaBulletinHandle);
            }

            _lEngineVistas[tab] = vista;
        }

        LEngineObserverAttach(vista.LVistaBulletinHandle);
        return vista;
    }

    public LVista? LEngineVistaRead(long id)
    {
        lock (LEngineGate)
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

        lock (LEngineGate)
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

        lock (LEngineGate)
        {
            long id = parent.LVistaChosen ?? 0;
            IReadOnlyList<LEntry> entries = parent.LVistaSubject switch
            {
                LSubject.LSubjectTag => LEngineEntryFind(new LTag(id, string.Empty)),
                LSubject.LSubjectRegister => LEngineEntryFind(new LRegister(id, LStateValue.LStateValueUnspecified)),
                LSubject.LSubjectExample => LEngineEntryFind(new LExample(id, string.Empty,
                    LStateValue.LStateValueUnspecified, LStateAnchor.LStateAnchorUnspecified)),
                LSubject.LSubjectSituation => LEngineEntryFind(new LSituation(id,
                    LStateValue.LStateValueUnspecified,
                    LStateValue.LStateValueUnspecified,
                    LStateValue.LStateValueUnspecified)),
                LSubject.LSubjectReference =>
                    LEngineEntryFind(LReferenceClerk.LReferenceClerkBlank with { LReferenceId = id }),
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
        return LEntryClerkTwin.LTwinRead(entries);
    }

    public IReadOnlyList<string> LEngineNameResolve(IReadOnlyList<string> labels)
    {
        return LEntryClerkTwin.LTwinNameResolve(labels);
    }

    private static string[] LEngineTwinRead<LEngineRow>(
        IReadOnlyList<LEngineRow> rows, Func<LEngineRow, string> name, Func<LEngineRow, long> id)
    {
        return LEntryClerkTwin.LTwinRead(rows, name, id);
    }

    private static string LEngineNameRead(LStateValue value, string unknown, string fallback)
    {
        return LEntryClerkTwin.LTwinNameRead(value, unknown, fallback);
    }

    private IReadOnlyDictionary<long, string> LEngineEpithetScan(IReadOnlyList<LEntry> entries)
    {
        if (!LEngineSettingsHeld.LSettingsEpithet || entries.Count == 0)
        {
            return new Dictionary<long, string>();
        }

        long[] ids = new long[entries.Count];
        for (int index = 0; index < ids.Length; index++)
        {
            ids[index] = entries[index].LEntryId;
        }

        return _lEngineStaff.LEngineStaffEntry.LEntryEpithetScan(ids);
    }

    public IReadOnlyList<LCatalogFavorite> LEngineFavoriteFind(string query)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffFavorite.LFavoriteClerkFind(query);
        }
    }

    public IReadOnlyList<LCatalogFavorite> LEngineFavoriteFind(string query, LCatalogOrder order)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffFavorite.LFavoriteClerkFind(query, order);
        }
    }

    public IReadOnlyList<LCatalogFavorite> LEngineFavoriteFind(string query, LCatalogOrder order, LCatalogFilter filter)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffFavorite.LFavoriteClerkFind(query, order, filter);
        }
    }

    public IReadOnlyList<LVistaRow> LEngineFavoriteFind(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        lock (LEngineGate)
        {
            IReadOnlyList<LCatalogFavorite> favorites = LEngineFavoriteFind(
                vista.LVistaQuery, vista.LVistaOrder, vista.LVistaFilter);
            List<LEntry> entries = new(favorites.Count);
            foreach (LCatalogFavorite favorite in favorites)
            {
                entries.Add(favorite.LCatalogFavoriteEntry);
            }

            return LEngineVistaBuild(entries, vista.LVistaChosen);
        }
    }

    public bool LEngineFavoriteCheck(long entryId)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffFavorite.LFavoriteClerkCheck(entryId);
        }
    }

    public void LEngineFavoriteSave(long entryId)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffFavorite.LFavoriteClerkSave(entryId);
        }

        LEngineBulletinRaise(LSubject.LSubjectFavorite, entryId);
    }

    public void LEngineFavoriteDelete(long entryId)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffFavorite.LFavoriteClerkDelete(entryId);
        }

        LEngineBulletinRaise(LSubject.LSubjectFavorite, entryId);
    }

    internal LDraft? LEngineVistaLoad(LVista vista)
    {
        lock (LEngineGate)
        {
            if (vista.LVistaChosen is not long id || id <= 0)
            {
                return null;
            }

            LDraft draft = new(0, vista.LVistaTab, id, LClaimClerk.LDraftBlank, LEngineStampRead());
            return vista.LVistaSubject switch
            {
                LSubject.LSubjectEntry => LEngineEntryLoad(id) is LEntryDraft entry
                    ? draft with { LDraftContent = entry } : null,
                LSubject.LSubjectExample => LEngineExampleRead(id) is LExample example
                    ? draft with { LDraftExample = example } : null,
                LSubject.LSubjectSituation => LEngineSituationRead(id) is LSituation situation
                    ? draft with { LDraftSituation = situation } : null,
                LSubject.LSubjectReference => LEngineReferenceRead(id) is LReference reference
                    ? draft with
                    {
                        LDraftReference = reference,
                        LDraftAuthor = LEngineAuthorRead(id, LOwner.LOwnerReference),
                    }
                    : null,
                LSubject.LSubjectAuthor => LEngineAuthorRead(id) is LAuthor author
                    ? draft with { LDraftAuthorHeld = author } : null,
                LSubject.LSubjectTag => LEngineTagRead().FirstOrDefault(row => row.LTagId == id) is LTag tag
                    ? draft with { LDraftTag = tag } : null,
                LSubject.LSubjectRegister => LEngineRegisterFind(string.Empty, LCatalogOrder.LCatalogOrderName)
                    .FirstOrDefault(row => row.LCatalogRegisterStored.LRegisterId == id) is LCatalogRegister register
                    ? draft with { LDraftRegister = register.LCatalogRegisterStored } : null,
                _ => null,
            };
        }
    }
}
