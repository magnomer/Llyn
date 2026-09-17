using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LPortraitLink(
    long LPortraitLinkId,
    string LPortraitLinkHeadword,
    string LPortraitLinkLanguage)
{
    public static void LPortraitLinkRead(IReadOnlyList<LCardDraft> cards, List<long> ids)
    {
        ArgumentNullException.ThrowIfNull(cards);
        ArgumentNullException.ThrowIfNull(ids);

        foreach (LCardDraft card in cards)
        {
            foreach (long id in card.LCardDraftTranslation)
            {
                LPortraitLinkAdd(ids, id);
            }

            foreach (LSentenceDraft sentence in card.LCardDraftSentence)
            {
                if (sentence.LSentenceDraftExample is not LExampleDraft example)
                {
                    continue;
                }

                foreach (LMentionDraft mention in example.LExampleDraftMention)
                {
                    LPortraitLinkAdd(ids, mention.LMentionDraftEntry);
                }
            }

            LPortraitLinkRead(card.LCardDraftChild, ids);
        }
    }

    private static void LPortraitLinkAdd(List<long> ids, long id)
    {
        if (id > 0 && !ids.Contains(id))
        {
            ids.Add(id);
        }
    }
}
