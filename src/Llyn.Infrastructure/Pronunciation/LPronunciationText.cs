using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Llyn.Infrastructure;

/// <summary>
/// Extracts and normalizes pronunciation text scraped from HTML. Tolerant on purpose: given a
/// pattern that matches an IPA element's opening tag, it reads the element's full inner content by
/// walking nested spans to the matching close, strips any inner markup, and returns the first
/// non-empty phonetic form. This survives real markup such as Cambridge's
/// <c>&lt;span class="ipa"&gt;...&lt;span class="sp"&gt;ə&lt;/span&gt;l&lt;/span&gt;</c>, where naive
/// "up to the first &lt;/span&gt;" matching would truncate the value.
/// </summary>
internal static class LPronunciationText
{
    private static readonly Regex LPronunciationTextPattern = new("<[^>]+>", RegexOptions.Compiled);

    /// <summary>
    /// Returns the first non-empty normalized phonetic found by <paramref name="openPattern"/>
    /// (which must match an IPA element's opening tag) across all matches in
    /// <paramref name="html"/>, or <c>null</c> when none qualifies.
    /// </summary>
    public static string? LPronunciationTextRead(string html, Regex openPattern)
    {
        foreach (Match open in openPattern.Matches(html))
        {
            string? inner = LPronunciationTextScan(html, open.Index + open.Length);
            if (inner is null)
            {
                continue;
            }

            string phonetic = LPronunciationTextNormalize(LPronunciationTextPattern.Replace(inner, string.Empty));
            if (phonetic.Length > 0)
            {
                return phonetic;
            }
        }

        return null;
    }

    /// <summary>
    /// Decodes HTML entities and strips enclosing IPA delimiters, so the caller stores just the
    /// phonetic characters. The UI supplies its own surrounding brackets.
    /// </summary>
    public static string LPronunciationTextNormalize(string raw)
    {
        string decoded = WebUtility.HtmlDecode(raw).Trim();
        return decoded.Trim('/', '[', ']', '(', ')').Trim();
    }

    /// <summary>
    /// Reads a span's inner HTML starting just after its opening tag, returning the substring up to
    /// the matching <c>&lt;/span&gt;</c> while accounting for nested spans, or <c>null</c> when the
    /// close is never found.
    /// </summary>
    private static string? LPronunciationTextScan(string html, int start)
    {
        int depth = 1;
        int index = start;

        while (index < html.Length)
        {
            int next = html.IndexOf('<', index);
            if (next < 0)
            {
                return null;
            }

            if (LPronunciationTextMatch(html, next, "</span"))
            {
                depth--;
                if (depth == 0)
                {
                    return html.Substring(start, next - start);
                }

                index = next + 6;
            }
            else if (LPronunciationTextMatch(html, next, "<span"))
            {
                depth++;
                index = next + 5;
            }
            else
            {
                index = next + 1;
            }
        }

        return null;
    }

    private static bool LPronunciationTextMatch(string html, int index, string token)
    {
        return index + token.Length <= html.Length
            && string.Compare(html, index, token, 0, token.Length, StringComparison.OrdinalIgnoreCase) == 0;
    }
}
