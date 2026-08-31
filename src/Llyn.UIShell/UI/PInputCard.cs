using System.ComponentModel;

namespace Llyn.UIShell;

internal sealed class PInputCard : INotifyPropertyChanged
{
    private readonly string _prefix;
    private int _order;

    internal PInputCard(string prefix, int order)
    {
        _prefix = prefix;
        _order = order;
    }

    public int Order
    {
        get => _order;
        set
        {
            if (_order == value)
            {
                return;
            }

            _order = value;
            Raise(nameof(Order));
            Raise(nameof(Title));
        }
    }

    public string Title => $"{_prefix} {_order}";

    public event PropertyChangedEventHandler? PropertyChanged;

    private void Raise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
