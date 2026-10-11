using System;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LSheetBox
{
    internal static void LSheetFanqieAppend(
        StringBuilder sheet, LLiveryPage livery, LPortraitPage portrait, LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(livery);
        ArgumentNullException.ThrowIfNull(label);

        LSheetBoxAppend(
            sheet,
            LSheetSection.LSheetSectionFind(portrait, label.LPortraitLabelFanqie),
            "fanqie",
            livery.LLiveryPageOpened.Contains(LFoldBox.LFoldBoxFanqie));
    }

    internal static void LSheetScriptAppend(
        StringBuilder sheet, LLiveryPage livery, LPortraitPage portrait, LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(livery);
        ArgumentNullException.ThrowIfNull(label);

        LSheetBoxAppend(
            sheet,
            LSheetSection.LSheetSectionFind(portrait, label.LPortraitLabelScript),
            "script",
            livery.LLiveryPageOpened.Contains(LFoldBox.LFoldBoxScript));
    }

    private static void LSheetBoxAppend(StringBuilder sheet, LPortraitSection? section, string style, bool opened)
    {
        ArgumentNullException.ThrowIfNull(sheet);

        if (section is null)
        {
            return;
        }

        sheet.Append("<details class=\"box ").Append(style).Append('"')
            .Append(opened ? " open" : string.Empty)
            .Append(">\n<summary>")
            .Append(LSheet.LSheetNormalize(section.LPortraitSectionHeading))
            .Append("</summary>\n");
        LSheetSection.LSheetSectionAppend(sheet, section with { LPortraitSectionHeading = string.Empty });
        sheet.Append("</details>\n");
    }
}
