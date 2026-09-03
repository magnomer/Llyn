using System;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PClipItem : INotifyPropertyChanged
{
    private readonly LRecording _lRecording;

    private string _pClipItemAction;
    private bool _pClipItemReady = true;

    internal PClipItem(LRecording recording, string action)
    {
        _lRecording = recording;
        _pClipItemAction = action;
        PClipItemSource = recording.LRecordingSource;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string PClipItemSource { get; }

    public string PClipItemAction
    {
        get => _pClipItemAction;
        set
        {
            if (string.Equals(_pClipItemAction, value, StringComparison.Ordinal))
            {
                return;
            }

            _pClipItemAction = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PClipItemAction)));
        }
    }

    public bool PClipItemReady
    {
        get => _pClipItemReady;
        set
        {
            if (_pClipItemReady == value)
            {
                return;
            }

            _pClipItemReady = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PClipItemReady)));
        }
    }

    internal LRecording PClipItemModel => _lRecording;
}
