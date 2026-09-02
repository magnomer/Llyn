using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyDictionary<string, int> LEngineUsageRead(LOwner owner)
    {
        return owner == LOwner.LOwnerExample
            ? new LExampleArchive(_lEngineDatabase).LExampleReferenceRead()
            : new LSituationArchive(_lEngineDatabase).LSituationReferenceRead();
    }

    public IReadOnlyList<LUsage> LEngineUsageRead(string id, LOwner owner)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        return owner == LOwner.LOwnerExample
            ? new LExampleLink(_lEngineDatabase).LExampleUsageRead(id)
            : new LSituationArchive(_lEngineDatabase).LSituationUsageRead(id);
    }
}
