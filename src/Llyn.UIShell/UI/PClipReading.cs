using System;
using System.ComponentModel;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PClipReading : INotifyPropertyChanged
{
    private readonly LRecording _lRecording;

    private string _pClipReadingAction;
    private bool _pClipReadingReady = true;

    internal PClipReading(LRecording recording, string label, ImageSource? flag, string action)
    {
        _lRecording = recording;
        PClipReadingVariety = recording.LRecordingVariety;
        PClipReadingLabel = label;
        PClipReadingFlag = flag;
        _pClipReadingAction = action;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string PClipReadingVariety { get; }

    public string PClipReadingLabel { get; }

    public ImageSource? PClipReadingFlag { get; }

    public string PClipReadingAction
    {
        get => _pClipReadingAction;
        set
        {
            if (string.Equals(_pClipReadingAction, value, StringComparison.Ordinal))
            {
                return;
            }

            _pClipReadingAction = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PClipReadingAction)));
        }
    }

    public bool PClipReadingReady
    {
        get => _pClipReadingReady;
        set
        {
            if (_pClipReadingReady == value)
            {
                return;
            }

            _pClipReadingReady = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PClipReadingReady)));
        }
    }

    internal LRecording PClipReadingModel => _lRecording;
}
