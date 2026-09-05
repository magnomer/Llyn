using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LMeaning> LEngineMeaningFind(string query)
    {
        ArgumentNullException.ThrowIfNull(query);
        return new LMeaningArchive(_lEngineDatabase).LMeaningFind(query);
    }

    public LRelation LEngineRelationCreate(LRelation relation)
    {
        ArgumentNullException.ThrowIfNull(relation);

        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();
        LEngineTargetValidate(relation.LRelationTargetEntry, relation.LRelationTargetMeaning);

        LRelation stored = new LRelationArchive(_lEngineDatabase).LRelationCreate(relation);

        session.LDatabaseSessionCommit();
        return stored;
    }

    public IReadOnlyList<LRelation> LEngineRelationRead(string meaningId)
    {
        return new LRelationArchive(_lEngineDatabase).LRelationRead(meaningId);
    }

    public void LEngineRelationUpdate(LRelation relation)
    {
        new LRelationArchive(_lEngineDatabase).LRelationUpdate(relation);
    }

    public void LEngineRelationMove(string id, int position)
    {
        new LRelationArchive(_lEngineDatabase).LRelationMove(id, position);
    }

    public void LEngineRelationDelete(string id)
    {
        new LRelationArchive(_lEngineDatabase).LRelationDelete(id);
    }

    public LSynonym LEngineSynonymCreate(LSynonym synonym)
    {
        ArgumentNullException.ThrowIfNull(synonym);

        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();
        LEngineTargetValidate(synonym.LSynonymTargetEntry, synonym.LSynonymTargetMeaning);

        LSynonym stored = new LSynonymArchive(_lEngineDatabase).LSynonymCreate(synonym);

        session.LDatabaseSessionCommit();
        return stored;
    }

    public IReadOnlyList<LSynonym> LEngineSynonymRead(string collocationId)
    {
        return new LSynonymArchive(_lEngineDatabase).LSynonymRead(collocationId);
    }

    public void LEngineSynonymUpdate(LSynonym synonym)
    {
        ArgumentNullException.ThrowIfNull(synonym);

        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();
        LEngineTargetValidate(synonym.LSynonymTargetEntry, synonym.LSynonymTargetMeaning);

        new LSynonymArchive(_lEngineDatabase).LSynonymUpdate(synonym);

        session.LDatabaseSessionCommit();
    }

    public void LEngineSynonymMove(string id, int position)
    {
        new LSynonymArchive(_lEngineDatabase).LSynonymMove(id, position);
    }

    public void LEngineSynonymDelete(string id)
    {
        new LSynonymArchive(_lEngineDatabase).LSynonymDelete(id);
    }

    private void LEngineTargetValidate(string? entryId, string? meaningId)
    {
        bool hasEntry = !string.IsNullOrWhiteSpace(entryId);
        bool hasMeaning = !string.IsNullOrWhiteSpace(meaningId);
        if (hasEntry == hasMeaning)
        {
            throw new LRefusal(LRefusal.LRefusalTarget);
        }

        bool found = hasEntry
            ? new LEntryArchive(_lEngineDatabase).LEntryRead(entryId!) is not null
            : new LMeaningArchive(_lEngineDatabase).LMeaningSingleRead(meaningId!) is not null;
        if (!found)
        {
            throw new LRefusal(LRefusal.LRefusalTarget);
        }
    }
}
