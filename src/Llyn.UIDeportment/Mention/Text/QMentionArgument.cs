using System.Windows;

namespace Llyn.UIDeportment;

public sealed class QMentionArgument : RoutedEventArgs
{
    internal QMentionArgument(RoutedEvent routed, QMention origin, string text, int unit)
        : base(routed, origin)
    {
        QMentionArgumentText = text;
        QMentionArgumentUnit = unit;
    }

    internal QMentionArgument(RoutedEvent routed, QMention origin)
        : base(routed, origin)
    {
        QMentionArgumentText = string.Empty;
    }

    public string QMentionArgumentText { get; }

    public int QMentionArgumentUnit { get; }

    public QMention QMentionArgumentOrigin => (QMention)OriginalSource;

    public long QMentionArgumentSentence => QMentionArgumentOrigin.QMentionSentence;
}
