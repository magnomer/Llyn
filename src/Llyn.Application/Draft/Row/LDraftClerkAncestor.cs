using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LDraftClerkAncestor
{
    private readonly LEntryVault _lDraftClerkEntries;
    private readonly LAncestorVault _lDraftClerkAncestors;
    private readonly LIdentity _lDraftClerkIdentity;
    private LAncestorShelf? _lDraftClerkShelf;

    public LDraftClerkAncestor(LEntryVault entries, LAncestorVault ancestors, LIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(ancestors);
        ArgumentNullException.ThrowIfNull(identity);
        _lDraftClerkEntries = entries;
        _lDraftClerkAncestors = ancestors;
        _lDraftClerkIdentity = identity;
    }

    public LEntryDraft? LAncestorApply(LEntryDraft content, LRequest request)
    {
        ArgumentNullException.ThrowIfNull(content);

        return request switch
        {
            LRequestAncestorText sent => LAncestorChange(
                content,
                sent.LRequestAncestorId,
                box => box with
                {
                    LAncestorDraftText = sent.LRequestText ?? string.Empty,
                    LAncestorDraftTarget = null,
                }),
            LRequestAncestorTarget sent => LAncestorTargetApply(content, sent),
            LRequestAncestorLanguage sent => LAncestorChange(
                content, sent.LRequestAncestorId, box => LAncestorLanguageApply(box, sent)),
            LRequestAncestorGloss sent => LAncestorChange(
                content,
                sent.LRequestAncestorId,
                box => box with { LAncestorDraftGloss = sent.LRequestText ?? string.Empty }),
            LRequestAncestorRelation sent => LAncestorChange(
                content, sent.LRequestAncestorId, box => LAncestorRelationApply(box, sent.LRequestRelationId)),
            LRequestAncestorInsertion sent => LAncestorInsert(content, sent.LRequestAncestorId),
            LRequestAncestorAddition sent => LAncestorAdd(content, sent.LRequestAncestorId),
            LRequestAncestorRemoval sent => LAncestorRemove(content, sent.LRequestAncestorId),
            LRequestAncestorRestoration sent => LAncestorRestore(content, sent),
            _ => null,
        };
    }

    private LEntryDraft LAncestorInsert(LEntryDraft content, long ancestorId)
    {
        IReadOnlyList<LAncestorDraft> boxes = content.LEntryDraftAncestors;
        long? child = null;
        int position = 0;
        if (ancestorId != 0)
        {
            int index = LAncestorFind(boxes, ancestorId);
            if (boxes[index].LAncestorDraftTarget is not null)
            {
                throw new LRefusal(LRefusal.LRefusalLink);
            }

            child = ancestorId;
            position = index + 1;
        }

        long fresh = _lDraftClerkIdentity.LIdentityCreate();
        List<LAncestorDraft> written = new(boxes.Count + 1);
        foreach (LAncestorDraft box in boxes)
        {
            written.Add(box.LAncestorDraftChild == child ? box with { LAncestorDraftChild = fresh } : box);
        }

        written.Insert(position, new LAncestorDraft(fresh, child));
        return content with { LEntryDraftAncestors = written };
    }

    private LEntryDraft LAncestorAdd(LEntryDraft content, long ancestorId)
    {
        IReadOnlyList<LAncestorDraft> boxes = content.LEntryDraftAncestors;
        int index = LAncestorFind(boxes, ancestorId);

        List<LAncestorDraft> written = [.. boxes];
        written.Insert(
            index + 1,
            new LAncestorDraft(_lDraftClerkIdentity.LIdentityCreate(), boxes[index].LAncestorDraftChild));
        return content with { LEntryDraftAncestors = written };
    }

    private static LEntryDraft LAncestorRemove(LEntryDraft content, long ancestorId)
    {
        IReadOnlyList<LAncestorDraft> boxes = content.LEntryDraftAncestors;
        int index = LAncestorFind(boxes, ancestorId);
        long? child = boxes[index].LAncestorDraftChild;

        List<LAncestorDraft> written = new(boxes.Count);
        for (int held = 0; held < boxes.Count; held++)
        {
            if (held == index)
            {
                written.AddRange(boxes
                    .Where(box => box.LAncestorDraftChild == ancestorId)
                    .Select(box => box with { LAncestorDraftChild = child }));
            }
            else if (boxes[held].LAncestorDraftChild != ancestorId)
            {
                written.Add(boxes[held]);
            }
        }

        return content with { LEntryDraftAncestors = written };
    }

    private LEntryDraft LAncestorTargetApply(LEntryDraft content, LRequestAncestorTarget request)
    {
        IReadOnlyList<LAncestorDraft> boxes = content.LEntryDraftAncestors;
        int index = LAncestorFind(boxes, request.LRequestAncestorId);
        LEntry entry = (request.LRequestEntryId > 0 ? _lDraftClerkEntries.LEntryRead(request.LRequestEntryId) : null)
            ?? throw new LRefusal(LRefusal.LRefusalLink);

        LAncestorDraft box = boxes[index];
        HashSet<long> branch = LAncestorBranchRead(boxes, box.LAncestorDraftId);
        List<LAncestorDraft> written = new(boxes.Count);
        List<LAncestorDraft> sources = [];
        foreach (LAncestorDraft held in boxes)
        {
            if (held.LAncestorDraftId == box.LAncestorDraftId)
            {
                written.Add(box with
                {
                    LAncestorDraftTarget = entry.LEntryId,
                    LAncestorDraftText = entry.LEntryHeadword,
                    LAncestorDraftLanguage = string.Empty,
                    LAncestorDraftTongue = null,
                });
            }
            else
            {
                (branch.Contains(held.LAncestorDraftId) ? sources : written).Add(held);
            }
        }

        if (sources.Count > 0)
        {
            _lDraftClerkShelf = new LAncestorShelf(request.LRequestDraftId, box, sources);
        }

        return content with { LEntryDraftAncestors = written };
    }

    private LEntryDraft LAncestorRestore(LEntryDraft content, LRequestAncestorRestoration request)
    {
        LAncestorShelf shelf = _lDraftClerkShelf is LAncestorShelf held
            && held.LAncestorShelfDraft == request.LRequestDraftId
            && held.LAncestorShelfBox.LAncestorDraftId == request.LRequestAncestorId
                ? held
                : throw new LRefusal(LRefusal.LRefusalItem);

        IReadOnlyList<LAncestorDraft> boxes = content.LEntryDraftAncestors;
        int index = LAncestorFind(boxes, request.LRequestAncestorId);
        if (boxes[index].LAncestorDraftTarget is null)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        List<LAncestorDraft> written = [.. boxes];
        written[index] = boxes[index] with
        {
            LAncestorDraftTarget = null,
            LAncestorDraftText = shelf.LAncestorShelfBox.LAncestorDraftText,
            LAncestorDraftLanguage = shelf.LAncestorShelfBox.LAncestorDraftLanguage,
            LAncestorDraftTongue = shelf.LAncestorShelfBox.LAncestorDraftTongue,
        };
        written.InsertRange(index + 1, shelf.LAncestorShelfSources);
        _lDraftClerkShelf = null;
        return content with { LEntryDraftAncestors = written };
    }

    private LAncestorDraft LAncestorLanguageApply(LAncestorDraft box, LRequestAncestorLanguage request)
    {
        if (box.LAncestorDraftTarget is not null)
        {
            throw new LRefusal(LRefusal.LRefusalLink);
        }

        if (request.LRequestTongueId == 0)
        {
            return box with
            {
                LAncestorDraftLanguage = (request.LRequestText ?? string.Empty).Trim(),
                LAncestorDraftTongue = null,
            };
        }

        if (!_lDraftClerkAncestors.LAncestorTongueRead()
                .Any(tongue => tongue.LAncestorTongueId == request.LRequestTongueId))
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        return box with { LAncestorDraftLanguage = string.Empty, LAncestorDraftTongue = request.LRequestTongueId };
    }

    private LAncestorDraft LAncestorRelationApply(LAncestorDraft box, long relationId)
    {
        if (relationId == 0)
        {
            return box with { LAncestorDraftRelation = null };
        }

        if (!_lDraftClerkAncestors.LAncestorRelationRead()
                .Any(relation => relation.LAncestorRelationId == relationId))
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        return box with { LAncestorDraftRelation = relationId };
    }

    private static HashSet<long> LAncestorBranchRead(IReadOnlyList<LAncestorDraft> boxes, long ancestorId)
    {
        HashSet<long> branch = [];
        Queue<long> pending = new([ancestorId]);
        while (pending.Count > 0)
        {
            long held = pending.Dequeue();
            foreach (LAncestorDraft box in boxes)
            {
                if (box.LAncestorDraftChild == held
                    && box.LAncestorDraftId != ancestorId
                    && branch.Add(box.LAncestorDraftId))
                {
                    pending.Enqueue(box.LAncestorDraftId);
                }
            }
        }

        return branch;
    }

    private static int LAncestorFind(IReadOnlyList<LAncestorDraft> boxes, long ancestorId)
    {
        int index = LDraftClerkList.LDraftListFind(boxes, ancestorId, static box => box.LAncestorDraftId);
        return index < 0 ? throw new LRefusal(LRefusal.LRefusalItem) : index;
    }

    private static LEntryDraft LAncestorChange(
        LEntryDraft content, long ancestorId, Func<LAncestorDraft, LAncestorDraft> change)
    {
        IReadOnlyList<LAncestorDraft> boxes = LDraftClerkList.LDraftListChange(
            content.LEntryDraftAncestors, ancestorId, static box => box.LAncestorDraftId, change)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        return content with { LEntryDraftAncestors = boxes };
    }

    private sealed record LAncestorShelf(
        long LAncestorShelfDraft,
        LAncestorDraft LAncestorShelfBox,
        IReadOnlyList<LAncestorDraft> LAncestorShelfSources);
}
