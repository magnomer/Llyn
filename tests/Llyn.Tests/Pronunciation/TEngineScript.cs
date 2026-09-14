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
    public async Task ScriptFind_TwoStyles_ReadsImagesCaptionsAndGlossInPackOrder()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineScriptPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TEngineScriptPages));

        IReadOnlyList<LScriptImage> found = await engine.TEngineScriptFind(
            "整", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal(3, found.Count);
        Assert.Equal(("整", "Seal", 0, "Shuowen", "Seal gloss", "PNG-A"), TScriptImageRead(found[0]));
        Assert.Equal(("整", "Clerical", 0, "Stone Han", "", "PNG-B"), TScriptImageRead(found[1]));
        Assert.Equal(("整", "Clerical", 1, "", "", "PNG-C"), TScriptImageRead(found[2]));
    }

    [Fact]
    public async Task ScriptFind_ImageUnreachable_SkipsThatFormAlone()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineScriptPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        Dictionary<string, string> pages = new(TEngineScriptPages);
        pages.Remove("https://example.test/image?text=b");
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(pages));

        IReadOnlyList<LScriptImage> found = await engine.TEngineScriptFind(
            "整", pack.TLanguageFixtureName, CancellationToken.None);

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
        engine.TEngineObserverAttach(observer);
        LEntry entry = engine.TEngineEntrySave(TScriptDraftCreate("整齊", pack.TLanguageFixtureName));

        Assert.Empty(engine.TEngineScriptRead(entry.LEntryId));

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
    public async Task ScriptRead_SourcesSilent_AsksOncePerSession()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineScriptPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new("nothing here", HttpStatusCode.OK);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        LEntry entry = engine.TEngineEntrySave(TScriptDraftCreate("整", pack.TLanguageFixtureName));

        Assert.Empty(engine.TEngineScriptRead(entry.LEntryId));
        await TScriptCountCheck(handler, 2);
        await TScriptSettle(engine, entry.LEntryId);

        Assert.Empty(engine.TEngineScriptRead(entry.LEntryId));
        await Task.Delay(200);

        Assert.Equal(2, handler.TSourceHandlerCount);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM script;"));
    }

    [Fact]
    public void ScriptRead_LanguageWithoutStyles_ReadsEmptyWithoutFetch()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("{}");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new("nothing here", HttpStatusCode.OK);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        LEntry entry = engine.TEngineEntrySave(TScriptDraftCreate("整", pack.TLanguageFixtureName));

        Assert.Empty(engine.TEngineScriptRead(entry.LEntryId));
        Assert.False(engine.TEngineScriptCheck(entry.LEntryId));
        Assert.Equal(0, handler.TSourceHandlerCount);
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

    private sealed class TScriptObserver : LObserver
    {
        private readonly TaskCompletionSource<LBulletin> _tScriptObserverRaised =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal Task<LBulletin> TScriptObserverRaised => _tScriptObserverRaised.Task;

        public void LObserverBulletinHandle(LBulletin bulletin)
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectScript)
            {
                _tScriptObserverRaised.TrySetResult(bulletin);
            }
        }
    }
}
