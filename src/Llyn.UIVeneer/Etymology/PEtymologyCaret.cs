using System;
using System.ComponentModel;

namespace Llyn.UIVeneer;

internal sealed class PEtymologyCaret : INotifyPropertyChanged
{
    private string _pEtymologyCaretText = string.Empty;

    public string PEtymologyCaretText
    {
        get => _pEtymologyCaretText;
        set
        {
            string written = value ?? string.Empty;
            if (string.Equals(_pEtymologyCaretText, written, StringComparison.Ordinal))
            {
                return;
            }

            _pEtymologyCaretText = written;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PEtymologyCaretText)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
