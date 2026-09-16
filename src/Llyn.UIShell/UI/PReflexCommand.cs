using System.Windows.Input;

namespace Llyn.UIShell;

public static class PReflexCommand
{
    public static RoutedCommand PReflexCommandAddition { get; } =
        new(nameof(PReflexCommandAddition), typeof(PReflexCommand));

    public static RoutedCommand PReflexCommandRemoval { get; } =
        new(nameof(PReflexCommandRemoval), typeof(PReflexCommand));

    public static RoutedCommand PReflexCommandMain { get; } =
        new(nameof(PReflexCommandMain), typeof(PReflexCommand));

    public static RoutedCommand PReflexCommandRenewal { get; } =
        new(nameof(PReflexCommandRenewal), typeof(PReflexCommand));

    public static RoutedCommand PReflexCommandAnchor { get; } =
        new(nameof(PReflexCommandAnchor), typeof(PReflexCommand));
}
