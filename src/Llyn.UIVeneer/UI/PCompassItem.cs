using System.ComponentModel;
using System.Windows;

namespace Llyn.UIVeneer;

internal sealed class PCompassItem : INotifyPropertyChanged
{
    private bool _pCompassItemCurrent;

    internal PCompassItem(FrameworkElement target, string label, string number, int depth)
    {
        PCompassItemTarget = target;
        PCompassItemLabel = label;
        PCompassItemName = label;
        PCompassItemNumber = number;
        PCompassItemIndent = new Thickness(depth * 13, 0, 0, 0);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    internal FrameworkElement PCompassItemTarget { get; }

    public string PCompassItemLabel { get; }

    public string PCompassItemName { get; internal set; }

    public string PCompassItemNumber { get; }

    public Thickness PCompassItemIndent { get; }

    public bool PCompassItemCurrent
    {
        get => _pCompassItemCurrent;

        internal set
        {
            if (_pCompassItemCurrent == value)
            {
                return;
            }

            _pCompassItemCurrent = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PCompassItemCurrent)));
        }
    }
}
