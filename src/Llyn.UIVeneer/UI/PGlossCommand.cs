using System.Windows.Input;

namespace Llyn.UIVeneer;

public static class PGlossCommand
{
    public static RoutedCommand PGlossCommandRemoval { get; } =
        new(nameof(PGlossCommandRemoval), typeof(PGlossCommand));
}
