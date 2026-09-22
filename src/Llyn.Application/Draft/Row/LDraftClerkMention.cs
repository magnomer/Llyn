using System;
using System.Collections.Generic;
using System.Text;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LDraftClerkMention
{
    private readonly LEntryVault _lDraftClerkEntries;
    private readonly LMeaningVault _lDraftClerkMeanings;
    private readonly LIdentity _lDraftClerkIdentity;

    public LDraftClerkMention(LEntryVault entries, LMeaningVault meanings, LIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(meanings);
        ArgumentNullException.ThrowIfNull(identity);
        _lDraftClerkEntries = entries;
        _lDraftClerkMeanings = meanings;
        _lDraftClerkIdentity = identity;
    }

    public LDraft LMentionAdd(LDraft draft, LRequestMentionAddition request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.LRequestOffset < 0 || request.LRequestLength <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        LMentionValidate(request.LRequestEntryId, request.LRequestSenseId);

        return LMentionApply(draft, request.LRequestCardId, request.LRequestSentenceId, example =>
        {
            int end = request.LRequestOffset + request.LRequestLength;
            if (end > LMentionRuneRead(example.LExampleDraftText.LStateValueShow()).Count)
            {
                throw new LRefusal(LRefusal.LRefusalItem);
            }

            List<LMentionDraft> kept = [];
            foreach (LMentionDraft mention in example.LExampleDraftMention)
            {
                if (mention.LMentionDraftOffset >= end
                    || mention.LMentionDraftOffset + mention.LMentionDraftLength <= request.LRequestOffset)
                {
                    kept.Add(mention);
                }
            }

            kept.Add(new LMentionDraft(
                _lDraftClerkIdentity.LIdentityCreate(),
                request.LRequestOffset,
                request.LRequestLength,
                request.LRequestEntryId,
                request.LRequestSenseId));
            return LMentionDraft.LMentionDraftSort(kept);
        });
    }

    public static LDraft LMentionRemove(LDraft draft, LRequestMentionRemoval request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LMentionApply(
            draft,
            request.LRequestCardId,
            request.LRequestSentenceId,
            example => LDraftClerkList.LDraftListRemove(
                example.LExampleDraftMention, request.LRequestMentionId, static row => row.LMentionDraftId));
    }

    public LDraft LMentionChange(LDraft draft, LRequestMentionSense request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LMentionApply(draft, request.LRequestCardId, request.LRequestSentenceId, example =>
            LDraftClerkList.LDraftListChange(
                example.LExampleDraftMention,
                request.LRequestMentionId,
                static row => row.LMentionDraftId,
                mention =>
                {
                    if (mention.LMentionDraftEntry <= 0)
                    {
                        throw new LRefusal(LRefusal.LRefusalLink);
                    }

                    LMentionValidate(mention.LMentionDraftEntry, request.LRequestSenseId);
                    return mention with { LMentionDraftSense = request.LRequestSenseId };
                })
            ?? throw new LRefusal(LRefusal.LRefusalItem));
    }

    private void LMentionValidate(long entryId, long senseId)
    {
        if (entryId < 0 || senseId < 0)
        {
            throw new LRefusal(LRefusal.LRefusalLink);
        }

        if (entryId == 0)
        {
            if (senseId != 0)
            {
                throw new LRefusal(LRefusal.LRefusalLink);
            }

            return;
        }

        if (_lDraftClerkEntries.LEntryRead(entryId) is null)
        {
            throw new LRefusal(LRefusal.LRefusalLink);
        }

        if (senseId == 0)
        {
            return;
        }

        LMeaning? sense = _lDraftClerkMeanings.LMeaningSingleRead(senseId);
        if (sense is null || sense.LMeaningEntryId != entryId)
        {
            throw new LRefusal(LRefusal.LRefusalLink);
        }
    }

    private static LDraft LMentionApply(
        LDraft draft,
        long cardId,
        long sentenceId,
        Func<LExampleDraft, IReadOnlyList<LMentionDraft>> change)
    {
        ArgumentNullException.ThrowIfNull(draft);

        if (cardId == 0 && sentenceId == 0)
        {
            LExample held = draft.LDraftExample ?? throw new LRefusal(LRefusal.LRefusalExample);
            LExampleDraft written = LExampleDraft.LExampleDraftCreate(held);

            return draft with { LDraftExample = held with { LExampleMention = LMentionRead(change(written)) } };
        }

        LEntryDraft content = LDraftClerkSentence.LSentenceChange(draft.LDraftContent, cardId, sentenceId, sentence =>
        {
            LExampleDraft example = sentence.LSentenceDraftExample ?? throw new LRefusal(LRefusal.LRefusalItem);
            return sentence with { LSentenceDraftExample = example with { LExampleDraftMention = change(example) } };
        });

        return draft with { LDraftContent = content };
    }

    public static LExampleDraft LMentionUpdate(LExampleDraft example, LStateValue text)
    {
        ArgumentNullException.ThrowIfNull(example);
        ArgumentNullException.ThrowIfNull(text);

        return example with
        {
            LExampleDraftText = text,
            LExampleDraftMention = LMentionUpdate(
                example.LExampleDraftText.LStateValueShow(), text.LStateValueShow(), example.LExampleDraftMention),
        };
    }

    public static LExample LMentionUpdate(LExample example, LStateValue text)
    {
        ArgumentNullException.ThrowIfNull(example);
        ArgumentNullException.ThrowIfNull(text);

        IReadOnlyList<LMentionDraft> shifted = LMentionUpdate(
            example.LExampleText.LStateValueShow(),
            text.LStateValueShow(),
            LMentionRead(example.LExampleMention));

        return example with { LExampleText = text, LExampleMention = LMentionRead(shifted) };
    }

    public static IReadOnlyList<LMentionDraft> LMentionUpdate(
        string before, string after, IReadOnlyList<LMentionDraft> mentions)
    {
        if (mentions.Count == 0 || string.Equals(before, after, StringComparison.Ordinal))
        {
            return mentions;
        }

        List<Rune> old = LMentionRuneRead(before);
        List<Rune> renewed = LMentionRuneRead(after);
        int shared = Math.Min(old.Count, renewed.Count);

        int prefix = 0;
        while (prefix < shared && old[prefix] == renewed[prefix])
        {
            prefix++;
        }

        int suffix = 0;
        while (suffix < shared - prefix && old[old.Count - 1 - suffix] == renewed[renewed.Count - 1 - suffix])
        {
            suffix++;
        }

        int delta = renewed.Count - old.Count;
        List<LMentionDraft> kept = new(mentions.Count);
        foreach (LMentionDraft mention in mentions)
        {
            int end = mention.LMentionDraftOffset + mention.LMentionDraftLength;
            if (end <= prefix)
            {
                kept.Add(mention);
            }
            else if (mention.LMentionDraftOffset >= old.Count - suffix)
            {
                kept.Add(mention with { LMentionDraftOffset = mention.LMentionDraftOffset + delta });
            }
        }

        return kept;
    }

    public static IReadOnlyList<LMentionDraft> LMentionRead(IReadOnlyList<LMention> mentions)
    {
        ArgumentNullException.ThrowIfNull(mentions);

        List<LMentionDraft> drafts = new(mentions.Count);
        foreach (LMention mention in mentions)
        {
            drafts.Add(LMentionDraft.LMentionDraftCreate(mention));
        }

        return drafts;
    }

    public static IReadOnlyList<LMention> LMentionRead(IReadOnlyList<LMentionDraft> drafts)
    {
        ArgumentNullException.ThrowIfNull(drafts);

        List<LMention> mentions = new(drafts.Count);
        foreach (LMentionDraft draft in drafts)
        {
            mentions.Add(draft.LMentionDraftResolve());
        }

        return mentions;
    }

    private static List<Rune> LMentionRuneRead(string text)
    {
        List<Rune> runes = [];
        foreach (Rune rune in text.EnumerateRunes())
        {
            runes.Add(rune);
        }

        return runes;
    }
}
