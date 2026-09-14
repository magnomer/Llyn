using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Llyn.Infrastructure;

internal static class LPronunciationText
{
    private static readonly Regex LPronunciationTextPattern = new("<[^>]+>", RegexOptions.Compiled);

    private static readonly char[] LPronunciationTextDelimiters = ['/', '[', ']', '(', ')', ','];

    public static IReadOnlyList<string> LPronunciationTextScan(string html, Regex openPattern, int skip)
    {
        List<string> found = [];
        MatchCollection matches = openPattern.Matches(html);
        for (int index = Math.Max(0, skip); index < matches.Count; index++)
        {
            Match open = matches[index];
            string? inner = LPronunciationInnerRead(html, open.Index + open.Length);
            if (inner is null)
            {
                continue;
            }

            string phonetic = LPronunciationTextPattern.Replace(inner, string.Empty);
            if (phonetic.Trim().Length > 0)
            {
                found.Add(phonetic);
            }
        }

        return found;
    }

    public static string LPronunciationTextNormalize(string raw)
    {
        string decoded = WebUtility.HtmlDecode(raw).Trim();
        return decoded.Trim(LPronunciationTextDelimiters).Trim();
    }

    private static string? LPronunciationInnerRead(string html, int start)
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
