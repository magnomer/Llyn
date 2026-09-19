using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyDictionary<long, int> LEngineUsageRead(LOwner owner)
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

    public IReadOnlyList<LUsage> LEngineUsageRead(long id, LOwner owner)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

            IReadOnlyList<LUsage> rows = owner switch
            {
                LOwner.LOwnerExample => new LExampleLink(_lEngineDatabase).LExampleUsageRead(id),
                LOwner.LOwnerReference => new LReferenceUsage(_lEngineDatabase).LReferenceUsageRead(id),
                LOwner.LOwnerAuthor => new LAuthorUsage(_lEngineDatabase).LAuthorUsageRead(id),
                LOwner.LOwnerSituation => new LSituationArchive(_lEngineDatabase).LSituationUsageRead(id),
                _ => throw LEngineOwnerRaise(owner),
            };
            return LEngineUsageResolve(rows, _lEngineEntries, _lEngineSettings.LSettingsEpithet);
        }
    }

    private static IReadOnlyList<LUsage> LEngineUsageResolve(
        IReadOnlyList<LUsage> rows, LEntryVault entries, bool epithet)
    {
        List<LUsage> named = new(rows.Count);
        foreach (LUsage row in rows)
        {
            named.Add(row with
            {
                LUsageEpithet = epithet && row.LUsageEntry > 0
                    ? entries.LEntryEpithetRead(row.LUsageEntry)
                    : string.Empty,
            });
        }

        int[] places = new int[rows.Count];
        for (int index = 0; index < places.Length; index++)
        {
            places[index] = index;
        }

        LTwin.LTwinNameApply(places, place => rows[place].LUsageHeadword,
            (place, name) => named[place] = named[place] with { LUsageName = name });
        return named;
    }
}
