using System.Windows.Input;

namespace Llyn.UIDeportment;

public static class QReflexCommand
{
    public static RoutedCommand QReflexCommandAddition { get; } =
        new(nameof(QReflexCommandAddition), typeof(QReflexCommand));

    public static RoutedCommand QReflexCommandRemoval { get; } =
        new(nameof(QReflexCommandRemoval), typeof(QReflexCommand));

    public static RoutedCommand QReflexCommandMain { get; } =
        new(nameof(QReflexCommandMain), typeof(QReflexCommand));

    public static RoutedCommand QReflexCommandRenewal { get; } =
        new(nameof(QReflexCommandRenewal), typeof(QReflexCommand));

    public static RoutedCommand QReflexCommandAnchor { get; } =
        new(nameof(QReflexCommandAnchor), typeof(QReflexCommand));
}
