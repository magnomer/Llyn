namespace Llyn.Conduct;

public sealed record CRecording(
    string CRecordingSource,
    string? CRecordingAddress,
    int CRecordingOrder,
    bool CRecordingReached,
    string CRecordingVariety)
{
    public bool CRecordingAddressed => !string.IsNullOrEmpty(CRecordingAddress);
}
