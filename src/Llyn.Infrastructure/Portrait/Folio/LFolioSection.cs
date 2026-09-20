using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LFolioSection
{
    private const string LFolioSectionJoin = "   ·   ";

    public static void LFolioSectionAppend(
        StringBuilder body, LPortraitSection section, List<LPortraitAsset> plates, LTheme theme)
    {
        ArgumentNullException.ThrowIfNull(body);
        ArgumentNullException.ThrowIfNull(section);
        ArgumentNullException.ThrowIfNull(plates);
        ArgumentNullException.ThrowIfNull(theme);

        switch (section.LPortraitSectionRole)
        {
            case LPortraitRole.LPortraitRoleCard:
            case LPortraitRole.LPortraitRoleKind:
                LFolioCardAppend(body, section, plates, theme);
                break;
            case LPortraitRole.LPortraitRolePhrase:
                LFolioLineAppend(body, "Phrase", section.LPortraitSectionLine);
                LFolioBodyAppend(body, section, plates, theme);
                break;
            case LPortraitRole.LPortraitRoleScene:
                LFolioChipAppend(body, section.LPortraitSectionChip, "Scene");
                LFolioBodyAppend(body, section, plates, theme);
                break;
            case LPortraitRole.LPortraitRoleTone:
                LFolioChipAppend(body, section.LPortraitSectionChip, "Tone");
                LFolioBodyAppend(body, section, plates, theme);
                break;
            case LPortraitRole.LPortraitRoleLabel:
                LFolioChipAppend(body, section.LPortraitSectionChip, "Label");
                LFolioBodyAppend(body, section, plates, theme);
                break;
            case LPortraitRole.LPortraitRoleBridge:
                LFolioBodyAppend(body, section, plates, theme);
                break;
            case LPortraitRole.LPortraitRoleQuote:
                LFolioQuoteAppend(body, section, plates, theme);
                break;
            case LPortraitRole.LPortraitRoleUsage:
                LFolioUsageAppend(body, section);
                break;
            default:
                LFolioBandAppend(body, section, plates, theme);
                break;
        }
    }

    private static void LFolioBandAppend(
        StringBuilder body, LPortraitSection section, List<LPortraitAsset> plates, LTheme theme)
    {
        if (section.LPortraitSectionHeading.Length > 0)
        {
            LFolioLine.LFolioLineAppend(body, "Band", section.LPortraitSectionHeading);
        }

        LFolioChipAppend(body, section.LPortraitSectionChip, "Sound");
        LFolioLineAppend(body, "Row", section.LPortraitSectionLine);
        LFolioBodyAppend(body, section, plates, theme);
    }

    private static void LFolioCardAppend(
        StringBuilder body, LPortraitSection section, List<LPortraitAsset> plates, LTheme theme)
    {
        string line = LFolioStyle.LFolioStyleNormalize(theme.LThemeRead("line"));
        string raised = LFolioStyle.LFolioStyleNormalize(theme.LThemeRead("accentSoft"));
        bool named = section.LPortraitSectionRole == LPortraitRole.LPortraitRoleCard;

        body.Append("<w:tbl><w:tblPr><w:tblW w:w=\"5000\" w:type=\"pct\"/><w:tblBorders>")
            .Append("<w:top w:val=\"single\" w:sz=\"6\" w:color=\"").Append(line).Append("\"/>")
            .Append("<w:left w:val=\"single\" w:sz=\"6\" w:color=\"").Append(line).Append("\"/>")
            .Append("<w:bottom w:val=\"single\" w:sz=\"6\" w:color=\"").Append(line).Append("\"/>")
            .Append("<w:right w:val=\"single\" w:sz=\"6\" w:color=\"").Append(line).Append("\"/>")
            .Append("<w:insideH w:val=\"single\" w:sz=\"6\" w:color=\"").Append(line).Append("\"/>")
            .Append("</w:tblBorders><w:tblCellMar>")
            .Append("<w:top w:w=\"200\" w:type=\"dxa\"/><w:left w:w=\"280\" w:type=\"dxa\"/>")
            .Append("<w:bottom w:w=\"200\" w:type=\"dxa\"/><w:right w:w=\"280\" w:type=\"dxa\"/>")
            .Append("</w:tblCellMar></w:tblPr><w:tblGrid><w:gridCol w:w=\"9360\"/></w:tblGrid>");

        body.Append("<w:tr><w:tc><w:tcPr><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"")
            .Append(raised).Append("\"/></w:tcPr>");
        LFolioLine.LFolioLineAppend(
            body,
            named ? "CardTitle" : "CardKind",
            section.LPortraitSectionPosition.ToString(CultureInfo.InvariantCulture) + "  ",
            section.LPortraitSectionHeading);
        body.Append("</w:tc></w:tr>");

        body.Append("<w:tr><w:tc>");

        int start = 0;
        if (section.LPortraitSectionChild.Count > 0
            && section.LPortraitSectionChild[0].LPortraitSectionRole == LPortraitRole.LPortraitRolePhrase)
        {
            LFolioSectionAppend(body, section.LPortraitSectionChild[0], plates, theme);
            start = 1;
        }

        LFolioLineAppend(body, "Sense", section.LPortraitSectionLine);
        LFolioBodyAppend(body, section, plates, theme, start);

        body.Append("<w:p><w:pPr><w:spacing w:after=\"0\"/></w:pPr></w:p>");
        body.Append("</w:tc></w:tr></w:tbl>");
        body.Append("<w:p><w:pPr><w:spacing w:after=\"0\"/></w:pPr></w:p>");
    }

    private static void LFolioQuoteAppend(
        StringBuilder body, LPortraitSection section, List<LPortraitAsset> plates, LTheme theme)
    {
        if (section.LPortraitSectionLine.Count > 0)
        {
            LPortraitLine first = section.LPortraitSectionLine[0];
            string mark = first.LPortraitLineLabel.Length > 0 ? "•  " + first.LPortraitLineLabel + "  " : "•  ";
            LFolioLine.LFolioLineAppend(body, "Quote", mark, first.LPortraitLineText);
        }

        for (int index = 1; index < section.LPortraitSectionLine.Count; index++)
        {
            LPortraitLine row = section.LPortraitSectionLine[index];
            string tag = row.LPortraitLineLabel.Length > 0 ? row.LPortraitLineLabel + "  " : string.Empty;
            LFolioLine.LFolioLineAppend(body, "RowDetail", "     " + tag, row.LPortraitLineText);
        }

        LFolioChipAppend(body, section.LPortraitSectionChip, "Label");
        LFolioBodyAppend(body, section, plates, theme);
    }

    private static void LFolioUsageAppend(StringBuilder body, LPortraitSection section)
    {
        foreach (LPortraitLink link in section.LPortraitSectionLink)
        {
            LFolioLine.LFolioLineAppend(body, "Row", "→  " + link.LPortraitLinkHeadword, string.Empty);
        }

        List<string> parts = [];
        foreach (LPortraitLine line in section.LPortraitSectionLine)
        {
            if (line.LPortraitLineText.Length > 0)
            {
                parts.Add(line.LPortraitLineText);
            }

            parts.Add(line.LPortraitLineLabel);
        }

        foreach (LPortraitLink link in section.LPortraitSectionLink)
        {
            parts.Add(link.LPortraitLinkLanguage);
        }

        LFolioLine.LFolioLineAppend(body, "RowDetail", "     " + string.Join(LFolioSectionJoin, parts));
    }

    private static void LFolioBodyAppend(
        StringBuilder body, LPortraitSection section, List<LPortraitAsset> plates, LTheme theme, int start = 0)
    {
        if (section.LPortraitSectionLink.Count > 0)
        {
            List<string> bridges = new List<string>();
            foreach (LPortraitLink link in section.LPortraitSectionLink)
            {
                bridges.Add(link.LPortraitLinkHeadword + " (" + link.LPortraitLinkLanguage + ")");
            }

            LFolioLine.LFolioLineAppend(body, "Bridge", "→ " + string.Join(LFolioSectionJoin, bridges));
        }

        if (section.LPortraitSectionNote.Length > 0)
        {
            LFolioNote.LFolioNoteAppend(body, section.LPortraitSectionNote);
        }

        for (int index = start; index < section.LPortraitSectionChild.Count; index++)
        {
            LFolioSectionAppend(body, section.LPortraitSectionChild[index], plates, theme);
        }

        foreach (LPortraitMedia image in section.LPortraitSectionImage)
        {
            if (LPortraitAsset.LPortraitAssetLoad(image.LPortraitMediaLocation) is not LPortraitAsset held)
            {
                LFolioLine.LFolioLineAppend(body, "Plate", image.LPortraitMediaLocation);
                continue;
            }

            plates.Add(held);
            (int width, int height) = LPortraitSize.LPortraitSizeRead(held.LPortraitAssetData);
            (long across, long down) = LFolioPlateClamp(width, height);
            LFolioLine.LFolioLineDraw(body, plates.Count, across, down);

            if (image.LPortraitMediaSpan.Length > 0)
            {
                LFolioLine.LFolioLineAppend(body, "Label", image.LPortraitMediaSpan);
            }
        }

        foreach (LPortraitMedia video in section.LPortraitSectionVideo)
        {
            string span = video.LPortraitMediaSpan.Length > 0
                ? "  ·  " + video.LPortraitMediaSpan
                : string.Empty;

            LFolioLine.LFolioLineAppend(body, "Plate", "▶  ", video.LPortraitMediaLocation + span);
        }
    }

    private static void LFolioLineAppend(StringBuilder body, string style, IReadOnlyList<LPortraitLine> lines)
    {
        foreach (LPortraitLine line in lines)
        {
            if (line.LPortraitLineLabel.Length == 0)
            {
                LFolioLine.LFolioLineAppend(body, style, line.LPortraitLineText);
            }
            else
            {
                LFolioLine.LFolioLineAppend(body, style, line.LPortraitLineLabel + "  ", line.LPortraitLineText);
            }
        }
    }

    private static void LFolioChipAppend(StringBuilder body, IReadOnlyList<string> chips, string style)
    {
        if (chips.Count > 0)
        {
            LFolioLine.LFolioLineAppend(body, style, string.Join(LFolioSectionJoin, chips));
        }
    }

    private static (long LFolioAcross, long LFolioDown) LFolioPlateClamp(int width, int height)
    {
        const long ceiling = 4114800L;

        if (width <= 0 || height <= 0)
        {
            return (ceiling, ceiling * 9 / 16);
        }

        long across = (long)width * 9525L;
        long down = (long)height * 9525L;

        if (across <= ceiling)
        {
            return (across, down);
        }

        return (ceiling, down * ceiling / across);
    }
}
