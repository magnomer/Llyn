using System.Windows.Input;

namespace Llyn.UIVeneer;

public static class PEtymologyCommand
{
    public static RoutedCommand PEtymologyCommandLink { get; } =
        new(nameof(PEtymologyCommandLink), typeof(PEtymologyCommand));

    public static RoutedCommand PEtymologyCommandUnlink { get; } =
        new(nameof(PEtymologyCommandUnlink), typeof(PEtymologyCommand));

    public static RoutedCommand PEtymologyCommandAddition { get; } =
        new(nameof(PEtymologyCommandAddition), typeof(PEtymologyCommand));

    public static RoutedCommand PEtymologyCommandRemoval { get; } =
        new(nameof(PEtymologyCommandRemoval), typeof(PEtymologyCommand));

    public static RoutedCommand PEtymologyCommandEntry { get; } =
        new(nameof(PEtymologyCommandEntry), typeof(PEtymologyCommand));
}
