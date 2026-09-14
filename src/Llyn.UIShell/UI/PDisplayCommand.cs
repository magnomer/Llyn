using System.Windows.Input;

namespace Llyn.UIShell;

public static class PDisplayCommand
{
    public static RoutedCommand PDisplayCommandPortrait { get; } =
        new(nameof(PDisplayCommandPortrait), typeof(PDisplayCommand));
}
