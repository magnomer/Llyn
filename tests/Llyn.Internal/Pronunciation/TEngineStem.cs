using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineStem
{
    private const string TEngineStemPack =
        """
        { "fanqie": [
            { "name": "Broad", "source": "Wiki", "url": "https://example.test/wiki/{word}?raw",
              "match": "{word}",
              "line": "\"(?<initial>.)(?<rime>.)(?<division>.)(?<rounded>[開合]) (?<tone>.)(?<spelling>..)\"",
              "rounded": "合" } ],
          "shengfu": { "source": "Wiki", "url": "https://example.test/series/{word}?raw",
            "match": "\\{ \"\\d+\", \"(?<shengfu>[^\"]+)\"" } }
        """;

    private const string TEngineStemBooks =
        """
        return {
            "來東一開 去盧貢"
        }
        """;

    private const string TEngineStemModule =
        """
        return {
            { "8436", "龍", "東", "0", "龍", "b·roŋ", "" },
        }
        """;

    private static readonly TimeSpan TEngineStemPatience = TimeSpan.FromSeconds(5);

    [Fact]
    public async Task StemApply_FetchedSeries_OpensAsAPageWithItsEntries()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineStemPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        Dictionary<string, string> pages = new()
        {
            ["https://example.test/wiki/%E9%BE%8D?raw"] = TEngineStemBooks,
            ["https://example.test/series/%E9%BE%8D?raw"] = TEngineStemModule,
        };
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(pages));
        string language = pack.TLanguageFixtureName;
        LEntry entry = engine.TEngineEntrySave(TStemDraftCreate("龍", language));

        engine.TEngineFanqieStart(entry.LEntryId);
        await TStemSettle(engine, entry.LEntryId);

        Assert.NotNull(engine.TEngineStemFind());
        LStem stem = Assert.IsType<LStem>(engine.TEngineStemFind(language, "龍"));
        Assert.Equal(1, stem.LStemCount);
        LStemPage page = engine.TEngineStemResolve(stem.LStemId);
        Assert.Equal((language, "龍"), (page.LStemPageLanguage, page.LStemPageKey));
        Assert.Equal("龍", Assert.Single(page.LStemPageCharacters));
        Assert.Equal(
            entry.LEntryId,
            Assert.Single(engine.TEngineKindredFind(language, [stem.LStemId], string.Empty)).LVistaRowId);
    }

    [Fact]
    public void StemFind_PackWithoutSeries_StoresNoSeriesOfItsOwn()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            """
            { "fanqie": [
                { "name": "Broad", "source": "Wiki", "url": "https://example.test/wiki/{word}?raw",
                  "match": "{word}",
                  "line": "\"(?<initial>.)(?<rime>.)(?<division>.)(?<rounded>[開合]) (?<tone>.)(?<spelling>..)\"" } ] }
            """);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Null(engine.TEngineStemFind(pack.TLanguageFixtureName, "龍"));
        Assert.True(engine.TEngineStemResolve(null).LStemPageEmpty);
    }

    private static async Task TStemSettle(LEngine engine, long entryId)
    {
        DateTime deadline = DateTime.UtcNow + TEngineStemPatience;
        while (engine.TEngineFanqieCheck(entryId))
        {
            Assert.True(DateTime.UtcNow < deadline, "Waited for the series fetch to settle.");
            await Task.Delay(20);
        }
    }

    private static LEntryDraft TStemDraftCreate(string headword, string language)
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
