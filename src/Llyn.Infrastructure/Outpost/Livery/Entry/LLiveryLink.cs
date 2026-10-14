using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LLiveryLink
{
    internal static string LLiveryLinkFormat(string text, long id, Func<long, string> note)
    {
        string shown = LLiveryHeader.LLiveryTextFormat(text);
        string target = LLiveryTargetRead(id, note);
        return target.Length == 0 ? shown : "[" + shown + "](:/" + target + ")";
    }

    internal static string LLiveryAnchorFormat(string text, long id, Func<long, string> note)
    {
        string shown = LLiveryHeader.LLiveryTextFormat(text);
        string target = LLiveryTargetRead(id, note);
        return target.Length == 0 ? shown : "<a href=\":/" + WebUtility.HtmlEncode(target) + "\">" + shown + "</a>";
    }

    internal static string LLiveryEtymonFormat(string text, long id, Func<long, string> note)
    {
        string shown = LLiveryHeader.LLiveryHtmlFormat(text);
        string target = LLiveryTargetRead(id, note);
        return target.Length == 0 ? shown : "<a href=\":/" + WebUtility.HtmlEncode(target) + "\">" + shown + "</a>";
    }

    private static string LLiveryTargetRead(long id, Func<long, string> note)
    {
        ArgumentNullException.ThrowIfNull(note);

        return id > 0 ? note(id) ?? string.Empty : string.Empty;
    }

    internal static string LLiveryMentionFormat(
        string text, IReadOnlyList<LMentionDraft> mentions, Func<long, string> note)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(mentions);

        StringBuilder line = new StringBuilder();
        int done = text.Length - text.TrimStart().Length;
        int last = text.TrimEnd().Length;
        foreach (LMentionDraft mention in mentions)
        {
            int start = mention.LMentionDraftOffset;
            int end = start + mention.LMentionDraftLength;
            if (!mention.LMentionDraftLinked || start < done || mention.LMentionDraftLength <= 0 || end > last)
            {
                continue;
            }

            line.Append(LLiveryBreakFormat(text[done..start]))
                .Append(LLiveryLinkFormat(text[start..end], mention.LMentionDraftEntry, note));
            done = end;
        }

        return line.Append(LLiveryBreakFormat(text[done..Math.Max(done, last)])).ToString().Trim();
    }

    private static string LLiveryBreakFormat(string text)
    {
        string[] lines = text.Replace("\r", string.Empty, StringComparison.Ordinal).Split('\n');
        for (int place = 0; place < lines.Length; place++)
        {
            lines[place] = LLiveryHeader.LLiveryTextFormat(lines[place]);
        }

        return string.Join("\\\n", lines);
    }
}
