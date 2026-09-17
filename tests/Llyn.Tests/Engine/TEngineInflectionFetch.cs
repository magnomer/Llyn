using System.Net;
using System.Net.Http;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineInflectionFetch
{
    private const string TInflectionFetchPast =
        "<b class=\"Latn form-of lang-en spast-form-of\" lang=\"en\"><a href=\"./went\">went</a></b>";

    private const string TInflectionFetchBody =
        TInflectionFetchPast +
        "<b class=\"Latn form-of lang-en past|part-form-of\" lang=\"en\"><a href=\"./gone\">gone</a></b>" +
        "<b class=\"Latn form-of lang-en p-form-of\" lang=\"en\"><a href=\"./goes\">goes</a></b>";

    private static readonly TimeSpan TInflectionFetchPatience = TimeSpan.FromSeconds(5);

    [Fact]
    public async Task InflectionStart_BothFormsFound_MarksSlotsSpecifiedRaisesOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TInflectionFetchBody, HttpStatusCode.OK));
        TInflectionObserver observer = new();
        engine.TEngineObserverAttach(observer);
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(TInflectionDraftCreate("go"));

        engine.TEngineInflectionStart(entry.LEntryId);
        LBulletin raised = await observer.TInflectionObserverRaised.WaitAsync(TInflectionFetchPatience);

        Assert.Equal(entry.LEntryId, raised.LBulletinId);
        IReadOnlyList<LParadigmSlot> slots = engine.TEngineParadigmRead(entry.LEntryId);
        Assert.Equal(["went", "gone"], slots.Select(slot => slot.LParadigmSlotInflection?.LInflectionText));
        Assert.All(slots, slot => Assert.Equal(LState.LStateSpecified, slot.LParadigmSlotState));
        Assert.All(
            slots,
            slot => Assert.Equal(
                slot.LParadigmSlotSpeech.LSpeechValueId, slot.LParadigmSlotInflection?.LInflectionSpeechId));
        await Task.Delay(200);
        Assert.Equal(1, observer.TInflectionObserverCount);
    }

    [Fact]
    public async Task InflectionStart_OnlyPastFound_MarksParticipleUnknown()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TInflectionFetchPast, HttpStatusCode.OK));
        TInflectionObserver observer = new();
        engine.TEngineObserverAttach(observer);
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(TInflectionDraftCreate("go"));

        engine.TEngineInflectionStart(entry.LEntryId);
        await observer.TInflectionObserverRaised.WaitAsync(TInflectionFetchPatience);

        IReadOnlyList<LParadigmSlot> slots = engine.TEngineParadigmRead(entry.LEntryId);
        Assert.Equal(LState.LStateSpecified, slots[0].LParadigmSlotState);
        Assert.Equal("went", slots[0].LParadigmSlotInflection?.LInflectionText);
        Assert.Equal(LState.LStateUnknown, slots[1].LParadigmSlotState);
        Assert.Null(slots[1].LParadigmSlotInflection);
    }

    [Fact]
    public async Task InflectionStart_NothingFound_RaisesSoUnknownShows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("<p class=\"lang-en\">nothing</p>", HttpStatusCode.OK));
        TInflectionObserver observer = new();
        engine.TEngineObserverAttach(observer);
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(TInflectionDraftCreate("must"));

        engine.TEngineInflectionStart(entry.LEntryId);
        LBulletin raised = await observer.TInflectionObserverRaised.WaitAsync(TInflectionFetchPatience);

        Assert.Equal(entry.LEntryId, raised.LBulletinId);
        Assert.Empty(engine.TEngineInflectionRead(entry.LEntryId));
        Assert.All(
            engine.TEngineParadigmRead(entry.LEntryId),
            slot => Assert.Equal(LState.LStateUnknown, slot.LParadigmSlotState));
    }

    [Fact]
    public async Task InflectionStart_SourceUnreachable_MarksLostAndAsksOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new(string.Empty, HttpStatusCode.ServiceUnavailable);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        TInflectionObserver observer = new();
        engine.TEngineObserverAttach(observer);
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(TInflectionDraftCreate("go"));

        engine.TEngineInflectionStart(entry.LEntryId);
        await observer.TInflectionObserverRaised.WaitAsync(TInflectionFetchPatience);
        int asked = handler.TSourceHandlerCount;
        engine.TEngineInflectionStart(entry.LEntryId);
        await Task.Delay(200);

        Assert.Equal(asked, handler.TSourceHandlerCount);
        Assert.False(engine.TEngineInflectionCheck(entry.LEntryId));
        Assert.All(
            engine.TEngineParadigmRead(entry.LEntryId),
            slot => Assert.Equal(LState.LStateUnspecified, slot.LParadigmSlotState));
        Assert.Empty(engine.TEngineInflectionRead(entry.LEntryId));

        engine.TEngineInflectionSet(entry.LEntryId, []);
        engine.TEngineInflectionStart(entry.LEntryId);
        await TInflectionCountCheck(handler, asked + 1);
    }

    [Fact]
    public async Task InflectionStart_WhilePending_KeepsFirstFetch()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TaskCompletionSource gate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TSourceHandler handler = new(TInflectionFetchBody, HttpStatusCode.OK, gate.Task);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        TInflectionObserver observer = new();
        engine.TEngineObserverAttach(observer);
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(TInflectionDraftCreate("go"));

        engine.TEngineInflectionStart(entry.LEntryId);
        Assert.True(engine.TEngineInflectionCheck(entry.LEntryId));
        engine.TEngineInflectionStart(entry.LEntryId);
        gate.SetResult();
        await observer.TInflectionObserverRaised.WaitAsync(TInflectionFetchPatience);

        Assert.Equal(1, handler.TSourceHandlerCount);
        Assert.False(engine.TEngineInflectionCheck(entry.LEntryId));
        Assert.Equal(2, engine.TEngineInflectionRead(entry.LEntryId).Count);
    }

    [Fact]
    public async Task InflectionStart_EntryHeldInDraft_AsksNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new(TInflectionFetchBody, HttpStatusCode.OK);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(TInflectionDraftCreate("go"));
        LDraft draft = engine.TEngineDraftStart("test", entry.LEntryId);

        engine.TEngineInflectionStart(entry.LEntryId);
        await Task.Delay(200);

        Assert.Equal(0, handler.TSourceHandlerCount);
        engine.TEngineDraftDelete(draft.LDraftId);
        engine.TEngineInflectionStart(entry.LEntryId);
        await TInflectionCountCheck(handler, 1);
    }

    [Fact]
    public async Task InflectionStart_SettingOff_AsksNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TInterface.TSettingsSave(
            workspace.TWorkspaceFolder, TInterface.TSettingsCreate("en", frequency: false, morphology: false));
        TSourceHandler handler = new(TInflectionFetchBody, HttpStatusCode.OK);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        TInflectionObserver observer = new();
        engine.TEngineObserverAttach(observer);
        LEntry entry = engine.TEngineEntrySave(TInflectionDraftCreate("go"));

        engine.TEngineInflectionStart(entry.LEntryId);
        await Task.Delay(200);

        Assert.Equal(0, handler.TSourceHandlerCount);
        Assert.Empty(engine.TEngineInflectionRead(entry.LEntryId));
        Assert.False(observer.TInflectionObserverRaised.IsCompleted);
    }

    [Fact]
    public async Task EntryUpdate_HeadwordChangedMidFlight_DiscardsForms()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TaskCompletionSource gate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TInflectionFetchBody, gate.Task));
        TInflectionObserver observer = new();
        engine.TEngineObserverAttach(observer);
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(TInflectionDraftCreate("go"));

        engine.TEngineInflectionStart(entry.LEntryId);
        LEntryDraft loaded = engine.TEngineEntryLoad(entry.LEntryId)!;
        engine.TEngineEntryUpdate(entry.LEntryId, loaded with { LEntryDraftHeadword = "goes" });
        gate.SetResult();
        Task settled = await Task.WhenAny(observer.TInflectionObserverRaised, Task.Delay(500));

        Assert.NotSame(observer.TInflectionObserverRaised, settled);
        Assert.Empty(engine.TEngineInflectionRead(entry.LEntryId));
        Assert.All(
            engine.TEngineParadigmRead(entry.LEntryId),
            slot => Assert.Equal(LState.LStateUnspecified, slot.LParadigmSlotState));
    }

    [Fact]
    public async Task InflectionSet_AfterMiss_ClearsUnknownMark()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(TInflectionFetchPast, HttpStatusCode.OK));
        TInflectionObserver observer = new();
        engine.TEngineObserverAttach(observer);
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(TInflectionDraftCreate("go"));
        engine.TEngineInflectionStart(entry.LEntryId);
        await observer.TInflectionObserverRaised.WaitAsync(TInflectionFetchPatience);
        Assert.Equal(LState.LStateUnknown, engine.TEngineParadigmRead(entry.LEntryId)[1].LParadigmSlotState);

        engine.TEngineInflectionSet(entry.LEntryId, []);

        Assert.All(
            engine.TEngineParadigmRead(entry.LEntryId),
            slot => Assert.Equal(LState.LStateUnspecified, slot.LParadigmSlotState));
    }

    [Fact]
    public async Task InflectionStart_AfterReopen_KeepsUnknownSlot()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new(TInflectionFetchPast, HttpStatusCode.OK);
        LEntry entry = await TInflectionMissRun(workspace, handler);

        using LEngine reopened = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        int asked = handler.TSourceHandlerCount;
        reopened.TEngineInflectionStart(entry.LEntryId);
        await Task.Delay(200);

        Assert.Equal(asked, handler.TSourceHandlerCount);
        Assert.False(reopened.TEngineInflectionCheck(entry.LEntryId));
        IReadOnlyList<LParadigmSlot> slots = reopened.TEngineParadigmRead(entry.LEntryId);
        Assert.Equal(LState.LStateSpecified, slots[0].LParadigmSlotState);
        Assert.Equal("went", slots[0].LParadigmSlotInflection?.LInflectionText);
        Assert.Equal(LState.LStateUnknown, slots[1].LParadigmSlotState);
    }

    [Fact]
    public async Task InflectionStart_AfterReopen_LostEntryAsksNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new(string.Empty, HttpStatusCode.ServiceUnavailable);
        LEntry entry = await TInflectionMissRun(workspace, handler);

        using LEngine reopened = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        int asked = handler.TSourceHandlerCount;
        reopened.TEngineInflectionStart(entry.LEntryId);
        await Task.Delay(200);

        Assert.Equal(asked, handler.TSourceHandlerCount);
        Assert.False(reopened.TEngineInflectionCheck(entry.LEntryId));
        Assert.All(
            reopened.TEngineParadigmRead(entry.LEntryId),
            slot => Assert.Equal(LState.LStateUnspecified, slot.LParadigmSlotState));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM lacuna;"));
    }

    [Fact]
    public async Task InflectionSet_AfterReopen_ClearsLacuna()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TSourceHandler handler = new(TInflectionFetchPast, HttpStatusCode.OK);
        LEntry entry = await TInflectionMissRun(workspace, handler);

        using LEngine reopened = workspace.TWorkspaceEngineStart(new HttpClient(handler));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM lacuna;"));
        reopened.TEngineInflectionSet(entry.LEntryId, []);

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM lacuna;"));
        Assert.All(
            reopened.TEngineParadigmRead(entry.LEntryId),
            slot => Assert.Equal(LState.LStateUnspecified, slot.LParadigmSlotState));
        int asked = handler.TSourceHandlerCount;
        reopened.TEngineInflectionStart(entry.LEntryId);
        await TInflectionCountCheck(handler, asked + 1);
    }

    private static async Task<LEntry> TInflectionMissRun(TWorkspace workspace, TSourceHandler handler)
    {
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler, false));
        TInflectionObserver observer = new();
        engine.TEngineObserverAttach(observer);
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(TInflectionDraftCreate("go"));

        engine.TEngineInflectionStart(entry.LEntryId);
        await observer.TInflectionObserverRaised.WaitAsync(TInflectionFetchPatience);
        return entry;
    }

    private static async Task TInflectionCountCheck(TSourceHandler handler, int count)
    {
        DateTime deadline = DateTime.UtcNow + TInflectionFetchPatience;
        while (handler.TSourceHandlerCount < count)
        {
            Assert.True(DateTime.UtcNow < deadline, $"Waited for {count} requests, saw {handler.TSourceHandlerCount}.");
            await Task.Delay(20);
        }
    }

    private static LEntryDraft TInflectionDraftCreate(string headword)
    {
        return TInterface.TEntryDraftCreate(
            headword,
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "to move", [], [], [], [], [], 1)],
            [],
            string.Empty,
            null,
            ["Verb, transitive"]);
    }

    private sealed class TInflectionObserver : LObserver
    {
        private readonly TaskCompletionSource<LBulletin> _tInflectionObserverRaised =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        private int _tInflectionObserverCount;

        internal Task<LBulletin> TInflectionObserverRaised => _tInflectionObserverRaised.Task;

        internal int TInflectionObserverCount => Volatile.Read(ref _tInflectionObserverCount);

        public void LObserverBulletinHandle(LBulletin bulletin)
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectInflection)
            {
                Interlocked.Increment(ref _tInflectionObserverCount);
                _tInflectionObserverRaised.TrySetResult(bulletin);
            }
        }
    }
}
