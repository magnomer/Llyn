using System.Collections.Generic;
using System.Windows;
using Llyn.Core;

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

    public IReadOnlyList<LMention>? PMentionArgumentMention => PMentionArgumentOrigin.PMentionMention;
}
