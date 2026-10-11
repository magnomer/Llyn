using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LSheetDeck
{
    internal static void LSheetDeckAppend(
        StringBuilder sheet,
        LLiveryPage livery,
        LPortraitPage portrait,
        string heading,
        IReadOnlyList<LCardDraft> cards,
        bool collocated)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(livery);
        ArgumentNullException.ThrowIfNull(cards);

        if (LSheetSection.LSheetSectionFind(portrait, heading) is not LPortraitSection band)
        {
            return;
        }

        sheet.Append("<section class=\"band deck\">\n<h2>")
            .Append(LSheet.LSheetNormalize(band.LPortraitSectionHeading))
            .Append("</h2>\n");
        for (int index = 0; index < band.LPortraitSectionChild.Count; index++)
        {
            long id = index < cards.Count ? cards[index].LCardDraftId : 0;
            LSheetFaceAppend(sheet, band.LPortraitSectionChild[index], id, livery.LLiveryPageFold, collocated);
        }

        sheet.Append("</section>\n");
    }

    private static void LSheetFaceAppend(
        StringBuilder sheet, LPortraitSection face, long id, IReadOnlySet<long> fold, bool collocated)
    {
        bool stored = id > 0;
        if (stored)
        {
            sheet.Append("<details class=\"card\"")
                .Append(fold.Contains(id) ? string.Empty : " open")
                .Append(">\n<summary>");
        }
        else
        {
            sheet.Append("<article class=\"card\">\n<header>");
        }

        sheet.Append("<span class=\"rank\">")
            .Append(face.LPortraitSectionPosition.ToString(CultureInfo.InvariantCulture))
            .Append("</span>");
        if (face.LPortraitSectionRole == LPortraitRole.LPortraitRoleCard)
        {
            sheet.Append("<span class=\"title\">").Append(LSheet.LSheetNormalize(face.LPortraitSectionHeading))
                .Append("</span>");
        }
        else
        {
            sheet.Append("<span class=\"caption\">").Append(LSheet.LSheetNormalize(face.LPortraitSectionHeading))
                .Append("</span>");
            if (stored)
            {
                sheet.Append("<span class=\"peek\">").Append(LSheet.LSheetNormalize(LSheetPeekRead(face, collocated)))
                    .Append("</span>");
            }
        }

        sheet.Append(stored ? "</summary>\n<section>\n" : "</header>\n<section>\n");
        LSheetSection.LSheetSenseAppend(sheet, face);
        sheet.Append(stored ? "</section>\n</details>\n" : "</section>\n</article>\n");
    }

    private static string LSheetPeekRead(LPortraitSection face, bool collocated)
    {
        if (!collocated)
        {
            return face.LPortraitSectionLine.Count > 0 ? face.LPortraitSectionLine[0].LPortraitLineText : string.Empty;
        }

        foreach (LPortraitSection child in face.LPortraitSectionChild)
        {
            if (child.LPortraitSectionRole == LPortraitRole.LPortraitRolePhrase && child.LPortraitSectionLine.Count > 0)
            {
                return child.LPortraitSectionLine[0].LPortraitLineText;
            }
        }

        return string.Empty;
    }
}
