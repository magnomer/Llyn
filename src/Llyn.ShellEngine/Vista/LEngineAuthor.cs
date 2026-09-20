using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            return _lEngineAuthorClerk.LAuthorClerkFind(query, order);
        }
    }

    public LCatalogAuthor? LEngineAuthorFind(long id)
    {
        lock (_lEngineGate)
        {
            return _lEngineAuthorClerk.LAuthorClerkFind(id);
        }
    }

    public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(string query, long except, int limit)
    {
        lock (_lEngineGate)
        {
            return _lEngineAuthorClerk.LAuthorClerkFind(query, except, limit);
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
        lock (_lEngineGate)
        {
            return _lEngineReferenceClerk.LReferenceOeuvreFind(author, query, kind, order);
        }
    }

    public IReadOnlyList<LFellow> LEngineFellowFind(long authorId)
    {
        lock (_lEngineGate)
        {
            return _lEngineAuthorClerk.LFellowFind(authorId);
        }
    }

    public void LEngineAuthorAbsorb(long kept, long dropped)
    {
        lock (_lEngineGate)
        {
            _lEngineAuthorClerk.LAuthorClerkAbsorb(kept, dropped);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, dropped);
        LEngineBulletinRaise(LSubject.LSubjectAuthor, kept);
    }

    internal LAuthor LEngineAuthorCreate(LAuthor author)
    {
        LAuthor created;
        lock (_lEngineGate)
        {
            created = _lEngineAuthorClerk.LAuthorClerkCreate(author);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, created.LAuthorId);
        return created;
    }

    public LAuthor? LEngineAuthorRead(long id)
    {
        lock (_lEngineGate)
        {
            return _lEngineAuthorClerk.LAuthorClerkRead(id);
        }
    }

    public IReadOnlyList<LAuthor> LEngineAuthorRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineAuthorClerk.LAuthorClerkRead();
        }
    }

    public IReadOnlyList<LAuthor> LEngineAuthorFind(string query)
    {
        lock (_lEngineGate)
        {
            return _lEngineAuthorClerk.LAuthorClerkFind(query);
        }
    }

    public IReadOnlyList<LAuthor> LEngineBylineFind(long draft, string query, int limit)
    {
        if (draft == 0)
        {
            return [];
        }

        IReadOnlyList<LAuthor> credited = LEngineDraftRead(draft)?.LDraftAuthor ?? [];
        lock (_lEngineGate)
        {
            return _lEngineAuthorClerk.LBylineFind(credited, query, limit);
        }
    }

    public IReadOnlyList<LAuthor> LEngineAuthorRead(long ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            return _lEngineAuthorClerk.LAuthorClerkRead(ownerId, owner);
        }
    }

    public IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> LEngineAuthorRead(LOwner owner)
    {
        lock (_lEngineGate)
        {
            return _lEngineAuthorClerk.LAuthorClerkRead(owner);
        }
    }

    internal void LEngineAuthorUpdate(LAuthor author)
    {
        lock (_lEngineGate)
        {
            _lEngineAuthorClerk.LAuthorClerkUpdate(author);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, author.LAuthorId);
    }

    internal void LEngineAuthorAttach(long referenceId, long authorId, int position)
    {
        lock (_lEngineGate)
        {
            _lEngineAuthorClerk.LAuthorClerkAttach(referenceId, authorId, position);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, authorId);
    }

    internal void LEngineAuthorDetach(long referenceId, long authorId)
    {
        lock (_lEngineGate)
        {
            _lEngineAuthorClerk.LAuthorClerkDetach(referenceId, authorId);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, authorId);
    }

    internal void LEngineAuthorDelete(long id)
    {
        lock (_lEngineGate)
        {
            _lEngineAuthorClerk.LAuthorClerkDelete(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, id);
    }

    internal void LEngineAuthorDelete(long id, bool detach)
    {
        lock (_lEngineGate)
        {
            _lEngineAuthorClerk.LAuthorClerkDelete(id, detach);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, id);
    }

    internal LDraft LEngineAuthorStart(string origin, long? authorId)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);
            return _lEngineCitationClerk.LAuthorStart(origin, authorId);
        }
    }

    internal LAuthor LEngineAuthorCommit(long id)
    {
        LAuthor settled;
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            settled = _lEngineCitationClerk.LAuthorCommit(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, settled.LAuthorId);
        return settled;
    }
}
