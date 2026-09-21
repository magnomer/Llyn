using System.Windows.Input;

namespace Llyn.UIVeneer;

public static class PStemCommand
{
    public static RoutedCommand PStemCommandEntry { get; } =
        new(nameof(PStemCommandEntry), typeof(PStemCommand));
}
