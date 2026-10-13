using Llyn.Core;
using Llyn.Infrastructure;
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
        long id = Assert.IsType<long>(engine.TEngineStemFind(language, "龍"));
        LStem stem = Assert.IsType<LStem>(engine.TEngineStemRead(id));
        Assert.Equal(1, stem.LStemCount);
        Assert.Null(engine.TEngineStemFind(language, string.Empty));
        Assert.Null(engine.TEngineStemFind(language, null));
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

    [Fact]
    public void StemResolve_StoredOutOfCodePointOrder_ListsCharactersByCodePoint()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("""{ "language": "Fixture" }""");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        string rare = char.ConvertFromUtf32(0x20000);
        string compatibility = char.ConvertFromUtf32(0xF900);
        LShengfuArchive shengfu = TInterface.TShengfuArchiveCreate(workspace.TWorkspaceDatabase);
        LStemArchive archive = TInterface.TStemArchiveCreate(workspace.TWorkspaceDatabase);
        foreach (string character in new[] { rare, "江", compatibility, "工" })
        {
            shengfu.TShengfuSave(language, TInterface.TShengfuCreate(character, "工"));
            archive.TStemApply(language, character, ["工"]);
        }

        LStem stem = Assert.IsType<LStem>(archive.TStemFind(language, "工"));
        Assert.Equal([rare, "江", compatibility, "工"], archive.TStemCharacterRead(stem.LStemId));

        LStemPage page = engine.TEngineStemResolve(stem.LStemId);

        Assert.Equal(["工", "江", compatibility, rare], page.LStemPageCharacters);
    }

    [Fact]
    public void KindredFind_StoredOutOfHeadwordOrder_ListsInThePanelOrder()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("""{ "language": "Fixture" }""");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        LShengfuArchive shengfu = TInterface.TShengfuArchiveCreate(workspace.TWorkspaceDatabase);
        LStemArchive archive = TInterface.TStemArchiveCreate(workspace.TWorkspaceDatabase);
        shengfu.TShengfuSave(language, TInterface.TShengfuCreate("工", "工"));
        archive.TStemApply(language, "工", ["工"]);
        engine.TEngineEntrySave(TStemDraftCreate("工b", language));
        engine.TEngineEntrySave(TStemDraftCreate("工a", language));
        LStem stem = Assert.IsType<LStem>(archive.TStemFind(language, "工"));
        LVista reverse = engine.TEngineVistaStart("kindred", LCatalogOrder.LCatalogOrderReverse);

        IReadOnlyList<LVistaRow> plain = engine.TEngineKindredFind(language, [stem.LStemId], string.Empty);
        IReadOnlyList<LVistaRow> reversed = engine.TEngineKindredFind(language, [stem.LStemId], string.Empty, reverse);

        Assert.Equal(["工a", "工b"], plain.Select(row => row.LVistaRowHeadword));
        Assert.Equal(["工b", "工a"], reversed.Select(row => row.LVistaRowHeadword));
    }

    [Fact]
    public void StemResolve_MembersWithAndWithoutEntry_MatchByHeadwordAndCreateNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("""{ "language": "Fixture" }""");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        LShengfuArchive shengfu = TInterface.TShengfuArchiveCreate(workspace.TWorkspaceDatabase);
        LStemArchive archive = TInterface.TStemArchiveCreate(workspace.TWorkspaceDatabase);
        foreach (string character in new[] { "工", "江" })
        {
            shengfu.TShengfuSave(language, TInterface.TShengfuCreate(character, "工"));
            archive.TStemApply(language, character, ["工"]);
        }

        engine.TEngineEntrySave(TStemDraftCreate("工", language) with
        {
            LEntryDraftReflexes = [TInterface.TReflexDraftCreate("Korean", string.Empty, "공")],
        });
        LFanqieArchive fanqie = TInterface.TFanqieArchiveCreate(workspace.TWorkspaceDatabase);
        fanqie.TFanqieSave(language, "江", [TInterface.TFanqieRowCreate("江", "book", "古雙", reading: "kaewng")]);
        fanqie.TFanqieRepresentativeSet(Assert.Single(fanqie.TFanqieRead(language, "江")).LFanqieRowId, 1);
        LStem stem = Assert.IsType<LStem>(archive.TStemFind(language, "工"));
        long entries = workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;");

        LStemPage page = engine.TEngineStemResolve(stem.LStemId);

        Assert.Equal(["工", "江"], page.LStemPageMembers.Select(member => member.LStemMemberCharacter));
        LStemMember written = page.LStemPageMembers[0];
        LReflexDraft reflex = Assert.Single(written.LStemMemberReflexes);
        Assert.Equal(("Korean", "공"), (reflex.LReflexDraftLanguage, reflex.LReflexDraftText));
        Assert.Single(written.LStemMemberGuises);
        LStemMember bare = page.LStemPageMembers[1];
        Assert.Empty(bare.LStemMemberReflexes);
        Assert.Equal(["kaewng"], bare.LStemMemberReadings);
        Assert.Equal("/kaewng/", bare.LStemMemberReading);
        Assert.Equal(entries, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
    }

    [Fact]
    public void StemSpread_MemberOpenedThenClosed_KeepsItsStateApartFromTheEntryPage()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            """
            { "reflex": [
                { "language": "Jin", "url": "https://example.test/{word}", "match": "(?<text>x)", "folded": true },
                { "language": "Wu", "url": "https://example.test/{word}", "match": "(?<text>x)" } ] }
            """);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string language = pack.TLanguageFixtureName;
        LShengfuArchive shengfu = TInterface.TShengfuArchiveCreate(workspace.TWorkspaceDatabase);
        LStemArchive archive = TInterface.TStemArchiveCreate(workspace.TWorkspaceDatabase);
        foreach (string character in new[] { "工", "江", "紅" })
        {
            shengfu.TShengfuSave(language, TInterface.TShengfuCreate(character, "工"));
            archive.TStemApply(language, character, ["工"]);
        }

        long gong = engine.TEngineEntrySave(TStemDraftCreate("工", language) with
        {
            LEntryDraftReflexes =
            [
                TInterface.TReflexDraftCreate("Jin", string.Empty, "kung"),
                TInterface.TReflexDraftCreate("Wu", string.Empty, "kon"),
            ],
        }).LEntryId;
        long jiang = engine.TEngineEntrySave(TStemDraftCreate("江", language) with
        {
            LEntryDraftReflexes = [TInterface.TReflexDraftCreate("Wu", string.Empty, "kaon")],
        }).LEntryId;
        LStem stem = Assert.IsType<LStem>(archive.TStemFind(language, "工"));

        engine.TEngineStemSpread(stem.LStemId, "工", true);
        engine.TEngineStemSpread(stem.LStemId, "紅", true);
        LStemPage opened = engine.TEngineStemResolve(stem.LStemId);
        engine.TEngineReflexSpread(jiang, true);
        engine.TEngineStemSpread(stem.LStemId, "工", false);
        LStemPage closed = engine.TEngineStemResolve(stem.LStemId);

        Assert.Equal([true, false, false], opened.LStemPageMembers.Select(static member => member.LStemMemberOpened));
        Assert.Equal(
            [true, false, false],
            opened.LStemPageMembers.Select(
                static member => member.LStemMemberGuises.Any(static guise => guise.LReflexGuiseFolded)));
        Assert.False(engine.TEngineSpreadCheck(gong));
        Assert.All(closed.LStemPageMembers, static member => Assert.False(member.LStemMemberOpened));
        Assert.True(engine.TEngineSpreadCheck(jiang));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM stem_fold;"));
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
