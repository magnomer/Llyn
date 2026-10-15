using System.Windows.Input;

namespace Llyn.UIDeportment;

public static class QGlossCommand
{
    public static RoutedCommand QGlossCommandRemoval { get; } =
        new(nameof(QGlossCommandRemoval), typeof(QGlossCommand));
}
