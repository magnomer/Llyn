using System.Windows.Input;

namespace Llyn.UIDeportment;

public static class PFanqieCommand
{
    public static RoutedCommand PFanqieCommandInitial { get; } =
        new(nameof(PFanqieCommandInitial), typeof(PFanqieCommand));

    public static RoutedCommand PFanqieCommandStem { get; } =
        new(nameof(PFanqieCommandStem), typeof(PFanqieCommand));

    public static RoutedCommand PFanqieCommandRime { get; } =
        new(nameof(PFanqieCommandRime), typeof(PFanqieCommand));

    public static RoutedCommand PFanqieCommandRepresentative { get; } =
        new(nameof(PFanqieCommandRepresentative), typeof(PFanqieCommand));
}
