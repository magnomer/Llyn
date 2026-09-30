using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TErrand
{
    [Fact]
    public void ErrandRecordingRead_EngineRecording_CarriesEveryFieldBothWays()
    {
        LRecording recording = TInterface.TRecordingCreate("Oxford", "https://example.test/a.mp3", 2, false, "British");

        CRecording? mirror = TInterfaceConduct.TErrandRecordingRead(recording);

        Assert.Equal(new CRecording("Oxford", "https://example.test/a.mp3", 2, false, "British"), mirror);
        Assert.Equal(recording, TInterfaceConduct.TErrandRecordingRead(mirror!));
    }

    [Fact]
    public void ErrandRecordingRead_NoRecording_ReturnsNone()
    {
        Assert.Null(TInterfaceConduct.TErrandRecordingRead((LRecording?)null));
    }

    [Fact]
    public void ErrandCandidateRead_EngineCandidate_CarriesEveryField()
    {
        LCandidate candidate = TInterface.TCandidateCreate("Wiktionary", "ˈhæpi", 1, true, "American");

        Assert.Equal(
            new CCandidate("Wiktionary", "ˈhæpi", 1, true, "American", null),
            TInterfaceConduct.TErrandCandidateRead(candidate));
    }

    [Fact]
    public void ErrandCandidateRead_NoCandidate_ReturnsNone()
    {
        Assert.Null(TInterfaceConduct.TErrandCandidateRead(null));
    }

    [Fact]
    public async Task ErrandStart_NoTenureHeld_StartsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TInterfaceConduct.TDeskCreate(engine, "Input", TEnvoyFake.TEnvoyCreate(false, []));

        CClipRoll roll = desk.CDeskErrand.CErrandRecordingStart(0);

        Assert.Empty(roll.CClipRollRows);
        Assert.True(roll.CClipRollEmpty);
        Assert.False(roll.CClipRollSearching);
        Assert.Equal("Downloader.Empty", roll.CClipRollNotice);
        Assert.Null(await desk.CDeskErrand.CErrandPreviewStart(
            new CRecording("Tagged", "https://example.test/gb.mp3", 0, true, "British")));
        Assert.False(desk.CDeskErrand.CErrandTranscriptionStart("happy", 0, "ipa", static _ => { }));
        desk.CDeskErrand.CErrandCancel();
    }

    [Fact]
    public void ErrandHarvestResonate_ThreeSteps_RaisesTheClipOnceEach()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CErrand errand = TErrandCreate(engine);
        List<string> notices = [];
        errand.CErrandClipChanged += roll => notices.Add(
            $"{roll.CClipRollRows[0].CClipItemNotice} {roll.CClipRollRows[0].CClipItemReady} "
            + $"{roll.CClipRollSearching} {roll.CClipRollEmpty}");
        CRecording recording = new("Tagged", "https://example.test/gb.mp3", 0, true, "British");

        errand.TErrandHarvestResonate(new CHarvestStep("Tagged", 0, null, false));
        errand.TErrandHarvestResonate(new CHarvestStep("Tagged", 0, recording, false));
        errand.TErrandHarvestResonate(new CHarvestStep(string.Empty, 0, null, true));

        Assert.Equal(["Downloader.Searching False False False", " True False False", " True False False"], notices);
    }

    [Fact]
    public void ErrandHarvestResonate_SourcesOutOfOrder_KeepsThePackOrderOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CErrand errand = TErrandCreate(engine);
        CClipRoll? shown = null;
        errand.CErrandClipChanged += roll => shown = roll;

        errand.TErrandHarvestResonate(new CHarvestStep("Late", 2, null, false));
        errand.TErrandHarvestResonate(new CHarvestStep("Early", 1, null, false));
        errand.TErrandHarvestResonate(new CHarvestStep("Early", 1, null, false));

        Assert.NotNull(shown);
        Assert.Equal(["Early", "Late"], shown.CClipRollRows.Select(static row => row.CClipItemSource));
        Assert.All(shown.CClipRollRows, static row => Assert.Equal("Downloader.Searching", row.CClipItemNotice));
        Assert.All(shown.CClipRollRows, static row => Assert.False(row.CClipItemReady));
        Assert.False(shown.CClipRollEmpty);
    }

    [Fact]
    public void ErrandHarvestResonate_NoAddress_ChoosesMissingWhenReachedAndBrokenOtherwise()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CErrand errand = TErrandCreate(engine);
        CClipRoll? shown = null;
        errand.CErrandClipChanged += roll => shown = roll;

        errand.TErrandHarvestResonate(
            new CHarvestStep("Reached", 0, new CRecording("Reached", null, 0, true, ""), false));
        errand.TErrandHarvestResonate(new CHarvestStep("Lost", 1, new CRecording("Lost", null, 1, false, ""), false));

        Assert.NotNull(shown);
        Assert.Equal(
            [("Downloader.Missing", false), ("Downloader.Broken", false)],
            shown.CClipRollRows.Select(static row => (row.CClipItemNotice, row.CClipItemReady)));
    }

    [Fact]
    public void ErrandHarvestResonate_EmptyAnswerAfterARecording_KeepsTheRecording()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CErrand errand = TErrandCreate(engine);
        CClipRoll? shown = null;
        errand.CErrandClipChanged += roll => shown = roll;
        CRecording found = new("Tagged", "https://example.test/gb.mp3", 0, true, "British");
        CRecording plain = new("Tagged", "https://example.test/any.mp3", 0, true, string.Empty);

        errand.TErrandHarvestResonate(new CHarvestStep("Tagged", 0, found, false));
        errand.TErrandHarvestResonate(
            new CHarvestStep("Tagged", 0, new CRecording("Tagged", null, 0, true, ""), false));
        errand.TErrandHarvestResonate(new CHarvestStep("Tagged", 0, plain, false));

        Assert.NotNull(shown);
        CClipItem row = Assert.Single(shown.CClipRollRows);
        Assert.True(row.CClipItemReady);
        Assert.Equal(string.Empty, row.CClipItemNotice);
        Assert.Equal(
            [
                new CClipReading(
                    found, CSounding.CSoundingVarietyRead(string.Empty, "British"), false, "Downloader.Use",
                    true, false, false, false),
                new CClipReading(
                    plain, CSounding.CSoundingVarietyRead(string.Empty, string.Empty), false, "Downloader.Use",
                    true, false, false, false),
            ],
            row.CClipItemReading);
        Assert.Equal(string.Empty, row.CClipItemReading[1].CClipReadingVariety.CVarietyEnsign);
    }

    [Fact]
    public void ErrandLookupResonate_ThreeSteps_RaisesOneEventEach()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CErrand errand = TInterfaceConduct.TDeskCreate(engine, "Input", TEnvoyFake.TEnvoyCreate(false, []))
            .CDeskErrand;
        List<string> notices = [];
        errand.CErrandLookupStarted += (source, order) => notices.Add($"source {source} {order}");
        errand.CErrandCandidateAdded += candidate => notices.Add($"candidate {candidate.CCandidatePhonetic}");
        errand.CErrandLookupFinished += () => notices.Add("end");
        CCandidate candidate = new("Wiktionary", "ˈhæpi", 1, true, "American", null);

        errand.CErrandLookupResonate(new CLookupStep("Wiktionary", 1, null, false));
        errand.CErrandLookupResonate(new CLookupStep("Wiktionary", 1, candidate, false));
        errand.CErrandLookupResonate(new CLookupStep(string.Empty, 0, null, true));

        Assert.Equal(["source Wiktionary 1", "candidate ˈhæpi", "end"], notices);
    }

    [Fact]
    public async Task ErrandRecordingSave_NoSearchRunning_AttachesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CErrand errand = TInterfaceConduct.TDeskCreate(engine, "Input", TEnvoyFake.TEnvoyCreate(false, []))
            .CDeskErrand;

        Assert.False(await errand.CErrandRecordingSave(
            new CRecording("Tagged", "https://example.test/gb.mp3", 0, true, "British")));
        Assert.Null(await errand.CErrandPreviewStart(
            new CRecording("Tagged", "https://example.test/gb.mp3", 0, true, "British")));
        Assert.False(errand.CErrandTranscriptionHeld);
        Assert.Equal(string.Empty, errand.CErrandTranscriptionLanguage);
        Assert.False(errand.CErrandTranscriptionFlagged);
        Assert.False(errand.CErrandTranscriptionPrimary);
        Assert.Equal(0, errand.CErrandTranscriptionTarget);
        Assert.Equal(string.Empty, errand.CErrandTranscriptionScheme);
        Assert.False(errand.CErrandTranscriptionSchemed);
    }

    private static CErrand TErrandCreate(LEngine engine) =>
        TInterfaceConduct.TDeskCreate(engine, "Input", TEnvoyFake.TEnvoyCreate(false, [])).CDeskErrand;
}
