using System;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

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
