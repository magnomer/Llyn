using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LDraft(
    long LDraftId,
    string LDraftOrigin,
    long LDraftEntryId,
    LEntryDraft LDraftContent,
    DateTimeOffset LDraftMoment,
    LExample? LDraftExample = null,
    LSituation? LDraftSituation = null,
    LReference? LDraftReference = null,
    int LDraftVersion = 0,
    IReadOnlyList<LAuthor>? LDraftAuthor = null,
    LAuthor? LDraftAuthorHeld = null)
{
    public long? LDraftStored => LDraftEntryId == 0 ? null : LDraftEntryId;

    public string LDraftAuthorName => LDraftAuthorHeld?.LAuthorName ?? string.Empty;

    public string LDraftMentionLanguage => LDraftExample?.LExampleLanguage ?? LDraftContent.LEntryDraftLanguage;

    public IReadOnlyList<LAuthor> LDraftAuthor { get; init; } = LDraftAuthor ?? [];

    public IReadOnlyList<LAuthorRow> LDraftCreditRead()
    {
        return LAuthorRow.LAuthorRowCreate(LDraftAuthor);
    }

    public LExampleDraft? LDraftExampleRead()
    {
        return LDraftExample is LExample held ? LExampleDraft.LExampleDraftCreate(held) : null;
    }

    public LExampleDraft? LDraftExampleRead(LCardKind kind, long cardId, long sentenceId)
    {
        IReadOnlyList<LCardDraft> cards = kind == LCardKind.LCardKindCollocation
            ? LDraftContent.LEntryDraftCollocations
            : LDraftContent.LEntryDraftMeanings;
        return LDraftSentenceRead(cards, cardId, sentenceId)?.LSentenceDraftExample;
    }

    public LDraft LDraftNormalize()
    {
        return this with
        {
            LDraftContent = LDraftContent.LEntryDraftNormalize(),
            LDraftExample = LDraftExample?.LExampleNormalize(),
            LDraftSituation = LDraftSituation?.LSituationNormalize(),
            LDraftReference = LDraftReference?.LReferenceNormalize(),
        };
    }

    private static LSentenceDraft? LDraftSentenceRead(IReadOnlyList<LCardDraft> cards, long cardId, long sentenceId)
    {
        foreach (LCardDraft card in cards)
        {
            if (card.LCardDraftId != cardId)
            {
                if (LDraftSentenceRead(card.LCardDraftChild, cardId, sentenceId) is LSentenceDraft found)
                {
                    return found;
                }

                continue;
            }

            foreach (LSentenceDraft sentence in card.LCardDraftSentence)
            {
                if (sentence.LSentenceDraftId == sentenceId)
                {
                    return sentence;
                }
            }

            return null;
        }

        return null;
    }
}
