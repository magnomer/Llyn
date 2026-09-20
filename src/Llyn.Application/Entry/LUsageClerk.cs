using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LUsageClerk
{
    private readonly LAuthorVault _lUsageClerkAuthors;
    private readonly LEntryVault _lUsageClerkEntries;
    private readonly LExampleVault _lUsageClerkExamples;
    private readonly LReferenceVault _lUsageClerkReferences;
    private readonly LSituationVault _lUsageClerkSituations;

    public LUsageClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lUsageClerkAuthors = rig.LRigAuthors;
        _lUsageClerkEntries = rig.LRigEntries;
        _lUsageClerkExamples = rig.LRigExamples;
        _lUsageClerkReferences = rig.LRigReferences;
        _lUsageClerkSituations = rig.LRigSituations;
    }

    public IReadOnlyDictionary<long, int> LUsageClerkRead(LOwner owner)
    {
        return owner switch
        {
            LOwner.LOwnerExample => _lUsageClerkExamples.LExampleReferenceRead(),
            LOwner.LOwnerReference => _lUsageClerkReferences.LReferenceUsageRead(),
            LOwner.LOwnerSituation => _lUsageClerkSituations.LSituationReferenceRead(),
            _ => throw LUsageOwnerRaise(owner),
        };
    }

    public IReadOnlyList<LUsage> LUsageClerkRead(long id, LOwner owner, bool epithet)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        IReadOnlyList<LUsage> rows = owner switch
        {
            LOwner.LOwnerExample => _lUsageClerkExamples.LExampleUsageRead(id),
            LOwner.LOwnerReference => _lUsageClerkReferences.LReferenceUsageRead(id),
            LOwner.LOwnerAuthor => _lUsageClerkAuthors.LAuthorUsageRead(id),
            LOwner.LOwnerSituation => _lUsageClerkSituations.LSituationUsageRead(id),
            _ => throw LUsageOwnerRaise(owner),
        };
        return LUsageClerkResolve(rows, _lUsageClerkEntries, epithet);
    }

    public static IReadOnlyList<LUsage> LUsageClerkResolve(
        IReadOnlyList<LUsage> rows, LEntryVault entries, bool epithet)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(entries);

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

    private static ArgumentOutOfRangeException LUsageOwnerRaise(LOwner owner)
    {
        return new ArgumentOutOfRangeException(
            nameof(owner), owner, "This entity has no usage from that kind of row.");
    }
}
