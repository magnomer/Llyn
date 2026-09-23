using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LAuthorFacade
{
    private readonly LEngine _lAuthorFacadeEngine;
    private readonly object _lAuthorFacadeGate;

    public LAuthorFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lAuthorFacadeEngine = engine;
        _lAuthorFacadeGate = engine.LEngineGate;
    }

    private LEngineStaff LAuthorFacadeStaff => _lAuthorFacadeEngine.LEngineStaffHeld;

    public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(string query, LCatalogOrder order)
    {
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffAuthor.LAuthorClerkFind(query, order);
        }
    }

    public LCatalogAuthor? LEngineAuthorFind(long id)
    {
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffAuthor.LAuthorClerkFind(id);
        }
    }

    public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(string query, long except, int limit)
    {
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffAuthor.LAuthorClerkFind(query, except, limit);
        }
    }

    public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(LVista vista, string uncredited)
    {
        ArgumentNullException.ThrowIfNull(vista);
        ArgumentNullException.ThrowIfNull(uncredited);

        IReadOnlyList<LCatalogAuthor> rows = LEngineAuthorFind(vista);
        if (vista.LVistaQueried)
        {
            return rows;
        }

        IReadOnlyList<LCatalogReference> orphan = LEngineOeuvreFind(
            0, string.Empty, LCatalogFilter.LCatalogFilterEmpty, LCatalogOrder.LCatalogOrderName);
        if (orphan.Count == 0)
        {
            return rows;
        }

        int cited = 0;
        foreach (LCatalogReference row in orphan)
        {
            cited += row.LCatalogReferenceUsage;
        }

        return [new LCatalogAuthor(new LAuthor(0, uncredited), orphan.Count, cited, vista.LVistaMatch(0)), .. rows];
    }

    public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);
        IReadOnlyList<LCatalogAuthor> found = LEngineAuthorFind(vista.LVistaQuery, vista.LVistaOrder);
        List<LCatalogAuthor> rows = new(found.Count);
        string[] names = LVistaFacade.LEngineTwinRead(
            found, row => row.LCatalogAuthorName, row => row.LCatalogAuthorStored.LAuthorId);
        for (int index = 0; index < found.Count; index++)
        {
            LCatalogAuthor row = found[index];
            rows.Add(row with
            {
                LCatalogAuthorName = names[index],
                LCatalogAuthorChosen = row.LCatalogAuthorStored.LAuthorId == vista.LVistaChosen,
            });
        }

        return rows;
    }

    public IReadOnlyList<LCatalogReference> LEngineOeuvreFind(LVista roll, LVista oeuvre)
    {
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(oeuvre);
        IReadOnlyList<LCatalogReference> found = LEngineOeuvreFind(
            roll.LVistaChosen, oeuvre.LVistaQuery, roll.LVistaFilter, oeuvre.LVistaOrder);
        return _lAuthorFacadeEngine.LEngineReference.LEngineReferenceRead(found, oeuvre.LVistaChosen);
    }

    public IReadOnlyList<LCatalogReference> LEngineOeuvreFind(
        long? author,
        string query,
        LCatalogFilter kind,
        LCatalogOrder order)
    {
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffReference.LReferenceOeuvreFind(author, query, kind, order);
        }
    }

    public IReadOnlyList<LFellow> LEngineFellowFind(long authorId)
    {
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffAuthor.LFellowFind(authorId);
        }
    }

    public void LEngineAuthorAbsorb(long kept, long dropped)
    {
        lock (_lAuthorFacadeGate)
        {
            LAuthorFacadeStaff.LEngineStaffAuthor.LAuthorClerkAbsorb(kept, dropped);
        }

        _lAuthorFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectAuthor, dropped);
        _lAuthorFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectAuthor, kept);
    }

    internal LAuthor LEngineAuthorCreate(LAuthor author)
    {
        LAuthor created;
        lock (_lAuthorFacadeGate)
        {
            created = LAuthorFacadeStaff.LEngineStaffAuthor.LAuthorClerkCreate(author);
        }

        _lAuthorFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectAuthor, created.LAuthorId);
        return created;
    }

    public LAuthor? LEngineAuthorRead(long id)
    {
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffAuthor.LAuthorClerkRead(id);
        }
    }

    public IReadOnlyList<LAuthor> LEngineAuthorRead()
    {
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffAuthor.LAuthorClerkRead();
        }
    }

    public IReadOnlyList<LAuthor> LEngineAuthorFind(string query)
    {
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffAuthor.LAuthorClerkFind(query);
        }
    }

    public IReadOnlyList<LAuthor> LEngineBylineFind(long draft, string query, int limit)
    {
        if (draft == 0)
        {
            return [];
        }

        IReadOnlyList<LAuthor> credited = _lAuthorFacadeEngine.LEngineDraft.LEngineDraftRead(draft)?.LDraftAuthor ?? [];
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffAuthor.LBylineFind(credited, query, limit);
        }
    }

    public IReadOnlyList<LAuthor> LEngineAuthorRead(long ownerId, LOwner owner)
    {
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffAuthor.LAuthorClerkRead(ownerId, owner);
        }
    }

    public IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> LEngineAuthorRead(LOwner owner)
    {
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffAuthor.LAuthorClerkRead(owner);
        }
    }

    internal void LEngineAuthorUpdate(LAuthor author)
    {
        lock (_lAuthorFacadeGate)
        {
            LAuthorFacadeStaff.LEngineStaffAuthor.LAuthorClerkUpdate(author);
        }

        _lAuthorFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectAuthor, author.LAuthorId);
    }

    internal void LEngineAuthorAttach(long referenceId, long authorId, int position)
    {
        lock (_lAuthorFacadeGate)
        {
            LAuthorFacadeStaff.LEngineStaffAuthor.LAuthorClerkAttach(referenceId, authorId, position);
        }

        _lAuthorFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectAuthor, authorId);
    }

    internal void LEngineAuthorDetach(long referenceId, long authorId)
    {
        lock (_lAuthorFacadeGate)
        {
            LAuthorFacadeStaff.LEngineStaffAuthor.LAuthorClerkDetach(referenceId, authorId);
        }

        _lAuthorFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectAuthor, authorId);
    }

    internal void LEngineAuthorDelete(long id)
    {
        lock (_lAuthorFacadeGate)
        {
            LAuthorFacadeStaff.LEngineStaffAuthor.LAuthorClerkDelete(id);
        }

        _lAuthorFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectAuthor, id);
    }

    internal void LEngineAuthorDelete(long id, bool detach)
    {
        lock (_lAuthorFacadeGate)
        {
            LAuthorFacadeStaff.LEngineStaffAuthor.LAuthorClerkDelete(id, detach);
        }

        _lAuthorFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectAuthor, id);
    }

    internal LDraft LEngineAuthorStart(string origin, long? authorId)
    {
        lock (_lAuthorFacadeGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);
            return LAuthorFacadeStaff.LEngineStaffCitation.LAuthorStart(origin, authorId);
        }
    }

    internal LAuthor LEngineAuthorCommit(long id)
    {
        LAuthor settled;
        lock (_lAuthorFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            _lAuthorFacadeEngine.LEngineDraft.LEngineDraftValidate(id);
            settled = LAuthorFacadeStaff.LEngineStaffCitation.LAuthorCommit(id);
        }

        _lAuthorFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectAuthor, settled.LAuthorId);
        return settled;
    }
}
