using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LOutline
{
    public static string LOutlineFormat(LPortrait portrait)
    {
        ArgumentNullException.ThrowIfNull(portrait);

        StringBuilder page = new StringBuilder();

        page.Append("# ").Append(LOutlineNormalize(portrait.LPortraitHeadword));

        if (portrait.LPortraitFavorite)
        {
            page.Append(" ★");
        }

        page.Append("\n\n");

        List<string> crest = new List<string>();
        if (portrait.LPortraitLanguage.Length > 0)
        {
            crest.Add("*" + LOutlineNormalize(portrait.LPortraitLanguage) + "*");
        }

        if (portrait.LPortraitPronunciation.Length > 0)
        {
            crest.Add("**[" + LOutlineNormalize(portrait.LPortraitPronunciation) + "]**");
        }

        if (portrait.LPortraitSpeech.Count > 0)
        {
            crest.Add(LOutlineNormalize(string.Join(", ", portrait.LPortraitSpeech)));
        }

        if (crest.Count > 0)
        {
            page.Append(string.Join(" · ", crest)).Append("\n\n");
        }

        LOutlineBandAppend(
            page, portrait.LPortraitLabel.LPortraitLabelMeanings, portrait.LPortraitMeaning);
        LOutlineBandAppend(
            page, portrait.LPortraitLabel.LPortraitLabelCollocations, portrait.LPortraitCollocation);

        if (portrait.LPortraitIncoming.Count > 0)
        {
            page.Append("## ")
                .Append(LOutlineNormalize(portrait.LPortraitLabel.LPortraitLabelIncoming))
                .Append("\n\n");

            foreach (LPortraitUsage usage in portrait.LPortraitIncoming)
            {
                page.Append("- **")
                    .Append(LOutlineNormalize(usage.LPortraitUsageHeadword))
                    .Append("**");

                if (usage.LPortraitUsageTitle.Length > 0)
                {
                    page.Append(" — ").Append(LOutlineNormalize(usage.LPortraitUsageTitle));
                }

                page.Append(" *(")
                    .Append(LOutlineNormalize(usage.LPortraitUsageOwner))
                    .Append(", ")
                    .Append(LOutlineNormalize(usage.LPortraitUsageLanguage))
                    .Append(")*\n");
            }

            page.Append('\n');
        }

        if (portrait.LPortraitNote.Length > 0)
        {
            page.Append("## ")
                .Append(LOutlineNormalize(portrait.LPortraitLabel.LPortraitLabelNote))
                .Append("\n\n")
                .Append(LOutlineNormalize(portrait.LPortraitNote))
                .Append("\n");
        }

        return page.ToString();
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

    private static void LOutlineBandAppend(
        StringBuilder page, string heading, IReadOnlyList<LPortraitCard> cards)
    {
        if (cards.Count == 0)
        {
            return;
        }

        page.Append("## ").Append(LOutlineNormalize(heading)).Append("\n\n");

        foreach (LPortraitCard card in cards)
        {
            LOutlineCardAppend(page, card);
        }
    }

    private static void LOutlineCardAppend(StringBuilder page, LPortraitCard card)
    {
        bool named = card.LPortraitCardTitle.Length > 0;

        page.Append("### ")
            .Append(card.LPortraitCardPosition.ToString(CultureInfo.InvariantCulture))
            .Append(" · ")
            .Append(LOutlineNormalize(named ? card.LPortraitCardTitle : card.LPortraitCardKind))
            .Append("\n\n");

        if (card.LPortraitCardExpression.Length > 0)
        {
            page.Append("**")
                .Append(LOutlineNormalize(card.LPortraitCardExpression))
                .Append("**\n\n");
        }

        if (card.LPortraitCardMeaning.Length > 0)
        {
            page.Append(LOutlineNormalize(card.LPortraitCardMeaning)).Append("\n\n");
        }

        if (card.LPortraitCardSituation.Count > 0)
        {
            List<string> scenes = new List<string>();
            foreach (string situation in card.LPortraitCardSituation)
            {
                scenes.Add(LOutlineNormalize(situation));
            }

            page.Append("*").Append(string.Join(" · ", scenes)).Append("*\n\n");
        }

        if (card.LPortraitCardTranslation.Count > 0)
        {
            List<string> bridges = new List<string>();
            foreach (LPortraitLink link in card.LPortraitCardTranslation)
            {
                bridges.Add(LOutlineNormalize(link.LPortraitLinkHeadword)
                    + " (" + LOutlineNormalize(link.LPortraitLinkLanguage) + ")");
            }

            page.Append("→ ").Append(string.Join(" · ", bridges)).Append("\n\n");
        }

        foreach (LPortraitExample example in card.LPortraitCardExample)
        {
            page.Append("- ");

            if (example.LPortraitExampleFrame.Length > 0)
            {
                page.Append("**")
                    .Append(LOutlineNormalize(example.LPortraitExampleFrame))
                    .Append("** ");
            }

            page.Append(LOutlineNormalize(example.LPortraitExampleText)).Append('\n');
        }

        if (card.LPortraitCardExample.Count > 0)
        {
            page.Append('\n');
        }

        if (card.LPortraitCardTag.Count > 0)
        {
            List<string> labels = new List<string>();
            foreach (string tag in card.LPortraitCardTag)
            {
                labels.Add("`" + tag + "`");
            }

            page.Append(string.Join(" ", labels)).Append("\n\n");
        }

        foreach (LPortraitMedia image in card.LPortraitCardImage)
        {
            page.Append("![](").Append(image.LPortraitMediaLocation).Append(")\n\n");
        }

        foreach (LPortraitMedia video in card.LPortraitCardVideo)
        {
            page.Append("[▶ ")
                .Append(LOutlineNormalize(video.LPortraitMediaLocation))
                .Append("](")
                .Append(video.LPortraitMediaLocation)
                .Append(')');

            if (video.LPortraitMediaSpan.Length > 0)
            {
                page.Append(" · ").Append(LOutlineNormalize(video.LPortraitMediaSpan));
            }

            page.Append("\n\n");
        }
    }
}
