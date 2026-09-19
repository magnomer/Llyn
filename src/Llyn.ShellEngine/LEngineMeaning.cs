using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LMeaning LEngineMeaningCreate(LMeaning meaning)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(meaning);
            LMeaning created = _lEngineMeanings.LMeaningCreate(meaning);
            LEngineUpdatedSet(created.LMeaningEntryId);
            return created;
        }
    }

    public LMeaning? LEngineMeaningRead(long id)
    {
        lock (_lEngineGate)
        {
            return _lEngineMeanings.LMeaningSingleRead(id);
        }
    }

    public IReadOnlyList<LMeaning> LEngineMeaningRead(long ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            if (owner != LOwner.LOwnerEntry)
            {
                throw LEngineOwnerRaise(owner);
            }

            return _lEngineMeanings.LMeaningRead(ownerId);
        }
    }

    internal void LEngineMeaningUpdate(LMeaning meaning)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(meaning);
            _lEngineMeanings.LMeaningUpdate(meaning);
            LEngineUpdatedSet(meaning.LMeaningEntryId);
        }
    }

    internal void LEngineMeaningMove(long id, int position)
    {
        lock (_lEngineGate)
        {
            _lEngineMeanings.LMeaningMove(id, position);
            LEngineUpdatedSet(id, false);
        }
    }

    internal void LEngineMeaningDelete(long id)
    {
        lock (_lEngineGate)
        {
            LMeaningVault meanings = _lEngineMeanings;
            long? entryId = meanings.LMeaningHolderRead(id);
            meanings.LMeaningDelete(id);
            if (entryId is long held)
            {
                LEngineUpdatedSet(held);
            }
        }
    }

    internal LCollocation LEngineCollocationCreate(LCollocation collocation)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(collocation);
            LCollocation created = _lEngineCollocations.LCollocationCreate(collocation);
            LEngineUpdatedSet(created.LCollocationEntryId);
            return created;
        }
    }

    internal IReadOnlyList<LCollocation> LEngineCollocationRead(long ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            if (owner != LOwner.LOwnerEntry)
            {
                throw LEngineOwnerRaise(owner);
            }

            return _lEngineCollocations.LCollocationRead(ownerId);
        }
    }

    internal void LEngineCollocationUpdate(LCollocation collocation)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(collocation);
            _lEngineCollocations.LCollocationUpdate(collocation);
            LEngineUpdatedSet(collocation.LCollocationEntryId);
        }
    }

    internal void LEngineCollocationMove(long id, int position)
    {
        lock (_lEngineGate)
        {
            _lEngineCollocations.LCollocationMove(id, position);
            LEngineUpdatedSet(id, true);
        }
    }

    internal void LEngineCollocationDelete(long id)
    {
        lock (_lEngineGate)
        {
            LCollocationVault collocations = _lEngineCollocations;
            long? entryId = collocations.LCollocationHolderRead(id);
            collocations.LCollocationDelete(id);
            if (entryId is long held)
            {
                LEngineUpdatedSet(held);
            }
        }
    }
}
