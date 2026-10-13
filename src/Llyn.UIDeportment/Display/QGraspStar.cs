using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Llyn.UIDeportment;

public sealed class QGraspStar
{
    private const double QGraspStarSize = 16;

    private const double QGraspStarGap = 3;

    private const double QGraspHitSlack = 4;

    private readonly FrameworkElement _qGraspElement;

    private readonly DependencyProperty _qGraspStep;

    private readonly DependencyProperty _qGraspLimit;

    private readonly Action<int?> _qGraspHovered;

    private readonly Action<int> _qGraspPressed;

    private readonly Action<int> _qGraspMoved;

    private readonly ImageSource _qGraspStarImage;

    private IReadOnlyList<IReadOnlyList<int>> _qGraspFill = [];

    private IReadOnlyList<bool> _qGraspUnrated = [];

    private IReadOnlyList<IReadOnlyList<int>> _qGraspHit = [];

    public QGraspStar(
        FrameworkElement grasp,
        DependencyProperty step,
        DependencyProperty limit,
        Action<int?> hovered,
        Action<int> pressed,
        Action<int> moved,
        ImageSource? star)
    {
        _qGraspElement = grasp;
        _qGraspStep = step;
        _qGraspLimit = limit;
        _qGraspHovered = hovered ?? throw new ArgumentNullException(nameof(hovered));
        _qGraspPressed = pressed ?? throw new ArgumentNullException(nameof(pressed));
        _qGraspMoved = moved ?? throw new ArgumentNullException(nameof(moved));
        _qGraspStarImage = star ?? throw new ArgumentNullException(nameof(star));
        grasp.IsEnabledChanged += (_, _) => grasp.Opacity = grasp.IsEnabled ? 1 : 0.4;
    }

    private int? QGraspHover { get; set; }

    private int QGraspPointed => QGraspHover ?? QGraspCurrent;

    private int QGraspCurrent => (int)_qGraspElement.GetValue(_qGraspStep);

    private int QGraspLast => (int)_qGraspElement.GetValue(_qGraspLimit);

    public void QGraspStarRefine(
        IReadOnlyList<IReadOnlyList<int>> fill,
        IReadOnlyList<bool> unrated,
        IReadOnlyList<IReadOnlyList<int>> hit)
    {
        _qGraspFill = fill;
        _qGraspUnrated = unrated;
        _qGraspHit = hit;
        _qGraspElement.InvalidateMeasure();
        _qGraspElement.InvalidateVisual();
    }

    public Size QGraspSizeDraw()
    {
        double width = 2 * QGraspHitSlack;
        double gap = 0;
        foreach (IReadOnlyList<int> _ in _qGraspHit)
        {
            width += gap + QGraspStarSize;
            gap = QGraspStarGap;
        }

        return new Size(width, QGraspStarSize + 2 * QGraspHitSlack);
    }

    public void QGraspHoverRefine(Point point)
    {
        QGraspPreviewRefine(QGraspStepDraw(point));
    }

    public void QGraspLeaveRefine()
    {
        if (QGraspHover is not null)
        {
            QGraspPreviewRefine(null);
        }
    }

    public void QGraspPressRefine(Point point)
    {
        _qGraspElement.Focus();
        _qGraspPressed(QGraspStepDraw(point));
    }

    public void QGraspKeyRefine(KeyEventArgs e)
    {
        int? move = e.Key switch
        {
            Key.Left => -1,
            Key.Right => 1,
            Key.Home => -QGraspLast,
            Key.End => QGraspLast,
            _ => null,
        };

        if (move is not int step)
        {
            return;
        }

        _qGraspMoved(step);
        e.Handled = true;
    }

    public void QGraspDraw(DrawingContext context, Brush fill, Brush empty, Brush unrated, Brush preview)
    {
        int shown = QGraspPointed;
        IReadOnlyList<int> fills = _qGraspFill.ElementAtOrDefault(shown) ?? [];
        Brush back = empty;
        double faded = 0.55;
        if (_qGraspUnrated.ElementAtOrDefault(shown))
        {
            back = unrated;
            faded = 0.35;
        }

        Brush front = QGraspHover is null ? fill : preview;

        context.DrawRectangle(Brushes.Transparent, null, new Rect(_qGraspElement.RenderSize));
        context.PushTransform(new TranslateTransform(QGraspHitSlack, QGraspHitSlack));
        double pitch = QGraspStarSize + QGraspStarGap;
        Rect frame = new(0, 0, QGraspStarSize, QGraspStarSize);
        double left = 0;

        foreach (int filled in fills)
        {
            double? clip = filled switch
            {
                2 => QGraspStarSize,
                1 => QGraspStarSize / 2,
                _ => null,
            };
            context.PushTransform(new TranslateTransform(left, 0));
            context.PushOpacity(faded);
            QGraspStarDraw(context, back, frame);
            context.Pop();

            if (clip is double width)
            {
                context.PushClip(new RectangleGeometry(new Rect(0, 0, width, QGraspStarSize)));
                QGraspStarDraw(context, front, frame);
                context.Pop();
            }

            context.Pop();
            left += pitch;
        }

        context.Pop();
    }

    private void QGraspPreviewRefine(int? hovered)
    {
        QGraspHover = hovered;
        _qGraspElement.InvalidateVisual();
        _qGraspHovered(hovered);
    }

    private void QGraspStarDraw(DrawingContext context, Brush ink, Rect frame)
    {
        context.PushOpacityMask(new ImageBrush(_qGraspStarImage));
        context.DrawRectangle(ink, null, frame);
        context.Pop();
    }

    private int QGraspStepDraw(Point point)
    {
        double pitch = QGraspStarSize + QGraspStarGap;
        double x = point.X - QGraspHitSlack + QGraspStarGap / 2;
        double left = 0;
        int step = 0;
        foreach (IReadOnlyList<int> hit in _qGraspHit)
        {
            step = x - left < pitch / 2 ? hit[0] : hit[1];
            if (x < left + pitch)
            {
                break;
            }

            left += pitch;
        }

        return step;
    }
}
