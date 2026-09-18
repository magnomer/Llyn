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

    internal LVista(LEngine engine, long id, string tab, LCatalogOrder order, LCatalogFilter filter, bool blank)
    {
        _lEngine = engine;
        LVistaId = id;
        LVistaTab = tab;
        LVistaOrder = order;
        LVistaFilter = filter;
        LVistaBlank = blank;
    }

    public long LVistaId { get; }

    public string LVistaTab { get; }

    public bool LVistaBlank { get; }

    public LCatalogOrder LVistaOrder { get; private set; }

    public LCatalogFilter LVistaFilter { get; private set; }

    public string LVistaQuery { get; private set; } = string.Empty;

    public long? LVistaChosen { get; private set; }

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
