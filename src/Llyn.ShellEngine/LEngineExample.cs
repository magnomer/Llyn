using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LExample LEngineExampleCreate(LExample example)
    {
        ArgumentNullException.ThrowIfNull(example);
        return new LExampleArchive(_lEngineDatabase).LExampleCreate(example);
    }

    public LExample? LEngineExampleRead(string id)
    {
        return new LExampleArchive(_lEngineDatabase).LExampleRead(id);
    }

    public IReadOnlyList<LExample> LEngineExampleRead()
    {
        return new LExampleArchive(_lEngineDatabase).LExampleRead();
    }

    public IReadOnlyList<LExample> LEngineExampleRead(string ownerId, LOwner owner)
    {
        LExampleLink examples = new(_lEngineDatabase);
        return owner switch
        {
            LOwner.LOwnerEntry => examples.LExampleEntryRead(ownerId),
            LOwner.LOwnerSense => examples.LExampleSenseRead(ownerId),
            LOwner.LOwnerCollocation => examples.LExampleCollocationRead(ownerId),
            _ => throw LEngineOwnerRaise(owner),
        };
    }

    public void LEngineExampleUpdate(LExample example)
    {
        new LExampleArchive(_lEngineDatabase).LExampleUpdate(example);
    }

    public void LEngineExampleUpdate(string exampleId, LStateValue reference)
    {
        new LExampleArchive(_lEngineDatabase).LExampleSourceUpdate(exampleId, reference);
    }

    public void LEngineExampleAttach(string ownerId, string exampleId, int position, LOwner owner)
    {
        LExampleLink examples = new(_lEngineDatabase);
        switch (owner)
        {
            case LOwner.LOwnerEntry:
                examples.LExampleEntryAttach(ownerId, exampleId, position);
                return;
            case LOwner.LOwnerSense:
                examples.LExampleSenseAttach(ownerId, exampleId, position);
                return;
            case LOwner.LOwnerCollocation:
                examples.LExampleCollocationAttach(ownerId, exampleId, position);
                return;
            default:
                throw LEngineOwnerRaise(owner);
        }
    }

    public void LEngineExampleDetach(string ownerId, string exampleId, LOwner owner)
    {
        LExampleLink examples = new(_lEngineDatabase);
        switch (owner)
        {
            case LOwner.LOwnerEntry:
                examples.LExampleEntryDetach(ownerId, exampleId);
                return;
            case LOwner.LOwnerSense:
                examples.LExampleSenseDetach(ownerId, exampleId);
                return;
            case LOwner.LOwnerCollocation:
                examples.LExampleCollocationDetach(ownerId, exampleId);
                return;
            default:
                throw LEngineOwnerRaise(owner);
        }
    }

    public void LEngineExampleRemove(string ownerId, string exampleId, LOwner owner)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);

        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

        LEngineExampleDetach(ownerId, exampleId, owner);

        LExampleArchive examples = new(_lEngineDatabase);
        if (examples.LExampleReferenceRead(exampleId) == 0)
        {
            examples.LExampleDelete(exampleId);
        }

        session.LDatabaseSessionCommit();
    }

    public void LEngineExampleDelete(string id)
    {
        new LExampleArchive(_lEngineDatabase).LExampleDelete(id);
    }

    public void LEngineExampleDelete(string id, bool detach)
    {
        new LExampleArchive(_lEngineDatabase).LExampleDelete(id, detach);
    }
}
