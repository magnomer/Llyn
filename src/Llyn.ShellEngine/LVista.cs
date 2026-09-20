using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LVista
{
    private readonly LEngine _lEngine;

    private readonly object _lVistaGate = new();

    private readonly List<(LSubject, Action<LBulletin>)> _lVistaObservers = [];

    private readonly List<(LSubject, Action<LBulletin>)> _lVistaChosenObservers = [];

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

    public long? LVistaStored => LVistaChosen is > 0 and long id ? id : null;

    public bool LVistaMatch(long id)
    {
        return LVistaChosen == id;
    }

    public bool LVistaOrderMatch(LCatalogOrder order)
    {
        return LVistaOrder == order;
    }

    public bool LVistaFiltered => LVistaFilter.LCatalogFilterActive;

    public bool LVistaQueried => LVistaQuery.Trim().Length > 0;

    public bool LVistaLeft => string.Equals(LVistaTab, "left", StringComparison.Ordinal);

    public bool LVistaInput => string.Equals(LVistaTab, "input", StringComparison.Ordinal);

    public LDraft? LVistaLoad()
    {
        LDraft? draft = _lEngine.LEngineVistaLoad(this);
        if (draft is null && LVistaStored is not null)
        {
            LVistaSelect(null);
        }

        return draft;
    }

    public static string LVistaFileRead(LVista? vista)
    {
        string headword;
        try
        {
            headword = vista?.LVistaLoad()?.LDraftContent.LEntryDraftHeadword ?? string.Empty;
        }
        catch (Exception)
        {
            headword = string.Empty;
        }

        string trimmed = headword.Trim();
        return trimmed.Length == 0 || vista is null
            ? "entry"
            : vista._lEngine.LEngineTrailRead().LTrailNameNormalize(trimmed);
    }

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
        LVistaEditingSaved?.Invoke(editing);
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

    public void LVistaToggle(long id)
    {
        LVistaSelect(LVistaMatch(id) ? null : id);
    }

    public void LVistaObserverAttach(LSubject subject, Action<LBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        lock (_lVistaGate)
        {
            _lVistaObservers.Add((subject, observer));
        }
    }

    public void LVistaChosenAttach(LSubject subject, Action<LBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        lock (_lVistaGate)
        {
            _lVistaChosenObservers.Add((subject, observer));
        }
    }

    public void LVistaObserverDetach(Action<LBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        lock (_lVistaGate)
        {
            _lVistaObservers.RemoveAll(pair => pair.Item2 == observer);
            _lVistaChosenObservers.RemoveAll(pair => pair.Item2 == observer);
        }
    }

    internal void LVistaBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject == LSubject.LSubjectVista && bulletin.LBulletinId != LVistaId)
        {
            return;
        }

        (LSubject, Action<LBulletin>)[] observers;
        (LSubject, Action<LBulletin>)[] chosenObservers;
        lock (_lVistaGate)
        {
            observers = [.. _lVistaObservers];
            chosenObservers = [.. _lVistaChosenObservers];
        }

        foreach ((LSubject subject, Action<LBulletin> observer) in observers)
        {
            if (subject == bulletin.LBulletinSubject)
            {
                observer(bulletin);
            }
        }

        foreach ((LSubject subject, Action<LBulletin> observer) in chosenObservers)
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
                observer(bulletin);
            }
        }
    }
}
