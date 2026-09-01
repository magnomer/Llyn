using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LInflection> LEngineInflectionRead(string entryId)
    {
        return new LInflectionArchive(_lEngineDatabase).LInflectionRead(entryId);
    }

    public void LEngineInflectionSet(string entryId, IReadOnlyList<LInflection> inflections)
    {
        ArgumentNullException.ThrowIfNull(inflections);
        new LInflectionArchive(_lEngineDatabase).LInflectionSet(entryId, inflections);
    }

    public void LEngineInflectionAppend(string entryId, IReadOnlyList<LInflection> inflections)
    {
        ArgumentNullException.ThrowIfNull(inflections);
        new LInflectionArchive(_lEngineDatabase).LInflectionAppend(entryId, inflections);
    }

    public void LEngineInflectionMove(string entryId, int position, int target)
    {
        new LInflectionArchive(_lEngineDatabase).LInflectionMove(entryId, position, target);
    }

    public void LEngineInflectionDelete(string entryId, int position)
    {
        new LInflectionArchive(_lEngineDatabase).LInflectionDelete(entryId, position);
    }
}
