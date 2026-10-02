using System.Windows;

namespace Llyn.UIDeportment;

public sealed class PMentionArgument : RoutedEventArgs
{
    internal PMentionArgument(RoutedEvent routed, PMention origin, int offset)
        : base(routed, origin)
    {
        PMentionArgumentOffset = offset;
    }

    public int PMentionArgumentOffset { get; }

    public PMention PMentionArgumentOrigin => (PMention)OriginalSource;

    public long PMentionArgumentSentence => PMentionArgumentOrigin.PMentionSentence;
}
