using System.Windows;
using Llyn.Core;

namespace Llyn.UIVeneer;

public sealed class PMentionArgument : RoutedEventArgs
{
    internal PMentionArgument(RoutedEvent routed, object source, int offset, LMentionPiece? piece)
        : base(routed, source)
    {
        PMentionArgumentOffset = offset;
        PMentionArgumentPiece = piece;
    }

    public int PMentionArgumentOffset { get; }

    public LMentionPiece? PMentionArgumentPiece { get; }
}
