using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyDictionary<string, int> LEngineUsageRead(LOwner owner)
    {
        return owner switch
        {
            LOwner.LOwnerExample => new LExampleArchive(_lEngineDatabase).LExampleReferenceRead(),
            LOwner.LOwnerReference => new LReferenceUsage(_lEngineDatabase).LReferenceUsageRead(),
            _ => new LSituationArchive(_lEngineDatabase).LSituationReferenceRead(),
        };
    }

    public IReadOnlyList<LUsage> LEngineUsageRead(string id, LOwner owner)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        return owner switch
        {
            LOwner.LOwnerExample => new LExampleLink(_lEngineDatabase).LExampleUsageRead(id),
            LOwner.LOwnerReference => new LReferenceUsage(_lEngineDatabase).LReferenceUsageRead(id),
            _ => new LSituationArchive(_lEngineDatabase).LSituationUsageRead(id),
        };
    }
}
