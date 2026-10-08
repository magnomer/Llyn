using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LSituationFacade : LSituationPort
{
    private readonly LEngine _lSituationFacadeEngine;
    private readonly object _lSituationFacadeGate;

    public LSituationFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lSituationFacadeEngine = engine;
        _lSituationFacadeGate = engine.LEngineGate;
    }

    public IReadOnlyList<LCatalogSituation> LEngineSituationFind(string query, LCatalogOrder order)
    {
        lock (_lSituationFacadeGate)
        {
            return LSituationFacadeStaff.LEngineStaffCatalog.LCatalogStaffSituation.LSituationClerkFind(query, order);
        }
    }

    public LSituationOffer LEngineSituationFind(LTenure held, long card, string text)
    {
        ArgumentNullException.ThrowIfNull(held);

        LDraft? draft = held.LTenureRead();
        lock (_lSituationFacadeGate)
        {
            return LSituationFacadeStaff.LEngineStaffCatalog.LCatalogStaffSituation
                .LSituationClerkFind(text, draft, card);
        }
    }

    public IReadOnlyList<LCatalogSituation> LEngineSituationFind(
        LVista vista, string unknown = "", string untitled = "")
    {
        ArgumentNullException.ThrowIfNull(vista);
        IReadOnlyList<LCatalogSituation> found = LEngineSituationFind(vista.LVistaQuery, vista.LVistaOrder);
        List<LCatalogSituation> rows = new(found.Count);
        string[] names = LVistaFacade.LEngineTwinRead(
            found,
            row => LVistaFacade.LEngineNameRead(row.LCatalogSituationStored.LSituationTitle, unknown, untitled),
            row => row.LCatalogSituationStored.LSituationId);
        for (int index = 0; index < found.Count; index++)
        {
            LCatalogSituation row = found[index];
            rows.Add(row with
            {
                LCatalogSituationName = names[index],
                LCatalogSituationKind = LVistaFacade.LEngineNameRead(
                    row.LCatalogSituationStored.LSituationKind, unknown, string.Empty),
                LCatalogSituationChosen = row.LCatalogSituationStored.LSituationId == vista.LVistaChosen,
            });
        }

        return rows;
    }

    internal LSituation? LEngineSituationRead(long id)
    {
        lock (_lSituationFacadeGate)
        {
            return LSituationFacadeStaff.LEngineStaffCatalog.LCatalogStaffSituation.LSituationClerkRead(id);
        }
    }

    internal void LEngineSituationDelete(long id, bool detach)
    {
        lock (_lSituationFacadeGate)
        {
            LSituationFacadeStaff.LEngineStaffCatalog.LCatalogStaffSituation.LSituationClerkDelete(id, detach);
        }

        _lSituationFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectSituation, id);
    }

    internal LDraft LEngineSituationStart(string origin, long? situationId)
    {
        lock (_lSituationFacadeGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);
            return LSituationFacadeStaff.LEngineStaffEntry.LEntryStaffCitation.LCitationClerkSituation
                .LSituationCitationStart(origin, situationId);
        }
    }

    internal LSituation LEngineSituationCommit(long id)
    {
        LSituation settled;
        lock (_lSituationFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            _lSituationFacadeEngine.LEngineDraft.LEngineDraftValidate(id);
            settled = LSituationFacadeStaff.LEngineStaffEntry.LEntryStaffCitation
                .LCitationClerkSituation.LSituationCitationCommit(id);
        }

        _lSituationFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectSituation, settled.LSituationId);
        return settled;
    }
    private LEngineStaff LSituationFacadeStaff => _lSituationFacadeEngine.LEngineStaffHeld;
}
