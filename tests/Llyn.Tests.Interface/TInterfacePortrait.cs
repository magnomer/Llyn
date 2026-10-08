using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Application;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;

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
        "Tags",
        new Dictionary<LUnit, string>
        {
            [LUnit.LUnitContent] = "Content word",
            [LUnit.LUnitFunction] = "Function word",
            [LUnit.LUnitMorpheme] = "Morpheme",
            [LUnit.LUnitWord] = "Word",
        });

    internal static LPortraitLink TPortraitLinkCreate(string headword, string language) =>
        new(headword, language);

    internal static LPortraitMedia TPortraitMediaCreate(string location, string span) =>
        new(location, span);

    internal static LTheme TThemeLoad() => LThemeLoader.LThemeLoaderLoad();

    internal static string TThemeRead(this LTheme theme, string name) => theme.LThemeRead(name);

    internal static string TSheetFormat(LPortraitPage page, LTheme theme) => LSheet.LSheetFormat(page, theme);

    internal static string TOutlineFormat(LPortraitPage page) => LOutline.LOutlineFormat(page);

    internal static LLiveryNote TLiveryFormat(
        LLiveryPage page,
        Func<string, string> lookup,
        Func<long, string>? note = null,
        Func<string, string, string, string>? link = null) =>
        new LLiverySheet(TThemeLoad()).LLiveryFormat(
            page,
            new string('a', 32),
            note ?? (static id => id.ToString(CultureInfo.InvariantCulture)),
            link ?? (static (_, _, _) => string.Empty),
            lookup);

    internal static LLiveryNote TLiveryFormat(
        LLiveryStem stem, Func<string, string> lookup, Func<long, string>? note = null) =>
        new LLiverySheet(TThemeLoad()).LLiveryFormat(
            stem,
            new string('a', 32),
            note ?? (static id => id.ToString(CultureInfo.InvariantCulture)),
            lookup);

    internal static LLiveryNote TLiveryFormat(
        LLiveryDiwei diwei, Func<string, string> lookup, Func<long, string>? note = null) =>
        new LLiverySheet(TThemeLoad()).LLiveryFormat(
            diwei,
            new string('a', 32),
            note ?? (static id => id.ToString(CultureInfo.InvariantCulture)),
            lookup);

    internal static LLiveryNote TLiveryFormat(
        LLiveryLanguage language, Func<string, string> lookup, Func<long, string>? note = null) =>
        new LLiverySheet(TThemeLoad()).LLiveryFormat(
            language,
            new string('a', 32),
            note ?? (static id => id.ToString(CultureInfo.InvariantCulture)),
            lookup);

    internal static Func<long, string> TCourierNoteBuild(string stamp, IReadOnlyList<LEntry> entries) =>
        LCourierClerk.LCourierNoteBuild(new LLiverySheet(TThemeLoad()), stamp, entries);

    internal static Task<LReceipt> TCourierSend(
        LEngine engine, Func<long, LLiveryPage?> page, Func<string, LLiveryLanguage> language) =>
        engine.LEngineStaffHeld.LEngineStaffWorkspace.LWorkspaceStaffCourier.LCourierClerkSend(
            page, language, static key => key, CancellationToken.None);

    internal static LLiveryLanguage TLiveryLanguageBuild(
        string name,
        IReadOnlyList<LCatalogPronunciation> pronunciation,
        IReadOnlyList<LLiveryStem> stem,
        IReadOnlyList<LLiveryDiwei> diwei) =>
        new(name, pronunciation, stem, diwei);

    internal static LLiveryStem TLiveryStemCreate(LStemPage page, IReadOnlyList<LEntry> entry) =>
        new(page, entry);

    internal static LStemPage TStemPageCreate(string language, string key, IReadOnlyList<string> characters) =>
        new(language, key, characters);

    internal static LLiveryDiwei TLiveryDiweiCreate(
        string kind, string language, string key, IReadOnlyList<LEntry> entry) =>
        new(kind, new LDiweiPage(language, key, []), entry);

    internal static LDiweiSection TDiweiSectionCreate(
        string label,
        IReadOnlyList<LDiweiLine> lines,
        IReadOnlyList<LTallyRow> tallies,
        bool switched,
        bool respelled) =>
        new(label, lines, tallies, switched, respelled);

    internal static LDiweiLine TDiweiLineCreate(
        string reading, string label, bool rounded, int rank, IReadOnlyList<string> characters) =>
        new(reading, label, rounded, rank, characters);

    internal static LTallyMark TTallyMarkCreate(string text, IReadOnlyList<string> characters) =>
        new(text, characters);

    internal static LTallyRow TTallyRowCreate(string language, string kind, IReadOnlyList<LTallyMark> marks) =>
        new(language, kind, marks);

    internal static LTheme TThemeCreate(IReadOnlyDictionary<string, string> colors) => new(colors);

    internal static string TLiveryStyleFormat(LTheme theme) => LLiveryStyle.LLiveryStyleFormat(theme);

    internal static LTranslationTarget TTranslationTargetCreate(long id, string headword, string language) =>
        new(id, headword, language);

    internal static LUsage TUsageCreate(
        long id, LOwner owner, long entryId, string headword, string language, LStateValue title) =>
        new(id, owner, entryId, headword, language, title);

    internal static LGlossDraft TGlossDraftCreate(long id, string language, LStateValue text) =>
        new(id, language, text);

    internal static LSentenceDraft TSentenceDraftCreate(LExampleDraft example) =>
        new(example, LStateValue.LStateValueUnspecified, LStateValue.LStateValueUnspecified);

    internal static LEtymologyDraft TEtymologyDraftCreate(string text, IReadOnlyList<LMentionDraft> mentions) =>
        new(text, mentions);

    internal static LFanqieGroup TFanqieGroupCreate(
        string heading, string label, string source, IReadOnlyList<LFanqieRow> rows, IReadOnlyList<string> stems) =>
        new(heading, label, source, rows, stems);

    internal static LScriptImage TScriptImageCreate(
        string character, string style, int position, string caption, string gloss, byte[] data) =>
        new(character, style, position, caption, gloss, data);

    internal static LScriptGroup TScriptGroupCreate(
        string heading, string style, string gloss, IReadOnlyList<LScriptImage> images) =>
        new(heading, style, gloss, images);

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
