using System.Windows.Input;

namespace Llyn.UIShell;

public static class PAccentCommand
{
    public static RoutedCommand PAccentCommandRemoval { get; } =
        new(nameof(PAccentCommandRemoval), typeof(PAccentCommand));
}
