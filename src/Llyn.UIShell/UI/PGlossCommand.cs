using System.Windows.Input;

namespace Llyn.UIShell;

public static class PGlossCommand
{
    public static RoutedCommand PGlossCommandRemoval { get; } = new(nameof(PGlossCommandRemoval), typeof(PGlossCommand));
}
