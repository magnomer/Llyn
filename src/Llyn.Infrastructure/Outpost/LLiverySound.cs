using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LLiverySound
{
    private const string LLiveryAnchorSeparator = " · ";

    private const char LLiveryToneJoiner = '-';

    public static void LLiverySoundAppend(
        StringBuilder sheet, LLiveryPage page, Func<string, string> lookup, LTheme theme)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(lookup);
        ArgumentNullException.ThrowIfNull(theme);

        LEntryDraft draft = page.LLiveryPageDraft;
        List<(LReflexDraft, LReflexGuise?)> shown = [];
        List<(LReflexDraft, LReflexGuise?)> folded = [];
        for (int index = 0; index < draft.LEntryDraftReflexes.Count; index++)
        {
            LReflexDraft reflex = draft.LEntryDraftReflexes[index];
            if (!reflex.LReflexDraftWritten)
            {
                continue;
            }

            LReflexGuise? guise = index < page.LLiveryPageGuise.Count ? page.LLiveryPageGuise[index] : null;
            bool hidden = guise?.LReflexGuiseFolded
                ?? page.LLiveryPageFolded.Contains(reflex.LReflexDraftLanguage, StringComparer.Ordinal);
            (hidden ? folded : shown).Add((reflex, guise));
        }

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
                    "<span class=\"llyn-accent\">" + LLiveryToneFormat(spoken, toned) + "</span>"
                    + LLiveryPlayerFormat(recorded)));
                if (primary && accent.LAccentSheetContour.Count > 0)
                {
                    lines.Add((string.Empty, LLiveryContourFormat(accent.LAccentSheetContour, theme)));
                }
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

    private static void LLiveryReadingAppend(
        StringBuilder sheet,
        List<(LReflexDraft, LReflexGuise?)> readings,
        IReadOnlyList<LFanqieRow> rows,
        string headword)
    {
        if (readings.Count == 0)
        {
            return;
        }

        sheet.Append("| | | | | |\n|---|---|---|---|---|\n");
        string? named = null;
        foreach ((LReflexDraft reflex, LReflexGuise? guise) in readings)
        {
            string language = reflex.LReflexDraftLanguage;
            string label = string.Equals(named, language, StringComparison.Ordinal) ? string.Empty : language;
            named = language;
            string text = guise?.LReflexGuiseRespelled == true && reflex.LReflexDraftRespelling.Length > 0
                ? reflex.LReflexDraftRespelling
                : reflex.LReflexDraftText;
            bool phonemic = guise?.LReflexGuisePhonemic == true;
            if (phonemic && !text.StartsWith('/'))
            {
                text = '/' + text + '/';
            }

            string reading = LLiveryToneFormat(text, phonemic);
            string old = LAnchor.LAnchorTextFormat(rows, reflex.LReflexDraftAnchors, headword, LLiveryAnchorSeparator);
            sheet.Append("| ").Append(LLiveryHeader.LLiveryTextFormat(label))
                .Append(" | ").Append(LLiveryHeader.LLiveryTextFormat(reflex.LReflexDraftKind))
                .Append(" | ")
                .Append(reflex.LReflexDraftMain ? "<span class=\"llyn-main\">" + reading + "</span>" : reading)
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
            : " " + LLiverySheet.LLiveryAudioHead + WebUtility.HtmlEncode(audio) + "\"></audio>";
    }

    private static string LLiveryContourFormat(IReadOnlyList<LContour> contour, LTheme theme)
    {
        string line = theme.LThemeRead("line");
        string muted = theme.LThemeRead("muted");
        string ink = theme.LThemeRead("ink");
        string family = WebUtility.HtmlEncode(theme.LThemeFamily);
        StringBuilder chart = new StringBuilder("<span class=\"llyn-contour\">");
        for (int index = 0; index < contour.Count; index++)
        {
            StringBuilder figure = new StringBuilder("<svg xmlns=\"http://www.w3.org/2000/svg\"");
            figure.Append(" width=\"72\" height=\"76\" viewBox=\"0 0 72 76\" font-family=\"")
                .Append(family).Append("\">");
            foreach (int level in LContour.LContourScale)
            {
                string height = (6 + ((LContour.LContourCeiling - level) * 12)).ToString(CultureInfo.InvariantCulture);
                figure.Append("<line stroke=\"").Append(line).Append("\" stroke-width=\"1\" x1=\"14\" x2=\"68\" y1=\"")
                    .Append(height).Append("\" y2=\"").Append(height).Append("\"/>");
                if (index == 0)
                {
                    figure.Append("<text fill=\"").Append(muted).Append("\" font-size=\"6\" x=\"2\" y=\"")
                        .Append(height).Append("\" dy=\"3\">")
                        .Append(level.ToString(CultureInfo.InvariantCulture)).Append("</text>");
                }
            }

            IReadOnlyList<int> levels = contour[index].LContourLevels;
            List<string> points = [];
            List<string> colors = [];
            for (int step = 0; step < levels.Count; step++)
            {
                int level = Math.Clamp(levels[step], LContour.LContourFloor, LContour.LContourCeiling);
                string height = (6 + ((LContour.LContourCeiling - level) * 12)).ToString(CultureInfo.InvariantCulture);
                double left = levels.Count == 1 ? 24 : 24 + (step * 34.0 / (levels.Count - 1));
                string color = theme.LThemeRead(LLiveryRoleFormat(LContour.LContourRoleRead(levels[step])));
                points.Add(left.ToString("0.#", CultureInfo.InvariantCulture) + "," + height);
                colors.Add(color);
                if (levels.Count == 1)
                {
                    points.Add("58," + height);
                    colors.Add(color);
                }
            }

            if (colors.Distinct(StringComparer.Ordinal).Count() == 1)
            {
                figure.Append("<polyline stroke=\"").Append(colors[0])
                    .Append("\" stroke-width=\"2\" fill=\"none\" points=\"").Append(string.Join(' ', points))
                    .Append("\"/>");
            }
            else if (colors.Count > 1)
            {
                figure.Append("<defs><linearGradient id=\"fade\" gradientUnits=\"userSpaceOnUse\"")
                    .Append(" x1=\"24\" x2=\"58\" y1=\"0\" y2=\"0\">");
                for (int step = 0; step < colors.Count; step++)
                {
                    string offset = (step / (double)(colors.Count - 1)).ToString("0.###", CultureInfo.InvariantCulture);
                    figure.Append("<stop stop-color=\"").Append(colors[step])
                        .Append("\" offset=\"").Append(offset).Append("\"/>");
                }

                figure.Append("</linearGradient></defs>")
                    .Append("<polyline stroke=\"url(#fade)\" stroke-width=\"2\" fill=\"none\" points=\"")
                    .Append(string.Join(' ', points)).Append("\"/>");
            }

            for (int step = 0; step < points.Count; step++)
            {
                string[] place = points[step].Split(',');
                figure.Append("<circle fill=\"").Append(colors[step]).Append("\" r=\"2.5\" cx=\"")
                    .Append(place[0]).Append("\" cy=\"").Append(place[1]).Append("\"/>");
            }

            figure.Append("<text fill=\"").Append(ink)
                .Append("\" font-size=\"9\" x=\"41\" y=\"72\" text-anchor=\"middle\">")
                .Append(WebUtility.HtmlEncode(contour[index].LContourText)).Append("</text></svg>");
            chart.Append("<img src=\"data:image/svg+xml;base64,")
                .Append(Convert.ToBase64String(Encoding.UTF8.GetBytes(figure.ToString())))
                .Append("\" class=\"llyn-contour-chart\">");
        }

        return chart.Append("</span>").ToString();
    }

    private static string LLiveryRoleFormat(LContourRole role)
    {
        return role switch
        {
            LContourRole.LContourRoleTop => "contourTop",
            LContourRole.LContourRoleHigh => "contourHigh",
            LContourRole.LContourRoleMid => "contourMid",
            LContourRole.LContourRoleLow => "contourLow",
            LContourRole.LContourRoleBottom => "contourBottom",
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null),
        };
    }

    private static string LLiveryToneFormat(string text, bool toned)
    {
        if (!toned)
        {
            return LLiveryHeader.LLiveryTextFormat(text);
        }

        StringBuilder marked = new StringBuilder();
        int start = 0;
        int index = 0;
        while (index < text.Length)
        {
            if (!char.IsAsciiDigit(text[index]))
            {
                index++;
                continue;
            }

            int end = index;
            while (end < text.Length
                   && (char.IsAsciiDigit(text[end])
                       || (text[end] == LLiveryToneJoiner
                           && end + 1 < text.Length
                           && char.IsAsciiDigit(text[end + 1]))))
            {
                end++;
            }

            marked.Append(LLiveryHeader.LLiveryTextFormat(text[start..index]))
                .Append("<span class=\"llyn-tone\">").Append(text, index, end - index).Append("</span>");
            start = end;
            index = end;
        }

        return marked.Append(LLiveryHeader.LLiveryTextFormat(text[start..])).ToString();
    }
}
