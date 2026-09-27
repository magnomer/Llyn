using System.Windows.Input;

namespace Llyn.UIDeportment;

internal static class QDiweiCommand
{
    public static RoutedCommand QDiweiCommandEntry { get; } =
        new(nameof(QDiweiCommandEntry), typeof(QDiweiCommand));

    public static RoutedCommand QDiweiCommandSwitch { get; } =
        new(nameof(QDiweiCommandSwitch), typeof(QDiweiCommand));
}
