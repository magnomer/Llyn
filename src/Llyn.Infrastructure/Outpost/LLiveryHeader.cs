using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LLiveryHeader
{
    private const int LLiveryStarCount = 5;

    private const int LLiveryGraspCeiling = 10;

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

        sheet.Append(page.LLiveryPageFavorite
            ? " <span class=\"llyn-heart llyn-on\">♥</span>"
            : " <span class=\"llyn-heart\">♡</span>");
        int grasp = Math.Clamp(page.LLiveryPageGrasp, 0, LLiveryGraspCeiling);
        LLiveryStarAppend(sheet, grasp);
        string rating = lookup("Grasp.Level" + grasp.ToString(CultureInfo.InvariantCulture));
        sheet.Append(" <span class=\"llyn-rating\">").Append(LLiveryTextFormat(rating)).Append("</span>\n\n");
    }

    public static void LLiveryChipAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(lookup);

        if (page.LLiveryPageDraft.LEntryDraftNames.Count > 0)
        {
            foreach (string name in page.LLiveryPageDraft.LEntryDraftNames)
            {
                sheet.Append("<span class=\"llyn-speech\">").Append(LLiveryTextFormat(name)).Append("</span> ");
            }

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
        string? media = Path.GetExtension(path ?? string.Empty).ToLowerInvariant() switch
        {
            ".svg" => "image/svg+xml",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => null,
        };
        if (path is null || media is null)
        {
            return string.Empty;
        }

        byte[] data;
        try
        {
            data = File.ReadAllBytes(path);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            return string.Empty;
        }

        return "<img src=\"data:" + media + ";base64," + Convert.ToBase64String(data) + "\" class=\"" + style + "\">";
    }

    private static void LLiveryStarAppend(StringBuilder sheet, int grasp)
    {
        for (int star = 0; star < LLiveryStarCount; star++)
        {
            int filled = grasp - (star * 2);
            sheet.Append(filled >= 2
                ? " <span class=\"llyn-star llyn-on\">★</span>"
                : filled == 1
                    ? " <span class=\"llyn-star llyn-half\">★</span>"
                    : grasp == 0
                        ? " <span class=\"llyn-star llyn-unrated\">★</span>"
                        : " <span class=\"llyn-star\">★</span>");
        }
    }
}
