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
