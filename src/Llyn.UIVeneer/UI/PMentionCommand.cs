using System.Windows.Input;

namespace Llyn.UIVeneer;

public static class PMentionCommand
{
    public static RoutedCommand PMentionCommandLink { get; } =
        new(nameof(PMentionCommandLink), typeof(PMentionCommand));

    public static RoutedCommand PMentionCommandChoose { get; } =
        new(nameof(PMentionCommandChoose), typeof(PMentionCommand));

    public static RoutedCommand PMentionCommandSilence { get; } =
        new(nameof(PMentionCommandSilence), typeof(PMentionCommand));

    public static RoutedCommand PMentionCommandUnlink { get; } =
        new(nameof(PMentionCommandUnlink), typeof(PMentionCommand));
}
