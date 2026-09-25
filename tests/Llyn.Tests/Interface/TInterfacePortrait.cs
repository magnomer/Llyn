using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LPortraitLabel TPortraitLabelRead() => new(
        "Unknown",
        "Meaning",
        "Meanings",
        "Collocation",
        "Collocations",
        "Links here",
        "Note",
        "Forms",
        "Paradigm",
        "Frequency",
        "Characters",
        "Character forms",
        "Rime books",
        "Example",
        "Gloss",
        "Source",
        "Mentions",
        "Etymology",
        "Situations",
        "Registers",
        "Translations",
        "Tags");

    internal static LPortraitLink TPortraitLinkCreate(string headword, string language) =>
        new(headword, language);

    internal static LPortraitMedia TPortraitMediaCreate(string location, string span) =>
        new(location, span);

    internal static LTheme TThemeLoad() => LThemeLoader.LThemeLoaderLoad();

    internal static string TThemeRead(this LTheme theme, string name) => theme.LThemeRead(name);

    internal static string TSheetFormat(LPortraitPage page, LTheme theme) => LSheet.LSheetFormat(page, theme);

    internal static string TOutlineFormat(LPortraitPage page) => LOutline.LOutlineFormat(page);

    internal static string TSheetNormalize(string? text) => LSheet.LSheetNormalize(text);

    internal static string TOutlineNormalize(string? text) => LOutline.LOutlineNormalize(text);

    internal static string TFolioLineFormat(string? text) => LFolioLine.LFolioLineFormat(text);

    internal static LPortraitLegend TPortraitLegendRead() => new(
        "Unknown",
        "Untitled",
        "Unwritten",
        "Not used yet",
        "Used in one place",
        "places use it",
        "Translation",
        "Source",
        "Authors",
        "Year",
        "URL",
        "Note",
        "Description",
        new Dictionary<LReferenceKind, string>());

    internal static IReadOnlyList<string> TPortraitTextRead(LPortraitPage page)
    {
        List<string> shown = [];
        TPortraitTextAdd(shown, page.LPortraitPageTitle);
        TPortraitTextAdd(shown, page.LPortraitPageLanguage);
        TPortraitTextAdd(shown, page.LPortraitPageLine);
        TPortraitTextAdd(shown, page.LPortraitPageChip);
        foreach (LPortraitSection section in page.LPortraitPageSection)
        {
            shown.AddRange(TPortraitTextRead(section));
        }

        return shown;
    }

    private static List<string> TPortraitTextRead(LPortraitSection section)
    {
        List<string> shown = [];
        if (section.LPortraitSectionRole
            is LPortraitRole.LPortraitRoleBand or LPortraitRole.LPortraitRoleCard or LPortraitRole.LPortraitRoleKind)
        {
            TPortraitTextAdd(shown, section.LPortraitSectionHeading);
        }

        TPortraitTextAdd(shown, section.LPortraitSectionLine);
        TPortraitTextAdd(shown, section.LPortraitSectionNote);
        TPortraitTextAdd(shown, section.LPortraitSectionChip);

        foreach (LPortraitLink link in section.LPortraitSectionLink)
        {
            TPortraitTextAdd(shown, link.LPortraitLinkHeadword);
            TPortraitTextAdd(shown, link.LPortraitLinkLanguage);
        }

        foreach (LPortraitMedia video in section.LPortraitSectionVideo)
        {
            TPortraitTextAdd(shown, video.LPortraitMediaLocation);
            TPortraitTextAdd(shown, video.LPortraitMediaSpan);
        }

        foreach (LPortraitSection child in section.LPortraitSectionChild)
        {
            shown.AddRange(TPortraitTextRead(child));
        }

        return shown;
    }

    private static void TPortraitTextAdd(List<string> shown, string text)
    {
        if (text.Length > 0)
        {
            shown.Add(text);
        }
    }

    private static void TPortraitTextAdd(List<string> shown, IReadOnlyList<string> chips)
    {
        foreach (string chip in chips)
        {
            TPortraitTextAdd(shown, chip);
        }
    }

    private static void TPortraitTextAdd(List<string> shown, IReadOnlyList<LPortraitLine> lines)
    {
        foreach (LPortraitLine line in lines)
        {
            TPortraitTextAdd(shown, line.LPortraitLineLabel);
            TPortraitTextAdd(shown, line.LPortraitLineText);
        }
    }

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
