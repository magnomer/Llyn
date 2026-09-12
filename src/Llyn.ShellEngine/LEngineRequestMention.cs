using System;
using System.Collections.Generic;
using System.Text;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private LDraft LEngineMentionAdd(LDraft draft, LRequestMentionAddition request)
    {
        if (request.LRequestOffset < 0 || request.LRequestLength <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        LEngineMentionValidate(request.LRequestEntryId, request.LRequestSenseId);

        return LEngineMentionApply(draft, request.LRequestCardId, request.LRequestSentenceId, example =>
        {
            int end = request.LRequestOffset + request.LRequestLength;
            if (end > LEngineRuneRead(example.LExampleDraftText.LStateValueShow()).Count)
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
                LEngineIdentityCreate(),
                request.LRequestOffset,
                request.LRequestLength,
                request.LRequestEntryId,
                request.LRequestSenseId));
            return LMentionDraft.LMentionDraftSort(kept);
        });
    }

    private static LDraft LEngineMentionRemove(LDraft draft, LRequestMentionRemoval request)
    {
        return LEngineMentionApply(
            draft,
            request.LRequestCardId,
            request.LRequestSentenceId,
            example => LEngineListRemove(
                example.LExampleDraftMention, request.LRequestMentionId, static row => row.LMentionDraftId));
    }

    private LDraft LEngineMentionChange(LDraft draft, LRequestMentionSense request)
    {
        return LEngineMentionApply(draft, request.LRequestCardId, request.LRequestSentenceId, example =>
            LEngineListChange(
                example.LExampleDraftMention,
                request.LRequestMentionId,
                static row => row.LMentionDraftId,
                mention =>
                {
                    if (mention.LMentionDraftEntry <= 0)
                    {
                        throw new LRefusal(LRefusal.LRefusalLink);
                    }

                    LEngineMentionValidate(mention.LMentionDraftEntry, request.LRequestSenseId);
                    return mention with { LMentionDraftSense = request.LRequestSenseId };
                })
            ?? throw new LRefusal(LRefusal.LRefusalItem));
    }

    private void LEngineMentionValidate(long entryId, long senseId)
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

        if (new LEntryArchive(_lEngineDatabase).LEntryRead(entryId) is null)
        {
            throw new LRefusal(LRefusal.LRefusalLink);
        }

        if (senseId == 0)
        {
            return;
        }

        LMeaning? sense = new LMeaningArchive(_lEngineDatabase).LMeaningSingleRead(senseId);
        if (sense is null || sense.LMeaningEntryId != entryId)
        {
            throw new LRefusal(LRefusal.LRefusalLink);
        }
    }

    private static LDraft LEngineMentionApply(
        LDraft draft,
        long cardId,
        long sentenceId,
        Func<LExampleDraft, IReadOnlyList<LMentionDraft>> change)
    {
        if (cardId == 0 && sentenceId == 0)
        {
            LExample held = draft.LDraftExample ?? throw new LRefusal(LRefusal.LRefusalExample);
            LExampleDraft written = LExampleDraft.LExampleDraftCreate(held);

            return draft with { LDraftExample = held with { LExampleMention = LEngineMentionRead(change(written)) } };
        }

        LEntryDraft content = LEngineSentenceChange(draft.LDraftContent, cardId, sentenceId, sentence =>
        {
            LExampleDraft example = sentence.LSentenceDraftExample ?? throw new LRefusal(LRefusal.LRefusalItem);
            return sentence with { LSentenceDraftExample = example with { LExampleDraftMention = change(example) } };
        });

        return draft with { LDraftContent = content };
    }

    private static LExampleDraft LEngineMentionUpdate(LExampleDraft example, LStateValue text)
    {
        return example with
        {
            LExampleDraftText = text,
            LExampleDraftMention = LEngineMentionUpdate(
                example.LExampleDraftText.LStateValueShow(), text.LStateValueShow(), example.LExampleDraftMention),
        };
    }

    private static LExample LEngineMentionUpdate(LExample example, LStateValue text)
    {
        IReadOnlyList<LMentionDraft> shifted = LEngineMentionUpdate(
            example.LExampleText.LStateValueShow(),
            text.LStateValueShow(),
            LEngineMentionRead(example.LExampleMention));

        return example with { LExampleText = text, LExampleMention = LEngineMentionRead(shifted) };
    }

    private static IReadOnlyList<LMentionDraft> LEngineMentionUpdate(
        string before, string after, IReadOnlyList<LMentionDraft> mentions)
    {
        if (mentions.Count == 0 || string.Equals(before, after, StringComparison.Ordinal))
        {
            return mentions;
        }

        List<Rune> old = LEngineRuneRead(before);
        List<Rune> renewed = LEngineRuneRead(after);
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

    private static IReadOnlyList<LMentionDraft> LEngineMentionRead(IReadOnlyList<LMention> mentions)
    {
        List<LMentionDraft> drafts = new(mentions.Count);
        foreach (LMention mention in mentions)
        {
            drafts.Add(LMentionDraft.LMentionDraftCreate(mention));
        }

        return drafts;
    }

    private static IReadOnlyList<LMention> LEngineMentionRead(IReadOnlyList<LMentionDraft> drafts)
    {
        List<LMention> mentions = new(drafts.Count);
        foreach (LMentionDraft draft in drafts)
        {
            mentions.Add(draft.LMentionDraftResolve());
        }

        return mentions;
    }

    private static List<Rune> LEngineRuneRead(string text)
    {
        List<Rune> runes = [];
        foreach (Rune rune in text.EnumerateRunes())
        {
            runes.Add(rune);
        }

        return runes;
    }
}
