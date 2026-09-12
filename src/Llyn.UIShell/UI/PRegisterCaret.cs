using System;
using System.ComponentModel;

namespace Llyn.UIShell;

internal sealed class PRegisterCaret : INotifyPropertyChanged
{
    private string _pRegisterCaretText = string.Empty;
    private string _pRegisterCaretHint = string.Empty;

    internal long PRegisterCaretId { get; set; }

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

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PRegisterCaretRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
