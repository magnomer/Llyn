using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineShengfu
{
    private const string TEngineShengfuPack =
        """
        { "fanqie": [
            { "name": "Broad", "source": "Wiki", "url": "https://example.test/wiki/{word}?raw",
              "match": "{word}",
              "line": "\"(?<initial>.)(?<rime>.)(?<division>.)(?<rounded>[開合]) (?<tone>.)(?<spelling>..)\"",
              "rounded": "合" } ],
          "shengfu": { "source": "Wiki", "url": "https://example.test/series/{word}?raw",
            "match": "\\{ \"\\d+\", \"(?<shengfu>[^\"]+)\"" } }
        """;

    private const string TEngineShengfuBooks =
        """
        return {
            "來東一開 去盧貢"
        }
        """;

    private const string TEngineShengfuModule =
        """
        return {
            { "9586", "弄", "東", "0", "弄", "roːŋs", "" },
        }
        """;

    private const string TEngineShengfuTwice =
        """
        return {
            { "8436", "龍", "東", "0", "龍", "b·roŋ", "" },
            { "8763", "尨", "東", "0", "厖", "mroːŋ", "同狵" }
        }
        """;

    private static readonly TimeSpan TEngineShengfuPatience = TimeSpan.FromSeconds(5);

    [Fact]
    public async Task ShengfuStart_SeriesModule_PrintsItOnTheCharactersFirstBlock()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineShengfuPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        Dictionary<string, string> pages = new()
        {
            ["https://example.test/wiki/%E5%BC%84?raw"] = TEngineShengfuBooks,
            ["https://example.test/series/%E5%BC%84?raw"] = TEngineShengfuModule,
        };
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(pages));
        LEntry entry = engine.TEngineEntrySave(TShengfuDraftCreate("弄", pack.TLanguageFixtureName));

        engine.TEngineFanqieStart(entry.LEntryId);
        await TShengfuSettle(engine, entry.LEntryId);

        LFanqieGroup group = Assert.Single(engine.TEngineFanqieDivide(entry.LEntryId));
        Assert.Equal("弄", Assert.Single(group.LFanqieGroupStems));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM shengfu;"));
    }

    [Fact]
    public async Task ShengfuStart_TwoSeriesRows_JoinsThemInAnswerOrder()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineShengfuPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        Dictionary<string, string> pages = new()
        {
            ["https://example.test/wiki/%E9%BE%8D?raw"] = TEngineShengfuBooks,
            ["https://example.test/series/%E9%BE%8D?raw"] = TEngineShengfuTwice,
        };
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(pages));
        LEntry entry = engine.TEngineEntrySave(TShengfuDraftCreate("龍", pack.TLanguageFixtureName));

        engine.TEngineFanqieStart(entry.LEntryId);
        await TShengfuSettle(engine, entry.LEntryId);

        Assert.Equal(
            new[] { "龍", "尨" },
            Assert.Single(engine.TEngineFanqieDivide(entry.LEntryId)).LFanqieGroupStems);
    }

    [Fact]
    public async Task ShengfuStart_PackWithoutSeries_LeavesEveryBlockWithout()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            """
            { "fanqie": [
                { "name": "Broad", "source": "Wiki", "url": "https://example.test/wiki/{word}?raw",
                  "match": "{word}",
                  "line": "\"(?<initial>.)(?<rime>.)(?<division>.)(?<rounded>[開合]) (?<tone>.)(?<spelling>..)\"" } ] }
            """);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        Dictionary<string, string> pages = new()
        {
            ["https://example.test/wiki/%E5%BC%84?raw"] = TEngineShengfuBooks,
        };
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(pages));
        LEntry entry = engine.TEngineEntrySave(TShengfuDraftCreate("弄", pack.TLanguageFixtureName));

        engine.TEngineFanqieStart(entry.LEntryId);
        await TShengfuSettle(engine, entry.LEntryId);

        Assert.Empty(Assert.Single(engine.TEngineFanqieDivide(entry.LEntryId)).LFanqieGroupStems);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM shengfu;"));
    }

    private static async Task TShengfuSettle(LEngine engine, long entryId)
    {
        DateTime deadline = DateTime.UtcNow + TEngineShengfuPatience;
        while (engine.TEngineFanqieCheck(entryId))
        {
            Assert.True(DateTime.UtcNow < deadline, "Waited for the series fetch to settle.");
            await Task.Delay(20);
        }
    }

    private static LEntryDraft TShengfuDraftCreate(string headword, string language)
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
