namespace Llyn.Conduct;

public sealed record CClipReading(
    CRecording CClipReadingRecording,
    CVariety CClipReadingVariety,
    bool CClipReadingFlagged,
    string CClipReadingAction,
    bool CClipReadingReady,
    bool CClipReadingFetching,
    bool CClipReadingPlaying,
    bool CClipReadingRefused);
