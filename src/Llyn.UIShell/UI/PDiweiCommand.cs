using System.Windows.Input;

namespace Llyn.UIShell;

public static class PDiweiCommand
{
    public static RoutedCommand PDiweiCommandEntry { get; } =
        new(nameof(PDiweiCommandEntry), typeof(PDiweiCommand));
}
