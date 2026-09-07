using System;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PNotationItem : INotifyPropertyChanged
{
    private string _pNotationItemReading = string.Empty;
    private string _pNotationItemNotice;
    private bool _pNotationItemReady;

    internal PNotationItem(string sourceLabel, int order, string notice)
    {
        PNotationItemSource = sourceLabel;
        PNotationItemOrder = order;
        _pNotationItemNotice = notice;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string PNotationItemSource { get; }

    internal int PNotationItemOrder { get; }

    public string PNotationItemReading
    {
        get => _pNotationItemReading;
        private set => PNotationItemChange(ref _pNotationItemReading, value, nameof(PNotationItemReading));
    }

    public string PNotationItemNotice
    {
        get => _pNotationItemNotice;
        private set => PNotationItemChange(ref _pNotationItemNotice, value, nameof(PNotationItemNotice));
    }

    public bool PNotationItemReady
    {
        get => _pNotationItemReady;
        private set => PNotationItemChange(ref _pNotationItemReady, value, nameof(PNotationItemReady));
    }

    internal void PNotationItemShow(LCandidate candidate, string missing, string broken)
    {
        if (!string.IsNullOrEmpty(candidate.LCandidatePhonetic))
        {
            PNotationItemReading = candidate.LCandidatePhonetic;
            PNotationItemNotice = string.Empty;
            PNotationItemReady = true;
            return;
        }

        PNotationItemReading = string.Empty;
        PNotationItemNotice = candidate.LCandidateReached ? missing : broken;
        PNotationItemReady = false;
    }

    private void PNotationItemChange(ref string held, string value, string name)
    {
        if (string.Equals(held, value, StringComparison.Ordinal))
        {
            return;
        }

        held = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private void PNotationItemChange(ref bool held, bool value, string name)
    {
        if (held == value)
        {
            return;
        }

        held = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
