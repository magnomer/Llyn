using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LSituation LEngineSituationCreate(LSituation situation)
    {
        ArgumentNullException.ThrowIfNull(situation);
        return new LSituationArchive(_lEngineDatabase).LSituationCreate(situation);
    }

    public IReadOnlyList<LSituation> LEngineSituationRead()
    {
        return new LSituationArchive(_lEngineDatabase).LSituationRead();
    }

    public IReadOnlyDictionary<string, int> LEngineUsageRead()
    {
        return new LSituationArchive(_lEngineDatabase).LSituationReferenceRead();
    }

    public IReadOnlyList<LUsage> LEngineUsageRead(string id)
    {
        return new LSituationArchive(_lEngineDatabase).LSituationUsageRead(id);
    }

    public LSituation? LEngineSituationRead(string id)
    {
        return new LSituationArchive(_lEngineDatabase).LSituationRead(id);
    }

    public IReadOnlyList<LSituation> LEngineSituationRead(string ownerId, LOwner owner)
    {
        LSituationArchive situations = new(_lEngineDatabase);
        return LEngineOwnerCheck(owner)
            ? situations.LSituationCollocationRead(ownerId)
            : situations.LSituationSenseRead(ownerId);
    }

    public void LEngineSituationUpdate(LSituation situation)
    {
        new LSituationArchive(_lEngineDatabase).LSituationUpdate(situation);
    }

    public void LEngineSituationUpdate(string situationId, LStateValue reference)
    {
        new LSituationArchive(_lEngineDatabase).LSituationSourceUpdate(situationId, reference);
    }

    public void LEngineSituationAttach(string ownerId, string situationId, int position, LOwner owner)
    {
        LSituationArchive situations = new(_lEngineDatabase);
        if (LEngineOwnerCheck(owner))
        {
            situations.LSituationCollocationAttach(ownerId, situationId, position);
            return;
        }

        situations.LSituationSenseAttach(ownerId, situationId, position);
    }

    public void LEngineSituationDetach(string ownerId, string situationId, LOwner owner)
    {
        LSituationArchive situations = new(_lEngineDatabase);
        if (LEngineOwnerCheck(owner))
        {
            situations.LSituationCollocationDetach(ownerId, situationId);
            return;
        }

        situations.LSituationSenseDetach(ownerId, situationId);
    }

    public void LEngineSituationRemove(string ownerId, string situationId, LOwner owner)
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

    public void LEngineSituationDelete(string id)
    {
        new LSituationArchive(_lEngineDatabase).LSituationDelete(id);
    }

    public void LEngineSituationDelete(string id, bool detach)
    {
        new LSituationArchive(_lEngineDatabase).LSituationDelete(id, detach);
    }
}
