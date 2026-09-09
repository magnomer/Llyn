using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyDictionary<string, int> LEngineUsageRead(LOwner owner)
    {
        lock (_lEngineGate)
        {
            return owner switch
            {
                LOwner.LOwnerExample => new LExampleArchive(_lEngineDatabase).LExampleReferenceRead(),
                LOwner.LOwnerReference => new LReferenceUsage(_lEngineDatabase).LReferenceUsageRead(),
                LOwner.LOwnerSituation => new LSituationArchive(_lEngineDatabase).LSituationReferenceRead(),
                _ => throw LEngineOwnerRaise(owner),
            };
        }
    }

    public IReadOnlyList<LUsage> LEngineUsageRead(string id, LOwner owner)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);

            return owner switch
            {
                LOwner.LOwnerExample => new LExampleLink(_lEngineDatabase).LExampleUsageRead(id),
                LOwner.LOwnerReference => new LReferenceUsage(_lEngineDatabase).LReferenceUsageRead(id),
                LOwner.LOwnerAuthor => new LAuthorUsage(_lEngineDatabase).LAuthorUsageRead(id),
                LOwner.LOwnerSituation => new LSituationArchive(_lEngineDatabase).LSituationUsageRead(id),
                _ => throw LEngineOwnerRaise(owner),
            };
        }
    }
}
