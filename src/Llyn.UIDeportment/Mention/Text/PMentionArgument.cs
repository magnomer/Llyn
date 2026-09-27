using System.Collections.Generic;
using System.Windows;
using Llyn.Conduct;

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

    public string PMentionArgumentText => PMentionArgumentOrigin.PMentionText;

    public string PMentionArgumentLanguage => PMentionArgumentOrigin.PMentionLanguage;

    public IReadOnlyList<CMention>? PMentionArgumentMention => PMentionArgumentOrigin.PMentionMention;
}
