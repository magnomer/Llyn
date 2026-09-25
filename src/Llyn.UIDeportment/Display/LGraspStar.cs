using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Llyn.UIDeportment;

public sealed class LGraspStar
{
    private const double LGraspStarSize = 16;

    private const double LGraspStarGap = 3;

    private const double LGraspHitSlack = 4;

    private readonly FrameworkElement _lGraspElement;

    private readonly DependencyProperty _lGraspStep;

    private readonly DependencyProperty _lGraspLimit;

    private readonly RoutedEvent _lGraspChanged;

    private readonly RoutedEvent _lGraspHovered;

    private readonly ImageSource _lGraspStarImage;

    private readonly ImageSource _lGraspGrayImage;

    public LGraspStar(
        FrameworkElement grasp,
        DependencyProperty step,
        DependencyProperty limit,
        RoutedEvent changed,
        RoutedEvent hovered,
        ImageSource? star,
        ImageSource? gray)
    {
        _lGraspElement = grasp;
        _lGraspStep = step;
        _lGraspLimit = limit;
        _lGraspChanged = changed;
        _lGraspHovered = hovered;
        _lGraspStarImage = star ?? throw new ArgumentNullException(nameof(star));
        _lGraspGrayImage = gray ?? throw new ArgumentNullException(nameof(gray));
        grasp.IsEnabledChanged += (_, _) => grasp.Opacity = grasp.IsEnabled ? 1 : 0.4;
    }

    public int? LGraspHover { get; private set; }

    public int LGraspPointed => LGraspHover ?? LGraspCurrent;

    private int LGraspCurrent => (int)_lGraspElement.GetValue(_lGraspStep);

    private int LGraspLast => (int)_lGraspElement.GetValue(_lGraspLimit);

    private int LGraspStarCount => LGraspLast / 2;

    public static object LGraspStepClamp(DependencyObject sender, object value, DependencyProperty limit)
    {
        return Math.Min((int)value, (int)sender.GetValue(limit));
    }

    public Size LGraspSizeResolve()
    {
        return new Size(
            LGraspStarCount * LGraspStarSize + (LGraspStarCount - 1) * LGraspStarGap + 2 * LGraspHitSlack,
            LGraspStarSize + 2 * LGraspHitSlack);
    }

    public void LGraspHoverHandle(Point point)
    {
        int hovered = LGraspStepResolve(point);
        if (LGraspHover != hovered)
        {
            LGraspHoverChange(hovered);
        }
    }

    public void LGraspLeaveHandle()
    {
        if (LGraspHover is not null)
        {
            LGraspHoverChange(null);
        }
    }

    public void LGraspPressHandle(Point point)
    {
        _lGraspElement.Focus();
        int chosen = LGraspStepResolve(point);
        LGraspStepChange(chosen == LGraspCurrent ? 0 : chosen);
    }

    public void LGraspKeyHandle(KeyEventArgs e)
    {
        int? next = e.Key switch
        {
            Key.Left => Math.Max(0, LGraspCurrent - 1),
            Key.Right => Math.Min(LGraspLast, LGraspCurrent + 1),
            Key.Home => 0,
            Key.End => LGraspLast,
            _ => null,
        };

        if (next is int step)
        {
            LGraspStepChange(step);
            e.Handled = true;
        }
    }

    public void LGraspDraw(DrawingContext context)
    {
        int shown = LGraspPointed;

        context.DrawRectangle(Brushes.Transparent, null, new Rect(_lGraspElement.RenderSize));
        context.PushTransform(new TranslateTransform(LGraspHitSlack, LGraspHitSlack));
        double pitch = LGraspStarSize + LGraspStarGap;
        Rect frame = new(0, 0, LGraspStarSize, LGraspStarSize);

        for (int star = 0; star < LGraspStarCount; star++)
        {
            int filled = Math.Clamp(shown - star * 2, 0, 2);
            context.PushTransform(new TranslateTransform(star * pitch, 0));
            context.PushOpacity(shown == 0 ? 0.35 : 0.55);
            context.DrawImage(_lGraspGrayImage, frame);
            context.Pop();

            if (filled > 0)
            {
                context.PushClip(new RectangleGeometry(
                    new Rect(0, 0, filled == 2 ? LGraspStarSize : LGraspStarSize / 2, LGraspStarSize)));
                context.PushOpacity(LGraspHover is null ? 1 : 0.7);
                context.DrawImage(_lGraspStarImage, frame);
                context.Pop();
                context.Pop();
            }

            context.Pop();
        }

        context.Pop();
    }

    private void LGraspHoverChange(int? hovered)
    {
        LGraspHover = hovered;
        _lGraspElement.InvalidateVisual();
        _lGraspElement.RaiseEvent(new RoutedEventArgs(_lGraspHovered, _lGraspElement));
    }

    private void LGraspStepChange(int step)
    {
        if (step == LGraspCurrent)
        {
            return;
        }

        _lGraspElement.SetValue(_lGraspStep, step);
        _lGraspElement.RaiseEvent(new RoutedEventArgs(_lGraspChanged, _lGraspElement));
    }

    private int LGraspStepResolve(Point point)
    {
        double pitch = LGraspStarSize + LGraspStarGap;
        double x = point.X - LGraspHitSlack + LGraspStarGap / 2;
        int star = (int)Math.Floor(x / pitch);
        star = Math.Clamp(star, 0, LGraspStarCount - 1);
        double within = x - star * pitch;
        return star * 2 + (within < pitch / 2 ? 1 : 2);
    }
}
