using System;
using System.ComponentModel;

namespace Llyn.UIShell;

internal sealed class PTranslationEntry : INotifyPropertyChanged
{
    private string _pTranslationEntryText = string.Empty;
    private string _pTranslationEntryHint = string.Empty;

    public string PTranslationEntryText
    {
        get => _pTranslationEntryText;
        set
        {
            string written = value ?? string.Empty;
            if (string.Equals(_pTranslationEntryText, written, StringComparison.Ordinal))
            {
                return;
            }

            _pTranslationEntryText = written;
            PTranslationEntryRaise(nameof(PTranslationEntryText));
        }
    }

    public string PTranslationEntryHint
    {
        get => _pTranslationEntryHint;
        set
        {
            string shown = value ?? string.Empty;
            if (string.Equals(_pTranslationEntryHint, shown, StringComparison.Ordinal))
            {
                return;
            }

            _pTranslationEntryHint = shown;
            PTranslationEntryRaise(nameof(PTranslationEntryHint));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PTranslationEntryRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
