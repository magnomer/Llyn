using System.Windows.Input;

namespace Llyn.UIDeportment;

public static class QAccentCommand
{
    public static RoutedCommand QAccentCommandAddition { get; } =
        new(nameof(QAccentCommandAddition), typeof(QAccentCommand));

    public static RoutedCommand QAccentCommandRemoval { get; } =
        new(nameof(QAccentCommandRemoval), typeof(QAccentCommand));

    public static RoutedCommand QAccentCommandNotation { get; } =
        new(nameof(QAccentCommandNotation), typeof(QAccentCommand));

    public static RoutedCommand QAccentCommandClip { get; } =
        new(nameof(QAccentCommandClip), typeof(QAccentCommand));

    public static RoutedCommand QAccentCommandPlayback { get; } =
        new(nameof(QAccentCommandPlayback), typeof(QAccentCommand));
}
