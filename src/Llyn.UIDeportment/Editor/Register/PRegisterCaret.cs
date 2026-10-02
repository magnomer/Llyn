using System;
using System.ComponentModel;

namespace Llyn.UIDeportment;

internal sealed class PRegisterCaret : INotifyPropertyChanged
{
    private string _pRegisterCaretText = string.Empty;
    private string _pRegisterCaretHint = string.Empty;
    private PRegister? _pRegisterCaretAnchor;

    public string PRegisterCaretText
    {
        get => _pRegisterCaretText;
        set
        {
            string written = value ?? string.Empty;
            if (string.Equals(_pRegisterCaretText, written, StringComparison.Ordinal))
            {
                return;
            }

            _pRegisterCaretText = written;
            PRegisterCaretRaise(nameof(PRegisterCaretText));
        }
    }

    public string PRegisterCaretHint
    {
        get => _pRegisterCaretHint;
        set
        {
            string shown = value ?? string.Empty;
            if (string.Equals(_pRegisterCaretHint, shown, StringComparison.Ordinal))
            {
                return;
            }

            _pRegisterCaretHint = shown;
            PRegisterCaretRaise(nameof(PRegisterCaretHint));
        }
    }

    public PRegister? PRegisterCaretAnchor
    {
        get => _pRegisterCaretAnchor;
        set
        {
            if (ReferenceEquals(_pRegisterCaretAnchor, value))
            {
                return;
            }

            _pRegisterCaretAnchor = value;
            PRegisterCaretRaise(nameof(PRegisterCaretAnchor));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PRegisterCaretRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
