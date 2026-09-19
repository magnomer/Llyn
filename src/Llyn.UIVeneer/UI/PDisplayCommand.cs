using System.Windows.Input;

namespace Llyn.UIVeneer;

public static class PDisplayCommand
{
    public static RoutedCommand PDisplayCommandPortrait { get; } =
        new(nameof(PDisplayCommandPortrait), typeof(PDisplayCommand));
}
