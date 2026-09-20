using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LTagClerk
{
    private readonly LTagVault _lTagClerkTags;

    public LTagClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lTagClerkTags = rig.LRigTags;
    }

    public IReadOnlyList<LTag> LTagClerkRead()
    {
        return _lTagClerkTags.LTagCatalogRead();
    }

    public IReadOnlyList<LTag> LTagClerkFind(string query, LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(query);
        query = query.Trim();

        List<LTag> found = [];
        foreach (LTag tag in _lTagClerkTags.LTagCatalogRead())
        {
            if (LCatalogTag.LCatalogTagMatch(tag, query))
            {
                found.Add(tag);
            }
        }

        return LCatalogTag.LCatalogTagSort(found, order);
    }

    public IReadOnlyList<LTag> LTagClerkRead(long ownerId, bool collocation)
    {
        return collocation
            ? _lTagClerkTags.LTagCollocationRead(ownerId)
            : _lTagClerkTags.LTagMeaningRead(ownerId);
    }

    public void LTagClerkSave(long ownerId, IReadOnlyList<LTag> written, bool collocation)
    {
        ArgumentNullException.ThrowIfNull(written);

        if (collocation)
        {
            _lTagClerkTags.LTagCollocationSave(ownerId, written);
            return;
        }

        _lTagClerkTags.LTagMeaningSave(ownerId, written);
    }

    public void LTagClerkSave(
        long ownerId, IReadOnlyList<LTagDraft> drafts, bool collocation, Dictionary<long, long> identity)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(identity);

        List<LTag> written = new(drafts.Count);
        foreach (LTagDraft draft in drafts)
        {
            written.Add(new LTag(draft.LTagDraftId, draft.LTagDraftText));
        }

        IReadOnlyList<long> resolved = collocation
            ? _lTagClerkTags.LTagCollocationSave(ownerId, written)
            : _lTagClerkTags.LTagMeaningSave(ownerId, written);

        for (int index = 0; index < drafts.Count; index++)
        {
            if (resolved[index] != 0)
            {
                LIdentity.LIdentityRecord(identity, drafts[index].LTagDraftId, resolved[index]);
            }
        }
    }

    public LTag LTagClerkCreate(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        long id = _lTagClerkTags.LTagResolve(text);
        return _lTagClerkTags.LTagRead(id) ?? throw new LRefusal(LRefusal.LRefusalItem);
    }

    public void LTagClerkChange(long tagId, string renamed)
    {
        _lTagClerkTags.LTagChange(tagId, renamed);
    }

    public void LTagClerkDelete(long tagId)
    {
        _lTagClerkTags.LTagDelete(tagId);
    }
}
