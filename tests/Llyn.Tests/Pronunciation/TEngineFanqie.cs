using System.Net;
using System.Net.Http;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineFanqie
{
    private const string TEngineFanqiePack =
        """
        { "fanqie": [
            { "name": "Broad", "url": "https://example.test/broad",
              "form": { "word": "{word}", "t": "g" },
              "match": "<span class='string'>{word}</span>", "busy": "Too fast",
              "spelling": "<a[^>]*>([^<]*)</a>",
              "split": "<br\\s*/?>",
              "head": "<b>(?<rime>[^<]+)</b>\\[(?<heading>[^\\]]+)\\]<s>(?<division>[^<]*)</s>",
              "column": "^(?<division>.)等(?<tone>.)$",
              "rounded": "<u>(?:<[^>]+>)*<span class='string'>{word}</span>" },
            { "name": "Collected", "url": "https://example.test/collected",
              "form": { "word": "{word}", "t": "j" },
              "match": "<span class='string'>{word}</span>", "busy": "Too fast" } ] }
        """;

    private const string TEngineWikiPack =
        """
        { "fanqie": [
            { "name": "Broad", "source": "Kaom", "url": "https://example.test/broad",
              "form": { "word": "{word}", "t": "g" },
              "match": "<span class='string'>{word}</span>" },
            { "name": "Broad", "source": "Wiki", "url": "https://example.test/wiki/{word}?raw",
              "match": "{word}",
              "line": "\"(?<initial>.)(?<rime>.)(?<division>.)(?<rounded>[開合]) (?<tone>.)(?<spelling>..)\"",
              "rounded": "合" } ] }
        """;

    private const string TEngineFanqieWiki =
        """
        return {
        	"知東三開 平陟弓",
        	"見桓一合 去古玩"
        }
        """;

    private const string TEngineFanqieGuan =
        """
        <table>
        <tr><th class="red">寒韻系</th><th>一等平</th><th>一等上</th><th>一等入</th></tr>
        <tr><td><b>見</b></td>
        <td><b>寒</b>[寒]<s>一</s><a href="x">古寒</a> 干<i>乾</i><br>
        <b>寒</b>[桓]<s>一</s><a href="x">古丸</a> <u><span class='string'>官</span></u><u>觀</u><br></td>
        <td></td>
        <td><b>寒</b>[曷]<s>一</s><a href="x">古達</a> 葛<span class='string'>官</span><br></td></tr>
        </table>
        """;

    private const string TEngineFanqieBroad =
        """
        <p class="word">Broad</p>
        <table><caption>Page 12</caption>
        <tr><th class="red">模韻系</th><th>一等平</th><th>一等上</th><th>一等去</th></tr>
        <tr><td><b>幫</b></td><td><b>模</b>[模]<s>一</s><a href="x">博孤</a> 逋<i>餔</i><br></td><td></td><td></td></tr>
        <tr><td><b>疑</b></td>
        <td><b>模</b>[模]<s>一</s><a href="x">五乎</a> <i>吾</i>鼯<span class='string'>吳</span>浯<br></td>
        <td><b>模</b>[姥]<s>一</s><a href="x">五古</a> 五<br></td><td></td></tr>
        </table>
        """;

    private const string TEngineFanqieCollected =
        """
        <table>
        <tr><th class="red">虞韻系</th><th>三等平</th><th>三等上</th></tr>
        <tr><td><b>疑</b></td>
        <td><b>虞</b>[虞]<s>三</s><a href="x">元俱</a> <i>虞</i><i><span class='string'>吳</span></i><br></td><td></td></tr>
        </table>
        <table>
        <tr><th class="red">模韻系</th><th>一等平</th><th>一等上</th></tr>
        <tr><td><b>疑</b></td>
        <td><b>模</b>[模]<s>一</s><a href="x">訛胡</a> <i>吾</i><i><span class='string'>吳</span></i><br></td><td></td></tr>
        </table>
        """;

    private static readonly TimeSpan TEngineFanqiePatience = TimeSpan.FromSeconds(5);

    private static readonly Dictionary<string, string> TEngineFanqiePages = new()
    {
        ["https://example.test/broad"] = TEngineFanqieBroad,
        ["https://example.test/collected"] = TEngineFanqieCollected,
    };

    [Fact]
    public async Task FanqieFind_LineBook_ReadsPartsFromPlainTextUnderItsSource()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineWikiPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        Dictionary<string, string> pages = new()
        {
            ["https://example.test/broad"] = TEngineFanqieBroad,
            ["https://example.test/wiki/%E5%90%B3?raw"] = TEngineFanqieWiki,
        };
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(pages));

        IReadOnlyList<LFanqieRow> found = await engine.TEngineFanqieFind(
            "吳", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal(["Kaom", "Wiki", "Wiki"], found.Select(row => row.LFanqieRowSource).ToList());
        Assert.Equal("知 東 三等平", found[1].LFanqieRowText);
        Assert.Equal(("知", "東", "", "三", "平", false), TFanqiePartRead(found[1]));
        Assert.Equal(["", "陟弓", "古玩"], found.Select(row => row.LFanqieRowSpelling).ToList());
        Assert.Equal(("見", "桓", "", "一", "去", true), TFanqiePartRead(found[2]));
        Assert.Equal([0, 0, 1], found.Select(row => row.LFanqieRowPosition).ToList());
    }

    [Fact]
    public async Task FanqieStart_TwoSourcesOneBook_StoresRowsOfBoth()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineWikiPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        Dictionary<string, string> pages = new()
        {
            ["https://example.test/broad"] = TEngineFanqieBroad,
            ["https://example.test/wiki/%E5%90%B3?raw"] = TEngineFanqieWiki,
        };
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(pages));
        LEntry entry = engine.TEngineEntrySave(TFanqieDraftCreate("吳", pack.TLanguageFixtureName));

        engine.TEngineFanqieStart(entry.LEntryId);
        await TFanqieSettle(engine, entry.LEntryId);

        IReadOnlyList<LFanqieRow> read = engine.TEngineFanqieRead(entry.LEntryId);
        Assert.Equal(["Kaom", "Wiki", "Wiki"], read.Select(row => row.LFanqieRowSource).ToList());
        Assert.Equal(3, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM fanqie;"));
    }

    [Fact]
    public async Task FanqieFind_LineBookNotFound_ReadsReachedAndEmpty()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineWikiPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new("gone", HttpStatusCode.NotFound);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        LEntry entry = engine.TEngineEntrySave(TFanqieDraftCreate("吳", pack.TLanguageFixtureName));

        engine.TEngineFanqieStart(entry.LEntryId);
        await TFanqieCountCheck(handler, 2);
        await TFanqieSettle(engine, entry.LEntryId);
        engine.TEngineFanqieStart(entry.LEntryId);
        await Task.Delay(200);

        Assert.Equal(2, handler.TSourceHandlerCount);
    }

    [Fact]
    public async Task FanqieFind_GroupedCell_ReadsEachGroupWithItsParts()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFanqiePack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        Dictionary<string, string> pages = new(TEngineFanqiePages)
        {
            ["https://example.test/broad"] = TEngineFanqieGuan,
            ["https://example.test/collected"] = "nothing",
        };
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(pages));

        IReadOnlyList<LFanqieRow> found = await engine.TEngineFanqieFind(
            "官", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal(2, found.Count);
        Assert.Equal("見 寒[桓] 一等平", found[0].LFanqieRowText);
        Assert.Equal(("見", "寒", "桓", "一", "平", true), TFanqiePartRead(found[0]));
        Assert.Equal("Broad", found[0].LFanqieRowSource);
        Assert.Equal(["古丸", "古達"], found.Select(row => row.LFanqieRowSpelling).ToList());
        Assert.Equal("見 寒[曷] 一等入", found[1].LFanqieRowText);
        Assert.Equal(
            ("曷", "入", false),
            (found[1].LFanqieRowHeading, found[1].LFanqieRowTone, found[1].LFanqieRowRounded));
    }

    [Fact]
    public async Task FanqieFind_TwoBooks_ReadsMarkedCellsInPackOrder()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFanqiePack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TEngineFanqiePages));

        IReadOnlyList<LFanqieRow> found = await engine.TEngineFanqieFind(
            "吳", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal(3, found.Count);
        Assert.Equal(("吳", "Broad", 0, "疑 模[模] 一等平"), TFanqieRowRead(found[0]));
        Assert.Equal(("吳", "Collected", 0, "疑 虞[虞] 三等平"), TFanqieRowRead(found[1]));
        Assert.Equal(("吳", "Collected", 1, "疑 模[模] 一等平"), TFanqieRowRead(found[2]));
        Assert.Equal(("疑", "模", "模", "一", "平"), (found[0].LFanqieRowInitial, found[0].LFanqieRowRime,
            found[0].LFanqieRowHeading, found[0].LFanqieRowDivision, found[0].LFanqieRowTone));
        Assert.Equal(("疑", "", "", "", ""), (found[1].LFanqieRowInitial, found[1].LFanqieRowRime,
            found[1].LFanqieRowHeading, found[1].LFanqieRowDivision, found[1].LFanqieRowTone));
    }

    [Fact]
    public async Task FanqieFind_OneBookBusy_ReadsTheOther()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFanqiePack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        Dictionary<string, string> pages = new(TEngineFanqiePages)
        {
            ["https://example.test/broad"] = "<div>Too fast</div>",
        };
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(pages));

        IReadOnlyList<LFanqieRow> found = await engine.TEngineFanqieFind(
            "吳", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal(["Collected", "Collected"], found.Select(row => row.LFanqieRowBook).ToList());
    }

    [Fact]
    public async Task FanqieRead_NothingStored_FetchesStoresAndRaises()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFanqiePack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TEngineFanqiePages));
        TFanqieObserver observer = new();
        engine.TEngineObserverAttach(observer);
        LEntry entry = engine.TEngineEntrySave(TFanqieDraftCreate("吳越", pack.TLanguageFixtureName));

        Assert.Empty(engine.TEngineFanqieRead(entry.LEntryId));
        engine.TEngineFanqieStart(entry.LEntryId);

        LBulletin raised = await observer.TFanqieObserverRaised.WaitAsync(TEngineFanqiePatience);
        Assert.Equal(entry.LEntryId, raised.LBulletinId);
        await TFanqieSettle(engine, entry.LEntryId);

        IReadOnlyList<LFanqieRow> read = engine.TEngineFanqieRead(entry.LEntryId);
        Assert.Equal(3, read.Count);
        Assert.All(read, row => Assert.Equal("吳", row.LFanqieRowCharacter));
        Assert.Equal(("吳", "Broad", 0, "疑 模[模] 一等平"), TFanqieRowRead(read[0]));
        Assert.Equal(("模", "一", "平"), (read[0].LFanqieRowRime, read[0].LFanqieRowDivision, read[0].LFanqieRowTone));
        Assert.Equal(3, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM fanqie;"));
    }

    [Fact]
    public async Task FanqieStart_BooksBusy_RaisesAndAsksAgainNextStart()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFanqiePack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new("<div>Too fast</div>", HttpStatusCode.OK);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        TFanqieObserver observer = new();
        engine.TEngineObserverAttach(observer);
        LEntry entry = engine.TEngineEntrySave(TFanqieDraftCreate("吳", pack.TLanguageFixtureName));

        engine.TEngineFanqieStart(entry.LEntryId);
        await TFanqieCountCheck(handler, 2);
        LBulletin raised = await observer.TFanqieObserverRaised.WaitAsync(TEngineFanqiePatience);
        await TFanqieSettle(engine, entry.LEntryId);

        Assert.Equal(entry.LEntryId, raised.LBulletinId);
        Assert.Empty(engine.TEngineFanqieRead(entry.LEntryId));
        engine.TEngineFanqieStart(entry.LEntryId);
        await TFanqieCountCheck(handler, 4);
        await TFanqieSettle(engine, entry.LEntryId);

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM fanqie;"));
    }

    [Fact]
    public async Task FanqieStart_BooksSilent_AsksOncePerSession()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFanqiePack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new("nothing here", HttpStatusCode.OK);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        LEntry entry = engine.TEngineEntrySave(TFanqieDraftCreate("吳", pack.TLanguageFixtureName));

        engine.TEngineFanqieStart(entry.LEntryId);
        await TFanqieCountCheck(handler, 2);
        await TFanqieSettle(engine, entry.LEntryId);

        Assert.Empty(engine.TEngineFanqieRead(entry.LEntryId));
        engine.TEngineFanqieStart(entry.LEntryId);
        await Task.Delay(200);

        Assert.Equal(2, handler.TSourceHandlerCount);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM fanqie;"));
    }

    [Fact]
    public void FanqieStart_LanguageWithoutBooks_ReadsEmptyWithoutFetch()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("{}");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new("nothing here", HttpStatusCode.OK);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        LEntry entry = engine.TEngineEntrySave(TFanqieDraftCreate("吳", pack.TLanguageFixtureName));

        engine.TEngineFanqieStart(entry.LEntryId);
        Assert.Empty(engine.TEngineFanqieRead(entry.LEntryId));
        Assert.False(engine.TEngineFanqieCheck(entry.LEntryId));
        Assert.Equal(0, handler.TSourceHandlerCount);
    }

    private static async Task TFanqieSettle(LEngine engine, long entryId)
    {
        DateTime deadline = DateTime.UtcNow + TEngineFanqiePatience;
        while (engine.TEngineFanqieCheck(entryId))
        {
            Assert.True(DateTime.UtcNow < deadline, "Waited for the fanqie fetch to settle.");
            await Task.Delay(20);
        }
    }

    private static async Task TFanqieCountCheck(TSourceHandler handler, int count)
    {
        DateTime deadline = DateTime.UtcNow + TEngineFanqiePatience;
        while (handler.TSourceHandlerCount < count)
        {
            Assert.True(DateTime.UtcNow < deadline, $"Waited for {count} requests, saw {handler.TSourceHandlerCount}.");
            await Task.Delay(20);
        }
    }

    private static (string, string, int, string) TFanqieRowRead(LFanqieRow row) =>
        (row.LFanqieRowCharacter, row.LFanqieRowBook, row.LFanqieRowPosition, row.LFanqieRowText);

    private static (string, string, string, string, string, bool) TFanqiePartRead(LFanqieRow row) =>
        (row.LFanqieRowInitial, row.LFanqieRowRime, row.LFanqieRowHeading,
         row.LFanqieRowDivision, row.LFanqieRowTone, row.LFanqieRowRounded);

    private static LEntryDraft TFanqieDraftCreate(string headword, string language)
    {
        return TInterface.TEntryDraftCreate(
            headword,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            []);
    }

    private sealed class TFanqieObserver : LObserver
    {
        private readonly TaskCompletionSource<LBulletin> _tFanqieObserverRaised =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal Task<LBulletin> TFanqieObserverRaised => _tFanqieObserverRaised.Task;

        public void LObserverBulletinHandle(LBulletin bulletin)
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectFanqie)
            {
                _tFanqieObserverRaised.TrySetResult(bulletin);
            }
        }
    }
}
