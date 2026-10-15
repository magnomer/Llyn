using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Llyn.UIDeportment;

internal sealed class QCardGhost : Adorner
{
    private readonly Brush _qCardGhostFace;
    private readonly double _qCardGhostLeft;
    private readonly double _qCardGhostWidth;
    private readonly double _qCardGhostHeight;
    private double _qCardGhostTop;

    internal QCardGhost(UIElement list, FrameworkElement card, double left)
        : base(list)
    {
        _qCardGhostFace = new VisualBrush(card) { Stretch = Stretch.None };
        _qCardGhostLeft = left;
        _qCardGhostWidth = card.ActualWidth;
        _qCardGhostHeight = card.ActualHeight;
        IsHitTestVisible = false;
        Opacity = 0.62;
    }

    internal double QCardGhostTop
    {
        set
        {
            if (_qCardGhostTop.Equals(value))
            {
                return;
            }

            _qCardGhostTop = value;
            InvalidateVisual();
        }
    }

    protected override void OnRender(DrawingContext drawing)
    {
        drawing.DrawRectangle(
            _qCardGhostFace,
            null,
            new Rect(_qCardGhostLeft, _qCardGhostTop, _qCardGhostWidth, _qCardGhostHeight));
    }
}
