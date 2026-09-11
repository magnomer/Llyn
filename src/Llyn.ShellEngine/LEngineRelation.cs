using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LMeaning> LEngineMeaningFind(string query)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(query);
            return new LMeaningArchive(_lEngineDatabase).LMeaningFind(query);
        }
    }

    public LRelation LEngineRelationCreate(LRelation relation)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(relation);

            using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();
            LEngineTargetValidate(relation.LRelationTargetEntry, relation.LRelationTargetMeaning);

            LRelation stored = new LRelationArchive(_lEngineDatabase).LRelationCreate(relation);

            session.LDatabaseSessionCommit();
            return stored;
        }
    }

    public IReadOnlyList<LRelation> LEngineRelationRead(long meaningId)
    {
        lock (_lEngineGate)
        {
            return new LRelationArchive(_lEngineDatabase).LRelationRead(meaningId);
        }
    }

    public void LEngineRelationUpdate(LRelation relation)
    {
        lock (_lEngineGate)
        {
            new LRelationArchive(_lEngineDatabase).LRelationUpdate(relation);
        }
    }

    public void LEngineRelationMove(long id, int position)
    {
        lock (_lEngineGate)
        {
            new LRelationArchive(_lEngineDatabase).LRelationMove(id, position);
        }
    }

    public void LEngineRelationDelete(long id)
    {
        lock (_lEngineGate)
        {
            new LRelationArchive(_lEngineDatabase).LRelationDelete(id);
        }
    }

    public LSynonym LEngineSynonymCreate(LSynonym synonym)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(synonym);

            using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();
            LEngineTargetValidate(synonym.LSynonymTargetEntry, synonym.LSynonymTargetMeaning);

            LSynonym stored = new LSynonymArchive(_lEngineDatabase).LSynonymCreate(synonym);

            session.LDatabaseSessionCommit();
            return stored;
        }
    }

    public IReadOnlyList<LSynonym> LEngineSynonymRead(long collocationId)
    {
        lock (_lEngineGate)
        {
            return new LSynonymArchive(_lEngineDatabase).LSynonymRead(collocationId);
        }
    }

    public void LEngineSynonymUpdate(LSynonym synonym)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(synonym);

            using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();
            LEngineTargetValidate(synonym.LSynonymTargetEntry, synonym.LSynonymTargetMeaning);

            new LSynonymArchive(_lEngineDatabase).LSynonymUpdate(synonym);

            session.LDatabaseSessionCommit();
        }
    }

    public void LEngineSynonymMove(long id, int position)
    {
        lock (_lEngineGate)
        {
            new LSynonymArchive(_lEngineDatabase).LSynonymMove(id, position);
        }
    }

    public void LEngineSynonymDelete(long id)
    {
        lock (_lEngineGate)
        {
            new LSynonymArchive(_lEngineDatabase).LSynonymDelete(id);
        }
    }

    private void LEngineTargetValidate(long? entryId, long? meaningId)
    {
        bool hasEntry = entryId > 0;
        bool hasMeaning = meaningId > 0;
        if (hasEntry == hasMeaning)
        {
            throw new LRefusal(LRefusal.LRefusalTarget);
        }

        bool found = hasEntry
            ? new LEntryArchive(_lEngineDatabase).LEntryRead(entryId!.Value) is not null
            : new LMeaningArchive(_lEngineDatabase).LMeaningSingleRead(meaningId!.Value) is not null;
        if (!found)
        {
            throw new LRefusal(LRefusal.LRefusalTarget);
        }
    }
}
