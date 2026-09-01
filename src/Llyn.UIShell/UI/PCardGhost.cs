using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Llyn.UIShell;

/// <summary>
/// The picture of a card that follows the pointer while that card is being dragged. The card itself
/// is left on screen and keeps its place in the list until the drag moves it, so what travels with
/// the pointer is this copy: the same card painted dimmed above the list, hit-testable by nothing so
/// the list underneath still hears where the pointer is.
/// </summary>
internal sealed class PCardGhost : Adorner
{
    private readonly Brush _pCardGhostFace;
    private readonly double _pCardGhostLeft;
    private readonly double _pCardGhostWidth;
    private readonly double _pCardGhostHeight;
    private double _pCardGhostTop;

    internal PCardGhost(UIElement list, FrameworkElement card, double left)
        : base(list)
    {
        // A brush of the live card rather than a snapshot of it: the card keeps its own look, and the
        // ghost is that look painted somewhere else.
        _pCardGhostFace = new VisualBrush(card) { Stretch = Stretch.None };
        _pCardGhostLeft = left;
        _pCardGhostWidth = card.ActualWidth;
        _pCardGhostHeight = card.ActualHeight;
        IsHitTestVisible = false;
        Opacity = 0.62;
    }

    /// <summary>
    /// Where the top of the ghost sits, in the coordinates of the list it is drawn over.
    /// </summary>
    internal double PCardGhostTop
    {
        set
        {
            if (_pCardGhostTop.Equals(value))
            {
                return;
            }

            _pCardGhostTop = value;
            InvalidateVisual();
        }
    }

    protected override void OnRender(DrawingContext drawing)
    {
        drawing.DrawRectangle(
            _pCardGhostFace,
            null,
            new Rect(_pCardGhostLeft, _pCardGhostTop, _pCardGhostWidth, _pCardGhostHeight));
    }
}
