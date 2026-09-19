using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyDictionary<long, int> LEngineUsageRead(LOwner owner)
    {
        lock (_lEngineGate)
        {
            return owner switch
            {
                LOwner.LOwnerExample => _lEngineExamples.LExampleReferenceRead(),
                LOwner.LOwnerReference => _lEngineReferences.LReferenceUsageRead(),
                LOwner.LOwnerSituation => _lEngineSituations.LSituationReferenceRead(),
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
                LOwner.LOwnerExample => _lEngineExamples.LExampleUsageRead(id),
                LOwner.LOwnerReference => _lEngineReferences.LReferenceUsageRead(id),
                LOwner.LOwnerAuthor => _lEngineAuthors.LAuthorUsageRead(id),
                LOwner.LOwnerSituation => _lEngineSituations.LSituationUsageRead(id),
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
