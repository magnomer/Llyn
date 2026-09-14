using System.ComponentModel;

namespace Llyn.UIShell;

internal sealed class PLedgerItem : INotifyPropertyChanged
{
    private string _pLedgerItemMeta = string.Empty;

    private bool _pLedgerItemChosen;

    internal PLedgerItem(string child, string title)
    {
        PLedgerItemChild = child;
        PLedgerItemTitle = title;
    }

    public string PLedgerItemChild { get; }

    public string PLedgerItemTitle { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

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
