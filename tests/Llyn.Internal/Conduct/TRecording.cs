using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TRecording
{
    [Fact]
    public void RecordingAddressed_AddressHeld_IsTrue()
    {
        Assert.True(new CRecording("Oxford", "https://example.test/a.mp3", 0, true, string.Empty).CRecordingAddressed);
    }

    [Fact]
    public void RecordingAddressed_EmptyAddress_IsFalse()
    {
        Assert.False(new CRecording("Oxford", null, 0, true, string.Empty).CRecordingAddressed);
        Assert.False(new CRecording("Oxford", string.Empty, 0, true, string.Empty).CRecordingAddressed);
    }

    [Fact]
    public void RecordingRegional_VarietyNamed_IsTrue()
    {
        Assert.True(new CRecording("Oxford", null, 0, true, "British").CRecordingRegional);
        Assert.False(new CRecording("Oxford", null, 0, true, string.Empty).CRecordingRegional);
    }
}
