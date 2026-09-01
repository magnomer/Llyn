using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LTag LEngineTagCreate(LTag tag)
    {
        ArgumentNullException.ThrowIfNull(tag);
        return new LTagArchive(_lEngineDatabase).LTagCreate(tag);
    }

    public LTag? LEngineTagRead(string id)
    {
        return new LTagArchive(_lEngineDatabase).LTagRead(id);
    }

    public IReadOnlyList<LTag> LEngineTagRead(string ownerId, LOwner owner)
    {
        LTagArchive tags = new(_lEngineDatabase);
        return LEngineOwnerCheck(owner)
            ? tags.LTagCollocationRead(ownerId)
            : tags.LTagSenseRead(ownerId);
    }

    public void LEngineTagUpdate(LTag tag)
    {
        new LTagArchive(_lEngineDatabase).LTagUpdate(tag);
    }

    public void LEngineTagAttach(string ownerId, string tagId, int position, LOwner owner)
    {
        LTagArchive tags = new(_lEngineDatabase);
        if (LEngineOwnerCheck(owner))
        {
            tags.LTagCollocationAttach(ownerId, tagId, position);
            return;
        }

        tags.LTagSenseAttach(ownerId, tagId, position);
    }

    public void LEngineTagDetach(string ownerId, string tagId, LOwner owner)
    {
        LTagArchive tags = new(_lEngineDatabase);
        if (LEngineOwnerCheck(owner))
        {
            tags.LTagCollocationDetach(ownerId, tagId);
            return;
        }

        tags.LTagSenseDetach(ownerId, tagId);
    }

    public void LEngineTagRemove(string ownerId, string tagId, LOwner owner)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tagId);

        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

        LEngineTagDetach(ownerId, tagId, owner);

        LTagArchive tags = new(_lEngineDatabase);
        if (tags.LTagReferenceRead(tagId) == 0)
        {
            tags.LTagDelete(tagId);
        }

        session.LDatabaseSessionCommit();
    }

    public void LEngineTagDelete(string id)
    {
        new LTagArchive(_lEngineDatabase).LTagDelete(id);
    }
}
