using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LOutlineSection
{
    public static void LOutlineSectionAppend(StringBuilder outline, LPortraitSection section, int depth)
    {
        ArgumentNullException.ThrowIfNull(outline);
        ArgumentNullException.ThrowIfNull(section);

        switch (section.LPortraitSectionRole)
        {
            case LPortraitRole.LPortraitRoleCard:
            case LPortraitRole.LPortraitRoleKind:
                LOutlineCardAppend(outline, section, depth);
                break;
            case LPortraitRole.LPortraitRolePhrase:
                LOutlineLineAppend(outline, section.LPortraitSectionLine, "**", "**");
                LOutlineBodyAppend(outline, section, depth);
                break;
            case LPortraitRole.LPortraitRoleScene:
                LOutlineChipAppend(outline, section.LPortraitSectionChip, "*", "*", " · ");
                LOutlineBodyAppend(outline, section, depth);
                break;
            case LPortraitRole.LPortraitRoleTone:
                LOutlineChipAppend(outline, section.LPortraitSectionChip, "`", "`", " · ");
                LOutlineBodyAppend(outline, section, depth);
                break;
            case LPortraitRole.LPortraitRoleLabel:
                LOutlineChipAppend(outline, section.LPortraitSectionChip, "`", "`", " ");
                LOutlineBodyAppend(outline, section, depth);
                break;
            case LPortraitRole.LPortraitRoleBridge:
                LOutlineBodyAppend(outline, section, depth);
                break;
            case LPortraitRole.LPortraitRoleQuote:
                LOutlineQuoteAppend(outline, section, depth);
                break;
            case LPortraitRole.LPortraitRoleUsage:
                LOutlineUsageAppend(outline, section);
                break;
            default:
                LOutlineBandAppend(outline, section, depth);
                break;
        }
    }

    private static void LOutlineBandAppend(StringBuilder outline, LPortraitSection section, int depth)
    {
        if (section.LPortraitSectionHeading.Length > 0)
        {
            outline.Append("## ").Append(LOutline.LOutlineNormalize(section.LPortraitSectionHeading)).Append("\n\n");
        }

        LOutlineChipAppend(outline, section.LPortraitSectionChip, "`", "`", " ");

        foreach (LPortraitLine line in section.LPortraitSectionLine)
        {
            outline.Append("- ").Append(LOutlineLineFormat(line)).Append('\n');
        }

        if (section.LPortraitSectionLine.Count > 0)
        {
            outline.Append('\n');
        }

        LOutlineBodyAppend(outline, section, depth);
    }

    private static void LOutlineCardAppend(StringBuilder outline, LPortraitSection section, int depth)
    {
        outline.Append(new string('#', Math.Min(depth + 2, 6)))
            .Append(' ')
            .Append(section.LPortraitSectionPosition.ToString(CultureInfo.InvariantCulture))
            .Append(" · ")
            .Append(LOutline.LOutlineNormalize(section.LPortraitSectionHeading))
            .Append("\n\n");

        int start = 0;
        if (section.LPortraitSectionChild.Count > 0
            && section.LPortraitSectionChild[0].LPortraitSectionRole == LPortraitRole.LPortraitRolePhrase)
        {
            LOutlineSectionAppend(outline, section.LPortraitSectionChild[0], depth + 1);
            start = 1;
        }

        LOutlineLineAppend(outline, section.LPortraitSectionLine, string.Empty, string.Empty);
        LOutlineBodyAppend(outline, section, depth, start);
    }

    private static void LOutlineUsageAppend(StringBuilder outline, LPortraitSection section)
    {
        outline.Append("- ");
        foreach (LPortraitLink link in section.LPortraitSectionLink)
        {
            outline.Append("**").Append(LOutline.LOutlineNormalize(link.LPortraitLinkHeadword)).Append("**");
        }

        foreach (LPortraitLine line in section.LPortraitSectionLine)
        {
            if (line.LPortraitLineText.Length > 0)
            {
                outline.Append(" — ").Append(LOutline.LOutlineNormalize(line.LPortraitLineText));
            }

            outline.Append(" *(").Append(LOutline.LOutlineNormalize(line.LPortraitLineLabel));
        }

        foreach (LPortraitLink link in section.LPortraitSectionLink)
        {
            outline.Append(", ").Append(LOutline.LOutlineNormalize(link.LPortraitLinkLanguage));
        }

        outline.Append(")*\n");
    }

    private static void LOutlineQuoteAppend(StringBuilder outline, LPortraitSection section, int depth)
    {
        if (section.LPortraitSectionLine.Count == 0)
        {
            return;
        }

        outline.Append("- ").Append(LOutlineLineFormat(section.LPortraitSectionLine[0])).Append('\n');

        for (int index = 1; index < section.LPortraitSectionLine.Count; index++)
        {
            outline.Append("  - ").Append(LOutlineLineFormat(section.LPortraitSectionLine[index])).Append('\n');
        }

        if (section.LPortraitSectionChip.Count > 0)
        {
            outline.Append("  - ").Append(LOutlineChipFormat(section.LPortraitSectionChip, "`", "`", " ")).Append('\n');
        }

        if (section.LPortraitSectionLink.Count > 0)
        {
            outline.Append("  - ").Append(LOutlineLinkFormat(section.LPortraitSectionLink)).Append('\n');
        }

        foreach (LPortraitSection child in section.LPortraitSectionChild)
        {
            if (child.LPortraitSectionRole == LPortraitRole.LPortraitRoleBridge)
            {
                outline.Append("  - ").Append(LOutlineLinkFormat(child.LPortraitSectionLink)).Append('\n');
            }
            else
            {
                LOutlineSectionAppend(outline, child, depth + 1);
            }
        }

        if (section.LPortraitSectionNote.Length > 0)
        {
            LOutlineNote.LOutlineNoteAppend(outline, section.LPortraitSectionNote);
        }

        LOutlineMediaAppend(outline, section);
    }

    private static void LOutlineBodyAppend(
        StringBuilder outline, LPortraitSection section, int depth, int start = 0)
    {
        if (section.LPortraitSectionLink.Count > 0)
        {
            outline.Append(LOutlineLinkFormat(section.LPortraitSectionLink)).Append("\n\n");
        }

        if (section.LPortraitSectionNote.Length > 0)
        {
            LOutlineNote.LOutlineNoteAppend(outline, section.LPortraitSectionNote);
        }

        bool quoting = false;
        for (int index = start; index < section.LPortraitSectionChild.Count; index++)
        {
            LPortraitSection child = section.LPortraitSectionChild[index];
            bool quote = child.LPortraitSectionRole
                is LPortraitRole.LPortraitRoleQuote or LPortraitRole.LPortraitRoleUsage;
            if (quoting && !quote)
            {
                outline.Append('\n');
            }

            quoting = quote;
            LOutlineSectionAppend(outline, child, depth + 1);
        }

        if (quoting)
        {
            outline.Append('\n');
        }

        LOutlineMediaAppend(outline, section);
    }

    private static void LOutlineMediaAppend(StringBuilder outline, LPortraitSection section)
    {
        foreach (LPortraitMedia image in section.LPortraitSectionImage)
        {
            outline.Append("![").Append(LOutline.LOutlineNormalize(image.LPortraitMediaSpan)).Append("](")
                .Append(image.LPortraitMediaLocation).Append(")\n\n");
        }

        foreach (LPortraitMedia video in section.LPortraitSectionVideo)
        {
            outline.Append("[▶ ")
                .Append(LOutline.LOutlineNormalize(video.LPortraitMediaLocation))
                .Append("](")
                .Append(video.LPortraitMediaLocation)
                .Append(')');

            if (video.LPortraitMediaSpan.Length > 0)
            {
                outline.Append(" · ").Append(LOutline.LOutlineNormalize(video.LPortraitMediaSpan));
            }

            outline.Append("\n\n");
        }
    }

    private static void LOutlineLineAppend(
        StringBuilder outline, IReadOnlyList<LPortraitLine> lines, string open, string close)
    {
        foreach (LPortraitLine line in lines)
        {
            outline.Append(open).Append(LOutlineLineFormat(line)).Append(close).Append("\n\n");
        }
    }

    private static void LOutlineChipAppend(
        StringBuilder outline, IReadOnlyList<string> chips, string open, string close, string join)
    {
        if (chips.Count > 0)
        {
            outline.Append(LOutlineChipFormat(chips, open, close, join)).Append("\n\n");
        }
    }

    private static string LOutlineLineFormat(LPortraitLine line)
    {
        string text = LOutline.LOutlineNormalize(line.LPortraitLineText);
        return line.LPortraitLineLabel.Length == 0
            ? text
            : "**" + LOutline.LOutlineNormalize(line.LPortraitLineLabel) + "** " + text;
    }

    private static string LOutlineLinkFormat(IReadOnlyList<LPortraitLink> links)
    {
        List<string> bridges = new List<string>();
        foreach (LPortraitLink link in links)
        {
            bridges.Add(LOutline.LOutlineNormalize(link.LPortraitLinkHeadword)
                + " (" + LOutline.LOutlineNormalize(link.LPortraitLinkLanguage) + ")");
        }

        return "→ " + string.Join(" · ", bridges);
    }

    private static string LOutlineChipFormat(IReadOnlyList<string> chips, string open, string close, string join)
    {
        List<string> marks = new List<string>();
        foreach (string chip in chips)
        {
            marks.Add(open + (open == "`" ? chip : LOutline.LOutlineNormalize(chip)) + close);
        }

        return string.Join(join, marks);
    }
}
