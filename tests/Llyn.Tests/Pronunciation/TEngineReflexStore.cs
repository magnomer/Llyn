using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineReflexStore
{
    [Fact]
    public async Task ReflexStart_NothingStored_FetchesStoresAndRaises()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TReflexFixture.TReflexPages));
        TReflexObserver observer = new();
        engine.TEngineObserverAttach(observer.TReflexObserverHandle);
        LEntry entry = engine.TEngineEntrySave(
            TReflexFixture.TReflexDraftCreate("弄", pack.TLanguageFixtureName));

        Assert.Empty(engine.TEngineReflexRead(entry.LEntryId));
        engine.TEngineReflexStart(entry.LEntryId);

        LBulletin raised = await observer.TReflexObserverRaised.WaitAsync(TReflexFixture.TReflexPatience);
        Assert.Equal(entry.LEntryId, raised.LBulletinId);
        await TReflexFixture.TReflexSettle(engine, entry.LEntryId);

        IReadOnlyList<LReflex> read = engine.TEngineReflexRead(entry.LEntryId);
        Assert.Equal(5, read.Count);
        Assert.Equal(("Japanese", "Kan-on", "ろう", "", true), TReflexFixture.TReflexRowRead(read[2]));
        Assert.Equal(("Mandarin", "", "nʊŋ⁵¹", "nòng", false), TReflexFixture.TReflexRowRead(read[3]));
        Assert.Equal("Beijing", read[3].LReflexRegion);
        Assert.Equal(5, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM reflex;"));
    }

    [Fact]
    public async Task ReflexStart_RowsStored_FetchesNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new(TReflexFixture.TReflexNong, HttpStatusCode.OK);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        LEntry entry = engine.TEngineEntrySave(
            TReflexFixture.TReflexDraftCreate("弄", pack.TLanguageFixtureName) with
            {
                LEntryDraftReflexes = [TInterface.TReflexDraftCreate("Korean", "", "롱")],
            });

        engine.TEngineReflexStart(entry.LEntryId);
        await Task.Delay(200);

        Assert.Equal(0, handler.TSourceHandlerCount);
        Assert.Equal("롱", Assert.Single(engine.TEngineReflexRead(entry.LEntryId)).LReflexText);
    }

    [Fact]
    public async Task ReflexRebuild_RowsStored_DropsThemAndFetchesAgain()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TReflexFixture.TReflexPages));
        LEntry entry = engine.TEngineEntrySave(
            TReflexFixture.TReflexDraftCreate("弄", pack.TLanguageFixtureName) with
            {
                LEntryDraftReflexes = [TInterface.TReflexDraftCreate("Korean", "", "롱")],
            });
        TReflexObserver observer = new();
        engine.TEngineObserverAttach(observer.TReflexObserverHandle);

        engine.TEngineReflexRebuild(entry.LEntryId);

        LBulletin raised = await observer.TReflexObserverRaised.WaitAsync(TReflexFixture.TReflexPatience);
        Assert.Equal(entry.LEntryId, raised.LBulletinId);
        await TReflexFixture.TReflexSettle(engine, entry.LEntryId);

        IReadOnlyList<LReflex> read = engine.TEngineReflexRead(entry.LEntryId);
        Assert.Equal(5, read.Count);
        Assert.Equal(("Korean", "", "롱(농) | 희롱할", "", true), TReflexFixture.TReflexRowRead(read[0]));
    }

    [Fact]
    public async Task ReflexRebuild_HandTypedMeaning_RestoresItByRowShape()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexMeaningPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TReflexPagesRead("corrected source")));
        LEntry entry = engine.TEngineEntrySave(
            TReflexFixture.TReflexDraftCreate("惡", pack.TLanguageFixtureName) with
            {
                LEntryDraftReflexes =
                [
                    TInterface.TReflexDraftCreate(
                        "Wu", "literary", "oʔ⁵⁵", meaning: "stale source", region: "Shanghai"),
                ],
            });
        LDraft started = engine.TEngineDraftStart("Input", entry.LEntryId);
        long row = Assert.Single(started.LDraftContent.LEntryDraftReflexes).LReflexDraftId;
        engine.TEngineRequestApply(TInterface.TReflexMeaningCreate(started.LDraftId, row, "hand typed"));
        engine.TEngineDraftCommit(started.LDraftId);

        engine.TEngineReflexRebuild(entry.LEntryId);
        await TReflexFixture.TReflexSettle(engine, entry.LEntryId);

        LReflex rebuilt = Assert.Single(engine.TEngineReflexRead(entry.LEntryId));
        Assert.Equal("hand typed", rebuilt.LReflexMeaning);
        Assert.True(rebuilt.LReflexOwned);
    }

    [Fact]
    public async Task ReflexRebuild_ScrapedMeaning_ReadsTheCorrectedSource()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexMeaningPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TReflexPagesRead("corrected source")));
        LEntry entry = engine.TEngineEntrySave(
            TReflexFixture.TReflexDraftCreate("惡", pack.TLanguageFixtureName) with
            {
                LEntryDraftReflexes =
                [
                    TInterface.TReflexDraftCreate(
                        "Wu", "literary", "oʔ⁵⁵", meaning: "stale source", region: "Shanghai"),
                ],
            });

        engine.TEngineReflexRebuild(entry.LEntryId);
        await TReflexFixture.TReflexSettle(engine, entry.LEntryId);

        LReflex rebuilt = Assert.Single(engine.TEngineReflexRead(entry.LEntryId));
        Assert.Equal("corrected source", rebuilt.LReflexMeaning);
        Assert.False(rebuilt.LReflexOwned);
    }

    [Fact]
    public void EpithetRead_KoreanRowsStored_JoinsClippedPieces()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(
            TReflexFixture.TReflexDraftCreate("惡", pack.TLanguageFixtureName) with
            {
                LEntryDraftReflexes =
                [
                    TInterface.TReflexDraftCreate("Korean", "", "악할 악(악)"),
                    TInterface.TReflexDraftCreate("Korean", "", "미워할 오"),
                    TInterface.TReflexDraftCreate("Japanese", "Go-on", "あく"),
                ],
            });

        Assert.Equal("악할 악, 미워할 오", engine.TEngineEpithetRead(entry.LEntryId));
        Assert.Equal(
            "악할 악, 미워할 오",
            TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase).TEntryEpithetRead(entry.LEntryId));

        engine.TEngineEpithetSave(false);

        Assert.Equal(string.Empty, engine.TEngineEpithetRead(entry.LEntryId));

        engine.TEngineReflexSet(entry.LEntryId, []);

        Assert.Equal(
            string.Empty,
            TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase).TEntryEpithetRead(entry.LEntryId));
    }

    [Fact]
    public async Task ReflexStart_PagesNotFound_AsksOncePerSession()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new("gone", HttpStatusCode.NotFound);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        LEntry entry = engine.TEngineEntrySave(
            TReflexFixture.TReflexDraftCreate("弄", pack.TLanguageFixtureName));

        engine.TEngineReflexStart(entry.LEntryId);
        await TReflexFixture.TReflexSettle(engine, entry.LEntryId);
        engine.TEngineReflexStart(entry.LEntryId);
        await Task.Delay(200);

        Assert.Equal(3, handler.TSourceHandlerCount);
        Assert.Empty(engine.TEngineReflexRead(entry.LEntryId));
    }

    [Fact]
    public async Task ReflexStart_HeldDraftWithoutRows_FillsTheDraft()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TReflexFixture.TReflexPages));
        LEntry entry = engine.TEngineEntrySave(
            TReflexFixture.TReflexDraftCreate("弄", pack.TLanguageFixtureName));
        LDraft started = engine.TEngineDraftStart("Input", entry.LEntryId);
        TReflexObserver observer = new(started.LDraftId);
        engine.TEngineObserverAttach(observer.TReflexObserverHandle);

        engine.TEngineReflexStart(entry.LEntryId);
        await observer.TReflexObserverRaised.WaitAsync(TReflexFixture.TReflexPatience);
        await TReflexFixture.TReflexSettle(engine, entry.LEntryId);

        LDraft? held = engine.TEngineDraftRead(started.LDraftId);
        Assert.NotNull(held);
        Assert.Equal(5, held.LDraftContent.LEntryDraftReflexes.Count);
        Assert.All(held.LDraftContent.LEntryDraftReflexes, row => Assert.True(row.LReflexDraftId > 0));
        Assert.False(engine.TEngineDraftCheck(started.LDraftId));
    }

    [Fact]
    public void WorkspaceOpen_MigratedBlankColumns_DerivesEpithetAndRespelling()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TReflexFixture.TReflexPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        long id;
        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            id = engine.TEngineEntrySave(
                TReflexFixture.TReflexDraftCreate("惡", pack.TLanguageFixtureName) with
                {
                    LEntryDraftReflexes =
                    [
                        TInterface.TReflexDraftCreate("Korean", "", "악할 악(악)"),
                        TInterface.TReflexDraftCreate("Cantonese", "", "t͡sɪŋ³⁵"),
                    ],
                }).LEntryId;
        }

        workspace.TWorkspaceScriptRun(
            $"""
            UPDATE entry SET epithet = '';
            UPDATE reflex SET respelling = '';
            UPDATE schema_version SET version = {LSchemaMigration.LSchemaMigrationVersion - 1};
            """);

        using LEngine reopened = workspace.TWorkspaceEngineStart();

        Assert.Equal("악할 악", reopened.TEngineEpithetRead(id));
        Assert.Equal(["", "tsiŋ³⁵"], reopened.TEngineReflexRead(id).Select(row => row.LReflexRespelling));
    }

    private const string TReflexMeaningPack =
        """
        { "reflex": [ { "language": "Wu", "region": "Shanghai", "url": "https://example.test/wiki/{word}",
          "match": "kind: (?<kind>[^;]+); text: (?<text>[^;]+); meaning: (?<meaning>[^<]+)",
          "every": true } ] }
        """;

    private static IReadOnlyDictionary<string, string> TReflexPagesRead(string meaning) =>
        new Dictionary<string, string>
        {
            ["https://example.test/wiki/%E6%83%A1"] = $"kind: literary; text: oʔ⁵⁵; meaning: {meaning}",
        };
}
