using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(string query, LCatalogOrder order)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffAuthor.LAuthorClerkFind(query, order);
        }
    }

    public LCatalogAuthor? LEngineAuthorFind(long id)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffAuthor.LAuthorClerkFind(id);
        }
    }

    public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(string query, long except, int limit)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffAuthor.LAuthorClerkFind(query, except, limit);
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
        string[] names = LEngineTwinRead(
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
        return LEngineReferenceRead(found, oeuvre.LVistaChosen);
    }

    public IReadOnlyList<LCatalogReference> LEngineOeuvreFind(
        long? author,
        string query,
        LCatalogFilter kind,
        LCatalogOrder order)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffReference.LReferenceOeuvreFind(author, query, kind, order);
        }
    }

    public IReadOnlyList<LFellow> LEngineFellowFind(long authorId)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffAuthor.LFellowFind(authorId);
        }
    }

    public void LEngineAuthorAbsorb(long kept, long dropped)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffAuthor.LAuthorClerkAbsorb(kept, dropped);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, dropped);
        LEngineBulletinRaise(LSubject.LSubjectAuthor, kept);
    }

    internal LAuthor LEngineAuthorCreate(LAuthor author)
    {
        LAuthor created;
        lock (LEngineGate)
        {
            created = _lEngineStaff.LEngineStaffAuthor.LAuthorClerkCreate(author);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, created.LAuthorId);
        return created;
    }

    public LAuthor? LEngineAuthorRead(long id)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffAuthor.LAuthorClerkRead(id);
        }
    }

    public IReadOnlyList<LAuthor> LEngineAuthorRead()
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffAuthor.LAuthorClerkRead();
        }
    }

    public IReadOnlyList<LAuthor> LEngineAuthorFind(string query)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffAuthor.LAuthorClerkFind(query);
        }
    }

    public IReadOnlyList<LAuthor> LEngineBylineFind(long draft, string query, int limit)
    {
        if (draft == 0)
        {
            return [];
        }

        IReadOnlyList<LAuthor> credited = LEngineDraftRead(draft)?.LDraftAuthor ?? [];
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffAuthor.LBylineFind(credited, query, limit);
        }
    }

    public IReadOnlyList<LAuthor> LEngineAuthorRead(long ownerId, LOwner owner)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffAuthor.LAuthorClerkRead(ownerId, owner);
        }
    }

    public IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> LEngineAuthorRead(LOwner owner)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffAuthor.LAuthorClerkRead(owner);
        }
    }

    internal void LEngineAuthorUpdate(LAuthor author)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffAuthor.LAuthorClerkUpdate(author);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, author.LAuthorId);
    }

    internal void LEngineAuthorAttach(long referenceId, long authorId, int position)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffAuthor.LAuthorClerkAttach(referenceId, authorId, position);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, authorId);
    }

    internal void LEngineAuthorDetach(long referenceId, long authorId)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffAuthor.LAuthorClerkDetach(referenceId, authorId);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, authorId);
    }

    internal void LEngineAuthorDelete(long id)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffAuthor.LAuthorClerkDelete(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, id);
    }

    internal void LEngineAuthorDelete(long id, bool detach)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffAuthor.LAuthorClerkDelete(id, detach);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, id);
    }

    internal LDraft LEngineAuthorStart(string origin, long? authorId)
    {
        lock (LEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);
            return _lEngineStaff.LEngineStaffCitation.LAuthorStart(origin, authorId);
        }
    }

    internal LAuthor LEngineAuthorCommit(long id)
    {
        LAuthor settled;
        lock (LEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            settled = _lEngineStaff.LEngineStaffCitation.LAuthorCommit(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, settled.LAuthorId);
        return settled;
    }
}
