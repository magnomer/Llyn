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
}
