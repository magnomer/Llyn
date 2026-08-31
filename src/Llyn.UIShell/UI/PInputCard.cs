using System.ComponentModel;

namespace Llyn.UIShell;

internal sealed class PInputCard : INotifyPropertyChanged
{
    private readonly string _pInputCardPrefix;
    private int _pInputCardOrder;

    internal PInputCard(string prefix, int order)
    {
        _pInputCardPrefix = prefix;
        _pInputCardOrder = order;
    }

    public int PInputCardOrder
    {
        get => _pInputCardOrder;
        set
        {
            if (_pInputCardOrder == value)
            {
                return;
            }

            _pInputCardOrder = value;
            PInputCardRaise(nameof(PInputCardOrder));
            PInputCardRaise(nameof(PInputCardTitle));
        }
    }

    public string PInputCardTitle => $"{_pInputCardPrefix} {_pInputCardOrder}";

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PInputCardRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
