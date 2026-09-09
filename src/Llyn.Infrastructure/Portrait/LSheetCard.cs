using System;
using System.Globalization;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSheetCard
{
    public static void LSheetCardAppend(StringBuilder page, LPortraitCard card)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(card);

        bool named = card.LPortraitCardTitle.Length > 0;

        page.Append("<article class=\"card\">\n<header><span class=\"rank\">")
            .Append(card.LPortraitCardPosition.ToString(CultureInfo.InvariantCulture))
            .Append("</span><span class=\"title")
            .Append(named ? string.Empty : " kind")
            .Append("\">")
            .Append(LSheet.LSheetNormalize(named ? card.LPortraitCardTitle : card.LPortraitCardKind))
            .Append("</span></header>\n<section>\n");

        if (card.LPortraitCardExpression.Length > 0)
        {
            page.Append("<p class=\"phrase\">")
                .Append(LSheet.LSheetNormalize(card.LPortraitCardExpression))
                .Append("</p>\n");
        }

        if (card.LPortraitCardMeaning.Length > 0)
        {
            page.Append("<p class=\"sense\">")
                .Append(LSheet.LSheetNormalize(card.LPortraitCardMeaning))
                .Append("</p>\n");
        }

        if (card.LPortraitCardSituation.Count > 0)
        {
            page.Append("<div class=\"scene\">");
            foreach (string situation in card.LPortraitCardSituation)
            {
                page.Append("<span>").Append(LSheet.LSheetNormalize(situation)).Append("</span>");
            }

            page.Append("</div>\n");
        }

        if (card.LPortraitCardTranslation.Count > 0)
        {
            page.Append("<div class=\"bridge\">");
            foreach (LPortraitLink link in card.LPortraitCardTranslation)
            {
                page.Append("<span><b>")
                    .Append(LSheet.LSheetNormalize(link.LPortraitLinkHeadword))
                    .Append("</b><i>")
                    .Append(LSheet.LSheetNormalize(link.LPortraitLinkLanguage))
                    .Append("</i></span>");
            }

            page.Append("</div>\n");
        }

        LSheetQuote.LSheetQuoteAppend(page, card);

        if (card.LPortraitCardTag.Count > 0)
        {
            page.Append("<div class=\"labels\">");
            foreach (string tag in card.LPortraitCardTag)
            {
                page.Append("<span>").Append(LSheet.LSheetNormalize(tag)).Append("</span>");
            }

            page.Append("</div>\n");
        }

        LSheetPlate.LSheetPlateAppend(page, card);

        page.Append("</section>\n</article>\n");
    }
}
