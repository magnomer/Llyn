using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LPortraitLabel TPortraitLabelRead() => LPortraitLabel.LPortraitLabelDefault;

    internal static LPortraitLink TPortraitLinkCreate(long id, string headword, string language) =>
        new(id, headword, language);

    internal static LPortraitMedia TPortraitMediaCreate(
        string location, string span, bool moving) =>
        new(location, span, moving);

    internal static LTheme TThemeLoad() => LTheme.LThemeLoad();

    internal static string TThemeRead(this LTheme theme, string name) => theme.LThemeRead(name);

    internal static string TSheetFormat(LPortraitPage page, LTheme theme) => LSheet.LSheetFormat(page, theme);

    internal static string TOutlineFormat(LPortraitPage page) => LOutline.LOutlineFormat(page);

    internal static string TSheetNormalize(string? text) => LSheet.LSheetNormalize(text);

    internal static string TOutlineNormalize(string? text) => LOutline.LOutlineNormalize(text);

    internal static string TFolioLineFormat(string? text) => LFolioLine.LFolioLineFormat(text);

    internal static LPortraitLegend TPortraitLegendRead() => LPortraitLegend.LPortraitLegendDefault;

    internal static IReadOnlyList<string> TPortraitTextRead(LPortraitPage page) =>
        LPortraitText.LPortraitTextRead(page);

    internal static LPortraitLine TPortraitLineCreate(
        string label, string text, string open = "", string close = "") =>
        new(label, text, open, close);

    internal static LPortraitSection TPortraitSectionCreate(
        string heading,
        IReadOnlyList<LPortraitLine> line,
        string note,
        IReadOnlyList<LPortraitMedia> image,
        IReadOnlyList<LPortraitMedia> video) =>
        new(heading, line, note, image, video);

    internal static LPortraitSection TPortraitSectionCreate(
        string heading,
        int position,
        IReadOnlyList<LPortraitLine> line,
        IReadOnlyList<string> chip,
        IReadOnlyList<LPortraitLink> link,
        IReadOnlyList<LPortraitMedia> image,
        IReadOnlyList<LPortraitMedia> video,
        IReadOnlyList<LPortraitSection> child,
        LPortraitRole role = LPortraitRole.LPortraitRoleBand) =>
        new(heading, line, string.Empty, image, video, position, chip, link, child, role);

    internal static LPortraitPage TPortraitPageCreate(
        string title,
        string language,
        IReadOnlyList<string> chip,
        IReadOnlyList<LPortraitSection> section) =>
        new(title, language, chip, section);

    internal static LPortraitPage TPortraitPageCreate(
        string title,
        string language,
        bool favorite,
        IReadOnlyList<LPortraitLine> line,
        IReadOnlyList<string> chip,
        IReadOnlyList<LPortraitSection> section) =>
        new(title, language, chip, section, favorite, line);

    internal static LPressTicket TPressTicketCreate(string printer, bool landscape, int copies) =>
        new(
            printer,
            LPressPaper.LPressPaperMetric,
            landscape,
            copies,
            true,
            LPressSide.LPressSideLong,
            LPressInk.LPressInkGray);

    internal static string TMarkdownNormalize(string? text) => LMarkdown.LMarkdownNormalize(text);

    internal static IReadOnlyList<LMarkdownBlock> TMarkdownParse(string? text) =>
        LMarkdown.LMarkdownParse(text);

    internal static void TFolioSave(LPortraitPage page, LTheme theme, string path)
    {
        LFolio.LFolioSave(page, theme, path);
    }
}
