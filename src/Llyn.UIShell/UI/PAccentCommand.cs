using System.Windows.Input;

namespace Llyn.UIShell;

public static class PAccentCommand
{
    public static RoutedCommand PAccentCommandAddition { get; } =
        new(nameof(PAccentCommandAddition), typeof(PAccentCommand));

    public static RoutedCommand PAccentCommandRemoval { get; } =
        new(nameof(PAccentCommandRemoval), typeof(PAccentCommand));

    public static RoutedCommand PAccentCommandNotation { get; } =
        new(nameof(PAccentCommandNotation), typeof(PAccentCommand));

    public static RoutedCommand PAccentCommandClip { get; } =
        new(nameof(PAccentCommandClip), typeof(PAccentCommand));

    public static RoutedCommand PAccentCommandPlayback { get; } =
        new(nameof(PAccentCommandPlayback), typeof(PAccentCommand));
}
