using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LSituation LEngineSituationCreate(LSituation situation)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffSituation.LSituationClerkCreate(situation);
        }
    }

    internal IReadOnlyList<LSituation> LEngineSituationRead()
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffSituation.LSituationClerkRead();
        }
    }

    public IReadOnlyList<LCatalogSituation> LEngineSituationFind(string query, LCatalogOrder order)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffSituation.LSituationClerkFind(query, order);
        }
    }

    public IReadOnlyList<LCatalogSituation> LEngineSituationFind(
        LVista vista, string unknown = "", string untitled = "")
    {
        ArgumentNullException.ThrowIfNull(vista);
        IReadOnlyList<LCatalogSituation> found = LEngineSituationFind(vista.LVistaQuery, vista.LVistaOrder);
        List<LCatalogSituation> rows = new(found.Count);
        string[] names = LEngineTwinRead(
            found,
            row => LEngineNameRead(row.LCatalogSituationStored.LSituationTitle, unknown, untitled),
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
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffSituation.LSituationClerkRead(id);
        }
    }

    internal IReadOnlyList<LSituation> LEngineSituationRead(long ownerId, LOwner owner)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffSituation.LSituationClerkRead(ownerId, owner);
        }
    }

    internal void LEngineSituationUpdate(LSituation situation)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffSituation.LSituationClerkUpdate(situation);
        }
    }

    internal void LEngineSituationAttach(long ownerId, long situationId, int position, LOwner owner)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffSituation.LSituationClerkAttach(ownerId, situationId, position, owner);
        }
    }

    internal void LEngineSituationDetach(long ownerId, long situationId, LOwner owner)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffSituation.LSituationClerkDetach(ownerId, situationId, owner);
        }
    }

    internal void LEngineSituationRemove(long ownerId, long situationId, LOwner owner)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffCard.LSituationRemove(ownerId, situationId, owner);
        }
    }

    internal void LEngineSituationDelete(long id)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffSituation.LSituationClerkDelete(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectSituation, id);
    }

    internal void LEngineSituationDelete(long id, bool detach)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffSituation.LSituationClerkDelete(id, detach);
        }

        LEngineBulletinRaise(LSubject.LSubjectSituation, id);
    }

    internal LDraft LEngineSituationStart(string origin, long? situationId)
    {
        lock (LEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);
            return _lEngineStaff.LEngineStaffCitation.LSituationStart(origin, situationId);
        }
    }

    internal LSituation LEngineSituationCommit(long id)
    {
        LSituation settled;
        lock (LEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            settled = _lEngineStaff.LEngineStaffCitation.LSituationCommit(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectSituation, settled.LSituationId);
        return settled;
    }
}
