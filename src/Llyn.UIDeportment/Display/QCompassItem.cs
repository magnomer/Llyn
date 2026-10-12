using System.ComponentModel;
using System.Windows;

namespace Llyn.UIDeportment;

public sealed class QCompassItem : INotifyPropertyChanged
{
    private bool _qCompassItemCurrent;

    public QCompassItem(FrameworkElement target, string name, string number, int depth, bool muted)
    {
        QCompassItemTarget = target;
        QCompassItemName = name;
        QCompassItemNumber = number;
        QCompassItemIndent = new Thickness(depth * 13, 0, 0, 0);
        QCompassItemMuted = muted;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public FrameworkElement QCompassItemTarget { get; }

    public string QCompassItemName { get; }

    public string QCompassItemNumber { get; }

    public Thickness QCompassItemIndent { get; }

    public bool QCompassItemMuted { get; }

    public bool QCompassItemCurrent
    {
        get => _qCompassItemCurrent;

        set
        {
            if (_qCompassItemCurrent == value)
            {
                return;
            }

            _qCompassItemCurrent = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QCompassItemCurrent)));
        }
    }
}
