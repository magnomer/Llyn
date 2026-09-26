using System.Windows.Input;

namespace Llyn.UIDeportment;

public static class PStemCommand
{
    public static RoutedCommand PStemCommandEntry { get; } =
        new(nameof(PStemCommandEntry), typeof(PStemCommand));
}
