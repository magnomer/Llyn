using System.ComponentModel;

namespace Llyn.UIDeportment;

internal sealed class QLedgerItem : INotifyPropertyChanged
{
    private string _pLedgerItemTitle;

    private string _pLedgerItemMeta = string.Empty;

    private bool _pLedgerItemChosen;

    internal QLedgerItem(string child, string title)
    {
        QLedgerItemChild = child;
        _pLedgerItemTitle = title;
    }

    public string QLedgerItemChild { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string QLedgerItemTitle
    {
        get => _pLedgerItemTitle;

        set
        {
            if (string.Equals(_pLedgerItemTitle, value, System.StringComparison.Ordinal))
            {
                return;
            }

            _pLedgerItemTitle = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QLedgerItemTitle)));
        }
    }

    public string QLedgerItemMeta
    {
        get => _pLedgerItemMeta;

        set
        {
            if (string.Equals(_pLedgerItemMeta, value, System.StringComparison.Ordinal))
            {
                return;
            }

            _pLedgerItemMeta = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QLedgerItemMeta)));
        }
    }

    public bool QLedgerItemChosen
    {
        get => _pLedgerItemChosen;

        set
        {
            if (_pLedgerItemChosen == value)
            {
                return;
            }

            _pLedgerItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QLedgerItemChosen)));
        }
    }
}
