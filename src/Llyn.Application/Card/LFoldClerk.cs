using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LFoldClerk
{
    private readonly LFoldVault _lFoldClerkFolds;

    public LFoldClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lFoldClerkFolds = rig.LRigKeeping.LRigKeepingFolds;
    }

    public IReadOnlySet<long> LFoldClerkRead(long entryId)
    {
        return entryId > 0 ? _lFoldClerkFolds.LFoldRead(entryId) : new HashSet<long>();
    }

    public void LFoldClerkSave(long cardId)
    {
        if (cardId > 0)
        {
            _lFoldClerkFolds.LFoldSave(cardId);
        }
    }

    public void LFoldClerkDelete(long cardId)
    {
        if (cardId > 0)
        {
            _lFoldClerkFolds.LFoldDelete(cardId);
        }
    }

    public bool LFoldClerkLoad(long entryId)
    {
        return entryId > 0 && _lFoldClerkFolds.LFoldReflexCheck(entryId);
    }

    public void LFoldClerkSpread(long entryId, bool opened)
    {
        if (entryId > 0)
        {
            _lFoldClerkFolds.LFoldReflexSpread(entryId, opened);
        }
    }

    public bool LFoldClerkLoad(long entryId, LFoldBox box)
    {
        return entryId > 0 && _lFoldClerkFolds.LFoldBoxCheck(entryId, box);
    }

    public void LFoldClerkSpread(long entryId, LFoldBox box, bool opened)
    {
        if (entryId > 0)
        {
            _lFoldClerkFolds.LFoldBoxSpread(entryId, box, opened);
        }
    }

    public void LFoldClerkSpread(long entryId, string key, bool opened)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (entryId > 0)
        {
            _lFoldClerkFolds.LFoldStemSpread(entryId, key, opened);
        }
    }
}
