using Llyn.Conduct;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TDeskSeal
{
    [Fact]
    public void DeskRecordingRead_EngineRecording_CarriesEveryFieldBothWays()
    {
        LRecording recording = TInterface.TRecordingCreate("Oxford", "https://example.test/a.mp3", 2, false, "British");

        CRecording? mirror = TInterfaceDeportment.TDeskRecordingRead(recording);

        Assert.Equal(new CRecording("Oxford", "https://example.test/a.mp3", 2, false, "British"), mirror);
        Assert.Equal(recording, TInterfaceDeportment.TDeskRecordingRead(mirror!));
    }

    [Fact]
    public void DeskRecordingRead_NoRecording_ReturnsNone()
    {
        Assert.Null(TInterfaceDeportment.TDeskRecordingRead((LRecording?)null));
    }

    [Fact]
    public void DeskCandidateRead_EngineCandidate_CarriesEveryField()
    {
        LCandidate candidate = TInterface.TCandidateCreate("Wiktionary", "ˈhæpi", 1, true, "American");

        Assert.Equal(
            new CCandidate("Wiktionary", "ˈhæpi", 1, true, "American", null),
            TInterfaceDeportment.TDeskCandidateRead(candidate));
    }

    [Fact]
    public void DeskCandidateRead_NoCandidate_ReturnsNone()
    {
        Assert.Null(TInterfaceDeportment.TDeskCandidateRead(null));
    }
}
