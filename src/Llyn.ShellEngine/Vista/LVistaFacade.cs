using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LVistaFacade
{
    private readonly LEngine _lVistaFacadeEngine;
    private readonly object _lVistaFacadeGate;
    private long _lVistaFacadeCount;

    private readonly Dictionary<string, LVista> _lVistaFacadeTabs = [];

    public LVistaFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lVistaFacadeEngine = engine;
        _lVistaFacadeGate = engine.LEngineGate;
    }

    public LVista LEngineVistaStart(
        string tab, LSubject? subject, LCatalogOrder order, LCatalogFilter filter, bool editing, bool blank = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tab);
        ArgumentNullException.ThrowIfNull(filter);

        long id = Interlocked.Increment(ref _lVistaFacadeCount);
        LVista vista = new(_lVistaFacadeEngine, id, tab, subject, order, filter, blank, editing);
        lock (_lVistaFacadeGate)
        {
            if (_lVistaFacadeTabs.TryGetValue(tab, out LVista? former))
            {
                _lVistaFacadeEngine.LEngineObserverDetach(former.LVistaBulletinHandle);
            }

            _lVistaFacadeTabs[tab] = vista;
        }

        _lVistaFacadeEngine.LEngineObserverAttach(vista.LVistaBulletinHandle);
        return vista;
    }

    public LVista? LEngineVistaRead(long id)
    {
        lock (_lVistaFacadeGate)
        {
            foreach (LVista vista in _lVistaFacadeTabs.Values)
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

        lock (_lVistaFacadeGate)
        {
            IReadOnlyList<LEntry> entries = _lVistaFacadeEngine.LEngineEntry.LEngineEntryFind(
                vista.LVistaQuery, vista.LVistaOrder, vista.LVistaFilter);
            return LEngineVistaBuild(entries, vista.LVistaChosen);
        }
    }

    public IReadOnlyList<LVistaRow> LEngineEntryFind(LVista? parent, LVista? child)
    {
        if (parent is null || child is null)
        {
            return [];
        }

        lock (_lVistaFacadeGate)
        {
            long id = parent.LVistaChosen ?? 0;
            IReadOnlyList<LEntry> entries = parent.LVistaSubject switch
            {
                LSubject.LSubjectTag => _lVistaFacadeEngine.LEngineEntry.LEngineEntryFind(new LTag(id, string.Empty)),
                LSubject.LSubjectRegister => _lVistaFacadeEngine.LEngineEntry.LEngineEntryFind(
                    new LRegister(id, LStateValue.LStateValueUnspecified)),
                LSubject.LSubjectExample => _lVistaFacadeEngine.LEngineEntry.LEngineEntryFind(
                    new LExample(id, string.Empty, LStateValue.LStateValueUnspecified,
                        LStateAnchor.LStateAnchorUnspecified)),
                LSubject.LSubjectSituation => _lVistaFacadeEngine.LEngineEntry.LEngineEntryFind(new LSituation(id,
                    LStateValue.LStateValueUnspecified,
                    LStateValue.LStateValueUnspecified,
                    LStateValue.LStateValueUnspecified)),
                LSubject.LSubjectReference =>
                    _lVistaFacadeEngine.LEngineEntry.LEngineEntryFind(
                        LReferenceClerk.LReferenceClerkBlank with { LReferenceId = id }),
                _ => [],
            };
            entries = LEntryClerk.LEntryClerkMatch(parent.LVistaFilter.LCatalogFilterApply(entries,
                entry => entry.LEntryLanguage), child.LVistaQuery);
            return LEngineVistaBuild(entries, child.LVistaChosen);
        }
    }

    internal IReadOnlyList<LVistaRow> LEngineVistaBuild(IReadOnlyList<LEntry> entries, long? chosen)
    {
        string[] names = LEngineTwinRead(entries);
        IReadOnlyDictionary<long, string> epithets;
        lock (_lVistaFacadeGate)
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

    internal static string[] LEngineTwinRead<LEngineRow>(
        IReadOnlyList<LEngineRow> rows, Func<LEngineRow, string> name, Func<LEngineRow, long> id)
    {
        return LEntryClerkTwin.LTwinRead(rows, name, id);
    }

    internal static string LEngineNameRead(LStateValue value, string unknown, string fallback)
    {
        return LEntryClerkTwin.LTwinNameRead(value, unknown, fallback);
    }

    private IReadOnlyDictionary<long, string> LEngineEpithetScan(IReadOnlyList<LEntry> entries)
    {
        if (!_lVistaFacadeEngine.LEngineSettingsHeld.LSettingsEpithet || entries.Count == 0)
        {
            return new Dictionary<long, string>();
        }

        long[] ids = new long[entries.Count];
        for (int index = 0; index < ids.Length; index++)
        {
            ids[index] = entries[index].LEntryId;
        }

        return _lVistaFacadeEngine.LEngineStaffHeld.LEngineStaffEntry.LEntryEpithetScan(ids);
    }

    public IReadOnlyList<LCatalogFavorite> LEngineFavoriteFind(string query)
    {
        lock (_lVistaFacadeGate)
        {
            return _lVistaFacadeEngine.LEngineStaffHeld.LEngineStaffFavorite.LFavoriteClerkFind(query);
        }
    }

    public IReadOnlyList<LCatalogFavorite> LEngineFavoriteFind(string query, LCatalogOrder order)
    {
        lock (_lVistaFacadeGate)
        {
            return _lVistaFacadeEngine.LEngineStaffHeld.LEngineStaffFavorite.LFavoriteClerkFind(query, order);
        }
    }

    public IReadOnlyList<LCatalogFavorite> LEngineFavoriteFind(string query, LCatalogOrder order, LCatalogFilter filter)
    {
        lock (_lVistaFacadeGate)
        {
            return _lVistaFacadeEngine.LEngineStaffHeld.LEngineStaffFavorite.LFavoriteClerkFind(query, order, filter);
        }
    }

    public IReadOnlyList<LVistaRow> LEngineFavoriteFind(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        lock (_lVistaFacadeGate)
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
        lock (_lVistaFacadeGate)
        {
            return _lVistaFacadeEngine.LEngineStaffHeld.LEngineStaffFavorite.LFavoriteClerkCheck(entryId);
        }
    }

    public void LEngineFavoriteSave(long entryId)
    {
        lock (_lVistaFacadeGate)
        {
            _lVistaFacadeEngine.LEngineStaffHeld.LEngineStaffFavorite.LFavoriteClerkSave(entryId);
        }

        _lVistaFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectFavorite, entryId);
    }

    public void LEngineFavoriteDelete(long entryId)
    {
        lock (_lVistaFacadeGate)
        {
            _lVistaFacadeEngine.LEngineStaffHeld.LEngineStaffFavorite.LFavoriteClerkDelete(entryId);
        }

        _lVistaFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectFavorite, entryId);
    }

    internal LDraft? LEngineVistaLoad(LVista vista)
    {
        lock (_lVistaFacadeGate)
        {
            if (vista.LVistaChosen is not long id || id <= 0)
            {
                return null;
            }

            LDraft draft = new(0, vista.LVistaTab, id, LClaimClerk.LDraftBlank,
                _lVistaFacadeEngine.LEngineSettings.LEngineStampRead());
            return vista.LVistaSubject switch
            {
                LSubject.LSubjectEntry => _lVistaFacadeEngine.LEngineEntry.LEngineEntryLoad(id) is LEntryDraft entry
                    ? draft with { LDraftContent = entry } : null,
                LSubject.LSubjectExample =>
                    _lVistaFacadeEngine.LEngineExample.LEngineExampleRead(id) is LExample example
                    ? draft with { LDraftExample = example } : null,
                LSubject.LSubjectSituation =>
                    _lVistaFacadeEngine.LEngineSituation.LEngineSituationRead(id) is LSituation situation
                    ? draft with { LDraftSituation = situation } : null,
                LSubject.LSubjectReference =>
                    _lVistaFacadeEngine.LEngineReference.LEngineReferenceRead(id) is LReference reference
                    ? draft with
                    {
                        LDraftReference = reference,
                        LDraftAuthor = _lVistaFacadeEngine.LEngineAuthor.LEngineAuthorRead(id, LOwner.LOwnerReference),
                    }
                    : null,
                LSubject.LSubjectAuthor => _lVistaFacadeEngine.LEngineAuthor.LEngineAuthorRead(id) is LAuthor author
                    ? draft with { LDraftAuthorHeld = author } : null,
                LSubject.LSubjectTag =>
                    _lVistaFacadeEngine.LEngineCard.LEngineTagRead().FirstOrDefault(row => row.LTagId == id) is LTag tag
                    ? draft with { LDraftTag = tag } : null,
                LSubject.LSubjectRegister => _lVistaFacadeEngine.LEngineCard.LEngineRegisterFind(
                    string.Empty, LCatalogOrder.LCatalogOrderName)
                    .FirstOrDefault(row => row.LCatalogRegisterStored.LRegisterId == id) is LCatalogRegister register
                    ? draft with { LDraftRegister = register.LCatalogRegisterStored } : null,
                _ => null,
            };
        }
    }
}
