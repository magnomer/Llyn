using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LMarkupClerkAncestor
{
    private readonly LEntryVault _lMarkupAncestorEntries;
    private readonly LAncestorVault _lMarkupAncestorVault;
    private readonly LAncestorClerk _lMarkupAncestorClerk;
    private readonly LMarkupClerkLink _lMarkupAncestorLink;

    public LMarkupClerkAncestor(LRig rig, LAncestorClerk ancestors, LMarkupClerkLink link)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(ancestors);
        ArgumentNullException.ThrowIfNull(link);
        _lMarkupAncestorEntries = rig.LRigEntries;
        _lMarkupAncestorVault = rig.LRigLexicon.LRigLexiconAncestors;
        _lMarkupAncestorClerk = ancestors;
        _lMarkupAncestorLink = link;
    }

    public IReadOnlyList<LMarkupAncestor> LMarkupAncestorCreate(IReadOnlyList<LAncestorDraft> map)
    {
        ArgumentNullException.ThrowIfNull(map);

        Dictionary<long, string> tongues = _lMarkupAncestorVault.LAncestorTongueRead()
            .ToDictionary(static tongue => tongue.LAncestorTongueId, static tongue => tongue.LAncestorTongueName);
        Dictionary<long, string> relations = _lMarkupAncestorVault.LAncestorRelationRead()
            .ToDictionary(
                static relation => relation.LAncestorRelationId, static relation => relation.LAncestorRelationName);

        Dictionary<long, int> keys = [];
        List<LMarkupAncestor> written = new(map.Count);
        Queue<long?> pending = new();
        pending.Enqueue(null);
        while (pending.TryDequeue(out long? child))
        {
            int position = 0;
            foreach (LAncestorDraft box in map.Where(box => box.LAncestorDraftChild == child))
            {
                if (keys.ContainsKey(box.LAncestorDraftId))
                {
                    continue;
                }

                keys[box.LAncestorDraftId] = written.Count + 1;
                pending.Enqueue(box.LAncestorDraftId);
                LEntry? entry = box.LAncestorDraftTarget is long target
                    ? _lMarkupAncestorEntries.LEntryRead(target)
                    : null;
                written.Add(new LMarkupAncestor(
                    written.Count + 1,
                    child is long held ? keys[held] : null,
                    position++,
                    entry?.LEntryHeadword ?? string.Empty,
                    entry is null ? box.LAncestorDraftText : string.Empty,
                    entry?.LEntryLanguage
                        ?? (box.LAncestorDraftTongue is long tongue
                            ? tongues.GetValueOrDefault(tongue, string.Empty)
                            : box.LAncestorDraftLanguage),
                    box.LAncestorDraftGloss,
                    box.LAncestorDraftRelation is long relation
                        ? relations.GetValueOrDefault(relation, string.Empty)
                        : string.Empty));
            }
        }

        return written;
    }

    public IReadOnlyList<LAncestorDraft> LMarkupAncestorResolve(
        IReadOnlyList<LMarkupAncestor> boxes,
        IReadOnlyDictionary<(string, string), long> prepared,
        List<LMarkupOmission> omissions)
    {
        ArgumentNullException.ThrowIfNull(boxes);
        ArgumentNullException.ThrowIfNull(prepared);
        ArgumentNullException.ThrowIfNull(omissions);

        Dictionary<int, LMarkupAncestor> keyed = [];
        foreach (LMarkupAncestor box in boxes)
        {
            if (box.LMarkupAncestorKey <= 0 || !keyed.TryAdd(box.LMarkupAncestorKey, box))
            {
                omissions.Add(new LMarkupOmission(0, $"ancestor {box.LMarkupAncestorKey}"));
            }
        }

        List<LAncestorDraft> drafts = new(keyed.Count);
        Queue<int?> pending = new();
        pending.Enqueue(null);
        while (pending.TryDequeue(out int? child))
        {
            foreach (LMarkupAncestor box in keyed.Values
                .Where(box => box.LMarkupAncestorChild == child)
                .OrderBy(static box => box.LMarkupAncestorPosition))
            {
                pending.Enqueue(box.LMarkupAncestorKey);
                drafts.Add(LMarkupAncestorResolve(box, keyed.Values, prepared, omissions));
            }
        }

        foreach (LMarkupAncestor box in keyed.Values)
        {
            if (!drafts.Exists(draft => draft.LAncestorDraftId == -box.LMarkupAncestorKey))
            {
                omissions.Add(new LMarkupOmission(0, $"ancestor {box.LMarkupAncestorKey}"));
            }
        }

        return drafts;
    }

    private LAncestorDraft LMarkupAncestorResolve(
        LMarkupAncestor box,
        IEnumerable<LMarkupAncestor> boxes,
        IReadOnlyDictionary<(string, string), long> prepared,
        List<LMarkupOmission> omissions)
    {
        string headword = box.LMarkupAncestorHeadword;
        string text = box.LMarkupAncestorText;
        long? target = null;
        if (headword.Length > 0)
        {
            bool sourced = boxes.Any(source => source.LMarkupAncestorChild == box.LMarkupAncestorKey);
            long found = sourced
                ? 0
                : _lMarkupAncestorLink.LMarkupEntryResolve(headword, box.LMarkupAncestorLanguage, prepared);
            if (found == 0)
            {
                omissions.Add(new LMarkupOmission(0, $"ancestor \"{headword}\""));
            }
            else
            {
                target = found;
            }

            text = headword;
        }

        LAncestorLanguage? language = target is null && box.LMarkupAncestorLanguage.Trim().Length > 0
            ? _lMarkupAncestorClerk.LAncestorTongueCreate(box.LMarkupAncestorLanguage)
            : null;
        long? relation = box.LMarkupAncestorRelation.Trim().Length > 0
            ? _lMarkupAncestorClerk.LAncestorRelationCreate(box.LMarkupAncestorRelation).LAncestorRelationId
            : null;

        return new LAncestorDraft(
            -box.LMarkupAncestorKey,
            box.LMarkupAncestorChild is int child ? -child : null,
            target,
            text,
            language is { LAncestorLanguageTongue: null } ? language.LAncestorLanguageName : string.Empty,
            language?.LAncestorLanguageTongue,
            box.LMarkupAncestorGloss,
            relation);
    }
}
