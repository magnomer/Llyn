using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LExampleFacade : LExamplePort
{
    private readonly LEngineHearth _lExampleFacadeHearth;
    private readonly LDraftFacade _lExampleFacadeDraft;
    private readonly object _lExampleFacadeGate;

    internal LExampleFacade(LEngineHearth hearth, LDraftFacade draft)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        ArgumentNullException.ThrowIfNull(draft);
        _lExampleFacadeHearth = hearth;
        _lExampleFacadeDraft = draft;
        _lExampleFacadeGate = _lExampleFacadeHearth.LEngineGate;
    }

    private LEngineStaff LExampleFacadeStaff => _lExampleFacadeHearth.LEngineStaffHeld;

    internal LExample? LEngineExampleRead(long id)
    {
        lock (_lExampleFacadeGate)
        {
            return LExampleFacadeStaff.LEngineStaffCatalog.LCatalogStaffExample.LExampleClerkRead(id);
        }
    }

    public IReadOnlyList<LCatalogExample> LEngineExampleFind(string query, LCatalogOrder order)
    {
        lock (_lExampleFacadeGate)
        {
            return LExampleFacadeStaff.LEngineStaffCatalog.LCatalogStaffExample.LExampleClerkFind(query, order);
        }
    }

    public IReadOnlyList<LCatalogExample> LEngineExampleFind(LVista vista, string unknown = "", string unwritten = "")
    {
        ArgumentNullException.ThrowIfNull(vista);
        IReadOnlyList<LCatalogExample> found = LEngineExampleFind(vista.LVistaQuery, vista.LVistaOrder);
        List<LCatalogExample> rows = new(found.Count);
        string[] names = LEntryClerkTwin.LTwinRead(
            found,
            row => LEntryClerkTwin.LTwinNameRead(row.LCatalogExampleStored.LExampleText, unknown, unwritten),
            static _ => string.Empty,
            row => row.LCatalogExampleStored.LExampleId);
        for (int index = 0; index < found.Count; index++)
        {
            LCatalogExample row = found[index];
            rows.Add(row with
            {
                LCatalogExampleName = names[index],
                LCatalogExampleText =
                    LEntryClerkTwin.LTwinNameRead(row.LCatalogExampleStored.LExampleText, unknown, unwritten),
                LCatalogExampleChosen = row.LCatalogExampleStored.LExampleId == vista.LVistaChosen,
            });
        }

        return rows;
    }

    public bool LEngineTextMatch(string field, string shown)
    {
        return LExampleClerk.LExampleTextMatch(field, shown);
    }

    public (string, IReadOnlyList<LMentionPiece>, string) LEngineLineRead(
        LSentenceDraft sentence, LSentenceOrder order, string mark, IReadOnlyDictionary<long, string> citations)
    {
        return LExampleClerk.LExampleLineRead(sentence, order, mark, citations);
    }

    internal void LEngineExampleDelete(long id, bool detach)
    {
        lock (_lExampleFacadeGate)
        {
            LExampleFacadeStaff.LEngineStaffCatalog.LCatalogStaffExample.LExampleClerkDelete(id, detach);
        }

        _lExampleFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectExample, id);
    }

    internal LDraft LEngineExampleStart(string origin, long? exampleId)
    {
        lock (_lExampleFacadeGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);
            return LExampleFacadeStaff.LEngineStaffEntry.LEntryStaffCitation.LCitationClerkExample
                .LExampleCitationStart(origin, exampleId);
        }
    }

    internal LExample LEngineExampleCommit(long id)
    {
        LExample settled;
        lock (_lExampleFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            _lExampleFacadeDraft.LEngineDraftValidate(id);
            settled = LExampleFacadeStaff.LEngineStaffEntry.LEntryStaffCitation
                .LCitationClerkExample.LExampleCitationCommit(id);
        }

        _lExampleFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectExample, settled.LExampleId);
        return settled;
    }
}
