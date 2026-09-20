using System;
using System.ComponentModel;

namespace Llyn.UIVeneer;

internal sealed class PLinkCaret : INotifyPropertyChanged
{
    private string _pLinkCaretText = string.Empty;
    private string _pLinkCaretHint = string.Empty;

    public string PLinkCaretText
    {
        get => _pLinkCaretText;
        set
        {
            string written = value ?? string.Empty;
            if (string.Equals(_pLinkCaretText, written, StringComparison.Ordinal))
            {
                return;
            }

            _pLinkCaretText = written;
            PLinkCaretRaise(nameof(PLinkCaretText));
        }
    }

    public string PLinkCaretHint
    {
        get => _pLinkCaretHint;
        set
        {
            string shown = value ?? string.Empty;
            if (string.Equals(_pLinkCaretHint, shown, StringComparison.Ordinal))
            {
                return;
            }

            _pLinkCaretHint = shown;
            PLinkCaretRaise(nameof(PLinkCaretHint));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PLinkCaretRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
