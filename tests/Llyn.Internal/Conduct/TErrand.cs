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
        CDesk desk = TInterfaceConduct.TDeskCreate(engine, "Input", TInterfaceConduct.TEnvoyCreate(false, []));

        Assert.False(desk.CDeskErrand.CErrandRecordingStart("happy", 0, static _ => { }));
        Assert.False(desk.CDeskErrand.CErrandTranscriptionStart("happy", 0, "ipa", static _ => { }));
        desk.CDeskErrand.CErrandCancel();
    }
}
