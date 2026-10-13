using System;
using System.Collections.Generic;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LOutline
{
    public static string LOutlineFormat(LPortraitPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        StringBuilder outline = new StringBuilder();

        outline.Append("# ").Append(LOutlineNormalize(page.LPortraitPageTitle));

        if (page.LPortraitPageFavorite)
        {
            outline.Append(" ★");
        }

        outline.Append("\n\n");

        List<string> crest = new List<string>();
        if (page.LPortraitPageLanguage.Length > 0)
        {
            crest.Add("*" + LOutlineNormalize(page.LPortraitPageLanguage) + "*");
        }

        foreach (LPortraitLine line in page.LPortraitPageLine)
        {
            string text = "**" + line.LPortraitLineOpener + LOutlineNormalize(line.LPortraitLineText)
                + line.LPortraitLineCloser + "**";
            crest.Add(line.LPortraitLineLabel.Length == 0
                ? text
                : "*" + LOutlineNormalize(line.LPortraitLineLabel) + "* " + text);
        }

        foreach (string chip in page.LPortraitPageChip)
        {
            crest.Add(LOutlineNormalize(chip));
        }

        if (crest.Count > 0)
        {
            outline.Append(string.Join(" · ", crest)).Append("\n\n");
        }

        foreach (LPortraitSection section in page.LPortraitPageSection)
        {
            LOutlineSection.LOutlineSectionAppend(outline, section, 0);
        }

        return outline.ToString();
    }

    public static string LOutlineNormalize(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        StringBuilder safe = new StringBuilder(text.Length);

        foreach (char letter in text)
        {
            if (letter is '\\' or '`' or '*' or '_' or '[' or ']' or '<' or '>' or '#' or '|')
            {
                safe.Append('\\');
            }

            safe.Append(letter);
        }

        return safe.ToString();
    }
}
