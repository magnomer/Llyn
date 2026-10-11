using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSheetSound
{
    internal const string LSheetAnchorSeparator = " · ";

    private const char LSheetToneJoiner = '-';

    public static void LSheetSoundAppend(
        StringBuilder sheet,
        LLiveryPage page,
        IReadOnlyList<string> ensigns,
        LTheme theme,
        Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(ensigns);
        ArgumentNullException.ThrowIfNull(theme);
        ArgumentNullException.ThrowIfNull(lookup);

        LEntryDraft draft = page.LLiveryPageDraft;
        (IReadOnlyList<(LReflexDraft, LReflexGuise?)> shown, IReadOnlyList<(LReflexDraft, LReflexGuise?)> folded) =
            LSheetReflexDivide(page);
        IReadOnlyList<LFanqieRow> rows = [.. page.LLiveryPageFanqie.SelectMany(static group => group.LFanqieGroupRows)];

        StringBuilder lines = new StringBuilder();
        LAccentSheet accent = page.LLiveryPageAccent;
        bool toned = LContour.LContourToneCheck(accent.LAccentSheetContour);
        foreach (LAccentRow row in accent.LAccentSheetRows.Prepend(accent.LAccentSheetPrimary))
        {
            if (row.LAccentRowText.Length == 0)
            {
                continue;
            }

            bool primary = ReferenceEquals(row, accent.LAccentSheetPrimary);
            string ensign = page.LLiveryPageEnsign.TryGetValue(row.LAccentRowVariety, out string? path)
                ? LSheetEnsign.LSheetEnsignRead(ensigns, path)
                : string.Empty;
            lines.Append(primary ? "<tr class=\"accent primary\"><th>" : "<tr class=\"accent\"><th>");
            if (ensign.Length > 0)
            {
                lines.Append("<span class=\"flag ").Append(ensign).Append("\" title=\"")
                    .Append(LSheet.LSheetNormalize(row.LAccentRowVariety)).Append("\"></span>");
            }
            else
            {
                lines.Append(LSheet.LSheetNormalize(row.LAccentRowVariety));
            }

            lines.Append("</th><td>");
            if (accent.LAccentSheetOpener.Length > 0)
            {
                lines.Append("<span class=\"bracket\">")
                    .Append(LSheet.LSheetNormalize(accent.LAccentSheetOpener)).Append("</span>");
            }

            lines.Append("<span class=\"text\">")
                .Append(LSheetToneFormat(row.LAccentRowText, toned, LSheet.LSheetNormalize)).Append("</span>");
            if (accent.LAccentSheetCloser.Length > 0)
            {
                lines.Append("<span class=\"bracket\">")
                    .Append(LSheet.LSheetNormalize(accent.LAccentSheetCloser)).Append("</span>");
            }

            lines.Append("</td></tr>\n");
            if (primary && accent.LAccentSheetContour.Count > 0)
            {
                lines.Append("<tr class=\"contour\"><th></th><td>")
                    .Append(LSheetContourFormat(accent.LAccentSheetContour, theme, LSheet.LSheetNormalize))
                    .Append("</td></tr>\n");
            }
        }

        foreach (LTranscriptionDraft spelled in LGlyph.LGlyphOtherRead(
                     page.LLiveryPageGlyph, page.LLiveryPageTranscription))
        {
            lines.Append("<tr class=\"transcription\"><th>")
                .Append(LSheet.LSheetNormalize(spelled.LTranscriptionDraftScheme)).Append("</th><td>")
                .Append(LSheet.LSheetNormalize(spelled.LTranscriptionDraftText)).Append("</td></tr>\n");
        }

        if (page.LLiveryPageGlyph is LGlyph glyph && page.LLiveryPageCell.Count > 0)
        {
            lines.Append("<tr class=\"glyph\"><th>")
                .Append(LSheet.LSheetNormalize(glyph.LGlyphName)).Append("</th><td>");
            foreach (LGlyphCell cell in page.LLiveryPageCell)
            {
                lines.Append("<span class=\"cell\">")
                    .Append(LSheet.LSheetNormalize(cell.LGlyphCellText)).Append("</span>");
            }

            lines.Append("</td></tr>\n");
        }

        if (shown.Count == 0 && folded.Count == 0 && lines.Length == 0)
        {
            return;
        }

        sheet.Append("<div class=\"sound\">\n");
        LSheetReflexAppend(sheet, shown, rows, draft.LEntryDraftHeadword);
        if (folded.Count > 0)
        {
            sheet.Append("<details class=\"fold\">\n<summary>")
                .Append(LSheet.LSheetNormalize(lookup("Reflex.More"))).Append("</summary>\n");
            LSheetReflexAppend(sheet, folded, rows, draft.LEntryDraftHeadword);
            sheet.Append("</details>\n");
        }

        if (lines.Length > 0)
        {
            sheet.Append("<table class=\"pronunciation\">\n").Append(lines).Append("</table>\n");
        }

        sheet.Append("</div>\n");
    }

    internal static (IReadOnlyList<(LReflexDraft, LReflexGuise?)>, IReadOnlyList<(LReflexDraft, LReflexGuise?)>)
        LSheetReflexDivide(LLiveryPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

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

            LReflexGuise? guise = LReflexGuise.LReflexGuiseFind(page.LLiveryPageGuise, index);
            bool hidden = guise?.LReflexGuiseFolded
                ?? page.LLiveryPageFolded.Contains(reflex.LReflexDraftLanguage, StringComparer.Ordinal);
            (hidden ? folded : shown).Add((reflex, guise));
        }

        return (shown, folded);
    }

    internal static IReadOnlyList<string> LSheetLabelScan(IReadOnlyList<(LReflexDraft, LReflexGuise?)> readings)
    {
        ArgumentNullException.ThrowIfNull(readings);

        List<string> labels = [];
        string? named = null;
        foreach ((LReflexDraft reflex, LReflexGuise? _) in readings)
        {
            string language = reflex.LReflexDraftLanguage;
            labels.Add(string.Equals(named, language, StringComparison.Ordinal) ? string.Empty : language);
            named = language;
        }

        return labels;
    }

    internal static string LSheetSlashFormat(string text, bool phonemic)
    {
        ArgumentNullException.ThrowIfNull(text);

        return phonemic && !text.StartsWith('/') ? '/' + text + '/' : text;
    }

    internal static string LSheetContourFormat(
        IReadOnlyList<LContour> contour, LTheme theme, Func<string, string> escape)
    {
        ArgumentNullException.ThrowIfNull(contour);
        ArgumentNullException.ThrowIfNull(theme);
        ArgumentNullException.ThrowIfNull(escape);

        string line = theme.LThemeRead("line");
        string muted = theme.LThemeRead("muted");
        string ink = theme.LThemeRead("ink");
        string family = escape(theme.LThemeFamily);
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
                string color = theme.LThemeRead(LSheetRoleFormat(LContour.LContourRoleRead(levels[step])));
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
                .Append(escape(contour[index].LContourText)).Append("</text></svg>");
            chart.Append("<img src=\"data:image/svg+xml;base64,")
                .Append(Convert.ToBase64String(Encoding.UTF8.GetBytes(figure.ToString())))
                .Append("\" class=\"llyn-contour-chart\">");
        }

        return chart.Append("</span>").ToString();
    }

    internal static string LSheetToneFormat(string text, bool toned, Func<string, string> escape)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(escape);

        if (!toned)
        {
            return escape(text);
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
                       || (text[end] == LSheetToneJoiner
                           && end + 1 < text.Length
                           && char.IsAsciiDigit(text[end + 1]))))
            {
                end++;
            }

            marked.Append(escape(text[start..index]))
                .Append("<span class=\"llyn-tone\">").Append(text, index, end - index).Append("</span>");
            start = end;
            index = end;
        }

        return marked.Append(escape(text[start..])).ToString();
    }

    private static void LSheetReflexAppend(
        StringBuilder sheet,
        IReadOnlyList<(LReflexDraft, LReflexGuise?)> readings,
        IReadOnlyList<LFanqieRow> rows,
        string headword)
    {
        if (readings.Count == 0)
        {
            return;
        }

        sheet.Append("<table class=\"reflex\">\n");
        IReadOnlyList<string> labels = LSheetLabelScan(readings);
        for (int index = 0; index < readings.Count; index++)
        {
            (LReflexDraft reflex, LReflexGuise? guise) = readings[index];
            string label = labels[index];
            bool phonemic = guise?.LReflexGuisePhonemic == true;
            string text = LSheetSlashFormat(guise?.LReflexGuiseShown ?? reflex.LReflexDraftText, phonemic);
            string old = LAnchor.LAnchorTextFormat(rows, reflex.LReflexDraftAnchors, headword, LSheetAnchorSeparator);
            sheet.Append(reflex.LReflexDraftMain ? "<tr class=\"main\">" : "<tr>")
                .Append("<th class=\"label\">").Append(LSheet.LSheetNormalize(label)).Append("</th>")
                .Append("<td class=\"kind\">").Append(LSheet.LSheetNormalize(reflex.LReflexDraftKind)).Append("</td>")
                .Append("<td class=\"text\">").Append(LSheetToneFormat(text, phonemic, LSheet.LSheetNormalize))
                .Append("</td>")
                .Append("<td class=\"romanization\">")
                .Append(LSheet.LSheetNormalize(reflex.LReflexDraftRomanization)).Append("</td>")
                .Append("<td class=\"meaning\">").Append(LSheet.LSheetNormalize(reflex.LReflexDraftMeaning))
                .Append("</td>")
                .Append("<td class=\"note\">").Append(LSheet.LSheetNormalize(reflex.LReflexDraftNote)).Append("</td>")
                .Append("<td class=\"anchor\">").Append(LSheet.LSheetNormalize(old)).Append("</td></tr>\n");
        }

        sheet.Append("</table>\n");
    }

    private static string LSheetRoleFormat(LContourRole role)
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
}
