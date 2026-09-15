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
                { "urls": ["https://example.test/a/{word}"], "strategy": "regex", "match": "a=(\\S+)", "group": 1 } ] },
            { "name": "Second", "attempts": [
                { "urls": ["https://example.test/b/{word}"], "strategy": "regex",
                  "match": "b=(\\S+)", "group": 1 } ] } ],
          "bands": [
            { "upTo": 10, "name": "Common" },
            { "match": "^[SW]1$", "name": "Very common" },
            { "match": "^\\d+$", "name": "Rare" } ] }
        """;

    private static readonly TimeSpan TEngineFrequencyPatience = TimeSpan.FromSeconds(5);

    [Fact]
    public async Task FrequencyFind_FirstSourceSilent_TakesSecondInWrittenOrder()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(
            new Dictionary<string, string>
            {
                ["https://example.test/a/tomato"] = "nothing here",
                ["https://example.test/b/tomato"] = "b=7",
            }));

        LFrequency? found = await engine.TEngineFrequencyFind(
            "tomato", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.NotNull(found);
        Assert.Equal(("Second", "7", "Common"), (found.LFrequencySource, found.LFrequencyRaw, found.LFrequencyBand));
    }

    [Fact]
    public async Task FrequencyFind_BothSourcesAnswer_TakesFirstInWrittenOrder()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(
            new Dictionary<string, string>
            {
                ["https://example.test/a/tomato"] = "a=S1",
                ["https://example.test/b/tomato"] = "b=7",
            }));

        LFrequency? found = await engine.TEngineFrequencyFind(
            "tomato", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.NotNull(found);
        Assert.Equal(
            ("First", "S1", "Very common"),
            (found.LFrequencySource, found.LFrequencyRaw, found.LFrequencyBand));
    }

    [Fact]
    public void BandResolve_RowsInPackOrder_PrefersFirstMatch()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Equal("Common", engine.TEngineBandResolve(pack.TLanguageFixtureName, "5"));
        Assert.Equal("Common", engine.TEngineBandResolve(pack.TLanguageFixtureName, "9.5"));
        Assert.Equal("Rare", engine.TEngineBandResolve(pack.TLanguageFixtureName, "50"));
        Assert.Equal("Very common", engine.TEngineBandResolve(pack.TLanguageFixtureName, "W1"));
        Assert.Null(engine.TEngineBandResolve(pack.TLanguageFixtureName, "W2"));
        Assert.Null(engine.TEngineBandResolve(" ", "5"));
    }

    [Fact]
    public void FrequencyRead_StoredValue_ResolvesBandFromPack()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName));
        TFrequencyStoredSet(workspace, entry.LEntryId, "First|5");

        LFrequency? read = engine.TEngineFrequencyRead(entry.LEntryId);

        Assert.NotNull(read);
        Assert.Equal(("First", "5", "Common"), (read.LFrequencySource, read.LFrequencyRaw, read.LFrequencyBand));
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

        Assert.Null(engine.TEngineFrequencyRead(entry.LEntryId));
        Assert.Null(engine.TEngineEntryRead(entry.LEntryId)!.LEntryFrequency);
        Assert.False(observer.TFrequencyObserverRaised.IsCompleted);
    }

    [Fact]
    public async Task EntryUpdate_HeadwordChanged_ClearsStoredValueThenRefetches()
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
        TFrequencyStoredSet(workspace, entry.LEntryId, "First|5");
        engine.TEngineFrequencySave(true);

        LEntryDraft loaded = engine.TEngineEntryLoad(entry.LEntryId)!;
        engine.TEngineEntryUpdate(entry.LEntryId, loaded with { LEntryDraftHeadword = "tomatoes" });

        Assert.Null(engine.TEngineEntryRead(entry.LEntryId)!.LEntryFrequency);

        gate.SetResult();
        LBulletin raised = await observer.TFrequencyObserverRaised.WaitAsync(TEngineFrequencyPatience);

        Assert.Equal(entry.LEntryId, raised.LBulletinId);
        Assert.Equal("First|7", engine.TEngineEntryRead(entry.LEntryId)!.LEntryFrequency);
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
    }

    [Fact]
    public void FrequencyRead_BlankLanguage_ReadsNullWithoutFill()
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

        Assert.Null(engine.TEngineFrequencyRead(entry.LEntryId));
        engine.TEngineFrequencyStart(entry.LEntryId);

        Assert.Null(engine.TEngineEntryRead(entry.LEntryId)!.LEntryFrequency);
        Assert.False(observer.TFrequencyObserverRaised.IsCompleted);
    }

    [Fact]
    public void FrequencyRead_BlankLanguageWithStoredValue_LeavesBandNull()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(TFrequencyDraftCreate("tomato", "English"));
        workspace.TWorkspaceScriptRun($"UPDATE entry SET language = '' WHERE entry_id = {entry.LEntryId};");
        TFrequencyStoredSet(workspace, entry.LEntryId, "First|5");

        LFrequency? read = engine.TEngineFrequencyRead(entry.LEntryId);

        Assert.NotNull(read);
        Assert.Equal(("First", "5"), (read.LFrequencySource, read.LFrequencyRaw));
        Assert.Null(read.LFrequencyBand);
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
        Assert.Null(engine.TEngineEntryRead(entry.LEntryId)!.LEntryFrequency);
    }

    [Fact]
    public async Task FrequencyRead_SourcesSilent_AsksOncePerSession()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new("nothing here", HttpStatusCode.OK);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));

        LEntry entry = engine.TEngineEntrySave(TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName));
        await TFrequencyCountCheck(handler, 2);
        await Task.Delay(200);

        Assert.Null(engine.TEngineFrequencyRead(entry.LEntryId));
        Assert.Null(engine.TEngineFrequencyRead(entry.LEntryId));
        await Task.Delay(200);

        Assert.Equal(2, handler.TSourceHandlerCount);
    }

    [Fact]
    public async Task FrequencyRead_SourcesUnreachable_AsksAgain()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new(string.Empty, HttpStatusCode.ServiceUnavailable);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));

        LEntry entry = engine.TEngineEntrySave(TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName));
        await TFrequencyCountCheck(handler, 2);
        await Task.Delay(200);

        Assert.Null(engine.TEngineFrequencyRead(entry.LEntryId));
        await TFrequencyCountCheck(handler, 4);
    }

    [Fact]
    public async Task FrequencyFind_FirstSourceAnswers_SkipsSecond()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TSourceHandler handler = new("a=2.5", HttpStatusCode.OK);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));

        LFrequency? found = await engine.TEngineFrequencyFind(
            "tomato", pack.TLanguageFixtureName, CancellationToken.None);

        Assert.NotNull(found);
        Assert.Equal(("First", "2.5", "Common"), (found.LFrequencySource, found.LFrequencyRaw, found.LFrequencyBand));
        Assert.Equal(1, handler.TSourceHandlerCount);
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

    private static void TFrequencyStoredSet(TWorkspace workspace, long entryId, string stored)
    {
        workspace.TWorkspaceScriptRun($"UPDATE entry SET frequency = '{stored}' WHERE entry_id = {entryId};");
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
