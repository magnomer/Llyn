using System.Windows.Input;

namespace Llyn.UIDeportment;

public static class QDisplayCommand
{
    public static RoutedCommand QDisplayCommandPortrait { get; } =
        new(nameof(QDisplayCommandPortrait), typeof(QDisplayCommand));
}
