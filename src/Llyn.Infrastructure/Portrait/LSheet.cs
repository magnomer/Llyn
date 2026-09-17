using System;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSheet
{
    public static string LSheetFormat(LPortraitPage page, LTheme theme)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(theme);

        StringBuilder sheet = new StringBuilder();

        sheet.Append("<!doctype html>\n<html>\n<head>\n<meta charset=\"utf-8\">\n")
            .Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\n")
            .Append("<title>")
            .Append(LSheetNormalize(page.LPortraitPageTitle))
            .Append("</title>\n<style>")
            .Append(LSheetStyle.LSheetStyleRead(theme))
            .Append("</style>\n</head>\n<body>\n<main class=\"portrait\">\n");

        LSheetCrestAppend(sheet, page);

        foreach (LPortraitSection section in page.LPortraitPageSection)
        {
            LSheetSection.LSheetSectionAppend(sheet, section);
        }

        sheet.Append("</main>\n</body>\n</html>\n");
        return sheet.ToString();
    }

    public static string LSheetNormalize(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        StringBuilder safe = new StringBuilder(text.Length);

        foreach (char letter in text)
        {
            switch (letter)
            {
                case '&':
                    safe.Append("&amp;");
                    break;
                case '<':
                    safe.Append("&lt;");
                    break;
                case '>':
                    safe.Append("&gt;");
                    break;
                case '"':
                    safe.Append("&quot;");
                    break;
                case '\'':
                    safe.Append("&#39;");
                    break;
                default:
                    safe.Append(letter);
                    break;
            }
        }

        return safe.ToString();
    }

    private static void LSheetCrestAppend(StringBuilder sheet, LPortraitPage page)
    {
        sheet.Append("<div class=\"crest\">\n<h1 class=\"headword\">")
            .Append(LSheetNormalize(page.LPortraitPageTitle))
            .Append("</h1>\n");

        if (page.LPortraitPageLanguage.Length > 0)
        {
            sheet.Append("<span class=\"tongue\"><i></i><span>")
                .Append(LSheetNormalize(page.LPortraitPageLanguage))
                .Append("</span></span>\n");
        }

        if (page.LPortraitPageFavorite)
        {
            sheet.Append("<span class=\"star\">★</span>\n");
        }

        sheet.Append("</div>\n");

        foreach (LPortraitLine line in page.LPortraitPageLine)
        {
            sheet.Append("<div class=\"sound\">");

            if (line.LPortraitLineLabel.Length > 0)
            {
                sheet.Append("<span>").Append(LSheetNormalize(line.LPortraitLineLabel)).Append("</span> ");
            }

            LSheetMarkAppend(sheet, line.LPortraitLineOpener);
            sheet.Append("<b>").Append(LSheetNormalize(line.LPortraitLineText)).Append("</b>");
            LSheetMarkAppend(sheet, line.LPortraitLineCloser);
            sheet.Append("</div>\n");
        }

        if (page.LPortraitPageChip.Count == 0)
        {
            return;
        }

        sheet.Append("<div class=\"speech\">");
        foreach (string chip in page.LPortraitPageChip)
        {
            sheet.Append("<span>").Append(LSheetNormalize(chip)).Append("</span>");
        }

        sheet.Append("</div>\n");
    }

    private static void LSheetMarkAppend(StringBuilder sheet, string mark)
    {
        if (mark.Length > 0)
        {
            sheet.Append("<em>").Append(LSheetNormalize(mark)).Append("</em>");
        }
    }
}
