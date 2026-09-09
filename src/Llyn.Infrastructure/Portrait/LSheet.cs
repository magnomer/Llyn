using System;
using System.Collections.Generic;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSheet
{
    public static string LSheetFormat(LPortrait portrait, LTheme theme)
    {
        ArgumentNullException.ThrowIfNull(portrait);
        ArgumentNullException.ThrowIfNull(theme);

        StringBuilder page = new StringBuilder();

        page.Append("<!doctype html>\n<html>\n<head>\n<meta charset=\"utf-8\">\n")
            .Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\n")
            .Append("<title>")
            .Append(LSheetNormalize(portrait.LPortraitHeadword))
            .Append("</title>\n<style>")
            .Append(LSheetStyle.LSheetStyleRead(theme))
            .Append("</style>\n</head>\n<body>\n<main class=\"portrait\">\n");

        LSheetCrestAppend(page, portrait);

        LSheetBandAppend(
            page, portrait.LPortraitLabel.LPortraitLabelMeanings, portrait.LPortraitMeaning);
        LSheetBandAppend(
            page, portrait.LPortraitLabel.LPortraitLabelCollocations, portrait.LPortraitCollocation);

        LSheetIncomingAppend(page, portrait);
        LSheetNoteAppend(page, portrait);

        page.Append("</main>\n</body>\n</html>\n");
        return page.ToString();
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

    private static void LSheetCrestAppend(StringBuilder page, LPortrait portrait)
    {
        page.Append("<div class=\"crest\">\n<h1 class=\"headword\">")
            .Append(LSheetNormalize(portrait.LPortraitHeadword))
            .Append("</h1>\n");

        if (portrait.LPortraitLanguage.Length > 0)
        {
            page.Append("<span class=\"tongue\"><i></i><span>")
                .Append(LSheetNormalize(portrait.LPortraitLanguage))
                .Append("</span></span>\n");
        }

        if (portrait.LPortraitFavorite)
        {
            page.Append("<span class=\"star\">★</span>\n");
        }

        page.Append("</div>\n");

        if (portrait.LPortraitPronunciation.Length > 0)
        {
            page.Append("<div class=\"sound\"><em>[</em><b>")
                .Append(LSheetNormalize(portrait.LPortraitPronunciation))
                .Append("</b><em>]</em></div>\n");
        }

        if (portrait.LPortraitSpeech.Count == 0)
        {
            return;
        }

        page.Append("<div class=\"speech\">");
        foreach (string speech in portrait.LPortraitSpeech)
        {
            page.Append("<span>").Append(LSheetNormalize(speech)).Append("</span>");
        }

        page.Append("</div>\n");
    }

    private static void LSheetBandAppend(
        StringBuilder page, string heading, IReadOnlyList<LPortraitCard> cards)
    {
        if (cards.Count == 0)
        {
            return;
        }

        page.Append("<section class=\"band\">\n<h2>")
            .Append(LSheetNormalize(heading))
            .Append("</h2>\n");

        foreach (LPortraitCard card in cards)
        {
            LSheetCard.LSheetCardAppend(page, card);
        }

        page.Append("</section>\n");
    }

    private static void LSheetIncomingAppend(StringBuilder page, LPortrait portrait)
    {
        if (portrait.LPortraitIncoming.Count == 0)
        {
            return;
        }

        page.Append("<section class=\"band\">\n<h2>")
            .Append(LSheetNormalize(portrait.LPortraitLabel.LPortraitLabelIncoming))
            .Append("</h2>\n<div class=\"rows\">\n");

        foreach (LPortraitUsage usage in portrait.LPortraitIncoming)
        {
            page.Append("<div class=\"row\"><span class=\"mark\">→</span>")
                .Append("<span class=\"body\"><b>")
                .Append(LSheetNormalize(usage.LPortraitUsageHeadword))
                .Append("</b>");

            if (usage.LPortraitUsageTitle.Length > 0)
            {
                page.Append("<span>")
                    .Append(LSheetNormalize(usage.LPortraitUsageTitle))
                    .Append("</span>");
            }

            page.Append("</span><span class=\"pill\">")
                .Append(LSheetNormalize(usage.LPortraitUsageOwner))
                .Append("</span><span class=\"speak\">")
                .Append(LSheetNormalize(usage.LPortraitUsageLanguage))
                .Append("</span></div>\n");
        }

        page.Append("</div>\n</section>\n");
    }

    private static void LSheetNoteAppend(StringBuilder page, LPortrait portrait)
    {
        if (portrait.LPortraitNote.Length == 0)
        {
            return;
        }

        page.Append("<section class=\"band\">\n<h2>")
            .Append(LSheetNormalize(portrait.LPortraitLabel.LPortraitLabelNote))
            .Append("</h2>\n<div class=\"note\">")
            .Append(LSheetNormalize(portrait.LPortraitNote))
            .Append("</div>\n</section>\n");
    }
}
