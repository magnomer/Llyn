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
                if (!ids.Contains(id))
                {
                    ids.Add(id);
                }
            }
        }
    }
}
