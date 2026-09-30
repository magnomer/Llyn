using System.Collections.Generic;
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
    public void ErrandStart_NoTenureHeld_StartsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TInterfaceConduct.TDeskCreate(engine, "Input", TEnvoyFake.TEnvoyCreate(false, []));

        Assert.False(desk.CDeskErrand.CErrandRecordingStart("happy", 0, static _ => { }));
        Assert.False(desk.CDeskErrand.CErrandTranscriptionStart("happy", 0, "ipa", static _ => { }));
        desk.CDeskErrand.CErrandCancel();
    }

    [Fact]
    public void ErrandHarvestResonate_ThreeSteps_RaisesOneEventEach()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CErrand errand = TInterfaceConduct.TDeskCreate(engine, "Input", TEnvoyFake.TEnvoyCreate(false, []))
            .CDeskErrand;
        List<string> notices = [];
        errand.CErrandHarvestStarted += (source, order) => notices.Add($"source {source} {order}");
        errand.CErrandRecordingAdded += recording => notices.Add($"recording {recording.CRecordingAddress}");
        errand.CErrandHarvestFinished += () => notices.Add("end");
        CRecording recording = new("Tagged", "https://example.test/gb.mp3", 0, true, "British");

        errand.CErrandHarvestResonate(new CHarvestStep("Tagged", 0, null, false));
        errand.CErrandHarvestResonate(new CHarvestStep("Tagged", 0, recording, false));
        errand.CErrandHarvestResonate(new CHarvestStep(string.Empty, 0, null, true));

        Assert.Equal(["source Tagged 0", "recording https://example.test/gb.mp3", "end"], notices);
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
        Assert.False(errand.CErrandRecordingHeld);
        Assert.Equal(string.Empty, errand.CErrandRecordingLanguage);
        Assert.False(errand.CErrandRecordingFlagged);
        Assert.False(errand.CErrandRecordingPrimary);
        Assert.Equal(0, errand.CErrandRecordingTarget);
        Assert.False(errand.CErrandTranscriptionHeld);
        Assert.Equal(string.Empty, errand.CErrandTranscriptionLanguage);
        Assert.False(errand.CErrandTranscriptionFlagged);
        Assert.False(errand.CErrandTranscriptionPrimary);
        Assert.Equal(0, errand.CErrandTranscriptionTarget);
        Assert.Equal(string.Empty, errand.CErrandTranscriptionScheme);
        Assert.False(errand.CErrandTranscriptionSchemed);
    }
}
