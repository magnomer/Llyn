using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LSituation LEngineSituationCreate(LSituation situation)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(situation);
            return new LSituationArchive(_lEngineDatabase).LSituationCreate(situation);
        }
    }

    public IReadOnlyList<LSituation> LEngineSituationRead()
    {
        lock (_lEngineGate)
        {
            return new LSituationArchive(_lEngineDatabase).LSituationRead();
        }
    }

    public LSituation? LEngineSituationRead(string id)
    {
        lock (_lEngineGate)
        {
            return new LSituationArchive(_lEngineDatabase).LSituationRead(id);
        }
    }

    public IReadOnlyList<LSituation> LEngineSituationRead(string ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            LSituationArchive situations = new(_lEngineDatabase);
            return LEngineOwnerCheck(owner)
                ? situations.LSituationCollocationRead(ownerId)
                : situations.LSituationMeaningRead(ownerId);
        }
    }

    public void LEngineSituationUpdate(LSituation situation)
    {
        lock (_lEngineGate)
        {
            new LSituationArchive(_lEngineDatabase).LSituationUpdate(situation);
        }
    }

    public void LEngineSituationAttach(string ownerId, string situationId, int position, LOwner owner)
    {
        lock (_lEngineGate)
        {
            LSituationArchive situations = new(_lEngineDatabase);
            if (LEngineOwnerCheck(owner))
            {
                situations.LSituationCollocationAttach(ownerId, situationId, position);
                return;
            }

            situations.LSituationMeaningAttach(ownerId, situationId, position);
        }
    }

    public void LEngineSituationDetach(string ownerId, string situationId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            LSituationArchive situations = new(_lEngineDatabase);
            if (LEngineOwnerCheck(owner))
            {
                situations.LSituationCollocationDetach(ownerId, situationId);
                return;
            }

            situations.LSituationMeaningDetach(ownerId, situationId);
        }
    }

    public void LEngineSituationRemove(string ownerId, string situationId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(situationId);

            using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

            LEngineSituationDetach(ownerId, situationId, owner);

            LSituationArchive situations = new(_lEngineDatabase);
            if (situations.LSituationReferenceRead(situationId) == 0)
            {
                situations.LSituationDelete(situationId);
            }

            session.LDatabaseSessionCommit();
        }
    }

    public void LEngineSituationDelete(string id)
    {
        lock (_lEngineGate)
        {
            new LSituationArchive(_lEngineDatabase).LSituationDelete(id);
        }
    }

    public void LEngineSituationDelete(string id, bool detach)
    {
        lock (_lEngineGate)
        {
            new LSituationArchive(_lEngineDatabase).LSituationDelete(id, detach);
        }
    }
}
