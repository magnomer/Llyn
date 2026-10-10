using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LLiveryCard
{
    public static void LLiveryCardAppend(
        StringBuilder sheet, LLiveryPage page, Func<long, string> note, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(lookup);

        LEntryDraft draft = page.LLiveryPageDraft;
        LLiveryDeckAppend(
            sheet, page, draft.LEntryDraftMeanings, lookup("Display.MeaningPlural"),
            lookup("Display.MeaningSingle"), note);
        LLiveryDeckAppend(
            sheet, page, draft.LEntryDraftCollocations, lookup("Display.Collocation"),
            lookup("Display.CollocationSingle"), note);
        LLiveryIncomingAppend(sheet, page, note, lookup);
    }

    private static void LLiveryDeckAppend(
        StringBuilder sheet,
        LLiveryPage page,
        IReadOnlyList<LCardDraft> cards,
        string heading,
        string blank,
        Func<long, string> note)
    {
        if (cards.Count == 0)
        {
            return;
        }

        sheet.Append("## ").Append(LLiveryHeader.LLiveryTextFormat(heading)).Append("\n\n");
        for (int place = 0; place < cards.Count; place++)
        {
            LLiveryFaceAppend(sheet, page, cards[place], LLiveryNumberFormat(cards[place], place), blank, note);
        }
    }

    private static void LLiveryFaceAppend(
        StringBuilder sheet, LLiveryPage page, LCardDraft card, string number, string blank, Func<long, string> note)
    {
        string title = card.LCardDraftTitle.LStateValueShow();
        string expression = card.LCardDraftExpression.LStateValueShow();
        string head = title.Length > 0 ? title : expression;
        bool stored = card.LCardDraftId > 0;
        if (stored)
        {
            sheet.Append("<details class=\"llyn-card\"")
                .Append(page.LLiveryPageFold.Contains(card.LCardDraftId) ? string.Empty : " open")
                .Append(">\n<summary>");
        }
        else
        {
            sheet.Append("<div class=\"llyn-card\">\n\n");
        }

        sheet.Append("<span class=\"llyn-number\">")
            .Append(LLiveryHeader.LLiveryTextFormat(number)).Append("</span> ")
            .Append(head.Length > 0 ? "<span class=\"llyn-title\">" : "<span class=\"llyn-title llyn-blank\">")
            .Append(LLiveryHeader.LLiveryTextFormat(head.Length > 0 ? head : blank)).Append("</span>")
            .Append(stored ? "</summary>\n\n" : "\n\n");
        if (title.Length > 0 && expression.Length > 0)
        {
            sheet.Append("<span class=\"llyn-expression\">").Append(LLiveryHeader.LLiveryTextFormat(expression))
                .Append("</span>\n\n");
        }

        string meaning = card.LCardDraftMeaning.LStateValueShow();
        if (meaning.Length > 0)
        {
            sheet.Append(LLiveryEtymology.LLiveryMentionFormat(meaning, [], note)).Append("\n\n");
        }

        List<string> situations = [];
        foreach (LSituationDraft situation in card.LCardDraftSituation)
        {
            situations.Add(situation.LSituationDraftTitle.LStateValueShow());
        }

        LLiveryBadgeAppend(sheet, "llyn-situation", situations);
        List<string> registers = [];
        foreach (LRegisterDraft register in card.LCardDraftRegister)
        {
            registers.Add(register.LRegisterDraftName.LStateValueShow());
        }

        LLiveryBadgeAppend(sheet, "llyn-register", registers);
        LLiveryTargetAppend(sheet, page, card, note);

        for (int place = 0; place < card.LCardDraftChild.Count; place++)
        {
            LCardDraft child = card.LCardDraftChild[place];
            LLiveryFaceAppend(sheet, page, child, number + "." + LLiveryNumberFormat(child, place), blank, note);
        }

        foreach (LSentenceDraft sentence in card.LCardDraftSentence)
        {
            LLiverySentenceAppend(sheet, page, sentence, note);
        }

        if (card.LCardDraftSentence.Count > 0)
        {
            sheet.Append('\n');
        }

        List<string> tags = [];
        foreach (LTagDraft tag in card.LCardDraftTag)
        {
            tags.Add(tag.LTagDraftText);
        }

        LLiveryBadgeAppend(sheet, "llyn-tag", tags);
        LLiveryMediaAppend(sheet, card);
        sheet.Append(stored ? "</details>\n\n" : "</div>\n\n");
    }

    private static string LLiveryNumberFormat(LCardDraft card, int place)
    {
        int number = card.LCardDraftPosition > 0 ? card.LCardDraftPosition : place + 1;
        return number.ToString(CultureInfo.InvariantCulture);
    }

    private static void LLiverySentenceAppend(
        StringBuilder sheet, LLiveryPage page, LSentenceDraft sentence, Func<long, string> note)
    {
        if (sentence.LSentenceDraftEmpty)
        {
            return;
        }

        sheet.Append("- ");
        foreach ((string style, LStateValue value) in new[]
                 {
                     ("llyn-particle", sentence.LSentenceDraftParticle),
                     ("llyn-dependence", sentence.LSentenceDraftDependence),
                 })
        {
            if (value.LStateValueShow().Length > 0)
            {
                sheet.Append("<span class=\"").Append(style).Append("\">")
                    .Append(LLiveryHeader.LLiveryTextFormat(value.LStateValueShow())).Append("</span> ");
            }
        }

        LExampleDraft? example = sentence.LSentenceDraftExample;
        if (example is not null)
        {
            sheet.Append(LLiveryEtymology.LLiveryMentionFormat(
                example.LExampleDraftText.LStateValueShow(), example.LExampleDraftMention, note));
            long reference = example.LExampleDraftReference.LStateAnchorShow();
            if (page.LLiveryPageSource.TryGetValue(reference, out string? byline) && byline.Length > 0)
            {
                sheet.Append(" <span class=\"llyn-byline\">").Append(LLiveryHeader.LLiveryTextFormat(byline))
                    .Append("</span>");
            }
        }

        foreach (LGlossDraft gloss in sentence.LSentenceDraftGloss)
        {
            string text = gloss.LGlossDraftText.LStateValueShow();
            if (text.Length > 0)
            {
                string flag = LLiveryHeader.LLiveryBannerFormat(page, gloss.LGlossDraftLanguage);
                sheet.Append("\\\n  ").Append(flag.Length > 0 ? flag + " " : string.Empty)
                    .Append("<span class=\"llyn-gloss\">").Append(LLiveryHeader.LLiveryTextFormat(text))
                    .Append("</span>");
            }
        }

        sheet.Append('\n');
    }

    private static void LLiveryTargetAppend(
        StringBuilder sheet, LLiveryPage page, LCardDraft card, Func<long, string> note)
    {
        if (!page.LLiveryPageTarget.TryGetValue(card.LCardDraftId, out IReadOnlyList<LTranslationTarget>? targets)
            || targets.Count == 0)
        {
            return;
        }

        foreach (LTranslationTarget target in targets)
        {
            string flag = LLiveryHeader.LLiveryBannerFormat(page, target.LTranslationTargetLanguage);
            sheet.Append("<span class=\"llyn-target\">").Append(flag.Length > 0 ? flag + " " : string.Empty)
                .Append(LLiveryEtymology.LLiveryLinkFormat(
                    target.LTranslationTargetHeadword, target.LTranslationTargetId, note));
            if (target.LTranslationTargetLanguage.Length > 0)
            {
                sheet.Append(" <span class=\"llyn-language\">")
                    .Append(LLiveryHeader.LLiveryTextFormat(target.LTranslationTargetLanguage)).Append("</span>");
            }

            sheet.Append("</span> ");
        }

        sheet.Length--;
        sheet.Append("\n\n");
    }

    private static void LLiveryBadgeAppend(StringBuilder sheet, string style, IReadOnlyList<string> texts)
    {
        bool written = false;
        foreach (string text in texts)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                continue;
            }

            sheet.Append(written ? " " : string.Empty).Append("<span class=\"").Append(style).Append("\">")
                .Append(LLiveryHeader.LLiveryTextFormat(text)).Append("</span>");
            written = true;
        }

        if (written)
        {
            sheet.Append("\n\n");
        }
    }

    private static void LLiveryMediaAppend(StringBuilder sheet, LCardDraft card)
    {
        foreach (LImageDraft image in card.LCardDraftImage)
        {
            string location = image.LImageDraftLocation.LStateValueShow().Trim();
            string picture = LLiveryAddressCheck(location)
                ? LLiveryAddressFormat(location)
                : Path.IsPathFullyQualified(location)
                    ? LLiveryHeader.LLiveryPictureFormat(location, "llyn-image")
                    : string.Empty;
            if (picture.Length > 0)
            {
                sheet.Append(picture).Append("\n\n");
            }
        }

        foreach (LVideoDraft video in card.LCardDraftVideo)
        {
            string location = video.LVideoDraftLocation.LStateValueShow().Trim();
            if (location.Length == 0)
            {
                continue;
            }

            sheet.Append(LLiveryAddressCheck(location)
                ? LLiveryAddressFormat(location)
                : LLiverySheet.LLiveryVideoHead + WebUtility.HtmlEncode(location) + "\"></video>");
            string span = video.LVideoDraftSpan.LStateValueShow();
            if (span.Length > 0)
            {
                sheet.Append(" <span class=\"llyn-span\">").Append(LLiveryHeader.LLiveryTextFormat(span))
                    .Append("</span>");
            }

            sheet.Append("\n\n");
        }
    }

    private static void LLiveryIncomingAppend(
        StringBuilder sheet, LLiveryPage page, Func<long, string> note, Func<string, string> lookup)
    {
        if (page.LLiveryPageIncoming.Count == 0)
        {
            return;
        }

        sheet.Append("## ").Append(LLiveryHeader.LLiveryTextFormat(lookup("Display.Translated")))
            .Append("\n\n<div class=\"llyn-card\">\n\n");
        foreach (LUsage usage in page.LLiveryPageIncoming)
        {
            sheet.Append(LLiveryEtymology.LLiveryLinkFormat(usage.LUsageName, usage.LUsageEntry, note));
            if (usage.LUsageEpithet.Length > 0)
            {
                sheet.Append(" <span class=\"llyn-epithet\">")
                    .Append(LLiveryHeader.LLiveryTextFormat(usage.LUsageEpithet)).Append("</span>");
            }

            string owner = usage.LUsageOwner switch
            {
                LOwner.LOwnerMeaning => lookup("Display.MeaningSingle"),
                LOwner.LOwnerCollocation => lookup("Display.CollocationSingle"),
                LOwner.LOwnerExample => lookup("Portrait.Example"),
                _ => string.Empty,
            };
            if (owner.Length > 0)
            {
                sheet.Append(" <span class=\"llyn-owner\">").Append(LLiveryHeader.LLiveryTextFormat(owner))
                    .Append("</span>");
            }

            if (usage.LUsageLanguage.Length > 0)
            {
                string flag = LLiveryHeader.LLiveryBannerFormat(page, usage.LUsageLanguage);
                sheet.Append(" <span class=\"llyn-language\">").Append(flag.Length > 0 ? flag + " " : string.Empty)
                    .Append(LLiveryHeader.LLiveryTextFormat(usage.LUsageLanguage)).Append("</span>");
            }

            sheet.Append("\n\n");
        }

        sheet.Append("</div>\n\n");
    }

    private static string LLiveryAddressFormat(string location)
    {
        string target = location.Replace("<", "%3C", StringComparison.Ordinal)
            .Replace(">", "%3E", StringComparison.Ordinal);
        return "[" + LLiveryHeader.LLiveryTextFormat(location) + "](<" + target + ">)";
    }

    private static bool LLiveryAddressCheck(string location)
    {
        return location.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || location.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
    }
}
