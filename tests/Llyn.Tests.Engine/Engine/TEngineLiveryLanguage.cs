using System.Globalization;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineLiveryLanguage
{
    private const string TLiveryLanguagePack =
        """
        { "fanqie": [
            { "name": "Broad", "source": "Wiki", "url": "https://example.test/wiki/{word}?raw",
              "match": "{word}",
              "line": "\"(?<initial>.)(?<rime>.)(?<division>.)(?<rounded>[開合]) (?<tone>.)(?<spelling>..)\"",
              "rounded": "合" } ],
          "shengfu": { "source": "Wiki", "url": "https://example.test/series/{word}?raw",
            "match": "\\{ \"\\d+\", \"(?<shengfu>[^\"]+)\"" } }
        """;

    private const string TLiveryLanguageBooks =
        """
        return {
            "來東一開 去盧貢"
        }
        """;

    private const string TLiveryLanguageModule =
        """
        return {
            { "8436", "龍", "東", "0", "龍", "b·roŋ", "" },
        }
        """;

    private const string TLiveryLanguageBook = "Classical Chinese";

    private static readonly TimeSpan TLiveryLanguagePatience = TimeSpan.FromSeconds(5);

    private static readonly LHypothesis TLiveryLanguageTables = TInterfaceFanqie.THypothesisCreate(
        new Dictionary<string, string> { ["來"] = "l" },
        new Dictionary<string, string> { ["寒 一"] = "an" },
        new Dictionary<string, IReadOnlyList<LHypothesisTone>>
        {
            ["平"] = [TInterfaceFanqie.THypothesisToneCreate("", [], "1")],
        });

    [Fact]
    public async Task LiveryRead_FetchedSeries_CarriesItsPageAndEntries()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TLiveryLanguagePack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        Dictionary<string, string> pages = new()
        {
            ["https://example.test/wiki/%E9%BE%8D?raw"] = TLiveryLanguageBooks,
            ["https://example.test/series/%E9%BE%8D?raw"] = TLiveryLanguageModule,
        };
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(pages));
        string language = pack.TLanguageFixtureName;
        LEntry entry = engine.TEngineEntrySave(TLiveryLanguageCreate("龍", language));
        engine.TEngineFanqieStart(entry.LEntryId);
        await TLiveryLanguageSettle(engine, entry.LEntryId);

        LLiveryLanguage page = engine.TLiveryRead(language);

        LLiveryStem series = Assert.Single(page.LLiveryLanguageStem);
        Assert.Equal((language, "龍"), (series.LLiveryStemPage.LStemPageLanguage, series.LLiveryStemPage.LStemPageKey));
        Assert.Equal("龍", Assert.Single(series.LLiveryStemPage.LStemPageCharacters));
        Assert.Equal(entry.LEntryId, Assert.Single(series.LLiveryStemEntry).LEntryId);
    }

    [Fact]
    public void LiveryRead_PlacedCharacter_ListsInitialRimeAndToneWithTheirKind()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TLiveryLanguagePlace(engine, workspace);

        LLiveryLanguage page = engine.TLiveryRead(TLiveryLanguageBook);

        Assert.Equal(
            [LDiwei.LDiweiInitial, LDiwei.LDiweiRime, LDiwei.LDiweiTone],
            page.LLiveryLanguageDiwei.Select(static row => row.LLiveryDiweiKind));
        Assert.Equal(
            ["來", "寒 I", "1"],
            page.LLiveryLanguageDiwei.Select(static row => row.LLiveryDiweiPage.LDiweiPageKey));
        Assert.All(
            page.LLiveryLanguageDiwei,
            row => Assert.Equal(entry.LEntryId, Assert.Single(row.LLiveryDiweiEntry).LEntryId));
    }

    [Fact]
    public void LiveryRead_RespellingAndTallyOn_SectionsFollowTheRespellingSetting()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TLiveryLanguagePlace(engine, workspace);
        IReadOnlyList<LDiweiSection> plain = [.. engine.TLiveryRead(TLiveryLanguageBook).LLiveryLanguageDiwei
            .SelectMany(static row => row.LLiveryDiweiPage.LDiweiPageSections)];
        engine.TEngineRespellingSave(true);
        engine.TEngineTallySave(true);
        Assert.True(engine.TEngineRespellingCheck(TLiveryLanguageBook));

        IReadOnlyList<LDiweiSection> tallied = [.. engine.TLiveryRead(TLiveryLanguageBook).LLiveryLanguageDiwei
            .SelectMany(static row => row.LLiveryDiweiPage.LDiweiPageSections)];

        Assert.NotEmpty(plain);
        Assert.All(plain, static section => Assert.False(section.LDiweiSectionRespelled));
        Assert.NotEmpty(tallied);
        Assert.All(
            tallied,
            static section => Assert.Equal(
                (true, true), (section.LDiweiSectionSwitched, section.LDiweiSectionRespelled)));
        Assert.Contains(tallied, static section => section.LDiweiSectionTallies.Count > 0);
    }

    [Fact]
    public void LiveryRead_LanguageWithoutBooksOrSeries_AnswersEmptyLists()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("""{ "language": "Fixture" }""");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");

        LLiveryLanguage page = engine.TLiveryRead(language);

        Assert.Equal(language, page.LLiveryLanguageName);
        Assert.Empty(page.LLiveryLanguageStem);
        Assert.Empty(page.LLiveryLanguageDiwei);
    }

    [Fact]
    public void LiveryFormat_SeriesNote_LinksTheStoredEntryAndLeavesAnUnstoredCharacterPlain()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TLiveryLanguageCreate("龍", TLiveryLanguageBook));
        LLiveryStem stem = TInterface.TLiveryStemCreate(
            TInterface.TStemPageCreate(TLiveryLanguageBook, "龍", ["龍", "瀧"]), [entry]);

        string body = TInterface.TLiveryFormat(stem, static key => key).LLiveryNoteBody;

        Assert.Contains(
            "<span class=\"llyn-stem\">[龍](:/" + entry.LEntryId.ToString(CultureInfo.InvariantCulture) + ")</span>",
            body);
        Assert.Contains("<span class=\"llyn-stem\">瀧</span>", body);
        Assert.DoesNotContain("[瀧]", body);
    }

    [Fact]
    public void LiveryFormat_SeriesNote_PrintsEachMemberReadingAndReflexRowsAndABareMemberAlone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TLiveryLanguageCreate("龍", TLiveryLanguageBook));
        LStemPage page = TInterface.TStemPageCreate(
            TLiveryLanguageBook,
            "龍",
            ["龍", "瀧"],
            [
                TInterface.TStemMemberCreate(
                    "龍", ["lyowng", "<b>"], [TInterface.TReflexDraftCreate("Korean", "", "룡")]),
                TInterface.TStemMemberCreate("瀧", [], []),
            ]);

        string body = TInterface.TLiveryFormat(TInterface.TLiveryStemCreate(page, [entry]), static key => key)
            .LLiveryNoteBody;

        Assert.Contains(
            "<div class=\"llyn-card\">\n\n<span class=\"llyn-stem\">[龍](:/"
            + entry.LEntryId.ToString(CultureInfo.InvariantCulture)
            + ")</span> <span class=\"llyn-accent\">/lyowng, &lt;b&gt;/</span>\n\n"
            + "| | | | | |\n|---|---|---|---|---|\n| Korean |  | 룡 |  |  |\n\n</div>",
            body);
        Assert.Contains("<div class=\"llyn-card\">\n\n<span class=\"llyn-stem\">瀧</span>\n\n</div>", body);
    }

    [Fact]
    public void LiveryFormat_RimeNote_PrintsSectionsInPageOrderWithReadingsAndTallies()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TLiveryLanguagePlace(engine, workspace);
        LLiveryDiwei placed = engine.TLiveryRead(TLiveryLanguageBook).LLiveryLanguageDiwei
            .Single(static row => row.LLiveryDiweiKind == LDiwei.LDiweiRime);
        LTallyRow tally = TInterface.TTallyRowCreate("Cantonese", "initial", [TInterface.TTallyMarkCreate("l", ["爛"])]);
        LDiweiPage page = placed.LLiveryDiweiPage with
        {
            LDiweiPageSections =
            [
                TInterface.TDiweiSectionCreate(
                    "Velars", [TInterface.TDiweiLineCreate("/k/", "見", false, 0, ["干"])], [], false, false),
                TInterface.TDiweiSectionCreate(
                    "Laryngeals", [TInterface.TDiweiLineCreate("/l/", "來", true, 1, ["爛"])], [tally], false, false),
            ],
        };

        string body = TInterface.TLiveryFormat(placed with { LLiveryDiweiPage = page }, static key => key)
            .LLiveryNoteBody;

        int velars = body.IndexOf("Velars", StringComparison.Ordinal);
        int laryngeals = body.IndexOf("Laryngeals", StringComparison.Ordinal);
        Assert.InRange(velars, 0, laryngeals - 1);
        Assert.Contains("| /k/ | 見 |  |", body);
        Assert.Contains("| /l/ | 來 | <span class=\"llyn-rounded\">合</span> |", body);
        Assert.Contains("<span class=\"llyn-mark\">l <span class=\"llyn-count\">1</span></span>", body);
        Assert.True(
            body.IndexOf("<span class=\"llyn-language\">Cantonese</span>", StringComparison.Ordinal) > laryngeals);
    }

    [Fact]
    public void LiveryFormat_ToneNote_PrintsTheToneLabelAndLinksThePlacedEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TLiveryLanguagePlace(engine, workspace);
        LLiveryDiwei tone = engine.TLiveryRead(TLiveryLanguageBook).LLiveryLanguageDiwei
            .Single(static row => row.LLiveryDiweiKind == LDiwei.LDiweiTone);

        string body = TInterface.TLiveryFormat(
            tone, static key => key == "Display.FanqieTone" ? "Tone {0}" : key).LLiveryNoteBody;

        Assert.Contains("# Tone 1\n", body);
        Assert.Contains("- [爛](:/" + entry.LEntryId.ToString(CultureInfo.InvariantCulture) + ")\n", body);
    }

    private static LEntry TLiveryLanguagePlace(LEngine engine, TWorkspace workspace)
    {
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "爛",
            TLiveryLanguageBook,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            [],
            reflexes: [TInterface.TReflexDraftCreate("Korean", string.Empty, "란")]));
        LFanqieArchive fanqie = TInterface.TFanqieArchiveCreate(workspace.TWorkspaceDatabase);
        fanqie.TFanqieSave(
            TLiveryLanguageBook, "爛", [TInterface.TFanqieRowCreate("爛", 0, "來", "寒", "一", "平")]);
        TInterface.TDiweiArchiveCreate(workspace.TWorkspaceDatabase)
            .TDiweiApply(TLiveryLanguageBook, "爛", TLiveryLanguageTables);
        IReadOnlyList<long> anchors =
            [.. fanqie.TFanqieRead(TLiveryLanguageBook, "爛").Select(static row => row.LFanqieRowId)];
        engine.TEntryAnchorApply(entry.LEntryId, anchors);
        return entry;
    }

    private static async Task TLiveryLanguageSettle(LEngine engine, long entryId)
    {
        DateTime deadline = DateTime.UtcNow + TLiveryLanguagePatience;
        while (engine.TEngineFanqieCheck(entryId))
        {
            Assert.True(DateTime.UtcNow < deadline, "Waited for the series fetch to settle.");
            await Task.Delay(20);
        }
    }

    private static LEntryDraft TLiveryLanguageCreate(string headword, string language)
    {
        return TInterface.TEntryDraftCreate(
            headword,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            []);
    }
}
