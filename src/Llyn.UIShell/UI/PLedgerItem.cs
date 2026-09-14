using System.ComponentModel;

namespace Llyn.UIShell;

internal sealed class PLedgerItem : INotifyPropertyChanged
{
    private string _pLedgerItemTitle;

    private string _pLedgerItemMeta = string.Empty;

    private bool _pLedgerItemChosen;

    internal PLedgerItem(string child, string title)
    {
        PLedgerItemChild = child;
        _pLedgerItemTitle = title;
    }

    public string PLedgerItemChild { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string PLedgerItemTitle
    {
        get => _pLedgerItemTitle;

        set
        {
            if (string.Equals(_pLedgerItemTitle, value, System.StringComparison.Ordinal))
            {
                return;
            }

            _pLedgerItemTitle = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PLedgerItemTitle)));
        }
    }

    public string PLedgerItemMeta
    {
        get => _pLedgerItemMeta;

        set
        {
            if (string.Equals(_pLedgerItemMeta, value, System.StringComparison.Ordinal))
            {
                return;
            }

            _pLedgerItemMeta = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PLedgerItemMeta)));
        }
    }

    public bool PLedgerItemChosen
    {
        get => _pLedgerItemChosen;

        set
        {
            if (_pLedgerItemChosen == value)
            {
                return;
            }

            _pLedgerItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PLedgerItemChosen)));
        }
    }
}
