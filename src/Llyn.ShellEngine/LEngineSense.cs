using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LSense LEngineSenseCreate(LSense sense)
    {
        ArgumentNullException.ThrowIfNull(sense);
        return new LSenseArchive(_lEngineDatabase).LSenseCreate(sense);
    }

    public LSense? LEngineSenseRead(string id)
    {
        return new LSenseArchive(_lEngineDatabase).LSenseSingleRead(id);
    }

    public IReadOnlyList<LSense> LEngineSenseRead(string ownerId, LOwner owner)
    {
        if (owner != LOwner.LOwnerEntry)
        {
            throw LEngineOwnerRaise(owner);
        }

        return new LSenseArchive(_lEngineDatabase).LSenseRead(ownerId);
    }

    public void LEngineSenseUpdate(LSense sense)
    {
        new LSenseArchive(_lEngineDatabase).LSenseUpdate(sense);
    }

    public void LEngineSenseMove(string id, int position)
    {
        new LSenseArchive(_lEngineDatabase).LSenseMove(id, position);
    }

    public void LEngineSenseDelete(string id)
    {
        new LSenseArchive(_lEngineDatabase).LSenseDelete(id);
    }

    public LCollocation LEngineCollocationCreate(LCollocation collocation)
    {
        ArgumentNullException.ThrowIfNull(collocation);
        return new LCollocationArchive(_lEngineDatabase).LCollocationCreate(collocation);
    }

    public IReadOnlyList<LCollocation> LEngineCollocationRead(string ownerId, LOwner owner)
    {
        if (owner != LOwner.LOwnerEntry)
        {
            throw LEngineOwnerRaise(owner);
        }

        return new LCollocationArchive(_lEngineDatabase).LCollocationRead(ownerId);
    }

    public void LEngineCollocationUpdate(LCollocation collocation)
    {
        new LCollocationArchive(_lEngineDatabase).LCollocationUpdate(collocation);
    }

    public void LEngineCollocationMove(string id, int position)
    {
        new LCollocationArchive(_lEngineDatabase).LCollocationMove(id, position);
    }

    public void LEngineCollocationDelete(string id)
    {
        new LCollocationArchive(_lEngineDatabase).LCollocationDelete(id);
    }
}
