using System.Windows;

namespace Llyn.UIVeneer;

public sealed class PMentionArgument : RoutedEventArgs
{
    internal PMentionArgument(RoutedEvent routed, object source, int offset)
        : base(routed, source)
    {
        PMentionArgumentOffset = offset;
    }

    public int PMentionArgumentOffset { get; }
}
