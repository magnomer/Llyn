using System.Windows;

namespace Llyn.UIDeportment;

public sealed class PMentionArgument : RoutedEventArgs
{
    internal PMentionArgument(RoutedEvent routed, PMention origin, string text, int unit)
        : base(routed, origin)
    {
        PMentionArgumentText = text;
        PMentionArgumentUnit = unit;
    }

    public string PMentionArgumentText { get; }

    public int PMentionArgumentUnit { get; }

    public PMention PMentionArgumentOrigin => (PMention)OriginalSource;

    public long PMentionArgumentSentence => PMentionArgumentOrigin.PMentionSentence;
}
