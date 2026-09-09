using System;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSheetQuote
{
    public static void LSheetQuoteAppend(StringBuilder page, LPortraitCard card)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(card);

        if (card.LPortraitCardExample.Count == 0)
        {
            return;
        }

        page.Append("<div class=\"quotes\">\n");

        foreach (LPortraitExample example in card.LPortraitCardExample)
        {
            page.Append("<div class=\"quote\"><span class=\"dot\">•</span>");

            if (example.LPortraitExampleFrame.Length > 0)
            {
                page.Append("<span class=\"frame\">")
                    .Append(LSheet.LSheetNormalize(example.LPortraitExampleFrame))
                    .Append("</span>");
            }

            page.Append("<span class=\"said\">")
                .Append(LSheet.LSheetNormalize(example.LPortraitExampleText))
                .Append("</span></div>\n");
        }

        page.Append("</div>\n");
    }
}
