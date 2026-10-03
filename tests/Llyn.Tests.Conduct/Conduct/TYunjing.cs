using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TYunjing
{
    internal const string TYunjingPack = """{ "language": "Fixture" }""";

    internal const string TYunjingBook = "Classical Chinese";

    [Fact]
    public async Task YunjingXiaoyunLoad_FixtureCell_AnswersTheCellEntriesAfterTheFill()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        CYunjing yunjing = TYunjingPrepare(atelier);
        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");

        CEnsignSheet<IReadOnlyList<CVistaRow>> sheet =
            await yunjing.CYunjingXiaoyunLoad(static (_, _) => static () => { });

        Assert.Equal(["爛"], sheet.CEnsignSheetRows.Select(static row => row.CVistaRowHeadword));
        Assert.Equal(atelier.CAtelierCatalog.CCatalogLanguageRead(), sheet.CEnsignSheetLanguages);
    }

    [Fact]
    public void YunjingShengmuRead_NothingPlaced_ListsNothingUnderTheBareKeys()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CYunjing yunjing = TYunjingPrepare(atelier);

        Assert.True(yunjing.LYunjingAllowed);
        Assert.Empty(yunjing.CYunjingShengmuRead());
        Assert.True(yunjing.CYunjingShengmuEmpty);
        Assert.Equal("Yunjing.ShengmuEmpty", yunjing.CYunjingShengmuKey);
        Assert.Empty(yunjing.CYunjingYunmuRead());
        Assert.True(yunjing.CYunjingYunmuEmpty);
        Assert.Equal("Yunjing.YunmuEmpty", yunjing.CYunjingYunmuKey);
        Assert.Empty(yunjing.CYunjingXiaoyunRead());
        Assert.True(yunjing.CYunjingXiaoyunEmpty);
        Assert.Equal("Yunjing.XiaoyunEmpty", yunjing.CYunjingXiaoyunKey);
        Assert.False(yunjing.CYunjingDiweiShown);
        Assert.True(yunjing.CYunjingDisplayShown);
        Assert.True(yunjing.CYunjingDiweiRead().CDiweiPageEmpty);
    }

    [Fact]
    public void YunjingShengmuRead_NoCellChosen_ListsTheFirstBookLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, TYunjingBook, "爛", "來");
        CYunjing yunjing = TYunjingPrepare(atelier);

        Assert.Equal(["來"], yunjing.CYunjingShengmuRead().Select(static row => row.CDiweiKey));
        Assert.Equal(["寒 I"], yunjing.CYunjingYunmuRead().Select(static row => row.CDiweiKey));
        Assert.DoesNotContain(yunjing.CYunjingShengmuRead(), static row => row.CDiweiChosen);
        Assert.False(yunjing.CYunjingShengmuEmpty);
        Assert.Empty(yunjing.CYunjingXiaoyunRead());
    }

    [Fact]
    public void YunjingDiweiOpen_FixtureCell_ListsThatLanguageWithTheCellChosen()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "孤", "見");
        CYunjing yunjing = TYunjingPrepare(atelier);
        int changed = 0;
        yunjing.CYunjingChanged += () => changed++;

        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");

        Assert.Equal(1, changed);
        Assert.True(yunjing.CYunjingDiweiShown);
        Assert.False(yunjing.CYunjingDisplayShown);
        Assert.Equal(["來", "見"], yunjing.CYunjingShengmuRead().Select(static row => row.CDiweiKey));
        Assert.Equal([true, false], yunjing.CYunjingShengmuRead().Select(static row => row.CDiweiChosen));
        Assert.Equal(["寒 I"], yunjing.CYunjingYunmuRead().Select(static row => row.CDiweiKey));
        Assert.Equal(["爛"], yunjing.CYunjingXiaoyunRead().Select(static row => row.CVistaRowHeadword));
        Assert.False(yunjing.CYunjingXiaoyunEmpty);
        Assert.Equal("Yunjing.XiaoyunVacant", yunjing.CYunjingXiaoyunKey);
        Assert.Equal("Yunjing.Shengmu", yunjing.CYunjingDiweiKey);
    }

    [Fact]
    public void YunjingDiweiOpen_UnknownKey_ChoosesNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        CYunjing yunjing = TYunjingPrepare(atelier);
        int changed = 0;
        yunjing.CYunjingChanged += () => changed++;

        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "見");

        Assert.Equal(0, changed);
        Assert.False(yunjing.CYunjingDiweiShown);
    }

    [Fact]
    public void YunjingDiweiSelect_ChosenRimeAgain_HidesThePage()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        CYunjing yunjing = TYunjingPrepare(atelier);
        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiRime, "寒 I");
        long rime = Assert.Single(yunjing.CYunjingYunmuRead()).CDiweiId;

        Assert.True(yunjing.CYunjingDiweiShown);
        Assert.Equal("Yunjing.Yunmu", yunjing.CYunjingDiweiKey);

        yunjing.CYunjingDiweiSelect(rime, true);

        Assert.False(yunjing.CYunjingDiweiShown);
        Assert.True(yunjing.CYunjingDisplayShown);
        Assert.Empty(yunjing.CYunjingXiaoyunRead());
        Assert.True(yunjing.CYunjingXiaoyunEmpty);
        Assert.Equal("Yunjing.XiaoyunEmpty", yunjing.CYunjingXiaoyunKey);

        yunjing.CYunjingDiweiSelect(null, true);
        yunjing.CYunjingDiweiSelect(rime, null);

        Assert.False(yunjing.CYunjingDiweiShown);
    }

    [Fact]
    public void YunjingDiweiRead_InitialCell_SectionsByDivisionWithSortedLines()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "蘭", "來");
        CYunjing yunjing = TYunjingPrepare(atelier);
        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");

        CDiweiPage page = yunjing.CYunjingDiweiRead();

        Assert.Equal((language, "來"), (page.CDiweiPageLanguage, page.CDiweiPageKey));
        Assert.False(page.CDiweiPageEmpty);
        CDiweiSection section = Assert.Single(page.CDiweiPageSections);
        Assert.Equal((false, false), (section.CDiweiSectionSwitched, section.CDiweiSectionRespelled));
        CDiweiLine line = Assert.Single(section.CDiweiSectionLines);
        Assert.Equal(
            ("寒", string.Empty, false), (line.CDiweiLineLabel, line.CDiweiLineReading, line.CDiweiLineRounded));
        Assert.Equal(["爛", "蘭"], line.CDiweiLineCharacters);
    }

    [Fact]
    public void YunjingDiweiRead_BookCell_CarriesTheFontsOfTheCellLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, TYunjingBook, "爛", "來");
        CYunjing yunjing = TYunjingPrepare(atelier);
        CDiweiPage blank = yunjing.CYunjingDiweiRead();
        yunjing.TYunjingDiweiOpen(TYunjingBook, LDiwei.LDiweiInitial, "來");

        CDiweiPage page = yunjing.CYunjingDiweiRead();

        Assert.Equal(new CFont(null, null, null), blank.CDiweiPageFont);
        Assert.Equal(new CFont(null, null, null), blank.CDiweiPageGlyph);
        const string family = "Microsoft JhengHei UI, Microsoft YaHei UI, Malgun Gothic";
        Assert.Equal(family, page.CDiweiPageFont.CFontFamily);
        Assert.Equal<double?>(40, page.CDiweiPageFont.CFontSize);
        Assert.Equal(family, page.CDiweiPageGlyph.CFontFamily);
        Assert.Equal<double?>(16, page.CDiweiPageGlyph.CFontSize);
    }

    [Fact]
    public void YunjingDiweiRead_ReflexUnderTheCell_CopiesTheTallyOfTheShownSet()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, TYunjingBook, "爛", "來");
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, TYunjingBook, "蘭", "來");
        CYunjing yunjing = TYunjingPrepare(atelier);
        yunjing.TYunjingDiweiOpen(TYunjingBook, LDiwei.LDiweiInitial, "來");

        CDiweiSection section = Assert.Single(yunjing.CYunjingDiweiRead().CDiweiPageSections);

        CTally tally = Assert.Single(section.CDiweiSectionTallies);
        Assert.Equal(("Korean", string.Empty), (tally.CTallyLanguage, tally.CTallyKind));
        CTallyMark mark = Assert.Single(tally.CTallyMarks);
        Assert.Equal(("ㄹ", 2), (mark.CTallyMarkText, mark.CTallyMarkCount));
        Assert.Equal(["爛", "蘭"], mark.CTallyMarkCharacters);
    }

    [Fact]
    public void YunjingTallyToggle_SwitchThenNull_SavesOnceAndRaisesOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CYunjing yunjing = TYunjingPrepare(atelier);
        int changed = 0;
        yunjing.CYunjingChanged += () => changed++;

        yunjing.CYunjingTallyToggle(true);

        Assert.Equal(1, changed);
        Assert.True(engine.TEngineSettingsRead().LSettingsTally);

        yunjing.CYunjingTallyToggle(null);

        Assert.Equal(1, changed);
    }

    [Fact]
    public void YunjingGlyphSelect_PageShown_OpensTheGlyphEntryInThePageLanguage()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        CYunjing yunjing = TYunjingPrepare(atelier);
        List<long> chosen = [];
        atelier.CAtelierNavigation.TNavigationTabAdd(
            "Library", static () => true, static () => 0, static _ => { }, chosen.Add);

        yunjing.CYunjingGlyphSelect("爛");

        Assert.Empty(chosen);

        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");
        yunjing.CYunjingGlyphSelect(string.Empty);
        yunjing.CYunjingGlyphSelect("爛");

        Assert.Equal([engine.TEngineGlyphResolve("爛", language).LEntryId], chosen);
    }

    [Fact]
    public void YunjingShengmuFind_UnmatchedQuery_UnchoosesAndReadsTheUnmatchedKeys()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        CYunjing yunjing = TYunjingPrepare(atelier);
        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");

        yunjing.CYunjingXiaoyunFind("zzz");

        Assert.Empty(yunjing.CYunjingXiaoyunRead());
        Assert.Equal("Yunjing.XiaoyunUnmatched", yunjing.CYunjingXiaoyunKey);

        yunjing.CYunjingYunmuFind("zzz");
        yunjing.CYunjingShengmuFind("zzz");

        Assert.Empty(yunjing.CYunjingYunmuRead());
        Assert.Equal("Yunjing.YunmuUnmatched", yunjing.CYunjingYunmuKey);
        Assert.Empty(yunjing.CYunjingShengmuRead());
        Assert.True(yunjing.CYunjingShengmuEmpty);
        Assert.Equal("Yunjing.ShengmuUnmatched", yunjing.CYunjingShengmuKey);
        Assert.False(yunjing.CYunjingDiweiShown);
    }

    [Fact]
    public void YunjingDiweiOpen_QueriedColumns_EmptiesBothQueriesBeforeTheOpening()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        CYunjing yunjing = TYunjingPrepare(atelier);
        yunjing.CYunjingShengmuFind("zzz");
        yunjing.CYunjingYunmuFind("zzz");
        List<string> seen = [];
        yunjing.CYunjingDiweiOpened += () => seen.Add(yunjing.CYunjingShengmuKey + " " + yunjing.CYunjingYunmuKey);

        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");

        Assert.Equal(["Yunjing.ShengmuEmpty Yunjing.YunmuEmpty"], seen);
        Assert.True(yunjing.CYunjingDiweiShown);
    }

    [Fact]
    public void YunjingShengmuSet_NullAfterAnOrder_KeepsTheChosenOne()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CYunjing yunjing = TYunjingPrepare(atelier);

        Assert.Equal(CCatalogOrder.CCatalogOrderName, yunjing.CYunjingShengmuOrder);
        Assert.Equal(CCatalogOrder.CCatalogOrderName, yunjing.CYunjingYunmuOrder);

        yunjing.CYunjingShengmuSet(CCatalogOrder.CCatalogOrderReverse);
        yunjing.CYunjingShengmuSet(null);
        yunjing.CYunjingYunmuSet(CCatalogOrder.CCatalogOrderUsage);
        yunjing.CYunjingYunmuSet(null);

        Assert.Equal(CCatalogOrder.CCatalogOrderReverse, yunjing.CYunjingShengmuOrder);
        Assert.Equal(CCatalogOrder.CCatalogOrderUsage, yunjing.CYunjingYunmuOrder);
    }

    [Fact]
    public async Task YunjingPortraitExport_ChosenEntry_WritesItAndNothingBefore()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        string path = Path.Combine(workspace.TWorkspaceFolder, "rotten.md");
        CYunjing yunjing = TYunjingPrepare(
            atelier, TEnvoyFake.TEnvoyFileCreate(path, CPortraitMedium.CPortraitMediumMarkdown, []));

        await yunjing.CYunjingPortraitExport();

        Assert.False(File.Exists(path));

        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");
        yunjing.CYunjingPanel.CPanelRowSelect(Assert.Single(yunjing.CYunjingXiaoyunRead()).CVistaRowId);
        await yunjing.CYunjingPortraitExport();

        Assert.Contains("爛", File.ReadAllText(path), StringComparison.Ordinal);
        Assert.Contains("爛", yunjing.TYunjingFileRead(), StringComparison.Ordinal);
        Assert.False(yunjing.CYunjingDiweiShown);
        Assert.True(yunjing.CYunjingDisplayShown);
    }

    [Fact]
    public void YunjingPanelRowOpen_ScribeOn_OpensTheEntryInTheEditorUntilClosed()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TYunjingPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
        CYunjing yunjing = TYunjingPrepare(atelier);
        yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");
        long entry = Assert.Single(yunjing.CYunjingXiaoyunRead()).CVistaRowId;

        yunjing.CYunjingPanel.CPanelRowOpen(entry);
        yunjing.CYunjingPanel.CPanelScribeToggle(true);

        Assert.True(yunjing.CYunjingEditorShown);
        Assert.Equal(entry, yunjing.CYunjingEditor.CEditorDesk.CDeskStoredRead());

        yunjing.CYunjingPanel.CPanelEntryClose();

        Assert.False(yunjing.CYunjingEditorShown);
        Assert.Null(yunjing.CYunjingEditor.CEditorDesk.CDeskStoredRead());
    }

    [Fact]
    public void YunjingOrderRead_Menu_OffersTheThreeOrderings()
    {
        Assert.Equal(
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderReverse,
                CCatalogOrder.CCatalogOrderUsage,
            ],
            CYunjing.CYunjingOrderRead());
    }

    [Fact]
    public void YunjingPortraitPrint_NoEntryChosen_PrintsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CYunjing yunjing = TYunjingPrepare(atelier);

        Assert.False(yunjing.CYunjingPanel.CPanelPressAllowed);
        Assert.Same(Task.CompletedTask, yunjing.CYunjingPortraitPrint());
    }

    internal static CYunjing TYunjingPrepare(CAtelier atelier)
    {
        return TYunjingPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
    }

    private static CYunjing TYunjingPrepare(CAtelier atelier, CEnvoy envoy)
    {
        CYunjing yunjing = CYunjing.CYunjingCreate(atelier, static () => true, envoy, static run => run());
        return yunjing;
    }
}
