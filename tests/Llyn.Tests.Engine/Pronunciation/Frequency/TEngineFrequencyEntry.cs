using System.Net;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineFrequencyEntry
{
    [Fact]
    public void FrequencyStart_SettingOff_WritesNothingRaisesNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequency.TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("a=7", HttpStatusCode.OK));
        TEngineFrequency.TFrequencyObserver observer = new();
        engine.TEngineObserverAttach(observer.TFrequencyObserverHandle);
        engine.TEngineFrequencySave(false);

        LEntryDraft draft = TEngineFrequency.TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName);
        LEntry entry = engine.TEngineEntrySave(draft);
        engine.TEngineFrequencyStart(entry.LEntryId);

        Assert.Empty(engine.TEngineFrequencyRead(entry.LEntryId));
        Assert.Equal(0, TEngineFrequency.TFrequencyCountRead(workspace, entry.LEntryId));
        Assert.False(observer.TFrequencyObserverRaised.IsCompleted);
    }

    [Fact]
    public async Task EntryUpdate_HeadwordChanged_ClearsStoredRowsThenRefetches()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequency.TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TaskCompletionSource gate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("a=7", gate.Task));
        TEngineFrequency.TFrequencyObserver observer = new();
        engine.TEngineObserverAttach(observer.TFrequencyObserverHandle);
        engine.TEngineFrequencySave(false);
        LEntryDraft draft = TEngineFrequency.TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName);
        LEntry entry = engine.TEngineEntrySave(draft);
        TEngineFrequency.TFrequencyStoredSet(workspace, entry.LEntryId, "First", "5", "Everyday");
        engine.TEngineFrequencySave(true);

        LEntryDraft loaded = engine.TEngineEntryLoad(entry.LEntryId)!;
        engine.TEngineEntryUpdate(entry.LEntryId, loaded with { LEntryDraftHeadword = "tomatoes" });

        Assert.Equal(0, TEngineFrequency.TFrequencyCountRead(workspace, entry.LEntryId));

        gate.SetResult();
        LBulletin raised = await observer.TFrequencyObserverRaised.WaitAsync(TEngineFrequency.TEngineFrequencyPatience);

        Assert.Equal(entry.LEntryId, raised.LBulletinId);
        Assert.Equal(1, TEngineFrequency.TFrequencyCountRead(workspace, entry.LEntryId));
        Assert.Equal(1, workspace.TWorkspaceCountRead(
            $"SELECT COUNT(*) FROM frequency WHERE entry_parent = {entry.LEntryId} " +
            "AND source = 'First' AND raw = '7' AND band = 'Advanced';"));
    }

    [Fact]
    public async Task FrequencyStart_EntryDeletedMidFlight_WritesNothing()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequency.TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TaskCompletionSource gate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("a=7", gate.Task));
        TEngineFrequency.TFrequencyObserver observer = new();
        engine.TEngineObserverAttach(observer.TFrequencyObserverHandle);

        LEntryDraft draft = TEngineFrequency.TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName);
        LEntry entry = engine.TEngineEntrySave(draft);
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
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequency.TEngineFrequencyPack);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("a=7", HttpStatusCode.OK));
        TEngineFrequency.TFrequencyObserver observer = new();
        engine.TEngineObserverAttach(observer.TFrequencyObserverHandle);
        engine.TEngineFrequencySave(false);
        LEntryDraft draft = TEngineFrequency.TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName);
        LEntry entry = engine.TEngineEntrySave(draft);
        workspace.TWorkspaceScriptRun($"UPDATE entry SET language = '' WHERE entry_id = {entry.LEntryId};");
        engine.TEngineFrequencySave(true);

        Assert.Empty(engine.TEngineFrequencyRead(entry.LEntryId));
        engine.TEngineFrequencyStart(entry.LEntryId);

        Assert.Equal(0, TEngineFrequency.TFrequencyCountRead(workspace, entry.LEntryId));
        Assert.False(observer.TFrequencyObserverRaised.IsCompleted);
    }

    [Fact]
    public async Task EntryUpdate_LanguageChangedMidFlight_DropsOldAnswer()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TEngineFrequency.TEngineFrequencyPack);
        using TLanguageFixture silent = TLanguageFixture.TLanguageFixtureCreate("{}");
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TaskCompletionSource gate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate("a=7", gate.Task));
        TEngineFrequency.TFrequencyObserver observer = new();
        engine.TEngineObserverAttach(observer.TFrequencyObserverHandle);

        LEntryDraft draft = TEngineFrequency.TFrequencyDraftCreate("tomato", pack.TLanguageFixtureName);
        LEntry entry = engine.TEngineEntrySave(draft);
        LEntryDraft loaded = engine.TEngineEntryLoad(entry.LEntryId)!;
        engine.TEngineEntryUpdate(entry.LEntryId, loaded with { LEntryDraftLanguage = silent.TLanguageFixtureName });
        gate.SetResult();

        Task settled = await Task.WhenAny(observer.TFrequencyObserverRaised, Task.Delay(500));

        Assert.NotSame(observer.TFrequencyObserverRaised, settled);
        Assert.Equal(0, TEngineFrequency.TFrequencyCountRead(workspace, entry.LEntryId));
    }
}
