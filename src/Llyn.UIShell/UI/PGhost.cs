using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace Llyn.UIShell;

/// <summary>
/// Floating preview shown while a sense or collocation card is dragged. The card is rasterized once
/// when the drag starts, and the snapshot follows the pointer above the window content until the
/// drag ends, so the dragged contents stay visible for the whole drag.
/// </summary>
internal sealed class PGhost : Adorner
{
    private readonly Image _pGhostImage;
    private readonly Size _pGhostSize;
    private Point _pGhostPoint;

    internal PGhost(FrameworkElement adornedElement, FrameworkElement card)
        : base(adornedElement)
    {
        IsHitTestVisible = false;
        _pGhostSize = new Size(card.ActualWidth, card.ActualHeight);
        _pGhostImage = new Image
        {
            Source = PGhostCopy(card),
            Width = _pGhostSize.Width,
            Height = _pGhostSize.Height,
            Opacity = 0.85,
            Effect = new DropShadowEffect { BlurRadius = 24, ShadowDepth = 8, Opacity = 0.3 }
        };

        AddVisualChild(_pGhostImage);
        AddLogicalChild(_pGhostImage);
    }

    internal void PGhostPlace(Point point)
    {
        _pGhostPoint = point;
        InvalidateArrange();
    }

    protected override int VisualChildrenCount => 1;

    protected override Visual GetVisualChild(int index) => _pGhostImage;

    protected override Size MeasureOverride(Size constraint)
    {
        _pGhostImage.Measure(_pGhostSize);
        return constraint;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _pGhostImage.Arrange(new Rect(_pGhostPoint, _pGhostSize));
        return finalSize;
    }

    // RenderTargetBitmap sizes are device pixels, so the capture runs at the card's own DPI to keep
    // the snapshot sharp on high-DPI displays.
    private static ImageSource PGhostCopy(FrameworkElement card)
    {
        DpiScale dpi = VisualTreeHelper.GetDpi(card);
        RenderTargetBitmap bitmap = new(
            (int)Math.Ceiling(card.ActualWidth * dpi.DpiScaleX),
            (int)Math.Ceiling(card.ActualHeight * dpi.DpiScaleY),
            dpi.DpiScaleX * 96,
            dpi.DpiScaleY * 96,
            PixelFormats.Pbgra32);
        bitmap.Render(card);
        bitmap.Freeze();
        return bitmap;
    }
}
