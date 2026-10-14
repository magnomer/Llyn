using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LLiveryHeader
{
    private const string LLiveryEscaped = "\\\u0060*_[]|~#!";

    public static void LLiveryHeaderAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(lookup);

        LEntryDraft draft = page.LLiveryPageDraft;
        sheet.Append("# ").Append(LLiveryTextFormat(draft.LEntryDraftHeadword));
        if (!string.IsNullOrWhiteSpace(draft.LEntryDraftLanguage))
        {
            string flag = LLiveryBannerFormat(page, draft.LEntryDraftLanguage);
            sheet.Append(" <span class=\"llyn-language\">")
                .Append(flag.Length > 0 ? flag + " " : string.Empty)
                .Append(LLiveryTextFormat(draft.LEntryDraftLanguage)).Append("</span>");
        }

        sheet.Append(" <span class=\"llyn-regard\">").Append(page.LLiveryPageFavorite
            ? "<span class=\"llyn-heart llyn-on\">♥</span>"
            : "<span class=\"llyn-heart\">♡</span>");
        int grasp = LGrasp.LGraspNormalize(page.LLiveryPageGrasp);
        LLiveryStarAppend(sheet, grasp);
        string rating = lookup(LGrasp.LGraspKeyRead(grasp));
        sheet.Append(" <span class=\"llyn-rating\">").Append(LLiveryTextFormat(rating)).Append("</span></span>\n\n");
    }

    public static void LLiveryChipAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(lookup);

        LEntryDraft draft = page.LLiveryPageDraft;
        IReadOnlyList<LEntryChip> chips = LEntryChip.LEntryChipRead(
            draft.LEntryDraftUnit, draft.LEntryDraftFreedom, draft.LEntryDraftSpeeches);
        foreach (LEntryChip chip in chips)
        {
            (string style, string text) = chip.LEntryChipKind switch
            {
                LEntryChipKind.LEntryChipUnit => ("llyn-unit", lookup(chip.LEntryChipText)),
                LEntryChipKind.LEntryChipFreedom => ("llyn-freedom", lookup(chip.LEntryChipText)),
                _ => ("llyn-speech", chip.LEntryChipText),
            };
            sheet.Append("<span class=\"").Append(style).Append("\">").Append(LLiveryTextFormat(text))
                .Append("</span> ");
        }

        if (chips.Count > 0)
        {
            sheet.Length--;
            sheet.Append("\n\n");
        }

        if (LFrequencyGauge.LFrequencyGaugeResolve(page.LLiveryPageFrequency, lookup("Frequency.Once"))
            is not LFrequencyGauge gauge)
        {
            return;
        }

        string source = WebUtility.HtmlEncode(gauge.LFrequencyGaugeSource)
            .Replace("\n", "&#10;", StringComparison.Ordinal);
        string tier = gauge.LFrequencyGaugeRanked
            ? " llyn-frequency-" + gauge.LFrequencyGaugeRank.ToLowerInvariant()
            : string.Empty;
        sheet.Append("<span class=\"llyn-frequency").Append(tier).Append("\" title=\"").Append(source).Append("\">")
            .Append(LLiveryTextFormat(lookup("Frequency." + gauge.LFrequencyGaugeRank)));
        int band = LFrequency.LFrequencyScale.Count - gauge.LFrequencyGaugeSpare;
        for (int pip = 0; pip < LFrequency.LFrequencyScale.Count; pip++)
        {
            sheet.Append(pip < band
                ? " <span class=\"llyn-pip llyn-on\">✦</span>"
                : " <span class=\"llyn-pip\">✦</span>");
        }

        sheet.Append("</span>\n\n");
    }

    internal static string LLiveryTextFormat(string text)
    {
        StringBuilder escaped = new StringBuilder();
        foreach (char letter in text ?? string.Empty)
        {
            if (letter is '\r' or '\n')
            {
                escaped.Append(' ');
                continue;
            }

            if (LLiveryEscaped.Contains(letter, StringComparison.Ordinal))
            {
                escaped.Append('\\');
            }

            escaped.Append(letter);
        }

        return WebUtility.HtmlEncode(escaped.ToString());
    }

    internal static string LLiveryHtmlFormat(string text)
    {
        return WebUtility.HtmlEncode((text ?? string.Empty).ReplaceLineEndings(" "));
    }

    internal static string LLiveryFlagFormat(string? path)
    {
        return LLiveryPictureFormat(path, "llyn-flag");
    }

    internal static string LLiveryBannerFormat(LLiveryPage page, string language)
    {
        return page.LLiveryPageBanner.TryGetValue(language, out string? path)
            ? LLiveryFlagFormat(path)
            : string.Empty;
    }

    internal static string LLiveryPictureFormat(string? path, string style)
    {
        return LPortraitAsset.LPortraitAssetLoad(path ?? string.Empty) is LPortraitAsset asset
            ? "<img src=\"" + asset.LPortraitAssetAddress + "\" class=\"" + style + "\">"
            : string.Empty;
    }

    private static void LLiveryStarAppend(StringBuilder sheet, int grasp)
    {
        foreach (int filled in LGrasp.LGraspStarScan(grasp))
        {
            sheet.Append(filled == 2
                ? " <span class=\"llyn-star llyn-on\">★</span>"
                : filled == 1
                    ? " <span class=\"llyn-star llyn-half\">★</span>"
                    : grasp == 0
                        ? " <span class=\"llyn-star llyn-unrated\">★</span>"
                        : " <span class=\"llyn-star\">★</span>");
        }
    }
}
