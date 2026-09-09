using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LFolioCard
{
    public static void LFolioCardAppend(
        StringBuilder body, LPortraitCard card, List<LPortraitAsset> plates, LTheme theme)
    {
        ArgumentNullException.ThrowIfNull(body);
        ArgumentNullException.ThrowIfNull(card);
        ArgumentNullException.ThrowIfNull(plates);
        ArgumentNullException.ThrowIfNull(theme);

        string line = LFolioStyle.LFolioStyleNormalize(theme.LThemeRead("line"));
        string raised = LFolioStyle.LFolioStyleNormalize(theme.LThemeRead("accentSoft"));

        body.Append("<w:tbl><w:tblPr><w:tblW w:w=\"5000\" w:type=\"pct\"/><w:tblBorders>")
            .Append("<w:top w:val=\"single\" w:sz=\"6\" w:color=\"").Append(line).Append("\"/>")
            .Append("<w:left w:val=\"single\" w:sz=\"6\" w:color=\"").Append(line).Append("\"/>")
            .Append("<w:bottom w:val=\"single\" w:sz=\"6\" w:color=\"").Append(line).Append("\"/>")
            .Append("<w:right w:val=\"single\" w:sz=\"6\" w:color=\"").Append(line).Append("\"/>")
            .Append("<w:insideH w:val=\"single\" w:sz=\"6\" w:color=\"").Append(line).Append("\"/>")
            .Append("</w:tblBorders><w:tblCellMar>")
            .Append("<w:top w:w=\"200\" w:type=\"dxa\"/><w:left w:w=\"280\" w:type=\"dxa\"/>")
            .Append("<w:bottom w:w=\"200\" w:type=\"dxa\"/><w:right w:w=\"280\" w:type=\"dxa\"/>")
            .Append("</w:tblCellMar></w:tblPr><w:tblGrid><w:gridCol w:w=\"9360\"/></w:tblGrid>");

        bool named = card.LPortraitCardTitle.Length > 0;

        body.Append("<w:tr><w:tc><w:tcPr><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"")
            .Append(raised).Append("\"/></w:tcPr>");
        LFolioLine.LFolioLineAppend(
            body,
            named ? "CardTitle" : "CardKind",
            card.LPortraitCardPosition.ToString(CultureInfo.InvariantCulture) + "  ",
            named ? card.LPortraitCardTitle : card.LPortraitCardKind);
        body.Append("</w:tc></w:tr>");

        body.Append("<w:tr><w:tc>");

        if (card.LPortraitCardExpression.Length > 0)
        {
            LFolioLine.LFolioLineAppend(body, "Phrase", card.LPortraitCardExpression);
        }

        if (card.LPortraitCardMeaning.Length > 0)
        {
            LFolioLine.LFolioLineAppend(body, "Sense", card.LPortraitCardMeaning);
        }

        if (card.LPortraitCardSituation.Count > 0)
        {
            LFolioLine.LFolioLineAppend(
                body, "Scene", string.Join("   ·   ", card.LPortraitCardSituation));
        }

        if (card.LPortraitCardRegister.Count > 0)
        {
            LFolioLine.LFolioLineAppend(
                body, "Tone", string.Join("   ·   ", card.LPortraitCardRegister));
        }

        if (card.LPortraitCardTranslation.Count > 0)
        {
            List<string> bridges = new List<string>();
            foreach (LPortraitLink link in card.LPortraitCardTranslation)
            {
                bridges.Add(link.LPortraitLinkHeadword + " (" + link.LPortraitLinkLanguage + ")");
            }

            LFolioLine.LFolioLineAppend(body, "Bridge", "→ " + string.Join("   ·   ", bridges));
        }

        foreach (LPortraitExample example in card.LPortraitCardExample)
        {
            string mark = example.LPortraitExampleFrame.Length > 0
                ? "•  " + example.LPortraitExampleFrame + "  "
                : "•  ";

            LFolioLine.LFolioLineAppend(body, "Quote", mark, example.LPortraitExampleText);
        }

        if (card.LPortraitCardTag.Count > 0)
        {
            LFolioLine.LFolioLineAppend(body, "Label", string.Join("   ·   ", card.LPortraitCardTag));
        }

        foreach (LPortraitMedia image in card.LPortraitCardImage)
        {
            if (LPortraitAsset.LPortraitAssetLoad(image.LPortraitMediaLocation) is not LPortraitAsset held)
            {
                LFolioLine.LFolioLineAppend(body, "Plate", image.LPortraitMediaLocation);
                continue;
            }

            plates.Add(held);
            (int width, int height) = LPortraitSize.LPortraitSizeRead(held.LPortraitAssetData);
            (long across, long down) = LFolioCardClamp(width, height);
            LFolioLine.LFolioLineDraw(body, plates.Count, across, down);
        }

        foreach (LPortraitMedia video in card.LPortraitCardVideo)
        {
            string span = video.LPortraitMediaSpan.Length > 0
                ? "  ·  " + video.LPortraitMediaSpan
                : string.Empty;

            LFolioLine.LFolioLineAppend(body, "Plate", "▶  ", video.LPortraitMediaLocation + span);
        }

        body.Append("</w:tc></w:tr></w:tbl>");
        body.Append("<w:p><w:pPr><w:spacing w:after=\"0\"/></w:pPr></w:p>");
    }

    private static (long Across, long Down) LFolioCardClamp(int width, int height)
    {
        const long ceiling = 4114800L;

        if (width <= 0 || height <= 0)
        {
            return (ceiling, ceiling * 9 / 16);
        }

        long across = (long)width * 9525L;
        long down = (long)height * 9525L;

        if (across <= ceiling)
        {
            return (across, down);
        }

        return (ceiling, down * ceiling / across);
    }
}
