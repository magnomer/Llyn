using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LLineageClerk
{
    public const int LLineageClerkDepth = 8;

    private readonly LEntryVault _lLineageClerkEntries;
    private readonly LAncestorVault _lLineageClerkAncestors;

    public LLineageClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lLineageClerkEntries = rig.LRigEntries;
        _lLineageClerkAncestors = rig.LRigLexicon.LRigLexiconAncestors;
    }

    public IReadOnlyList<LLineage> LLineageClerkRead(long entryId, IReadOnlyList<LAncestorDraft> map)
    {
        ArgumentNullException.ThrowIfNull(map);

        Dictionary<long, string> tongues = _lLineageClerkAncestors.LAncestorTongueRead()
            .ToDictionary(static tongue => tongue.LAncestorTongueId, static tongue => tongue.LAncestorTongueName);
        HashSet<long> path = entryId > 0 ? [entryId] : [];
        LLineageWalk walk = new(LAncestorDraft.LAncestorDraftResolve(map), false, 0, [], tongues, path);
        return LLineageBranchRead(walk, null);
    }

    private IReadOnlyList<LLineage> LLineageBranchRead(LLineageWalk walk, long? childId)
    {
        List<LLineage> branch = [];
        foreach (LAncestor box in walk.LLineageWalkMap
            .Where(box => box.LAncestorChildId == childId)
            .OrderBy(static box => box.LAncestorPosition))
        {
            if (!walk.LLineageWalkSeen.Add(box.LAncestorId))
            {
                continue;
            }

            List<LLineage> sources = [.. LLineageBranchRead(walk, box.LAncestorId)];
            LEntry? entry = box.LAncestorTargetId is long target ? _lLineageClerkEntries.LEntryRead(target) : null;
            if (entry is not null
                && walk.LLineageWalkDepth < LLineageClerkDepth
                && walk.LLineageWalkPath.Add(entry.LEntryId))
            {
                LLineageWalk borrowed = walk with
                {
                    LLineageWalkMap = _lLineageClerkAncestors.LAncestorRead(entry.LEntryId),
                    LLineageWalkBorrowed = true,
                    LLineageWalkDepth = walk.LLineageWalkDepth + 1,
                    LLineageWalkSeen = [],
                };
                sources.AddRange(LLineageBranchRead(borrowed, null));
                walk.LLineageWalkPath.Remove(entry.LEntryId);
            }

            branch.Add(new LLineage(
                box.LAncestorId,
                entry?.LEntryId,
                entry?.LEntryHeadword ?? box.LAncestorText,
                entry?.LEntryLanguage ?? LLineageLanguageRead(walk, box),
                box.LAncestorGloss,
                box.LAncestorRelationId,
                walk.LLineageWalkBorrowed,
                sources));
        }

        return branch;
    }

    private static string LLineageLanguageRead(LLineageWalk walk, LAncestor box)
    {
        return box.LAncestorTongueId is long tongue
            ? walk.LLineageWalkTongues.GetValueOrDefault(tongue, string.Empty)
            : box.LAncestorLanguage;
    }

    private sealed record LLineageWalk(
        IReadOnlyList<LAncestor> LLineageWalkMap,
        bool LLineageWalkBorrowed,
        int LLineageWalkDepth,
        HashSet<long> LLineageWalkSeen,
        IReadOnlyDictionary<long, string> LLineageWalkTongues,
        HashSet<long> LLineageWalkPath);
}
