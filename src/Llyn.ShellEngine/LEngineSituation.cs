using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LSituation LEngineSituationCreate(LSituation situation)
    {
        lock (_lEngineGate)
        {
            return _lEngineSituationClerk.LSituationClerkCreate(situation);
        }
    }

    internal IReadOnlyList<LSituation> LEngineSituationRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineSituationClerk.LSituationClerkRead();
        }
    }

    public IReadOnlyList<LCatalogSituation> LEngineSituationFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            return _lEngineSituationClerk.LSituationClerkFind(query, order);
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
        lock (_lEngineGate)
        {
            return _lEngineSituationClerk.LSituationClerkRead(id);
        }
    }

    internal IReadOnlyList<LSituation> LEngineSituationRead(long ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            return _lEngineSituationClerk.LSituationClerkRead(ownerId, owner);
        }
    }

    internal void LEngineSituationUpdate(LSituation situation)
    {
        lock (_lEngineGate)
        {
            _lEngineSituationClerk.LSituationClerkUpdate(situation);
        }
    }

    internal void LEngineSituationAttach(long ownerId, long situationId, int position, LOwner owner)
    {
        lock (_lEngineGate)
        {
            _lEngineSituationClerk.LSituationClerkAttach(ownerId, situationId, position, owner);
        }
    }

    internal void LEngineSituationDetach(long ownerId, long situationId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            _lEngineSituationClerk.LSituationClerkDetach(ownerId, situationId, owner);
        }
    }

    internal void LEngineSituationRemove(long ownerId, long situationId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            _lEngineCardClerk.LSituationRemove(ownerId, situationId, owner);
        }
    }

    internal void LEngineSituationDelete(long id)
    {
        lock (_lEngineGate)
        {
            _lEngineSituationClerk.LSituationClerkDelete(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectSituation, id);
    }

    internal void LEngineSituationDelete(long id, bool detach)
    {
        lock (_lEngineGate)
        {
            _lEngineSituationClerk.LSituationClerkDelete(id, detach);
        }

        LEngineBulletinRaise(LSubject.LSubjectSituation, id);
    }

    internal LDraft LEngineSituationStart(string origin, long? situationId)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);
            return _lEngineCitationClerk.LSituationStart(origin, situationId);
        }
    }

    internal LSituation LEngineSituationCommit(long id)
    {
        LSituation settled;
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            settled = _lEngineCitationClerk.LSituationCommit(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectSituation, settled.LSituationId);
        return settled;
    }
}
