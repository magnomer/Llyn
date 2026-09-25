using System.ComponentModel;
using System.Windows;

namespace Llyn.UIDeportment;

public sealed class LCompassItem : INotifyPropertyChanged
{
    private bool _lCompassItemCurrent;

    public LCompassItem(FrameworkElement target, string label, string number, int depth)
    {
        LCompassItemTarget = target;
        LCompassItemLabel = label;
        LCompassItemName = label;
        LCompassItemNumber = number;
        LCompassItemIndent = new Thickness(depth * 13, 0, 0, 0);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public FrameworkElement LCompassItemTarget { get; }

    public string LCompassItemLabel { get; }

    public string LCompassItemName { get; set; }

    public string LCompassItemNumber { get; }

    public Thickness LCompassItemIndent { get; }

    public bool LCompassItemCurrent
    {
        get => _lCompassItemCurrent;

        set
        {
            if (_lCompassItemCurrent == value)
            {
                return;
            }

            _lCompassItemCurrent = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LCompassItemCurrent)));
        }
    }
}
