using System;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

/// <summary>
/// Presentation item for one downloadable recording shown in the downloader menu. Wraps the domain
/// <see cref="LRecording"/> with the source label the row shows, and with that row's own download
/// state, so the menu can report a save on the row that was taken rather than on the one status line
/// the search owns.
/// </summary>
internal sealed class PDownloaderRecording : INotifyPropertyChanged
{
    private readonly LRecording _lRecording;

    private string _pDownloaderRecordingAction;
    private bool _pDownloaderRecordingReady = true;

    internal PDownloaderRecording(LRecording recording, string action)
    {
        _lRecording = recording;
        _pDownloaderRecordingAction = action;
        PDownloaderRecordingSource = recording.LRecordingSource;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string PDownloaderRecordingSource { get; }

    /// <summary>The label the row's taking button shows: what it offers, or how its download went.</summary>
    public string PDownloaderRecordingAction
    {
        get => _pDownloaderRecordingAction;
        set
        {
            if (string.Equals(_pDownloaderRecordingAction, value, StringComparison.Ordinal))
            {
                return;
            }

            _pDownloaderRecordingAction = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PDownloaderRecordingAction)));
        }
    }

    /// <summary>Whether the row's taking button still offers a download: false while one runs, and
    /// after one has been saved.</summary>
    public bool PDownloaderRecordingReady
    {
        get => _pDownloaderRecordingReady;
        set
        {
            if (_pDownloaderRecordingReady == value)
            {
                return;
            }

            _pDownloaderRecordingReady = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PDownloaderRecordingReady)));
        }
    }

    internal LRecording PDownloaderRecordingModel => _lRecording;
}
