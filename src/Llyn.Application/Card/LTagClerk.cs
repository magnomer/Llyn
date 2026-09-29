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

    public LTagOffer LTagClerkFind(string text, LDraft? draft, long card)
    {
        ArgumentNullException.ThrowIfNull(text);

        string word = text.Trim();
        if (word.Length == 0)
        {
            return new LTagOffer(text, [], false);
        }

        IReadOnlyList<LTagDraft> held = draft is null
            ? []
            : LDraftClerkCard.LCardFind(draft.LDraftContent, card)?.LCardDraftTag ?? [];

        List<LTagRow> rows = [];
        foreach (LTag tag in LTagClerkFind(word, LCatalogOrder.LCatalogOrderUsage))
        {
            string written = tag.LTagText.Trim();
            if (written.Length == 0 || LDraftClerkList.LDraftHeldCheck(held, static row => row.LTagDraftText, written))
            {
                continue;
            }

            (string lead, string mark, string tail) = LCatalog.LCatalogMarkFind(written, word);
            rows.Add(new LTagRow(tag.LTagId, lead, mark, tail));
            if (rows.Count == LCatalog.LCatalogOfferLimit)
            {
                break;
            }
        }

        return new LTagOffer(text, rows, rows.Count > 0);
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
}
