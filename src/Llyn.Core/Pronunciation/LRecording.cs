namespace Llyn.Core;

public sealed record LRecording(
    string LRecordingSource,
    string? LRecordingAddress,
    int LRecordingOrder,
    bool LRecordingReached,
    string LRecordingVariety)
{
    public bool LRecordingAddressed => !string.IsNullOrEmpty(LRecordingAddress);

    public bool LRecordingRegional => LRecordingVariety.Length > 0;
}
