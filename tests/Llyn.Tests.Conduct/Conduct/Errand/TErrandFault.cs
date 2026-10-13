using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TErrandFault
{
    [Fact]
    public async Task PreviewStart_HostBroken_ShowsTheNoticeAndMarksTheReadingRefused()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("{}");
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(new InvalidOperationException("broken")));
        engine.TEngineDelaySet(0);
        List<string> asked = [];
        CErrand errand = await TInterfaceConductDesk.TErrandClipStart(
            TInterfaceConductDesk.TErrandClipPrepare(engine, asked), pack.TLanguageFixtureName);
        CRecording found = new("Tagged", "https://example.test/gb.mp3", 0, true, "British");
        errand.TErrandHarvestResonate(new CHarvestStep("Tagged", 0, found, false));
        List<CClipRoll> changes = [];
        errand.CErrandClipChanged += changes.Add;
        asked.Clear();

        Assert.Null(await errand.CErrandPreviewStart(found).WaitAsync(TimeSpan.FromSeconds(5)));

        Assert.Equal(["Input.RecordingFailed"], asked);
        Assert.True(changes[^1].CClipRollRows[0].CClipItemReading[0].CClipReadingRefused);
    }

    [Fact]
    public async Task RecordingSave_HostBroken_ShowsTheNoticeAndOffersTheRetry()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("{}");
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(new InvalidOperationException("broken")));
        engine.TEngineDelaySet(0);
        List<string> asked = [];
        CErrand errand = await TInterfaceConductDesk.TErrandClipStart(
            TInterfaceConductDesk.TErrandClipPrepare(engine, asked), pack.TLanguageFixtureName);
        CRecording found = new("Tagged", "https://example.test/gb.mp3", 0, true, "British");
        errand.TErrandHarvestResonate(new CHarvestStep("Tagged", 0, found, false));
        List<CClipRoll> changes = [];
        errand.CErrandClipChanged += changes.Add;
        asked.Clear();

        Assert.False(await errand.CErrandRecordingSave(found).WaitAsync(TimeSpan.FromSeconds(5)));

        Assert.Equal(["Input.RecordingFailed"], asked);
        Assert.Equal("Downloader.Retry", changes[^1].CClipRollRows[0].CClipItemReading[0].CClipReadingAction);
    }

    [Fact]
    public async Task RecordingSave_StoreFaults_ShowsTheNoticeRecordsTheFaultAndOffersTheRetry()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("{}");
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        List<Exception> recorded = [];
        using LEngine engine = TErrandFaultStart(workspace, recorded);
        List<string> asked = [];
        TEditorFixture editor = TInterfaceConductDesk.TErrandClipPrepare(engine, asked);
        CErrand errand = await TInterfaceConductDesk.TErrandClipStart(editor, pack.TLanguageFixtureName);
        CRecording found = new("Tagged", "https://example.test/gb.mp3", 0, true, "British");
        errand.TErrandHarvestResonate(new CHarvestStep("Tagged", 0, found, false));
        List<CClipRoll> changes = [];
        errand.CErrandClipChanged += changes.Add;
        asked.Clear();

        Assert.False(await errand.CErrandRecordingSave(found).WaitAsync(TimeSpan.FromSeconds(5)));

        Assert.Equal(["Input.RecordingFailed"], asked);
        Assert.IsType<LVaultFault>(Assert.Single(recorded));
        CClipReading retry = changes[^1].CClipRollRows[0].CClipItemReading[0];
        Assert.Equal("Downloader.Retry", retry.CClipReadingAction);
        Assert.True(retry.CClipReadingReady);
        Assert.Equal(
            string.Empty,
            editor.TEditorFixtureDesk.TDeskRead()?.LDraftContent.LEntryDraftPronunciation?.LPronunciationDraftAudio
            ?? string.Empty);
        editor.TEditorFinish(false);
    }

    [Fact]
    public async Task PreviewStart_StoreFaults_ShowsTheNoticeRecordsTheFaultAndMarksTheReadingRefused()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate("{}");
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        List<Exception> recorded = [];
        using LEngine engine = TErrandFaultStart(workspace, recorded);
        List<string> asked = [];
        TEditorFixture editor = TInterfaceConductDesk.TErrandClipPrepare(engine, asked);
        CErrand errand = await TInterfaceConductDesk.TErrandClipStart(editor, pack.TLanguageFixtureName);
        CRecording found = new("Tagged", "https://example.test/gb.mp3", 0, true, "British");
        errand.TErrandHarvestResonate(new CHarvestStep("Tagged", 0, found, false));
        List<CClipRoll> changes = [];
        errand.CErrandClipChanged += changes.Add;
        asked.Clear();

        Assert.Null(await errand.CErrandPreviewStart(found).WaitAsync(TimeSpan.FromSeconds(5)));

        Assert.Equal(["Input.RecordingFailed"], asked);
        Assert.IsType<LVaultFault>(Assert.Single(recorded));
        CClipReading refused = changes[^1].CClipRollRows[0].CClipItemReading[0];
        Assert.False(refused.CClipReadingFetching);
        Assert.False(refused.CClipReadingPlaying);
        Assert.True(refused.CClipReadingRefused);
        editor.TEditorFinish(false);
    }

    [Fact]
    public async Task ErrandEnsignLoad_FlagBroken_ShowsTheNotice()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            "{ \"varieties\": { \"shown\": \"flag\", \"list\": [ { \"name\": \"British\", \"flag\": \"gb\" } ] } }");
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart(
            TPronunciationHelper.TSourceClientCreate(new InvalidOperationException("broken")));
        engine.TEngineDelaySet(0);
        List<string> asked = [];
        CErrand errand = await TInterfaceConductDesk.TErrandClipStart(
            TInterfaceConductDesk.TErrandClipPrepare(engine, asked), pack.TLanguageFixtureName);
        asked.Clear();

        CClipRoll roll = await errand.CErrandEnsignLoad(static (_, _) => static () => { });

        Assert.Equal(["Input.RecordingFailed"], asked);
        Assert.False(roll.CClipRollSearching);
    }

    private static LEngine TErrandFaultStart(TWorkspace workspace, List<Exception> recorded)
    {
        Dictionary<string, Func<object?[]?, object?>> stored = new()
        {
            ["LRecordingSave"] = _ => Task.FromException<string>(TInterface.TVaultFaultCreate("The disk is full.")),
            ["LRecordingPrepare"] = _ => Task.FromException<string>(TInterface.TVaultFaultCreate("Refused.")),
            ["LRecordingSweep"] = _ => null,
        };
        Dictionary<string, Func<object?[]?, object?>> audited = new()
        {
            ["LAuditRecord"] = args =>
            {
                recorded.Add((Exception)args![0]!);
                return null;
            },
        };
        LRig rig = workspace.TWorkspaceRigCreate() with
        {
            LRigSource = workspace.TWorkspaceRigCreate().LRigSource with
            {
                LRigSourceRecordings = TEngineFake.TEngineCreate<LRecordingVault>(stored),
            },
            LRigAudit = TEngineFake.TEngineCreate<LAuditVault>(audited),
        };
        LEngine engine = new(rig, _ => rig, _ => { });
        engine.TEngineDelaySet(0);
        return engine;
    }
}
