using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LVista : LObserver
{
    private readonly LEngine _lEngine;

    private readonly object _lVistaGate = new();

    private readonly List<(LSubject, LObserver)> _lVistaObservers = [];

    private readonly List<(LSubject, LObserver)> _lVistaChosenObservers = [];

    internal LVista(
        LEngine engine, long id, string tab, LSubject? subject, LCatalogOrder order, LCatalogFilter filter,
        bool blank, bool editing)
    {
        _lEngine = engine;
        LVistaId = id;
        LVistaTab = tab;
        LVistaOrder = order;
        LVistaFilter = filter;
        LVistaBlank = blank;
        LVistaEditing = editing;
        LVistaSubject = subject;
    }

    public long LVistaId { get; }

    public string LVistaTab { get; }

    public LSubject? LVistaSubject { get; }

    public bool LVistaBlank { get; }

    public LCatalogOrder LVistaOrder { get; private set; }

    public LCatalogFilter LVistaFilter { get; private set; }

    public string LVistaQuery { get; private set; } = string.Empty;

    public long? LVistaChosen { get; private set; }

    public bool LVistaEditing { get; private set; }

    public LDraft? LVistaLoad() => _lEngine.LEngineVistaLoad(this);

    public LRevision? LVistaDelete()
    {
        if (LVistaChosen is not long id || id <= 0)
        {
            return null;
        }

        LRevision? revision;
        switch (LVistaSubject)
        {
            case LSubject.LSubjectEntry:
                revision = _lEngine.LEngineEntryDelete(id);
                break;
            case LSubject.LSubjectExample:
                _lEngine.LEngineExampleDelete(id, true);
                revision = null;
                break;
            case LSubject.LSubjectSituation:
                _lEngine.LEngineSituationDelete(id, true);
                revision = null;
                break;
            case LSubject.LSubjectReference:
                _lEngine.LEngineReferenceDelete(id, true);
                revision = null;
                break;
            case LSubject.LSubjectAuthor:
                _lEngine.LEngineAuthorDelete(id, true);
                revision = null;
                break;
            default:
                return null;
        }

        if (LVistaChosen == id)
        {
            LVistaSelect(null);
        }

        return revision;
    }

    public void LVistaEditingSet(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);
        if (LVistaEditing == editing)
        {
            return;
        }

        LVistaEditing = editing;
        _lEngine.LEngineBulletinRaise(LSubject.LSubjectVista, LVistaId);
    }

    public void LVistaOrderSet(LCatalogOrder order)
    {
        if (order == LVistaOrder)
        {
            return;
        }

        LVistaOrder = order;
        _lEngine.LEngineLayoutSave([new LLayout(LVistaTab, LLayoutOrder: order)]);
        _lEngine.LEngineBulletinRaise(LSubject.LSubjectVista, LVistaId);
    }

    public void LVistaFilterSet(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        if (filter == LVistaFilter)
        {
            return;
        }

        LVistaFilter = filter;
        _lEngine.LEngineLayoutSave([new LLayout(LVistaTab, LLayoutFilter: filter)]);
        _lEngine.LEngineBulletinRaise(LSubject.LSubjectVista, LVistaId);
    }

    public void LVistaQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (string.Equals(query, LVistaQuery, StringComparison.Ordinal))
        {
            return;
        }

        LVistaQuery = query;
        _lEngine.LEngineBulletinRaise(LSubject.LSubjectVista, LVistaId);
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

    public void LVistaObserverAttach(LSubject subject, LObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        lock (_lVistaGate)
        {
            _lVistaObservers.Add((subject, observer));
        }
    }

    public void LVistaChosenAttach(LSubject subject, LObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        lock (_lVistaGate)
        {
            _lVistaChosenObservers.Add((subject, observer));
        }
    }

    public void LVistaObserverDetach(LObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        lock (_lVistaGate)
        {
            _lVistaObservers.RemoveAll(pair => ReferenceEquals(pair.Item2, observer));
            _lVistaChosenObservers.RemoveAll(pair => ReferenceEquals(pair.Item2, observer));
        }
    }

    public void LObserverBulletinHandle(LBulletin bulletin)
    {
        ArgumentNullException.ThrowIfNull(bulletin);

        if (bulletin.LBulletinSubject == LSubject.LSubjectVista && bulletin.LBulletinId != LVistaId)
        {
            return;
        }

        (LSubject, LObserver)[] observers;
        (LSubject, LObserver)[] chosenObservers;
        lock (_lVistaGate)
        {
            observers = [.. _lVistaObservers];
            chosenObservers = [.. _lVistaChosenObservers];
        }

        foreach ((LSubject subject, LObserver observer) in observers)
        {
            if (subject == bulletin.LBulletinSubject)
            {
                observer.LObserverBulletinHandle(bulletin);
            }
        }

        foreach ((LSubject subject, LObserver observer) in chosenObservers)
        {
            if (subject != bulletin.LBulletinSubject)
            {
                continue;
            }

            long? chosen;
            lock (_lVistaGate)
            {
                chosen = LVistaChosen;
            }

            if (bulletin.LBulletinId <= 0 || bulletin.LBulletinId == chosen)
            {
                observer.LObserverBulletinHandle(bulletin);
            }
        }
    }
}
