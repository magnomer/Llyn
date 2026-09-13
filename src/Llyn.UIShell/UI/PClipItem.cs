using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PClipItem : INotifyPropertyChanged
{
    private string _pClipItemNotice;
    private bool _pClipItemReady;

    internal PClipItem(string sourceLabel, int order, string notice)
    {
        PClipItemSource = sourceLabel;
        PClipItemOrder = order;
        _pClipItemNotice = notice;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string PClipItemSource { get; }

    internal int PClipItemOrder { get; }

    public ObservableCollection<PClipReading> PClipItemReading { get; } = [];

    public string PClipItemNotice
    {
        get => _pClipItemNotice;
        private set => PClipItemChange(ref _pClipItemNotice, value, nameof(PClipItemNotice));
    }

    public bool PClipItemReady
    {
        get => _pClipItemReady;
        private set => PClipItemChange(ref _pClipItemReady, value, nameof(PClipItemReady));
    }

    internal void PClipItemShow(LRecording recording, PClipReading? reading, string missing, string broken)
    {
        if (reading is not null)
        {
            PClipItemReading.Add(reading);
            PClipItemNotice = string.Empty;
            PClipItemReady = true;
            return;
        }

        if (PClipItemReading.Count > 0)
        {
            return;
        }

        PClipItemNotice = recording.LRecordingReached ? missing : broken;
        PClipItemReady = false;
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
