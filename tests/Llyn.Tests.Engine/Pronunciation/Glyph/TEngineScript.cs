using System.Net;
using System.Net.Http;
using System.Text;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineScript
{
    private const string TEngineScriptPack =
        """
        { "script": [
            { "name": "Seal", "url": "https://example.test/seal/search",
              "form": { "Character": "{word}", "Size": "36" },
              "match": "<td class=\"Cell\"><img src=\"([^\"]+)\" />(.*?)<td>",
              "image": 1, "caption": 2, "prefix": "https://example.test",
              "rewrite": [["size=\\d+", "size=200"]],
              "gloss": "<dd>(.*?)</dd>" },
            { "name": "Clerical", "url": "https://example.test/clerical/search",
              "form": { "Character": "{word}" },
              "match": "<td class=\"Cell\"><img src=\"([^\"]+)\" />(.*?)<td>",
              "image": 1, "caption": 2, "prefix": "https://example.test" } ] }
        """;

    private const string TEngineScriptSeal =
        """
        <td class="Cell"><img src="/image?text=a&size=36" /><br />Shuowen<td>
        <dd>Seal gloss</dd>
        """;

    private const string TEngineScriptClerical =
        """
        <td class="Cell"><img src="/image?text=b" /><br />Stone<br />Han<td>
        <td class="Cell"><img src="/image?text=c" /><td>
        """;

    [Fact]
    public async Task ScriptSourceFind_PatternTimesOut_AnswersNotReached()
    {
        using HttpClient client = TPronunciationHelper.TSourceClientCreate(
            new string('a', 40) + "!", HttpStatusCode.OK);

        (IReadOnlyList<LScriptImage> found, bool reached) =
            await TInterfaceSource.TScriptSourceFind(client, "(a+)+$", "整");

        Assert.Empty(found);
        Assert.False(reached);
    }

    private static readonly TimeSpan TEngineScriptPatience = TimeSpan.FromSeconds(5);

    private static readonly Dictionary<string, string> TEngineScriptPages = new()
    {
        ["https://example.test/seal/search"] = TEngineScriptSeal,
        ["https://example.test/clerical/search"] = TEngineScriptClerical,
        ["https://example.test/image?text=a&size=200"] = "PNG-A",
        ["https://example.test/image?text=b"] = "PNG-B",
        ["https://example.test/image?text=c"] = "PNG-C",
    };

    [Fact]
    public async Task ScriptStart_TwoStyles_StoresImagesAndGlossInPackOrder()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineScriptPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TEngineScriptPages));

        IReadOnlyList<LScriptImage> found = await TScriptFetchRead(engine, "整", pack.TLanguageFixtureName);

        Assert.Equal(3, found.Count);
        Assert.Equal(("整", "Seal", 0, "Shuowen", "Seal gloss", "PNG-A"), TScriptImageRead(found[0]));
        Assert.Equal(("整", "Clerical", 0, "Stone Han", "", "PNG-B"), TScriptImageRead(found[1]));
        Assert.Equal(("整", "Clerical", 1, "", "", "PNG-C"), TScriptImageRead(found[2]));
    }

    [Fact]
    public async Task ScriptRead_FetchedOutOfEpochOrder_ListsInDeclaredEpochOrder()
    {
        const string epochPack =
            """
            { "script": [
                { "name": "Clerical", "url": "https://example.test/clerical/search",
                  "form": { "Character": "{word}" },
                  "match": "<td class=\"Cell\"><img src=\"([^\"]+)\" />(.*?)<td>",
                  "image": 1, "caption": 2, "prefix": "https://example.test" } ],
              "epoch": [["Shang", "Shang"], ["Han", "Han"]] }
            """;
        Dictionary<string, string> pages = new()
        {
            ["https://example.test/clerical/search"] =
                """
                <td class="Cell"><img src="/image?text=b" /><br />Stone<br />Han<td>
                <td class="Cell"><img src="/image?text=c" /><td>
                <td class="Cell"><img src="/image?text=d" /><br />Bowl<br />Shang<td>
                """,
            ["https://example.test/image?text=b"] = "PNG-B",
            ["https://example.test/image?text=c"] = "PNG-C",
            ["https://example.test/image?text=d"] = "PNG-D",
        };
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(epochPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(pages));

        IReadOnlyList<LScriptImage> found = await TScriptFetchRead(engine, "整", pack.TLanguageFixtureName);

        Assert.Equal(["PNG-D", "PNG-B", "PNG-C"], found.Select(TScriptDataRead));
        Assert.Equal(["Shang", "Han", ""], found.Select(image => image.LScriptImageEpoch));
    }

    [Fact]
    public void ScriptImageSort_StoredOutOfPackOrder_ListsByCharacterStyleThenEpoch()
    {
        LEpoch[] epochs = [TInterface.TEpochCreate("early", "Early"), TInterface.TEpochCreate("late", "Late")];
        IReadOnlyList<LScriptStyle> styles =
        [
            TInterface.TScriptStyleCreate("seal") with { LScriptStyleEpoch = epochs },
            TInterface.TScriptStyleCreate("bronze") with { LScriptStyleEpoch = epochs },
        ];
        IReadOnlyList<LScriptImage> images =
        [
            TInterface.TScriptImageCreate("完", "bronze", 0, "1", "Late"),
            TInterface.TScriptImageCreate("完", "loose", 0, "2", "Early"),
            TInterface.TScriptImageCreate("完", "seal", 0, "3", string.Empty),
            TInterface.TScriptImageCreate("完", "seal", 1, "4", "Unknown"),
            TInterface.TScriptImageCreate("全", "seal", 0, "5", "Early"),
            TInterface.TScriptImageCreate("完", "bronze", 1, "6", "Early"),
            TInterface.TScriptImageCreate("完", "seal", 2, "7", "Late"),
            TInterface.TScriptImageCreate("完", "bronze", 2, "8", "Late"),
        ];

        IReadOnlyList<LScriptImage> sorted = TInterface.TScriptImageSort(images, styles, ["完", "全"]);

        Assert.Equal(
            ["7", "4", "3", "6", "1", "8", "2", "5"],
            sorted.Select(image => image.LScriptImageCaption));
    }

    [Fact]
    public void ScriptImageSort_CharacterOutsideHeadword_FollowsByCodePointThenStoredId()
    {
        IReadOnlyList<LScriptStyle> styles = [TInterface.TScriptStyleCreate("seal")];
        IReadOnlyList<LScriptImage> images =
        [
            TInterface.TScriptImageCreate("完", "seal", 0, "a", string.Empty, 9),
            TInterface.TScriptImageCreate("乙", "seal", 0, "b", string.Empty, 3),
            TInterface.TScriptImageCreate("全", "seal", 0, "c", string.Empty, 7),
            TInterface.TScriptImageCreate("完", "seal", 1, "d", string.Empty, 2),
        ];

        IReadOnlyList<LScriptImage> sorted = TInterface.TScriptImageSort(images, styles, ["全"]);

        Assert.Equal(["c", "b", "d", "a"], sorted.Select(image => image.LScriptImageCaption));
    }

    [Fact]
    public async Task ScriptStart_ImageUnreachable_SkipsThatFormAlone()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineScriptPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        Dictionary<string, string> pages = new(TEngineScriptPages);
        pages.Remove("https://example.test/image?text=b");
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(pages));

        IReadOnlyList<LScriptImage> found = await TScriptFetchRead(engine, "整", pack.TLanguageFixtureName);

        Assert.Equal(2, found.Count);
        Assert.Equal(
            ("Clerical", 0, "PNG-C"),
            (found[1].LScriptImageStyle, found[1].LScriptImagePosition, TScriptDataRead(found[1])));
    }

    [Fact]
    public async Task ScriptRead_NothingStored_FetchesStoresAndRaises()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineScriptPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TEngineScriptPages));
        TScriptObserver observer = new();
        engine.TEngineObserverAttach(observer.TScriptObserverHandle);
        LEntry entry = engine.TEngineEntrySave(TScriptDraftCreate("整齊", pack.TLanguageFixtureName));

        Assert.Empty(engine.TEngineScriptRead(entry.LEntryId));
        engine.TEngineScriptStart(entry.LEntryId);

        LBulletin raised = await observer.TScriptObserverRaised.WaitAsync(TEngineScriptPatience);
        Assert.Equal(entry.LEntryId, raised.LBulletinId);
        await TScriptSettle(engine, entry.LEntryId);

        IReadOnlyList<LScriptImage> read = engine.TEngineScriptRead(entry.LEntryId);
        Assert.Equal(6, read.Count);
        Assert.Equal(["整", "整", "整", "齊", "齊", "齊"], read.Select(image => image.LScriptImageCharacter).ToList());
        Assert.Equal(("整", "Seal", 0, "Shuowen", "Seal gloss", "PNG-A"), TScriptImageRead(read[0]));
        Assert.Equal(6, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM script;"));
    }

    [Fact]
    public async Task ScriptStart_SourcesSilent_RaisesAndAsksOncePerSession()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineScriptPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new("nothing here", HttpStatusCode.OK);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        TScriptObserver observer = new();
        engine.TEngineObserverAttach(observer.TScriptObserverHandle);
        LEntry entry = engine.TEngineEntrySave(TScriptDraftCreate("整", pack.TLanguageFixtureName));

        engine.TEngineScriptStart(entry.LEntryId);
        await TScriptCountCheck(handler, 2);
        LBulletin raised = await observer.TScriptObserverRaised.WaitAsync(TEngineScriptPatience);
        await TScriptSettle(engine, entry.LEntryId);

        Assert.Equal(entry.LEntryId, raised.LBulletinId);
        Assert.Empty(engine.TEngineScriptRead(entry.LEntryId));
        engine.TEngineScriptStart(entry.LEntryId);
        await Task.Delay(200);

        Assert.Equal(2, handler.TSourceHandlerCount);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM script;"));
    }

    [Fact]
    public async Task ScriptRebuild_ImagesStored_DropsThemAndFetchesAgain()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineScriptPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TEngineScriptPages));
        LEntry entry = engine.TEngineEntrySave(TScriptDraftCreate("整", pack.TLanguageFixtureName));

        engine.TEngineScriptStart(entry.LEntryId);
        await TScriptSettle(engine, entry.LEntryId);
        Assert.Equal(3, engine.TEngineScriptRead(entry.LEntryId).Count);

        engine.TEngineScriptRebuild(entry.LEntryId);
        await TScriptSettle(engine, entry.LEntryId);

        IReadOnlyList<LScriptImage> read = engine.TEngineScriptRead(entry.LEntryId);
        Assert.Equal(3, read.Count);
        Assert.Equal(("整", "Seal", 0, "Shuowen", "Seal gloss", "PNG-A"), TScriptImageRead(read[0]));
        Assert.Equal(3, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM script;"));
    }

    [Fact]
    public async Task ScriptRebuild_SourcesOnceSilent_AsksThemAgain()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineScriptPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new("nothing here", HttpStatusCode.OK);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        LEntry entry = engine.TEngineEntrySave(TScriptDraftCreate("整", pack.TLanguageFixtureName));

        engine.TEngineScriptStart(entry.LEntryId);
        await TScriptCountCheck(handler, 2);
        await TScriptSettle(engine, entry.LEntryId);

        engine.TEngineScriptRebuild(entry.LEntryId);
        await TScriptCountCheck(handler, 4);
        await TScriptSettle(engine, entry.LEntryId);

        Assert.Equal(4, handler.TSourceHandlerCount);
        Assert.Empty(engine.TEngineScriptRead(entry.LEntryId));
    }

    [Fact]
    public void ScriptRebuild_LanguageWithoutStyles_FetchesNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("{}");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new("nothing here", HttpStatusCode.OK);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        LEntry entry = engine.TEngineEntrySave(TScriptDraftCreate("整", pack.TLanguageFixtureName));

        engine.TEngineScriptRebuild(entry.LEntryId);

        Assert.Empty(engine.TEngineScriptRead(entry.LEntryId));
        Assert.Equal(0, handler.TSourceHandlerCount);
    }

    [Fact]
    public void ScriptStart_LanguageWithoutStyles_ReadsEmptyWithoutFetch()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("{}");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new("nothing here", HttpStatusCode.OK);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        LEntry entry = engine.TEngineEntrySave(TScriptDraftCreate("整", pack.TLanguageFixtureName));

        engine.TEngineScriptStart(entry.LEntryId);
        Assert.Empty(engine.TEngineScriptRead(entry.LEntryId));
        Assert.False(engine.TEngineScriptCheck(entry.LEntryId));
        Assert.Equal(0, handler.TSourceHandlerCount);
    }

    private static async Task<IReadOnlyList<LScriptImage>> TScriptFetchRead(
        LEngine engine, string headword, string language)
    {
        LEntry entry = engine.TEngineEntrySave(TScriptDraftCreate(headword, language));
        engine.TEngineScriptStart(entry.LEntryId);
        await TScriptSettle(engine, entry.LEntryId);
        return engine.TEngineScriptRead(entry.LEntryId);
    }

    private static async Task TScriptSettle(LEngine engine, long entryId)
    {
        DateTime deadline = DateTime.UtcNow + TEngineScriptPatience;
        while (engine.TEngineScriptCheck(entryId))
        {
            Assert.True(DateTime.UtcNow < deadline, "Waited for the script fetch to settle.");
            await Task.Delay(20);
        }
    }

    private static async Task TScriptCountCheck(TSourceHandler handler, int count)
    {
        DateTime deadline = DateTime.UtcNow + TEngineScriptPatience;
        while (handler.TSourceHandlerCount < count)
        {
            Assert.True(DateTime.UtcNow < deadline, $"Waited for {count} requests, saw {handler.TSourceHandlerCount}.");
            await Task.Delay(20);
        }
    }

    private static (string, string, int, string, string, string) TScriptImageRead(LScriptImage image) =>
        (image.LScriptImageCharacter,
         image.LScriptImageStyle,
         image.LScriptImagePosition,
         image.LScriptImageCaption,
         image.LScriptImageGloss,
         TScriptDataRead(image));

    private static string TScriptDataRead(LScriptImage image) =>
        Encoding.UTF8.GetString(image.LScriptImageData);

    private static LEntryDraft TScriptDraftCreate(string headword, string language)
    {
        return TInterface.TEntryDraftCreate(
            headword,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "orderly", [], [], [], [], [], 1)],
            []);
    }

    private sealed class TScriptObserver
    {
        private readonly TaskCompletionSource<LBulletin> _tScriptObserverRaised =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal Task<LBulletin> TScriptObserverRaised => _tScriptObserverRaised.Task;

        internal void TScriptObserverHandle(LBulletin bulletin)
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectScript)
            {
                _tScriptObserverRaised.TrySetResult(bulletin);
            }
        }
    }
}
