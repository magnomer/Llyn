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

public sealed class TXiesheng
{
    private const string TXieshengPack =
        """
        { "fanqie": [
            { "name": "Broad", "source": "Wiki", "url": "https://example.test/wiki/{word}?raw",
              "match": "{word}",
              "line": "\"(?<initial>.)(?<rime>.)(?<division>.)(?<rounded>[開合]) (?<tone>.)(?<spelling>..)\"",
              "rounded": "合" } ],
          "shengfu": { "source": "Wiki", "url": "https://example.test/series/{word}?raw",
            "match": "\\{ \"\\d+\", \"(?<shengfu>[^\"]+)\"" } }
        """;

    private const string TXieshengBooks =
        """
        return {
            "來東一開 去盧貢"
        }
        """;

    private const string TXieshengModule =
        """
        return {
            { "8436", "龍", "東", "0", "龍", "b·roŋ", "" },
        }
        """;

    private static readonly TimeSpan TXieshengPatience = TimeSpan.FromSeconds(5);

    [Fact]
    public void XieshengGroveRead_NothingFetched_ListsNothingUnderTheBareKeys()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CXiesheng xiesheng = TXieshengPrepare(atelier);

        Assert.True(xiesheng.LXieshengAllowed);
        Assert.Empty(xiesheng.CXieshengGroveRead());
        Assert.True(xiesheng.CXieshengGroveEmpty);
        Assert.Equal("Xiesheng.GroveEmpty", xiesheng.CXieshengGroveKey);
        Assert.Empty(xiesheng.CXieshengKindredRead());
        Assert.True(xiesheng.CXieshengKindredEmpty);
        Assert.Equal("Xiesheng.KindredEmpty", xiesheng.CXieshengKindredKey);
        Assert.True(xiesheng.CXieshengStemRead().CStemPageEmpty);
        Assert.False(xiesheng.CXieshengStemShown);
        Assert.True(xiesheng.CXieshengDisplayShown);
    }

    [Fact]
    public async Task XieshengStemOpen_FetchedSeries_ChoosesItAndListsItsEntry()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TXieshengPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TXieshengEngineStart(workspace);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        LEntry entry = await TXieshengStemSave(engine, language);
        CXiesheng xiesheng = TXieshengPrepare(atelier);
        int changed = 0;
        xiesheng.CXieshengChanged += () => changed++;

        xiesheng.TXieshengStemOpen(language, "龍");

        Assert.Equal(1, changed);
        CStem row = Assert.Single(xiesheng.CXieshengGroveRead());
        Assert.Equal(("龍", 1, true), (row.CStemKey, row.CStemCount, row.CStemChosen));
        Assert.True(xiesheng.CXieshengStemShown);
        Assert.False(xiesheng.CXieshengDisplayShown);
        CStemPage page = xiesheng.CXieshengStemRead();
        Assert.Equal((language, "龍"), (page.CStemPageLanguage, page.CStemPageKey));
        Assert.Equal(["龍"], page.CStemPageCharacters);
        Assert.Equal(entry.LEntryId, Assert.Single(xiesheng.CXieshengKindredRead()).CVistaRowId);
        Assert.False(xiesheng.CXieshengKindredEmpty);
        Assert.Equal("Xiesheng.KindredVacant", xiesheng.CXieshengKindredKey);
    }

    [Fact]
    public async Task XieshengStemOpen_BlankOrUnknownKey_ChoosesNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TXieshengPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TXieshengEngineStart(workspace);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        await TXieshengStemSave(engine, language);
        CXiesheng xiesheng = TXieshengPrepare(atelier);

        xiesheng.TXieshengStemOpen(language, null);
        xiesheng.TXieshengStemOpen(language, string.Empty);
        xiesheng.TXieshengStemOpen(language, "虎");

        Assert.DoesNotContain(xiesheng.CXieshengGroveRead(), static row => row.CStemChosen);
        Assert.False(xiesheng.CXieshengStemShown);
    }

    [Fact]
    public async Task XieshengStemSelect_ChosenTwiceThenCancelled_TogglesAndClears()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TXieshengPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TXieshengEngineStart(workspace);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        await TXieshengStemSave(engine, language);
        CXiesheng xiesheng = TXieshengPrepare(atelier);
        xiesheng.TXieshengStemOpen(language, "龍");
        long stem = Assert.Single(xiesheng.CXieshengGroveRead()).CStemId;

        xiesheng.CXieshengStemSelect(stem);

        Assert.False(xiesheng.CXieshengStemShown);

        xiesheng.CXieshengStemSelect(stem);

        Assert.True(xiesheng.CXieshengStemShown);

        xiesheng.CXieshengStemSelect(stem);
        xiesheng.CXieshengStemCancel();

        Assert.False(xiesheng.CXieshengStemShown);
        Assert.Empty(xiesheng.CXieshengKindredRead());
    }

    [Fact]
    public async Task XieshengGlyphSelect_StemShown_OpensTheGlyphEntryInTheSeriesLanguage()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TXieshengPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TXieshengEngineStart(workspace);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        await TXieshengStemSave(engine, language);
        CXiesheng xiesheng = TXieshengPrepare(atelier);
        List<long> chosen = [];
        atelier.CAtelierNavigation.TNavigationTabAdd(
            "Library", static () => true, static () => 0, static _ => { }, chosen.Add);

        xiesheng.CXieshengGlyphSelect("龍");

        Assert.Empty(chosen);

        xiesheng.TXieshengStemOpen(language, "龍");
        xiesheng.CXieshengGlyphSelect(string.Empty);
        xiesheng.CXieshengGlyphSelect("龍");

        Assert.Equal([engine.TEngineGlyphResolve("龍", language).LEntryId], chosen);
    }

    [Fact]
    public async Task XieshengGroveFind_UnmatchedQuery_ReadsTheUnmatchedKeys()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TXieshengPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TXieshengEngineStart(workspace);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        await TXieshengStemSave(engine, language);
        CXiesheng xiesheng = TXieshengPrepare(atelier);
        xiesheng.TXieshengStemOpen(language, "龍");

        xiesheng.CXieshengKindredFind("zzz");

        Assert.Empty(xiesheng.CXieshengKindredRead());
        Assert.Equal("Xiesheng.KindredUnmatched", xiesheng.CXieshengKindredKey);

        xiesheng.CXieshengGroveFind("zzz");

        Assert.Empty(xiesheng.CXieshengGroveRead());
        Assert.True(xiesheng.CXieshengGroveEmpty);
        Assert.Equal("Xiesheng.GroveUnmatched", xiesheng.CXieshengGroveKey);
        Assert.False(xiesheng.CXieshengStemShown);
    }

    [Fact]
    public async Task XieshengStemOpen_QueriedSeries_EmptiesTheQueryBeforeTheOpening()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TXieshengPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TXieshengEngineStart(workspace);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        await TXieshengStemSave(engine, language);
        CXiesheng xiesheng = TXieshengPrepare(atelier);
        xiesheng.CXieshengGroveFind("zzz");
        List<string> seen = [];
        xiesheng.CXieshengStemOpened += () => seen.Add(xiesheng.CXieshengGroveKey);

        xiesheng.TXieshengStemOpen(language, "龍");

        Assert.Equal(["Xiesheng.GroveEmpty"], seen);
        Assert.True(xiesheng.CXieshengStemShown);
    }

    [Fact]
    public void XieshengGroveSet_NullAfterAnOrder_KeepsTheChosenOne()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CXiesheng xiesheng = TXieshengPrepare(atelier);

        Assert.Equal(CCatalogOrder.CCatalogOrderName, xiesheng.CXieshengOrder);

        xiesheng.CXieshengGroveSet(CCatalogOrder.CCatalogOrderReverse);
        xiesheng.CXieshengGroveSet(null);

        Assert.Equal(CCatalogOrder.CCatalogOrderReverse, xiesheng.CXieshengOrder);
    }

    [Fact]
    public async Task XieshengPortraitExport_ChosenEntry_WritesItAndNothingBefore()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TXieshengPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TXieshengEngineStart(workspace);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        string language = pack.TLanguageFixtureName;
        LEntry entry = await TXieshengStemSave(engine, language);
        string path = Path.Combine(workspace.TWorkspaceFolder, "dragon.md");
        CXiesheng xiesheng = TXieshengPrepare(
            atelier, TInterfaceConduct.TEnvoyFileCreate(path, CPortraitMedium.CPortraitMediumMarkdown, []));

        await xiesheng.CXieshengPortraitExport();

        Assert.False(File.Exists(path));

        xiesheng.TXieshengStemOpen(language, "龍");
        xiesheng.CXieshengPanel.CPanelRowSelect(entry.LEntryId);
        await xiesheng.CXieshengPortraitExport();

        Assert.Contains("龍", File.ReadAllText(path), StringComparison.Ordinal);
        Assert.Contains("龍", xiesheng.TXieshengFileRead(), StringComparison.Ordinal);
        Assert.False(xiesheng.CXieshengStemShown);
        Assert.True(xiesheng.CXieshengDisplayShown);
    }

    [Fact]
    public async Task XieshengPanelRowOpen_ScribeOn_OpensTheEntryInTheEditorUntilClosed()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TXieshengPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = TXieshengEngineStart(workspace);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry entry = await TXieshengStemSave(engine, pack.TLanguageFixtureName);
        CXiesheng xiesheng = TXieshengPrepare(atelier);

        xiesheng.CXieshengPanel.CPanelRowOpen(entry.LEntryId);
        xiesheng.CXieshengPanel.CPanelScribeToggle(true);

        Assert.True(xiesheng.CXieshengEditorShown);
        Assert.Equal(entry.LEntryId, xiesheng.CXieshengEditor.CEditorDesk.CDeskStoredRead());

        xiesheng.CXieshengPanel.CPanelEntryClose();

        Assert.False(xiesheng.CXieshengEditorShown);
        Assert.Null(xiesheng.CXieshengEditor.CEditorDesk.CDeskStoredRead());
    }

    private static CXiesheng TXieshengPrepare(CAtelier atelier)
    {
        return TXieshengPrepare(atelier, TInterfaceConduct.TEnvoyCreate(false, []));
    }

    private static CXiesheng TXieshengPrepare(CAtelier atelier, CEnvoy envoy)
    {
        CXiesheng xiesheng = CXiesheng.CXieshengCreate(atelier, static () => true, envoy);
        xiesheng.CXieshengVistaRestore();
        return xiesheng;
    }

    private static LEngine TXieshengEngineStart(TWorkspace workspace)
    {
        Dictionary<string, string> pages = new()
        {
            ["https://example.test/wiki/%E9%BE%8D?raw"] = TXieshengBooks,
            ["https://example.test/series/%E9%BE%8D?raw"] = TXieshengModule,
        };
        return workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(pages));
    }

    private static async Task<LEntry> TXieshengStemSave(LEngine engine, string language)
    {
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "龍",
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a dragon", [], [], [], [], [], 1)],
            []));
        engine.TEngineFanqieStart(entry.LEntryId);
        DateTime deadline = DateTime.UtcNow + TXieshengPatience;
        while (engine.TEngineFanqieCheck(entry.LEntryId))
        {
            Assert.True(DateTime.UtcNow < deadline, "Waited for the series fetch to settle.");
            await Task.Delay(20);
        }

        return entry;
    }
}
