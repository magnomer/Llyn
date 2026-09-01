using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Llyn.Infrastructure;

internal static class LPronunciationText
{
    private static readonly Regex LPronunciationTextPattern = new("<[^>]+>", RegexOptions.Compiled);

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

    public static string LPronunciationTextNormalize(string raw)
    {
        string decoded = WebUtility.HtmlDecode(raw).Trim();
        return decoded.Trim('/', '[', ']', '(', ')').Trim();
    }

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
