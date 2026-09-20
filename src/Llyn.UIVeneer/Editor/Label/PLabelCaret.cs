using System;
using System.ComponentModel;

namespace Llyn.UIVeneer;

internal sealed class PLabelCaret : INotifyPropertyChanged
{
    private string _pLabelCaretText = string.Empty;
    private string _pLabelCaretHint = string.Empty;

    public string PLabelCaretText
    {
        get => _pLabelCaretText;
        set
        {
            string written = value ?? string.Empty;
            if (string.Equals(_pLabelCaretText, written, StringComparison.Ordinal))
            {
                return;
            }

            _pLabelCaretText = written;
            PLabelCaretRaise(nameof(PLabelCaretText));
        }
    }

    public string PLabelCaretHint
    {
        get => _pLabelCaretHint;
        set
        {
            string shown = value ?? string.Empty;
            if (string.Equals(_pLabelCaretHint, shown, StringComparison.Ordinal))
            {
                return;
            }

            _pLabelCaretHint = shown;
            PLabelCaretRaise(nameof(PLabelCaretHint));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PLabelCaretRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
