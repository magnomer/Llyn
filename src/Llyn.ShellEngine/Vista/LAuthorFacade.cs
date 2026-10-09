using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LAuthorFacade : LAuthorPort
{
    private const int LAuthorFacadeLimit = 8;

    private readonly LEngineHearth _lAuthorFacadeHearth;
    private readonly LDraftFacade _lAuthorFacadeDraft;
    private readonly LEntryFacade _lAuthorFacadeEntry;
    private readonly LReferenceFacade _lAuthorFacadeReference;
    private readonly LSettingsFacade _lAuthorFacadeSettings;
    private readonly object _lAuthorFacadeGate;

    internal LAuthorFacade(
        LEngineHearth hearth,
        LDraftFacade draft,
        LEntryFacade entry,
        LReferenceFacade reference,
        LSettingsFacade settings)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(reference);
        ArgumentNullException.ThrowIfNull(settings);
        _lAuthorFacadeHearth = hearth;
        _lAuthorFacadeDraft = draft;
        _lAuthorFacadeEntry = entry;
        _lAuthorFacadeReference = reference;
        _lAuthorFacadeSettings = settings;
        _lAuthorFacadeGate = _lAuthorFacadeHearth.LEngineGate;
    }

    private LEngineStaff LAuthorFacadeStaff => _lAuthorFacadeHearth.LEngineStaffHeld;

    public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(string query, LCatalogOrder order)
    {
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffCatalog.LCatalogStaffAuthor.LAuthorClerkFind(query, order);
        }
    }

    public LCatalogAuthor? LEngineAuthorFind(long id)
    {
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffCatalog.LCatalogStaffAuthor.LAuthorClerkFind(id);
        }
    }

    public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(string query, long except, int limit)
    {
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffCatalog.LCatalogStaffAuthor.LAuthorClerkFind(query, except, limit);
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

        string uncredited = _lAuthorFacadeSettings.LEngineTextRead("Guild.Uncredited");
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
            return LAuthorFacadeStaff.LEngineStaffCatalog.LCatalogStaffAuthor.LAuthorUnionRead(draft, kept);
        }
    }

    public LVita LEngineVitaRead(LVista? roll)
    {
        long? author = roll?.LVistaStored;
        IReadOnlyList<LUsage> usages = author is long id
            ? _lAuthorFacadeEntry.LEngineUsageRead(id, LOwner.LOwnerAuthor)
            : [];
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffCatalog.LCatalogStaffAuthor.LAuthorVitaRead(
                author, usages, _lAuthorFacadeSettings.LEngineTextRead);
        }
    }

    public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);
        IReadOnlyList<LCatalogAuthor> found = LEngineAuthorFind(vista.LVistaQuery, vista.LVistaOrder);
        List<LCatalogAuthor> rows = new(found.Count);
        string[] names = LEntryClerkTwin.LTwinRead(
            found, row => row.LCatalogAuthorName, static _ => string.Empty, row => row.LCatalogAuthorStored.LAuthorId);
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
        return _lAuthorFacadeReference.LEngineReferenceRead(found, oeuvre.LVistaChosen);
    }

    public IReadOnlyList<LCatalogReference> LEngineOeuvreFind(
        long? author,
        string query,
        LCatalogFilter kind,
        LCatalogOrder order)
    {
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffCatalog.LCatalogStaffReference
                .LReferenceOeuvreFind(author, query, kind, order);
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
            LAuthorFacadeStaff.LEngineStaffCatalog.LCatalogStaffAuthor.LAuthorClerkAbsorb(kept, dropped);
        }

        _lAuthorFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectAuthor, dropped);
        _lAuthorFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectAuthor, kept);
    }

    public LAuthor? LEngineAuthorRead(long id)
    {
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffCatalog.LCatalogStaffAuthor.LAuthorClerkRead(id);
        }
    }

    public IReadOnlyList<LBylineRow> LEngineBylineFind(long draft, string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (draft == 0 || string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        IReadOnlyList<LAuthor> credited;
        try
        {
            credited = _lAuthorFacadeDraft.LEngineDraftRead(draft)?.LDraftAuthor ?? [];
        }
        catch (Exception exception) when (LWorkspaceClerk.LWorkspaceStaleCheck(exception))
        {
            return [];
        }

        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffCatalog.LCatalogStaffAuthor
                .LBylineFind(credited, query, LAuthorFacadeLimit);
        }
    }

    public static string LEngineBylineRead(string? text)
    {
        return LAuthorClerk.LBylineRead(text);
    }

    public IReadOnlyList<LAuthorRow> LEngineCreditRead(LTenure? held)
    {
        held?.LTenurePersist();
        return LAuthorClerk.LAuthorCreditRead(held?.LTenureRead());
    }

    public IReadOnlyList<LAuthor> LEngineAuthorRead(long ownerId, LOwner owner)
    {
        lock (_lAuthorFacadeGate)
        {
            return LAuthorFacadeStaff.LEngineStaffCatalog.LCatalogStaffAuthor.LAuthorClerkRead(ownerId, owner);
        }
    }

    internal void LEngineAuthorDelete(long id, bool detach)
    {
        lock (_lAuthorFacadeGate)
        {
            LAuthorFacadeStaff.LEngineStaffCatalog.LCatalogStaffAuthor.LAuthorClerkDelete(id, detach);
        }

        _lAuthorFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectAuthor, id);
    }

    internal LDraft LEngineAuthorStart(string origin, long? authorId)
    {
        lock (_lAuthorFacadeGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);
            return LAuthorFacadeStaff.LEngineStaffEntry.LEntryStaffCitation
                .LCitationClerkAuthor.LAuthorCitationStart(origin, authorId);
        }
    }

    internal LAuthor LEngineAuthorCommit(long id)
    {
        LAuthor settled;
        lock (_lAuthorFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            _lAuthorFacadeDraft.LEngineDraftValidate(id);
            settled = LAuthorFacadeStaff.LEngineStaffEntry.LEntryStaffCitation
                .LCitationClerkAuthor.LAuthorCitationCommit(id);
        }

        _lAuthorFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectAuthor, settled.LAuthorId);
        return settled;
    }
}
