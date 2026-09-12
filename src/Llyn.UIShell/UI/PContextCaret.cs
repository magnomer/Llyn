using System;
using System.ComponentModel;

namespace Llyn.UIShell;

internal sealed class PContextCaret : INotifyPropertyChanged
{
    private string _pContextCaretText = string.Empty;
    private string _pContextCaretHint = string.Empty;

    internal long PContextCaretId { get; set; }

    public string PContextCaretText
    {
        get => _pContextCaretText;
        set
        {
            string written = value ?? string.Empty;
            if (string.Equals(_pContextCaretText, written, StringComparison.Ordinal))
            {
                return;
            }

            _pContextCaretText = written;
            PContextCaretRaise(nameof(PContextCaretText));
        }
    }

    public string PContextCaretHint
    {
        get => _pContextCaretHint;
        set
        {
            string shown = value ?? string.Empty;
            if (string.Equals(_pContextCaretHint, shown, StringComparison.Ordinal))
            {
                return;
            }

            _pContextCaretHint = shown;
            PContextCaretRaise(nameof(PContextCaretHint));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PContextCaretRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
