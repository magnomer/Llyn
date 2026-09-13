namespace Llyn.Core;

public sealed record LRecording(
    string LRecordingSource,
    string? LRecordingAddress,
    int LRecordingOrder,
    bool LRecordingReached,
    string LRecordingVariety);
