using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LMeaning LEngineMeaningCreate(LMeaning meaning)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(meaning);
            LMeaning created = new LMeaningArchive(_lEngineDatabase).LMeaningCreate(meaning);
            LEngineUpdatedSet(created.LMeaningEntryId);
            return created;
        }
    }

    public LMeaning? LEngineMeaningRead(long id)
    {
        lock (_lEngineGate)
        {
            return new LMeaningArchive(_lEngineDatabase).LMeaningSingleRead(id);
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

            return new LMeaningArchive(_lEngineDatabase).LMeaningRead(ownerId);
        }
    }

    public void LEngineMeaningUpdate(LMeaning meaning)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(meaning);
            new LMeaningArchive(_lEngineDatabase).LMeaningUpdate(meaning);
            LEngineUpdatedSet(meaning.LMeaningEntryId);
        }
    }

    public void LEngineMeaningMove(long id, int position)
    {
        lock (_lEngineGate)
        {
            new LMeaningArchive(_lEngineDatabase).LMeaningMove(id, position);
            LEngineUpdatedSet(id, false);
        }
    }

    public void LEngineMeaningDelete(long id)
    {
        lock (_lEngineGate)
        {
            LMeaningArchive meanings = new(_lEngineDatabase);
            long? entryId = meanings.LMeaningHolderRead(id);
            meanings.LMeaningDelete(id);
            if (entryId is long held)
            {
                LEngineUpdatedSet(held);
            }
        }
    }

    public LCollocation LEngineCollocationCreate(LCollocation collocation)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(collocation);
            LCollocation created = new LCollocationArchive(_lEngineDatabase).LCollocationCreate(collocation);
            LEngineUpdatedSet(created.LCollocationEntryId);
            return created;
        }
    }

    public IReadOnlyList<LCollocation> LEngineCollocationRead(long ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            if (owner != LOwner.LOwnerEntry)
            {
                throw LEngineOwnerRaise(owner);
            }

            return new LCollocationArchive(_lEngineDatabase).LCollocationRead(ownerId);
        }
    }

    public void LEngineCollocationUpdate(LCollocation collocation)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(collocation);
            new LCollocationArchive(_lEngineDatabase).LCollocationUpdate(collocation);
            LEngineUpdatedSet(collocation.LCollocationEntryId);
        }
    }

    public void LEngineCollocationMove(long id, int position)
    {
        lock (_lEngineGate)
        {
            new LCollocationArchive(_lEngineDatabase).LCollocationMove(id, position);
            LEngineUpdatedSet(id, true);
        }
    }

    public void LEngineCollocationDelete(long id)
    {
        lock (_lEngineGate)
        {
            LCollocationArchive collocations = new(_lEngineDatabase);
            long? entryId = collocations.LCollocationHolderRead(id);
            collocations.LCollocationDelete(id);
            if (entryId is long held)
            {
                LEngineUpdatedSet(held);
            }
        }
    }
}
