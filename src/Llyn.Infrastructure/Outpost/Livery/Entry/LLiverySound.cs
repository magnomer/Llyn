using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LLiverySound
{
    public static void LLiverySoundAppend(
        StringBuilder sheet, LLiveryPage page, Func<string, string> lookup, LTheme theme)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(lookup);
        ArgumentNullException.ThrowIfNull(theme);

        LEntryDraft draft = page.LLiveryPageDraft;
        (IReadOnlyList<(LReflexDraft, LReflexGuise?)> shown, IReadOnlyList<(LReflexDraft, LReflexGuise?)> folded) =
            LSheetSound.LSheetReflexDivide(page);

        IReadOnlyList<LFanqieRow> rows = [.. page.LLiveryPageFanqie.SelectMany(static group => group.LFanqieGroupRows)];
        LLiveryReadingAppend(sheet, shown, rows, draft.LEntryDraftHeadword);
        if (folded.Count > 0)
        {
            sheet.Append("<details class=\"llyn-more\">\n<summary>")
                .Append(WebUtility.HtmlEncode(lookup("Reflex.More"))).Append("</summary>\n\n");
            LLiveryReadingAppend(sheet, folded, rows, draft.LEntryDraftHeadword);
            sheet.Append("</details>\n\n");
        }

        List<(string, string)> lines = [];
        LAccentSheet accent = page.LLiveryPageAccent;
        bool toned = LContour.LContourToneCheck(accent.LAccentSheetContour);
        foreach (LAccentRow row in accent.LAccentSheetRows.Prepend(accent.LAccentSheetPrimary))
        {
            if (row.LAccentRowText.Length > 0)
            {
                string spoken = accent.LAccentSheetOpener + row.LAccentRowText + accent.LAccentSheetCloser;
                string flag = page.LLiveryPageEnsign.TryGetValue(row.LAccentRowVariety, out string? path)
                    ? LLiveryHeader.LLiveryFlagFormat(path)
                    : string.Empty;
                bool primary = ReferenceEquals(row, accent.LAccentSheetPrimary);
                LPronunciationDraft? recorded = primary
                    ? draft.LEntryDraftPronunciation
                    : draft.LEntryDraftPronunciations.FirstOrDefault(pronunciation =>
                        row.LAccentRowId != 0 && pronunciation.LPronunciationDraftId == row.LAccentRowId);
                lines.Add((
                    flag.Length > 0 ? flag : LLiveryHeader.LLiveryTextFormat(row.LAccentRowVariety),
                    LSheetSound.LSheetAxisFormat(
                        "<span class=\"llyn-accent\">" + LLiveryToneFormat(spoken, toned) + "</span>",
                        primary ? accent.LAccentSheetContour : [],
                        theme,
                        static text => WebUtility.HtmlEncode(text) ?? string.Empty)
                    + LLiveryPlayerFormat(recorded)));
            }
        }

        foreach (LTranscriptionDraft spelled in LGlyph.LGlyphOtherRead(
                     page.LLiveryPageGlyph, page.LLiveryPageTranscription))
        {
            lines.Add((
                LLiveryHeader.LLiveryTextFormat(spelled.LTranscriptionDraftScheme),
                "<span class=\"llyn-transcription\">"
                + LLiveryHeader.LLiveryTextFormat(spelled.LTranscriptionDraftText) + "</span>"));
        }

        if (page.LLiveryPageGlyph is LGlyph glyph && page.LLiveryPageCell.Count > 0)
        {
            string text = string.Concat(page.LLiveryPageCell.Select(static cell => cell.LGlyphCellText));
            lines.Add((
                LLiveryHeader.LLiveryTextFormat(glyph.LGlyphName),
                "<span class=\"llyn-glyph\">" + LLiveryHeader.LLiveryTextFormat(text) + "</span>"));
        }

        LLiveryRowAppend(sheet, lines);
    }

    internal static void LLiveryReadingAppend(
        StringBuilder sheet,
        IReadOnlyList<(LReflexDraft, LReflexGuise?)> readings,
        IReadOnlyList<LFanqieRow> rows,
        string headword)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(readings);
        ArgumentNullException.ThrowIfNull(rows);

        if (readings.Count == 0)
        {
            return;
        }

        sheet.Append("| | | | | | | |\n|---|---|---|---|---|---|---|\n");
        IReadOnlyList<string> labels = LSheetSound.LSheetLabelScan(readings);
        for (int index = 0; index < readings.Count; index++)
        {
            (LReflexDraft reflex, LReflexGuise? guise) = readings[index];
            string label = labels[index];
            string text = guise?.LReflexGuiseShown ?? reflex.LReflexDraftText;
            bool phonemic = guise?.LReflexGuisePhonemic == true;
            string reading = LLiveryToneFormat(LSheetSound.LSheetSlashFormat(text, phonemic), phonemic);
            string old = LAnchor.LAnchorTextFormat(
                rows, reflex.LReflexDraftAnchors, headword, LSheetSound.LSheetAnchorSeparator);
            sheet.Append("| ").Append(LLiveryHeader.LLiveryTextFormat(label))
                .Append(" | ").Append(LLiveryHeader.LLiveryTextFormat(reflex.LReflexDraftKind))
                .Append(" | ")
                .Append(reflex.LReflexDraftMain ? "<span class=\"llyn-main\">" + reading + "</span>" : reading)
                .Append(" | ").Append(LLiveryHeader.LLiveryTextFormat(reflex.LReflexDraftRomanization))
                .Append(" | ").Append(LLiveryHeader.LLiveryTextFormat(reflex.LReflexDraftMeaning))
                .Append(" | ")
                .Append(reflex.LReflexDraftNote.Length == 0
                    ? string.Empty
                    : "<span class=\"llyn-note\">"
                      + LLiveryHeader.LLiveryTextFormat(reflex.LReflexDraftNote) + "</span>")
                .Append(" | ").Append(LLiveryHeader.LLiveryTextFormat(old)).Append(" |\n");
        }

        sheet.Append('\n');
    }

    private static void LLiveryRowAppend(StringBuilder sheet, List<(string, string)> lines)
    {
        if (lines.Count == 0)
        {
            return;
        }

        sheet.Append("| | |\n|---|---|\n");
        foreach ((string label, string cell) in lines)
        {
            sheet.Append("| ").Append(label).Append(" | ").Append(cell).Append(" |\n");
        }

        sheet.Append('\n');
    }

    private static string LLiveryPlayerFormat(LPronunciationDraft? recorded)
    {
        string audio = recorded?.LPronunciationDraftAudio ?? string.Empty;
        return audio.Length == 0
            ? string.Empty
            : " " + LEngraverEntrance.LLiveryAudioHead + WebUtility.HtmlEncode(audio) + "\"></audio>";
    }

    private static string LLiveryToneFormat(string text, bool toned)
    {
        return LSheetSound.LSheetToneFormat(text, toned, LLiveryHeader.LLiveryTextFormat);
    }
}
