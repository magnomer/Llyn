using System;
using System.ComponentModel;

namespace Llyn.UIShell;

internal sealed class PTagEntry : INotifyPropertyChanged
{
    private string _pTagEntryText = string.Empty;
    private string _pTagEntryHint = string.Empty;

    public string PTagEntryText
    {
        get => _pTagEntryText;
        set
        {
            string written = value ?? string.Empty;
            if (string.Equals(_pTagEntryText, written, StringComparison.Ordinal))
            {
                return;
            }

            _pTagEntryText = written;
            PTagEntryRaise(nameof(PTagEntryText));
        }
    }

    public string PTagEntryHint
    {
        get => _pTagEntryHint;
        set
        {
            string shown = value ?? string.Empty;
            if (string.Equals(_pTagEntryHint, shown, StringComparison.Ordinal))
            {
                return;
            }

            _pTagEntryHint = shown;
            PTagEntryRaise(nameof(PTagEntryHint));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PTagEntryRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
