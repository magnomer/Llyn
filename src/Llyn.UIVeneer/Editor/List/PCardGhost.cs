using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Llyn.UIVeneer;

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
        _pCardGhostFace = new VisualBrush(card) { Stretch = Stretch.None };
        _pCardGhostLeft = left;
        _pCardGhostWidth = card.ActualWidth;
        _pCardGhostHeight = card.ActualHeight;
        IsHitTestVisible = false;
        Opacity = 0.62;
    }

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
