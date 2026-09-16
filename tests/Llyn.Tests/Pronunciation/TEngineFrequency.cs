using System.Net;
using System.Net.Http;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineFrequency
{
    private const string TEngineFrequencyPack =
        """
        { "frequency": [
            { "name": "First", "attempts": [
                { "urls": ["https://example.test/a/{word}"], "strategy": "regex", "match": "a=(\\S+)", "group": 1 } ],
              "once": { "total": 1000000 },
              "bands": [
                { "match": "^[SW]1$", "name": "Core" },
                { "match": "^\\d+$", "name": "Rare" } ] },
            { "name": "Second", "attempts": [
                { "urls": ["https://example.test/b/{word}"], "strategy": "regex",
                  "match": "b=(\\S+)", "group": 1 } ],
              "once": { "factor": 12 },
              "bands": [
                { "match": "^unranked$", "name": "Rare" } ] },
            { "name": "Third", "attempts": [
                { "urls": ["https://example.test/c/{word}"], "strategy": "regex",
                  "match": "c=(\\S+)", "group": 1 } ],
              "unit": "Level" } ] }
        """;

    private static readonly TimeSpan TEngineFrequencyPatience = TimeSpan.FromSeconds(5);

    [Fact]
    public async Task FrequencyFind_FirstSourceSilent_KeepsSecondAlone()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(
            new Dictionary<string, string>
            {
                ["https://example.test/a/tomato"] = "nothing here",
                ["https://example.test/b/tomato"] = "b=7",
            }));

        IReadOnlyList<LFrequency> found = await engine.TEngineFrequencyFind(
            "tomato", pack.TLanguageFixtureName, CancellationToken.None);

        LFrequency row = Assert.Single(found);
        Assert.Equal(
            ("Second", "7", "Core", 84L),
            (row.LFrequencySource, row.LFrequencyRaw, row.LFrequencyBand, row.LFrequencyOnce));
    }

    [Fact]
    public async Task FrequencyFind_BothSourcesAnswer_KeepsBothInWrittenOrder()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(
            new Dictionary<string, string>
            {
                ["https://example.test/a/tomato"] = "a=S1",
                ["https://example.test/b/tomato"] = "b=250",
            }));

        IReadOnlyList<LFrequency> found = await engine.TEngineFrequencyFind(
            "tomato", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal(
            [("First", "S1", "Core", null), ("Second", "250", "Core", 3000L)],
            found.Select(row => (row.LFrequencySource, row.LFrequencyRaw, row.LFrequencyBand, row.LFrequencyOnce)));
    }

    [Fact]
    public async Task FrequencyFind_UnitSource_StampsUnitOnNumericAnswerOnly()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(
            new Dictionary<string, string>
            {
                ["https://example.test/a/tomato"] = "a=S1",
                ["https://example.test/c/tomato"] = "c=3",
            }));

        IReadOnlyList<LFrequency> found = await engine.TEngineFrequencyFind(
            "tomato", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.Equal(
            [("First", "S1", null, null), ("Third", "3", "Level", null)],
            found.Select(row => (row.LFrequencySource, row.LFrequencyRaw, row.LFrequencyUnit, row.LFrequencyOnce)));
    }

    [Fact]
    public void BandResolve_NumericRaw_GradesByIntervalBeforePackPatterns()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Equal("Advanced", engine.TEngineBandResolve(pack.TLanguageFixtureName, "First", "5"));
        Assert.Equal("Everyday", engine.TEngineBandResolve(pack.TLanguageFixtureName, "First", "50"));
        Assert.Equal("Core", engine.TEngineBandResolve(pack.TLanguageFixtureName, "First", "500"));
        Assert.Equal("Rare", engine.TEngineBandResolve(pack.TLanguageFixtureName, "First", "0.5"));
        Assert.Equal("Core", engine.TEngineBandResolve(pack.TLanguageFixtureName, "First", "W1"));
        Assert.Null(engine.TEngineBandResolve(pack.TLanguageFixtureName, "First", "W2"));
        Assert.Equal("Core", engine.TEngineBandResolve(pack.TLanguageFixtureName, "Second", "100"));
        Assert.Equal("Advanced", engine.TEngineBandResolve(pack.TLanguageFixtureName, "Second", "50000"));
        Assert.Equal("Rare", engine.TEngineBandResolve(pack.TLanguageFixtureName, "Second", "unranked"));
        Assert.Null(engine.TEngineBandResolve(pack.TLanguageFixtureName, "Third", "5"));
        Assert.Null(engine.TEngineBandResolve(pack.TLanguageFixtureName, "Fourth", "5"));
        Assert.Null(engine.TEngineBandResolve(" ", "First", "5"));
    }

    [Fact]
    public void FrequencyRead_StaleStoredBand_RegradesFromRawAndStoresIt()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName));
        TFrequencyStoredSet(workspace, entry.LEntryId, "First", "5", "Stale");

        IReadOnlyList<LFrequency> read = engine.TEngineFrequencyRead(entry.LEntryId);

        LFrequency row = Assert.Single(read);
        Assert.Equal(
            ("First", "5", "Advanced", 200000L),
            (row.LFrequencySource, row.LFrequencyRaw, row.LFrequencyBand, row.LFrequencyOnce));
        Assert.Equal(1, workspace.TWorkspaceCountRead(
            $"SELECT COUNT(*) FROM frequency WHERE entry_parent = {entry.LEntryId} AND band = 'Advanced';"));
    }

    [Fact]
    public void FrequencyRead_MigratedRowWithoutBand_ResolvesAndStoresIt()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName));
        TFrequencyStoredSet(workspace, entry.LEntryId, "First", "5", null);

        IReadOnlyList<LFrequency> read = engine.TEngineFrequencyRead(entry.LEntryId);

        Assert.Equal("Advanced", Assert.Single(read).LFrequencyBand);
        Assert.Equal(1, workspace.TWorkspaceCountRead(
            $"SELECT COUNT(*) FROM frequency WHERE entry_parent = {entry.LEntryId} AND band = 'Advanced';"));
    }

    [Fact]
    public void FrequencyStart_SettingOff_WritesNothingRaisesNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("a=7", HttpStatusCode.OK));
        TFrequencyObserver observer = new();
        engine.TEngineObserverAttach(observer);
        engine.TEngineFrequencySave(false);

        LEntry entry = engine.TEngineEntrySave(TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName));
        engine.TEngineFrequencyStart(entry.LEntryId);

        Assert.Empty(engine.TEngineFrequencyRead(entry.LEntryId));
        Assert.Equal(0, TFrequencyCountRead(workspace, entry.LEntryId));
        Assert.False(observer.TFrequencyObserverRaised.IsCompleted);
    }

    [Fact]
    public async Task EntryUpdate_HeadwordChanged_ClearsStoredRowsThenRefetches()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TaskCompletionSource gate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("a=7", gate.Task));
        TFrequencyObserver observer = new();
        engine.TEngineObserverAttach(observer);
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName));
        TFrequencyStoredSet(workspace, entry.LEntryId, "First", "5", "Everyday");
        engine.TEngineFrequencySave(true);

        LEntryDraft loaded = engine.TEngineEntryLoad(entry.LEntryId)!;
        engine.TEngineEntryUpdate(entry.LEntryId, loaded with { LEntryDraftHeadword = "tomatoes" });

        Assert.Equal(0, TFrequencyCountRead(workspace, entry.LEntryId));

        gate.SetResult();
        LBulletin raised = await observer.TFrequencyObserverRaised.WaitAsync(TEngineFrequencyPatience);

        Assert.Equal(entry.LEntryId, raised.LBulletinId);
        Assert.Equal(1, TFrequencyCountRead(workspace, entry.LEntryId));
        Assert.Equal(1, workspace.TWorkspaceCountRead(
            $"SELECT COUNT(*) FROM frequency WHERE entry_parent = {entry.LEntryId} " +
            "AND source = 'First' AND raw = '7' AND band = 'Advanced';"));
    }

    [Fact]
    public async Task FrequencyStart_EntryDeletedMidFlight_WritesNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TaskCompletionSource gate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("a=7", gate.Task));
        TFrequencyObserver observer = new();
        engine.TEngineObserverAttach(observer);

        LEntry entry = engine.TEngineEntrySave(TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName));
        engine.TEngineEntryDelete(entry.LEntryId);
        gate.SetResult();

        Task settled = await Task.WhenAny(observer.TFrequencyObserverRaised, Task.Delay(500));

        Assert.NotSame(observer.TFrequencyObserverRaised, settled);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM frequency;"));
    }

    [Fact]
    public void FrequencyRead_BlankLanguage_ReadsNothingWithoutFill()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("a=7", HttpStatusCode.OK));
        TFrequencyObserver observer = new();
        engine.TEngineObserverAttach(observer);
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName));
        workspace.TWorkspaceScriptRun($"UPDATE entry SET language = '' WHERE entry_id = {entry.LEntryId};");
        engine.TEngineFrequencySave(true);

        Assert.Empty(engine.TEngineFrequencyRead(entry.LEntryId));
        engine.TEngineFrequencyStart(entry.LEntryId);

        Assert.Equal(0, TFrequencyCountRead(workspace, entry.LEntryId));
        Assert.False(observer.TFrequencyObserverRaised.IsCompleted);
    }

    [Fact]
    public void FrequencyRead_BlankLanguageWithStoredRow_LeavesBandAndOnceNull()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(TFrequencyDraftCreate("tomato", "English"));
        workspace.TWorkspaceScriptRun($"UPDATE entry SET language = '' WHERE entry_id = {entry.LEntryId};");
        TFrequencyStoredSet(workspace, entry.LEntryId, "First", "5", null);

        LFrequency read = Assert.Single(engine.TEngineFrequencyRead(entry.LEntryId));

        Assert.Equal(("First", "5"), (read.LFrequencySource, read.LFrequencyRaw));
        Assert.Null(read.LFrequencyBand);
        Assert.Null(read.LFrequencyOnce);
    }

    [Fact]
    public async Task EntryUpdate_LanguageChangedMidFlight_DropsOldAnswer()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TLanguageFixture silent = TLanguageFixture.TLanguageFixtureCreate("{}");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TaskCompletionSource gate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("a=7", gate.Task));
        TFrequencyObserver observer = new();
        engine.TEngineObserverAttach(observer);

        LEntry entry = engine.TEngineEntrySave(TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName));
        LEntryDraft loaded = engine.TEngineEntryLoad(entry.LEntryId)!;
        engine.TEngineEntryUpdate(entry.LEntryId, loaded with { LEntryDraftLanguage = silent.TLanguageFixtureName });
        gate.SetResult();

        Task settled = await Task.WhenAny(observer.TFrequencyObserverRaised, Task.Delay(500));

        Assert.NotSame(observer.TFrequencyObserverRaised, settled);
        Assert.Equal(0, TFrequencyCountRead(workspace, entry.LEntryId));
    }

    [Fact]
    public async Task FrequencyRead_SourcesSilent_AsksOncePerSession()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new("nothing here", HttpStatusCode.OK);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));

        LEntry entry = engine.TEngineEntrySave(TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName));
        await TFrequencyCountCheck(handler, 3);
        await Task.Delay(200);

        Assert.Empty(engine.TEngineFrequencyRead(entry.LEntryId));
        Assert.Empty(engine.TEngineFrequencyRead(entry.LEntryId));
        await Task.Delay(200);

        Assert.Equal(3, handler.TSourceHandlerCount);
    }

    [Fact]
    public async Task FrequencyRead_SourcesUnreachable_AsksAgain()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new(string.Empty, HttpStatusCode.ServiceUnavailable);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));

        LEntry entry = engine.TEngineEntrySave(TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName));
        await TFrequencyCountCheck(handler, 3);
        await Task.Delay(200);

        Assert.Empty(engine.TEngineFrequencyRead(entry.LEntryId));
        await TFrequencyCountCheck(handler, 6);
    }

    [Fact]
    public async Task FrequencyFind_EverySourceAsked_EvenAfterTheFirstAnswers()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TSourceHandler handler = new("a=2.5", HttpStatusCode.OK);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));

        IReadOnlyList<LFrequency> found = await engine.TEngineFrequencyFind(
            "tomato", pack.TLanguageFixtureName, CancellationToken.None);

        LFrequency row = Assert.Single(found);
        Assert.Equal(
            ("First", "2.5", "Advanced", 400000L),
            (row.LFrequencySource, row.LFrequencyRaw, row.LFrequencyBand, row.LFrequencyOnce));
        Assert.Equal(3, handler.TSourceHandlerCount);
    }

    private static async Task TFrequencyCountCheck(TSourceHandler handler, int count)
    {
        DateTime deadline = DateTime.UtcNow + TEngineFrequencyPatience;
        while (handler.TSourceHandlerCount < count)
        {
            Assert.True(DateTime.UtcNow < deadline, $"Waited for {count} requests, saw {handler.TSourceHandlerCount}.");
            await Task.Delay(20);
        }
    }

    private static LEntryDraft TFrequencyDraftCreate(string headword, string language)
    {
        return TInterface.TEntryDraftCreate(
            headword,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a red fruit", [], [], [], [], [], 1)],
            []);
    }

    private static void TFrequencyStoredSet(TWorkspace workspace, long entryId, string source, string raw, string? band)
    {
        string label = band is null ? "NULL" : $"'{band}'";
        workspace.TWorkspaceScriptRun(
            "INSERT OR REPLACE INTO frequency (entry_parent, source, raw, band, fetched_utc) " +
            $"VALUES ({entryId}, '{source}', '{raw}', {label}, '2026-01-01');");
    }

    private static long TFrequencyCountRead(TWorkspace workspace, long entryId)
    {
        return workspace.TWorkspaceCountRead($"SELECT COUNT(*) FROM frequency WHERE entry_parent = {entryId};");
    }

    private sealed class TFrequencyObserver : LObserver
    {
        private readonly TaskCompletionSource<LBulletin> _tFrequencyObserverRaised =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal Task<LBulletin> TFrequencyObserverRaised => _tFrequencyObserverRaised.Task;

        public void LObserverBulletinHandle(LBulletin bulletin)
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectFrequency)
            {
                _tFrequencyObserverRaised.TrySetResult(bulletin);
            }
        }
    }
}
