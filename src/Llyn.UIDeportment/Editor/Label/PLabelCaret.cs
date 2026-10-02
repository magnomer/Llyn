using System;
using System.ComponentModel;

namespace Llyn.UIDeportment;

internal sealed class PLabelCaret : INotifyPropertyChanged
{
    private string _pLabelCaretText = string.Empty;
    private string _pLabelCaretHint = string.Empty;
    private PLabelChip? _pLabelCaretAnchor;

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

    public PLabelChip? PLabelCaretAnchor
    {
        get => _pLabelCaretAnchor;
        set
        {
            if (ReferenceEquals(_pLabelCaretAnchor, value))
            {
                return;
            }

            _pLabelCaretAnchor = value;
            PLabelCaretRaise(nameof(PLabelCaretAnchor));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PLabelCaretRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
