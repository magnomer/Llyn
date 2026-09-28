using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LAuthorFacade
{
    private const int LAuthorFacadeLimit = 8;

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

    public IReadOnlyList<LCatalogAuthor> LEngineRollFind(LVista? vista)
    {
        if (vista is null)
        {
            return [];
        }

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

        string uncredited = _lAuthorFacadeEngine.LEngineSettings.LEngineTextRead("Guild.Uncredited");
        return [new LCatalogAuthor(new LAuthor(0, uncredited), orphan.Count, cited, vista.LVistaMatch(0)), .. rows];
    }

    public IReadOnlyList<LCatalogAuthor> LEngineUnionFind(LVista? roll, string typed)
    {
        ArgumentNullException.ThrowIfNull(typed);

        if (string.IsNullOrWhiteSpace(typed) || roll?.LVistaStored is not long author)
        {
            return [];
        }

        return LEngineAuthorFind(typed, author, LAuthorFacadeLimit);
    }

    public (string LUnionDropped, string LUnionKept) LEngineUnionRead(LTenure? held, long kept)
    {
        held?.LTenurePersist();
        LDraft? draft = held?.LTenureRead();
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffAuthor.LAuthorUnionRead(draft, kept);
        }
    }

    public LVita LEngineVitaRead(LVista? roll)
    {
        long? author = roll?.LVistaStored;
        IReadOnlyList<LUsage> usages = author is long id
            ? _lAuthorFacadeEngine.LEngineEntry.LEngineUsageRead(id, LOwner.LOwnerAuthor)
            : [];
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffAuthor.LAuthorVitaRead(
                author, usages, _lAuthorFacadeEngine.LEngineSettings.LEngineTextRead);
        }
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

    public IReadOnlyList<LCatalogReference> LEngineOeuvreFind(LVista? roll, LVista? oeuvre)
    {
        if (roll is null || oeuvre is null)
        {
            return [];
        }

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

    public string LEngineWorkFormat(int count)
    {
        return LAuthorClerk.LAuthorWorkFormat(count);
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

    public LAuthor? LEngineAuthorRead(long id)
    {
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffAuthor.LAuthorClerkRead(id);
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
