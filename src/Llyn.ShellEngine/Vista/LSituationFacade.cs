using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LSituationFacade
{
    private readonly LEngine _lSituationFacadeEngine;
    private readonly object _lSituationFacadeGate;

    public LSituationFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lSituationFacadeEngine = engine;
        _lSituationFacadeGate = engine.LEngineGate;
    }

    internal LSituation LEngineSituationCreate(LSituation situation)
    {
        lock (_lSituationFacadeGate)
        {
            return LSituationFacadeStaff.LEngineStaffSituation.LSituationClerkCreate(situation);
        }
    }

    internal IReadOnlyList<LSituation> LEngineSituationRead()
    {
        lock (_lSituationFacadeGate)
        {
            return LSituationFacadeStaff.LEngineStaffSituation.LSituationClerkRead();
        }
    }

    public IReadOnlyList<LCatalogSituation> LEngineSituationFind(string query, LCatalogOrder order)
    {
        lock (_lSituationFacadeGate)
        {
            return LSituationFacadeStaff.LEngineStaffSituation.LSituationClerkFind(query, order);
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
                LCatalogSituationChosen = row.LCatalogSituationStored.LSituationId == vista.LVistaChosen,
            });
        }

        return rows;
    }

    internal LSituation? LEngineSituationRead(long id)
    {
        lock (_lSituationFacadeGate)
        {
            return LSituationFacadeStaff.LEngineStaffSituation.LSituationClerkRead(id);
        }
    }

    internal IReadOnlyList<LSituation> LEngineSituationRead(long ownerId, LOwner owner)
    {
        lock (_lSituationFacadeGate)
        {
            return LSituationFacadeStaff.LEngineStaffSituation.LSituationClerkRead(ownerId, owner);
        }
    }

    internal void LEngineSituationUpdate(LSituation situation)
    {
        lock (_lSituationFacadeGate)
        {
            LSituationFacadeStaff.LEngineStaffSituation.LSituationClerkUpdate(situation);
        }
    }

    internal void LEngineSituationAttach(long ownerId, long situationId, int position, LOwner owner)
    {
        lock (_lSituationFacadeGate)
        {
            LSituationFacadeStaff.LEngineStaffSituation.LSituationClerkAttach(ownerId, situationId, position, owner);
        }
    }

    internal void LEngineSituationDetach(long ownerId, long situationId, LOwner owner)
    {
        lock (_lSituationFacadeGate)
        {
            LSituationFacadeStaff.LEngineStaffSituation.LSituationClerkDetach(ownerId, situationId, owner);
        }
    }

    internal void LEngineSituationRemove(long ownerId, long situationId, LOwner owner)
    {
        lock (_lSituationFacadeGate)
        {
            LSituationFacadeStaff.LEngineStaffCard.LSituationRemove(ownerId, situationId, owner);
        }
    }

    internal void LEngineSituationDelete(long id)
    {
        lock (_lSituationFacadeGate)
        {
            LSituationFacadeStaff.LEngineStaffSituation.LSituationClerkDelete(id);
        }

        _lSituationFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectSituation, id);
    }

    internal void LEngineSituationDelete(long id, bool detach)
    {
        lock (_lSituationFacadeGate)
        {
            LSituationFacadeStaff.LEngineStaffSituation.LSituationClerkDelete(id, detach);
        }

        _lSituationFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectSituation, id);
    }

    internal LDraft LEngineSituationStart(string origin, long? situationId)
    {
        lock (_lSituationFacadeGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);
            return LSituationFacadeStaff.LEngineStaffCitation.LSituationStart(origin, situationId);
        }
    }

    internal LSituation LEngineSituationCommit(long id)
    {
        LSituation settled;
        lock (_lSituationFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            _lSituationFacadeEngine.LEngineDraft.LEngineDraftValidate(id);
            settled = LSituationFacadeStaff.LEngineStaffCitation.LSituationCommit(id);
        }

        _lSituationFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectSituation, settled.LSituationId);
        return settled;
    }
    private LEngineStaff LSituationFacadeStaff => _lSituationFacadeEngine.LEngineStaffHeld;
}
