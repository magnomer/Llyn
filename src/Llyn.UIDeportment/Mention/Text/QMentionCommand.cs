using System.Windows.Input;

namespace Llyn.UIDeportment;

public static class QMentionCommand
{
    public static RoutedCommand QMentionCommandLink { get; } =
        new(nameof(QMentionCommandLink), typeof(QMentionCommand));

    public static RoutedCommand QMentionCommandChoose { get; } =
        new(nameof(QMentionCommandChoose), typeof(QMentionCommand));

    public static RoutedCommand QMentionCommandSilence { get; } =
        new(nameof(QMentionCommandSilence), typeof(QMentionCommand));

    public static RoutedCommand QMentionCommandUnlink { get; } =
        new(nameof(QMentionCommandUnlink), typeof(QMentionCommand));

    public static RoutedCommand QMentionCommandPick { get; } =
        new(nameof(QMentionCommandPick), typeof(QMentionCommand));
}
