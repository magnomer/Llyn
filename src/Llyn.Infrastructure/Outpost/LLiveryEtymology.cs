using System;
using System.Collections.Generic;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LLiveryEtymology
{
    public static void LLiveryEtymologyAppend(
        StringBuilder sheet, LLiveryPage page, Func<long, string> note, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(lookup);

        LEtymologyDraft? etymology = page.LLiveryPageDraft.LEntryDraftEtymology;
        bool narrated = etymology?.LEtymologyDraftNarrated == true;
        if (narrated || page.LLiveryPageEtymon.Count > 0)
        {
            sheet.Append("## ").Append(LLiveryHeader.LLiveryTextFormat(lookup("Display.Etymology"))).Append("\n\n");
            if (narrated)
            {
                sheet.Append(LLiveryMentionFormat(
                    etymology!.LEtymologyDraftText, etymology.LEtymologyDraftMentions, note)).Append("\n\n");
            }

            foreach (LTranslationTarget etymon in page.LLiveryPageEtymon)
            {
                sheet.Append("- ")
                    .Append(LLiveryLinkFormat(etymon.LTranslationTargetHeadword, etymon.LTranslationTargetId, note));
                if (etymon.LTranslationTargetLanguage.Length > 0)
                {
                    sheet.Append(" <span class=\"llyn-language\">")
                        .Append(LLiveryHeader.LLiveryTextFormat(etymon.LTranslationTargetLanguage)).Append("</span>");
                }

                sheet.Append('\n');
            }

            if (page.LLiveryPageEtymon.Count > 0)
            {
                sheet.Append('\n');
            }
        }

        string stored = page.LLiveryPageDraft.LEntryDraftNote ?? string.Empty;
        if (stored.Trim().Length > 0)
        {
            sheet.Append("## ").Append(LLiveryHeader.LLiveryTextFormat(lookup("Display.Note"))).Append("\n\n")
                .Append(stored.Replace("\r\n", "\n", StringComparison.Ordinal).Trim('\n')).Append("\n\n");
        }

        LLiveryStampAppend(sheet, page, lookup);
    }

    internal static string LLiveryLinkFormat(string text, long id, Func<long, string> note)
    {
        ArgumentNullException.ThrowIfNull(note);

        string shown = LLiveryHeader.LLiveryTextFormat(text);
        string target = id > 0 ? note(id) : string.Empty;
        return string.IsNullOrEmpty(target) ? shown : "[" + shown + "](:/" + target + ")";
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

    private static void LLiveryStampAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup)
    {
        if (string.IsNullOrEmpty(page.LLiveryPageCreated) && string.IsNullOrEmpty(page.LLiveryPageUpdated))
        {
            return;
        }

        List<string> lines = [];
        foreach ((string key, string stamp) in new[]
                 {
                     ("Display.Created", page.LLiveryPageCreated),
                     ("Display.Updated", page.LLiveryPageUpdated),
                 })
        {
            if (!string.IsNullOrEmpty(stamp))
            {
                lines.Add("<span class=\"llyn-stamp\">" + LLiveryHeader.LLiveryTextFormat(lookup(key)) + "</span> "
                    + LLiveryHeader.LLiveryTextFormat(stamp));
            }
        }

        sheet.Append("---\n\n").Append(string.Join("\\\n", lines)).Append("\n\n");
    }
}
