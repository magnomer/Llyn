using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;

namespace Llyn.UIDeportment;

public static class LSCustoms
{
    public static string LSCustomsLossResolve(LWindow? window, LMarkupMode mode, long target, string format)
    {
        ArgumentNullException.ThrowIfNull(format);

        if (window is null || mode != LMarkupMode.LMarkupModeReplace || target <= 0
            || window.LWindowEntryLoad(target) is not LEntryDraft stored)
        {
            return string.Empty;
        }

        return string.Format(
            CultureInfo.CurrentCulture,
            format,
            LSCustomsCardScan(stored.LEntryDraftMeanings),
            LSCustomsCardScan(stored.LEntryDraftCollocations));
    }

    public static int LSCustomsCardScan(IReadOnlyList<LCardDraft> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);

        int count = 0;
        foreach (LCardDraft card in cards)
        {
            count += 1 + LSCustomsCardScan(card.LCardDraftChild);
        }

        return count;
    }
}
