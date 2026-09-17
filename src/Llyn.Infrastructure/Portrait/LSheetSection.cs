using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSheetSection
{
    public static void LSheetSectionAppend(StringBuilder sheet, LPortraitSection section)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(section);

        switch (section.LPortraitSectionRole)
        {
            case LPortraitRole.LPortraitRoleCard:
            case LPortraitRole.LPortraitRoleKind:
                LSheetCardAppend(sheet, section);
                break;
            case LPortraitRole.LPortraitRolePhrase:
                LSheetPhraseAppend(sheet, section);
                break;
            case LPortraitRole.LPortraitRoleScene:
                LSheetChipAppend(sheet, section.LPortraitSectionChip, "scene");
                LSheetBodyAppend(sheet, section);
                break;
            case LPortraitRole.LPortraitRoleTone:
                LSheetChipAppend(sheet, section.LPortraitSectionChip, "tone");
                LSheetBodyAppend(sheet, section);
                break;
            case LPortraitRole.LPortraitRoleLabel:
                LSheetChipAppend(sheet, section.LPortraitSectionChip, "labels");
                LSheetBodyAppend(sheet, section);
                break;
            case LPortraitRole.LPortraitRoleBridge:
                LSheetBodyAppend(sheet, section);
                break;
            case LPortraitRole.LPortraitRoleQuote:
                LSheetQuoteAppend(sheet, section);
                break;
            case LPortraitRole.LPortraitRoleUsage:
                LSheetUsageAppend(sheet, section);
                break;
            default:
                LSheetBandAppend(sheet, section);
                break;
        }
    }

    private static void LSheetBandAppend(StringBuilder sheet, LPortraitSection section)
    {
        sheet.Append("<section class=\"band\">\n");

        if (section.LPortraitSectionHeading.Length > 0)
        {
            sheet.Append("<h2>").Append(LSheet.LSheetNormalize(section.LPortraitSectionHeading)).Append("</h2>\n");
        }

        LSheetChipAppend(sheet, section.LPortraitSectionChip, "speech");
        LSheetLineAppend(sheet, section.LPortraitSectionLine, 0);

        bool rows = section.LPortraitSectionChild.Count > 0
            && section.LPortraitSectionChild[0].LPortraitSectionRole == LPortraitRole.LPortraitRoleUsage;
        if (rows)
        {
            sheet.Append("<div class=\"rows\">\n");
        }

        LSheetBodyAppend(sheet, section);

        if (rows)
        {
            sheet.Append("</div>\n");
        }

        sheet.Append("</section>\n");
    }

    private static void LSheetCardAppend(StringBuilder sheet, LPortraitSection section)
    {
        bool named = section.LPortraitSectionRole == LPortraitRole.LPortraitRoleCard;

        sheet.Append("<article class=\"card\">\n<header><span class=\"rank\">")
            .Append(section.LPortraitSectionPosition.ToString(CultureInfo.InvariantCulture))
            .Append("</span><span class=\"title")
            .Append(named ? string.Empty : " kind")
            .Append("\">")
            .Append(LSheet.LSheetNormalize(section.LPortraitSectionHeading))
            .Append("</span></header>\n<section>\n");

        int start = 0;
        if (section.LPortraitSectionChild.Count > 0
            && section.LPortraitSectionChild[0].LPortraitSectionRole == LPortraitRole.LPortraitRolePhrase)
        {
            LSheetSectionAppend(sheet, section.LPortraitSectionChild[0]);
            start = 1;
        }

        foreach (LPortraitLine line in section.LPortraitSectionLine)
        {
            sheet.Append("<p class=\"sense\">");
            LSheetTagAppend(sheet, line.LPortraitLineLabel);
            sheet.Append(LSheet.LSheetNormalize(line.LPortraitLineText)).Append("</p>\n");
        }

        LSheetBodyAppend(sheet, section, start);

        sheet.Append("</section>\n</article>\n");
    }

    private static void LSheetPhraseAppend(StringBuilder sheet, LPortraitSection section)
    {
        foreach (LPortraitLine line in section.LPortraitSectionLine)
        {
            sheet.Append("<p class=\"phrase\">")
                .Append(LSheet.LSheetNormalize(line.LPortraitLineText))
                .Append("</p>\n");
        }

        LSheetBodyAppend(sheet, section);
    }

    private static void LSheetQuoteAppend(StringBuilder sheet, LPortraitSection section)
    {
        if (section.LPortraitSectionLine.Count > 0)
        {
            LPortraitLine first = section.LPortraitSectionLine[0];

            sheet.Append("<div class=\"quote\"><span class=\"dot\">•</span>");

            if (first.LPortraitLineLabel.Length > 0)
            {
                sheet.Append("<span class=\"frame\">")
                    .Append(LSheet.LSheetNormalize(first.LPortraitLineLabel))
                    .Append("</span>");
            }

            sheet.Append("<span class=\"said\">")
                .Append(LSheet.LSheetNormalize(first.LPortraitLineText))
                .Append("</span></div>\n");
        }

        LSheetLineAppend(sheet, section.LPortraitSectionLine, 1);
        LSheetChipAppend(sheet, section.LPortraitSectionChip, "labels");
        LSheetBodyAppend(sheet, section);
    }

    private static void LSheetUsageAppend(StringBuilder sheet, LPortraitSection section)
    {
        sheet.Append("<div class=\"row\"><span class=\"mark\">→</span><span class=\"body\">");

        foreach (LPortraitLink link in section.LPortraitSectionLink)
        {
            sheet.Append("<b>").Append(LSheet.LSheetNormalize(link.LPortraitLinkHeadword)).Append("</b>");
        }

        foreach (LPortraitLine line in section.LPortraitSectionLine)
        {
            if (line.LPortraitLineText.Length > 0)
            {
                sheet.Append("<span>").Append(LSheet.LSheetNormalize(line.LPortraitLineText)).Append("</span>");
            }
        }

        sheet.Append("</span>");

        foreach (LPortraitLine line in section.LPortraitSectionLine)
        {
            sheet.Append("<span class=\"pill\">")
                .Append(LSheet.LSheetNormalize(line.LPortraitLineLabel))
                .Append("</span>");
        }

        foreach (LPortraitLink link in section.LPortraitSectionLink)
        {
            sheet.Append("<span class=\"speak\">")
                .Append(LSheet.LSheetNormalize(link.LPortraitLinkLanguage))
                .Append("</span>");
        }

        sheet.Append("</div>\n");
    }

    private static void LSheetBodyAppend(StringBuilder sheet, LPortraitSection section, int start = 0)
    {
        if (section.LPortraitSectionLink.Count > 0)
        {
            sheet.Append("<div class=\"bridge\">");

            foreach (LPortraitLink link in section.LPortraitSectionLink)
            {
                sheet.Append("<span><b>")
                    .Append(LSheet.LSheetNormalize(link.LPortraitLinkHeadword))
                    .Append("</b><i>")
                    .Append(LSheet.LSheetNormalize(link.LPortraitLinkLanguage))
                    .Append("</i></span>");
            }

            sheet.Append("</div>\n");
        }

        if (section.LPortraitSectionNote.Length > 0)
        {
            sheet.Append("<div class=\"note\">\n");
            LSheetNote.LSheetNoteAppend(sheet, section.LPortraitSectionNote);
            sheet.Append("</div>\n");
        }

        bool quoting = false;
        for (int index = start; index < section.LPortraitSectionChild.Count; index++)
        {
            LPortraitSection child = section.LPortraitSectionChild[index];
            bool quote = child.LPortraitSectionRole == LPortraitRole.LPortraitRoleQuote;
            if (quote && !quoting)
            {
                sheet.Append("<div class=\"quotes\">\n");
            }
            else if (!quote && quoting)
            {
                sheet.Append("</div>\n");
            }

            quoting = quote;
            LSheetSectionAppend(sheet, child);
        }

        if (quoting)
        {
            sheet.Append("</div>\n");
        }

        LSheetPlate.LSheetPlateAppend(sheet, section.LPortraitSectionImage, section.LPortraitSectionVideo);
    }

    private static void LSheetLineAppend(StringBuilder sheet, IReadOnlyList<LPortraitLine> lines, int start)
    {
        if (lines.Count <= start)
        {
            return;
        }

        sheet.Append("<div class=\"lines\">\n");

        for (int index = start; index < lines.Count; index++)
        {
            sheet.Append("<div class=\"line\">");
            LSheetTagAppend(sheet, lines[index].LPortraitLineLabel);
            sheet.Append("<span>")
                .Append(LSheet.LSheetNormalize(lines[index].LPortraitLineText))
                .Append("</span></div>\n");
        }

        sheet.Append("</div>\n");
    }

    private static void LSheetTagAppend(StringBuilder sheet, string label)
    {
        if (label.Length > 0)
        {
            sheet.Append("<span class=\"tag\">").Append(LSheet.LSheetNormalize(label)).Append("</span>");
        }
    }

    private static void LSheetChipAppend(StringBuilder sheet, IReadOnlyList<string> chips, string style)
    {
        if (chips.Count == 0)
        {
            return;
        }

        sheet.Append("<div class=\"").Append(style).Append("\">");

        foreach (string chip in chips)
        {
            sheet.Append("<span>").Append(LSheet.LSheetNormalize(chip)).Append("</span>");
        }

        sheet.Append("</div>\n");
    }
}
