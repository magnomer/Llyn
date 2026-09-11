using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LInflection> LEngineInflectionRead(long entryId)
    {
        lock (_lEngineGate)
        {
            return new LInflectionArchive(_lEngineDatabase).LInflectionRead(entryId);
        }
    }

    public void LEngineInflectionSet(long entryId, IReadOnlyList<LInflection> inflections)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(inflections);
            new LInflectionArchive(_lEngineDatabase).LInflectionSet(entryId, inflections);
        }
    }

    public void LEngineInflectionAppend(long entryId, IReadOnlyList<LInflection> inflections)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(inflections);
            new LInflectionArchive(_lEngineDatabase).LInflectionAppend(entryId, inflections);
        }
    }

    public void LEngineInflectionMove(long entryId, int position, int target)
    {
        lock (_lEngineGate)
        {
            new LInflectionArchive(_lEngineDatabase).LInflectionMove(entryId, position, target);
        }
    }

    public void LEngineInflectionDelete(long entryId, int position)
    {
        lock (_lEngineGate)
        {
            new LInflectionArchive(_lEngineDatabase).LInflectionDelete(entryId, position);
        }
    }
}
