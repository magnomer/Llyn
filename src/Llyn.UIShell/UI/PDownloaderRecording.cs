using Llyn.Core;

namespace Llyn.UIShell;

/// <summary>
/// Presentation item for one downloadable recording shown in the downloader menu. Wraps the domain
/// <see cref="LRecording"/> with the source label the menu row displays, and can hand the domain
/// record back when the row is played or applied.
/// </summary>
internal sealed class PDownloaderRecording
{
    private readonly LRecording _lRecording;

    internal PDownloaderRecording(LRecording recording)
    {
        _lRecording = recording;
        PDownloaderRecordingSource = recording.LRecordingSource;
        PDownloaderRecordingAddress = recording.LRecordingAddress;
    }

    public string PDownloaderRecordingSource { get; }

    public string PDownloaderRecordingAddress { get; }

    internal LRecording PDownloaderRecordingModel => _lRecording;
}
