using System.Windows.Input;

namespace Llyn.UIDeportment;

public static class PDisplayCommand
{
    public static RoutedCommand PDisplayCommandPortrait { get; } =
        new(nameof(PDisplayCommandPortrait), typeof(PDisplayCommand));
}
