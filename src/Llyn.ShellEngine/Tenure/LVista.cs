using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LVista
{
    private readonly LEngineHearth _lVistaHearth;

    private readonly object _lVistaGate = new();

    private readonly LBulletinRoster _lVistaRoster = new(null);

    private readonly LBulletinRoster _lVistaChosenRoster;

    internal LVista(
        LEngineHearth hearth, long id, string tab, LSubject? subject, LCatalogOrder order, LCatalogFilter filter,
        bool blank, bool editing)
    {
        _lVistaHearth = hearth;
        _lVistaChosenRoster = new LBulletinRoster(LVistaChosenCheck);
        LVistaId = id;
        LVistaTab = tab;
        LVistaOrder = order;
        LVistaFilter = filter;
        LVistaBlank = blank;
        LVistaEditing = editing;
        LVistaSubject = subject;
    }

    public event Action<bool>? LVistaEditingSaved;

    public long LVistaId { get; }

    public string LVistaTab { get; }

    public LSubject? LVistaSubject { get; }

    public bool LVistaBlank { get; }

    public LCatalogOrder LVistaOrder { get; private set; }

    public LCatalogFilter LVistaFilter { get; private set; }

    public string LVistaQuery { get; private set; } = string.Empty;

    public long? LVistaChosen { get; private set; }

    public bool LVistaEditing { get; private set; }

    public long? LVistaStored => LVistaStoredCheck(LVistaChosen) ? LVistaChosen : null;

    public static bool LVistaStoredCheck(long? id)
    {
        return id is > 0;
    }

    public bool LVistaMatch(long id)
    {
        return LVistaChosen == id;
    }

    public bool LVistaFiltered => LVistaFilter.LCatalogFilterActive;

    public bool LVistaQueried => LVistaQuery.Trim().Length > 0;

    public bool LVistaNarrowed => LVistaFiltered || LVistaQueried;

    public bool LVistaLeft => string.Equals(LVistaTab, "left", StringComparison.Ordinal);

    public bool LVistaInput => string.Equals(LVistaTab, "input", StringComparison.Ordinal);

    public static LCatalogOrder LVistaOrderRead(LVista? vista)
    {
        return vista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderHeadword;
    }

    public static LCatalogFilter LVistaFilterRead(LVista? vista)
    {
        return vista?.LVistaFilter ?? LCatalogFilter.LCatalogFilterEmpty;
    }

    public void LVistaEditingSet(bool editing)
    {
        LVistaEditingSaved?.Invoke(editing);
        if (LVistaEditing == editing)
        {
            return;
        }

        LVistaEditing = editing;
        _lVistaHearth.LEngineBulletinRaise(LSubject.LSubjectVista, LVistaId);
    }

    public void LVistaOrderSet(LCatalogOrder? order)
    {
        if (order is not LCatalogOrder chosen || chosen == LVistaOrder)
        {
            return;
        }

        LVistaOrder = chosen;
        _lVistaHearth.LEngineBulletinRaise(LSubject.LSubjectVista, LVistaId);
    }

    public void LVistaFilterSet(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        if (filter == LVistaFilter)
        {
            return;
        }

        LVistaFilter = filter;
        _lVistaHearth.LEngineBulletinRaise(LSubject.LSubjectVista, LVistaId);
    }

    public void LVistaFilterSet(IReadOnlyList<string> hidden)
    {
        ArgumentNullException.ThrowIfNull(hidden);

        LVistaFilterSet(LCatalogClerk.LCatalogClerkCreate(hidden));
    }

    public void LVistaQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (string.Equals(query, LVistaQuery, StringComparison.Ordinal))
        {
            return;
        }

        LVistaQuery = query;
        _lVistaHearth.LEngineBulletinRaise(LSubject.LSubjectVista, LVistaId);
    }

    public void LVistaSelect(long? id)
    {
        if (id == LVistaChosen)
        {
            return;
        }

        lock (_lVistaGate)
        {
            LVistaChosen = id;
        }
    }

    public void LVistaToggle(long id)
    {
        LVistaSelect(LVistaMatch(id) ? null : id);
    }

    public void LVistaObserverAttach(LSubject subject, Action<LBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        lock (_lVistaGate)
        {
            _lVistaRoster.LBulletinRosterAttach(subject, observer, null);
        }
    }

    public void LVistaChosenAttach(LSubject subject, Action<LBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        lock (_lVistaGate)
        {
            _lVistaChosenRoster.LBulletinRosterAttach(subject, observer, null);
        }
    }

    internal void LVistaBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject == LSubject.LSubjectVista && bulletin.LBulletinId != LVistaId)
        {
            return;
        }

        (LSubject?, Action<LBulletin>, long?)[] snapshot;
        (LSubject?, Action<LBulletin>, long?)[] chosenSnapshot;
        lock (_lVistaGate)
        {
            snapshot = _lVistaRoster.LBulletinRosterRead();
            chosenSnapshot = _lVistaChosenRoster.LBulletinRosterRead();
        }

        _lVistaRoster.LBulletinRosterDispatch(bulletin, snapshot);
        _lVistaChosenRoster.LBulletinRosterDispatch(bulletin, chosenSnapshot);
    }

    private bool LVistaChosenCheck(LBulletin bulletin)
    {
        long? chosen;
        lock (_lVistaGate)
        {
            chosen = LVistaChosen;
        }

        return bulletin.LBulletinId <= 0 || bulletin.LBulletinId == chosen;
    }
}
