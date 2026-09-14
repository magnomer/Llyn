using System;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSheetPage
{
    public static string LSheetPageFormat(LPortraitPage page, LTheme theme)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(theme);

        StringBuilder sheet = new StringBuilder();

        sheet.Append("<!doctype html>\n<html>\n<head>\n<meta charset=\"utf-8\">\n")
            .Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\n")
            .Append("<title>")
            .Append(LSheet.LSheetNormalize(page.LPortraitPageTitle))
            .Append("</title>\n<style>")
            .Append(LSheetStyle.LSheetStyleRead(theme))
            .Append("</style>\n</head>\n<body>\n<main class=\"portrait\">\n");

        LSheetCrestAppend(sheet, page);

        foreach (LPortraitSection section in page.LPortraitPageSection)
        {
            LSheetSectionAppend(sheet, section);
        }

        sheet.Append("</main>\n</body>\n</html>\n");
        return sheet.ToString();
    }

    private static void LSheetCrestAppend(StringBuilder sheet, LPortraitPage page)
    {
        sheet.Append("<div class=\"crest\">\n<h1 class=\"headword\">")
            .Append(LSheet.LSheetNormalize(page.LPortraitPageTitle))
            .Append("</h1>\n</div>\n");

        if (page.LPortraitPageLanguage.Length == 0 && page.LPortraitPageChip.Count == 0)
        {
            return;
        }

        sheet.Append("<div class=\"speech\">");

        if (page.LPortraitPageLanguage.Length > 0)
        {
            sheet.Append("<span>").Append(LSheet.LSheetNormalize(page.LPortraitPageLanguage)).Append("</span>");
        }

        foreach (string chip in page.LPortraitPageChip)
        {
            sheet.Append("<span>").Append(LSheet.LSheetNormalize(chip)).Append("</span>");
        }

        sheet.Append("</div>\n");
    }

    private static void LSheetSectionAppend(StringBuilder sheet, LPortraitSection section)
    {
        sheet.Append("<section class=\"band\">\n");

        if (section.LPortraitSectionHeading.Length > 0)
        {
            sheet.Append("<h2>")
                .Append(LSheet.LSheetNormalize(section.LPortraitSectionHeading))
                .Append("</h2>\n");
        }

        if (section.LPortraitSectionLine.Count > 0)
        {
            sheet.Append("<div class=\"lines\">\n");

            foreach (LPortraitLine line in section.LPortraitSectionLine)
            {
                sheet.Append("<div class=\"line\">");

                if (line.LPortraitLineLabel.Length > 0)
                {
                    sheet.Append("<span class=\"tag\">")
                        .Append(LSheet.LSheetNormalize(line.LPortraitLineLabel))
                        .Append("</span>");
                }

                sheet.Append("<span>")
                    .Append(LSheet.LSheetNormalize(line.LPortraitLineText))
                    .Append("</span></div>\n");
            }

            sheet.Append("</div>\n");
        }

        if (section.LPortraitSectionNote.Length > 0)
        {
            sheet.Append("<div class=\"note\">\n");
            LSheetNote.LSheetNoteAppend(sheet, section.LPortraitSectionNote);
            sheet.Append("</div>\n");
        }

        LSheetPlate.LSheetPlateAppend(sheet, section.LPortraitSectionImage, section.LPortraitSectionVideo);

        sheet.Append("</section>\n");
    }
}
