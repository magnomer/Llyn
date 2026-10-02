using System.Windows.Input;

namespace Llyn.UIDeportment;

public static class QFanqieCommand
{
    public static RoutedCommand QFanqieCommandInitial { get; } =
        new(nameof(QFanqieCommandInitial), typeof(QFanqieCommand));

    public static RoutedCommand QFanqieCommandStem { get; } =
        new(nameof(QFanqieCommandStem), typeof(QFanqieCommand));

    public static RoutedCommand QFanqieCommandRime { get; } =
        new(nameof(QFanqieCommandRime), typeof(QFanqieCommand));

    public static RoutedCommand QFanqieCommandRepresentative { get; } =
        new(nameof(QFanqieCommandRepresentative), typeof(QFanqieCommand));
}
