using System.Net;
using System.Net.Http;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineFanqieSource
{
    internal const string TEngineWikiPack =
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

    internal const string TEngineFanqieWiki =
        """
        return {
            "知東三開 平陟弓",
            "見桓一合 去古玩"
        }
        """;

    [Fact]
    public async Task FanqieSourceFind_PatternTimesOut_AnswersNotReached()
    {
        using HttpClient client = TPronunciationHelper.TSourceClientCreate(
            new string('a', 40) + "!", HttpStatusCode.OK);

        (IReadOnlyList<LFanqieRow> found, bool reached) =
            await TInterfaceSource.TFanqieSourceFind(client, "(a+)+$", "整");

        Assert.Empty(found);
        Assert.False(reached);
    }

    [Fact]
    public async Task FanqieStart_LineBook_StoresPartsUnderItsSource()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineWikiPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        Dictionary<string, string> pages = new()
        {
            ["https://example.test/broad"] = TEngineFanqie.TEngineFanqieBroad,
            ["https://example.test/wiki/%E5%90%B3?raw"] = TEngineFanqieWiki,
        };
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(pages));

        IReadOnlyList<LFanqieRow> found = await TEngineFanqie.TFanqieFetchRead(engine, "吳", pack.TLanguageFixtureName);

        Assert.Equal(["Kaom", "Wiki", "Wiki"], found.Select(row => row.LFanqieRowSource).ToList());
        Assert.Equal("知 東 三等平", found[1].LFanqieRowText);
        Assert.Equal(("知", "東", "", "三", "平", false), TEngineFanqie.TFanqiePartRead(found[1]));
        Assert.Equal(["", "陟弓", "古玩"], found.Select(row => row.LFanqieRowSpelling).ToList());
        Assert.Equal(("見", "桓", "", "一", "去", true), TEngineFanqie.TFanqiePartRead(found[2]));
        Assert.Equal([0, 0, 1], found.Select(row => row.LFanqieRowPosition).ToList());
    }

    [Fact]
    public async Task FanqieStart_TwoSourcesOneBook_StoresRowsOfBoth()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineWikiPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        Dictionary<string, string> pages = new()
        {
            ["https://example.test/broad"] = TEngineFanqie.TEngineFanqieBroad,
            ["https://example.test/wiki/%E5%90%B3?raw"] = TEngineFanqieWiki,
        };
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(pages));
        LEntry entry = engine.TEngineEntrySave(TEngineFanqie.TFanqieDraftCreate("吳", pack.TLanguageFixtureName));

        engine.TEngineFanqieStart(entry.LEntryId);
        await TEngineFanqie.TFanqieSettle(engine, entry.LEntryId);

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
        LEntry entry = engine.TEngineEntrySave(TEngineFanqie.TFanqieDraftCreate("吳", pack.TLanguageFixtureName));

        engine.TEngineFanqieStart(entry.LEntryId);
        await TEngineFanqie.TFanqieCountCheck(handler, 2);
        await TEngineFanqie.TFanqieSettle(engine, entry.LEntryId);
        engine.TEngineFanqieStart(entry.LEntryId);
        await Task.Delay(200);

        Assert.Equal(2, handler.TSourceHandlerCount);
    }
}
