using System;
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

    private readonly RoutedEvent _qGraspChanged;

    private readonly RoutedEvent _qGraspHovered;

    private readonly ImageSource _qGraspStarImage;

    private readonly ImageSource _qGraspGrayImage;

    public QGraspStar(
        FrameworkElement grasp,
        DependencyProperty step,
        DependencyProperty limit,
        RoutedEvent changed,
        RoutedEvent hovered,
        ImageSource? star,
        ImageSource? gray)
    {
        _qGraspElement = grasp;
        _qGraspStep = step;
        _qGraspLimit = limit;
        _qGraspChanged = changed;
        _qGraspHovered = hovered;
        _qGraspStarImage = star ?? throw new ArgumentNullException(nameof(star));
        _qGraspGrayImage = gray ?? throw new ArgumentNullException(nameof(gray));
        grasp.IsEnabledChanged += (_, _) => grasp.Opacity = grasp.IsEnabled ? 1 : 0.4;
    }

    public int? QGraspHover { get; private set; }

    public int QGraspPointed => QGraspHover ?? QGraspCurrent;

    private int QGraspCurrent => (int)_qGraspElement.GetValue(_qGraspStep);

    private int QGraspLast => (int)_qGraspElement.GetValue(_qGraspLimit);

    private int QGraspStarCount => QGraspLast / 2;

    public static object QGraspLimitDraw(DependencyObject sender, object value, DependencyProperty limit)
    {
        return Math.Min((int)value, (int)sender.GetValue(limit));
    }

    public Size QGraspSizeDraw()
    {
        return new Size(
            QGraspStarCount * QGraspStarSize + (QGraspStarCount - 1) * QGraspStarGap + 2 * QGraspHitSlack,
            QGraspStarSize + 2 * QGraspHitSlack);
    }

    public void QGraspHoverRefine(Point point)
    {
        int hovered = QGraspStepDraw(point);
        if (QGraspHover != hovered)
        {
            QGraspPreviewRefine(hovered);
        }
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
        QGraspStepRefine(QGraspStepDraw(point));
    }

    public void QGraspKeyRefine(KeyEventArgs e)
    {
        int? next = e.Key switch
        {
            Key.Left => Math.Max(0, QGraspCurrent - 1),
            Key.Right => Math.Min(QGraspLast, QGraspCurrent + 1),
            Key.Home => 0,
            Key.End => QGraspLast,
            _ => null,
        };

        if (next is not int step)
        {
            return;
        }

        if (step != QGraspCurrent)
        {
            QGraspStepRefine(step);
        }

        e.Handled = true;
    }

    public void QGraspDraw(DrawingContext context)
    {
        int shown = QGraspPointed;

        context.DrawRectangle(Brushes.Transparent, null, new Rect(_qGraspElement.RenderSize));
        context.PushTransform(new TranslateTransform(QGraspHitSlack, QGraspHitSlack));
        double pitch = QGraspStarSize + QGraspStarGap;
        Rect frame = new(0, 0, QGraspStarSize, QGraspStarSize);

        for (int star = 0; star < QGraspStarCount; star++)
        {
            int filled = Math.Clamp(shown - star * 2, 0, 2);
            context.PushTransform(new TranslateTransform(star * pitch, 0));
            context.PushOpacity(shown == 0 ? 0.35 : 0.55);
            context.DrawImage(_qGraspGrayImage, frame);
            context.Pop();

            if (filled > 0)
            {
                context.PushClip(new RectangleGeometry(
                    new Rect(0, 0, filled == 2 ? QGraspStarSize : QGraspStarSize / 2, QGraspStarSize)));
                context.PushOpacity(QGraspHover is null ? 1 : 0.7);
                context.DrawImage(_qGraspStarImage, frame);
                context.Pop();
                context.Pop();
            }

            context.Pop();
        }

        context.Pop();
    }

    private void QGraspPreviewRefine(int? hovered)
    {
        QGraspHover = hovered;
        _qGraspElement.InvalidateVisual();
        _qGraspElement.RaiseEvent(new RoutedEventArgs(_qGraspHovered, _qGraspElement));
    }

    private void QGraspStepRefine(int step)
    {
        _qGraspElement.SetValue(_qGraspStep, step);
        _qGraspElement.RaiseEvent(new RoutedEventArgs(_qGraspChanged, _qGraspElement));
    }

    private int QGraspStepDraw(Point point)
    {
        double pitch = QGraspStarSize + QGraspStarGap;
        double x = point.X - QGraspHitSlack + QGraspStarGap / 2;
        int star = (int)Math.Floor(x / pitch);
        star = Math.Clamp(star, 0, QGraspStarCount - 1);
        double within = x - star * pitch;
        return star * 2 + (within < pitch / 2 ? 1 : 2);
    }
}
