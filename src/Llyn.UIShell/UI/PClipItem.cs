using System;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PClipItem : INotifyPropertyChanged
{
    private LRecording? _lRecording;

    private string _pClipItemAction;
    private string _pClipItemNotice;
    private bool _pClipItemFound;
    private bool _pClipItemReady = true;

    internal PClipItem(string sourceLabel, int order, string action, string notice)
    {
        PClipItemSource = sourceLabel;
        PClipItemOrder = order;
        _pClipItemAction = action;
        _pClipItemNotice = notice;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string PClipItemSource { get; }

    internal int PClipItemOrder { get; }

    public string PClipItemAction
    {
        get => _pClipItemAction;
        set => PClipItemChange(ref _pClipItemAction, value, nameof(PClipItemAction));
    }

    public string PClipItemNotice
    {
        get => _pClipItemNotice;
        private set => PClipItemChange(ref _pClipItemNotice, value, nameof(PClipItemNotice));
    }

    public bool PClipItemFound
    {
        get => _pClipItemFound;
        private set => PClipItemChange(ref _pClipItemFound, value, nameof(PClipItemFound));
    }

    public bool PClipItemReady
    {
        get => _pClipItemReady;
        set => PClipItemChange(ref _pClipItemReady, value, nameof(PClipItemReady));
    }

    internal LRecording PClipItemModel => _lRecording ?? throw new InvalidOperationException();

    internal void PClipItemShow(LRecording recording, string missing, string broken)
    {
        if (!string.IsNullOrEmpty(recording.LRecordingAddress))
        {
            _lRecording = recording;
            PClipItemNotice = string.Empty;
            PClipItemFound = true;
            return;
        }

        _lRecording = null;
        PClipItemNotice = recording.LRecordingReached ? missing : broken;
        PClipItemFound = false;
    }

    private void PClipItemChange(ref string held, string value, string name)
    {
        if (string.Equals(held, value, StringComparison.Ordinal))
        {
            return;
        }

        held = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private void PClipItemChange(ref bool held, bool value, string name)
    {
        if (held == value)
        {
            return;
        }

        held = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
