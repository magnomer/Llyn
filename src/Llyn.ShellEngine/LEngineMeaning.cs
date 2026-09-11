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
            return new LMeaningArchive(_lEngineDatabase).LMeaningCreate(meaning);
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
            new LMeaningArchive(_lEngineDatabase).LMeaningUpdate(meaning);
        }
    }

    public void LEngineMeaningMove(long id, int position)
    {
        lock (_lEngineGate)
        {
            new LMeaningArchive(_lEngineDatabase).LMeaningMove(id, position);
        }
    }

    public void LEngineMeaningDelete(long id)
    {
        lock (_lEngineGate)
        {
            new LMeaningArchive(_lEngineDatabase).LMeaningDelete(id);
        }
    }

    public LCollocation LEngineCollocationCreate(LCollocation collocation)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(collocation);
            return new LCollocationArchive(_lEngineDatabase).LCollocationCreate(collocation);
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
            new LCollocationArchive(_lEngineDatabase).LCollocationUpdate(collocation);
        }
    }

    public void LEngineCollocationMove(long id, int position)
    {
        lock (_lEngineGate)
        {
            new LCollocationArchive(_lEngineDatabase).LCollocationMove(id, position);
        }
    }

    public void LEngineCollocationDelete(long id)
    {
        lock (_lEngineGate)
        {
            new LCollocationArchive(_lEngineDatabase).LCollocationDelete(id);
        }
    }
}
