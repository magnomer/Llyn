using System;
using System.Collections.Generic;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LLiveryXiesheng
{
    public static void LLiveryXieshengAppend(
        StringBuilder sheet, LLiveryStem stem, Func<long, string> note, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(stem);
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(lookup);

        LStemPage page = stem.LLiveryStemPage;
        sheet.Append(LLiveryRime.LLiveryChipFormat("llyn-series", lookup("Xiesheng.Stem"))).Append("\n\n")
            .Append("# ").Append(LLiveryHeader.LLiveryTextFormat(page.LStemPageKey)).Append("\n\n");
        if (page.LStemPageEmpty)
        {
            sheet.Append(LLiveryRime.LLiveryChipFormat("llyn-vacant", lookup("Xiesheng.StemEmpty"))).Append("\n\n");
        }
        else
        {
            foreach (LStemMember member in page.LStemPageMembers)
            {
                LLiveryMemberAppend(sheet, member, stem.LLiveryStemEntry, note);
            }
        }

        LLiveryEntryAppend(sheet, stem.LLiveryStemEntry, note, lookup("Xiesheng.KindredVacant"));
    }

    private static void LLiveryMemberAppend(
        StringBuilder sheet, LStemMember member, IReadOnlyList<LEntry> entries, Func<long, string> note)
    {
        sheet.Append("<div class=\"llyn-card\">\n\n")
            .Append(LLiveryCharacterFormat(member.LStemMemberCharacter, entries, note));
        string reading = member.LStemMemberReading;
        if (reading.Length > 0)
        {
            sheet.Append(" <span class=\"llyn-accent\">").Append(LLiveryHeader.LLiveryTextFormat(reading))
                .Append("</span>");
        }

        sheet.Append("\n\n");
        LLiverySound.LLiveryReadingAppend(sheet, member.LStemMemberRows, [], member.LStemMemberCharacter);
        sheet.Append("</div>\n\n");
    }

    internal static string LLiveryCharacterFormat(
        string character, IReadOnlyList<LEntry> entries, Func<long, string> note)
    {
        ArgumentNullException.ThrowIfNull(entries);

        long id = 0;
        foreach (LEntry entry in entries)
        {
            if (entry.LEntryId > 0
                && (id == 0 || entry.LEntryId < id)
                && string.Equals(entry.LEntryHeadword, character, StringComparison.Ordinal))
            {
                id = entry.LEntryId;
            }
        }

        return "<span class=\"llyn-stem\">" + LLiveryEtymology.LLiveryLinkFormat(character, id, note) + "</span>";
    }

    internal static void LLiveryEntryAppend(
        StringBuilder sheet, IReadOnlyList<LEntry> entries, Func<long, string> note, string vacant)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(entries);

        if (entries.Count == 0)
        {
            if (!string.IsNullOrEmpty(vacant))
            {
                sheet.Append(LLiveryRime.LLiveryChipFormat("llyn-vacant", vacant)).Append("\n\n");
            }

            return;
        }

        foreach (LEntry entry in entries)
        {
            sheet.Append("- ")
                .Append(LLiveryEtymology.LLiveryLinkFormat(entry.LEntryHeadword, entry.LEntryId, note)).Append('\n');
        }

        sheet.Append('\n');
    }
}
