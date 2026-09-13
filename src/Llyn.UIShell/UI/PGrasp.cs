using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIShell;

public sealed class PGrasp : FrameworkElement
{
    private const int PGraspStarCount = LGrasp.LGraspStep / 2;

    private const double PGraspStarSize = 16;

    private const double PGraspStarGap = 3;

    private const double PGraspStrokeWidth = 1.4;

    private const double PGraspHitSlack = 4;

    private static readonly Geometry PGraspStarGeometry = Geometry.Parse(
        "M 8,0.7 L 10.06,5.62 15.4,6.05 11.34,9.55 12.58,14.75 8,11.95 3.42,14.75 4.66,9.55 0.6,6.05 5.94,5.62 Z");

    public static readonly DependencyProperty PGraspStepProperty = DependencyProperty.Register(
        nameof(PGraspStep),
        typeof(int),
        typeof(PGrasp),
        new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender),
        value => LGrasp.LGraspCheck((int)value));

    public static readonly DependencyProperty PGraspFillProperty = DependencyProperty.Register(
        nameof(PGraspFill),
        typeof(Brush),
        typeof(PGrasp),
        new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty PGraspEmptyProperty = DependencyProperty.Register(
        nameof(PGraspEmpty),
        typeof(Brush),
        typeof(PGrasp),
        new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty PGraspUnratedProperty = DependencyProperty.Register(
        nameof(PGraspUnrated),
        typeof(Brush),
        typeof(PGrasp),
        new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty PGraspPreviewProperty = DependencyProperty.Register(
        nameof(PGraspPreview),
        typeof(Brush),
        typeof(PGrasp),
        new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly RoutedEvent PGraspChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(PGraspChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(PGrasp));

    public static readonly RoutedEvent PGraspHoveredEvent = EventManager.RegisterRoutedEvent(
        nameof(PGraspHovered),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(PGrasp));

    private int? _pGraspHover;

    public PGrasp()
    {
        Focusable = true;
        Cursor = Cursors.Hand;
        SetResourceReference(PGraspFillProperty, "Theme.Grasp.Fill");
        SetResourceReference(PGraspEmptyProperty, "Theme.Grasp.Empty");
        SetResourceReference(PGraspUnratedProperty, "Theme.Grasp.Unrated");
        SetResourceReference(PGraspPreviewProperty, "Theme.Grasp.Preview");
        IsEnabledChanged += (_, _) => Opacity = IsEnabled ? 1 : 0.4;
    }

    public event RoutedEventHandler PGraspChanged
    {
        add => AddHandler(PGraspChangedEvent, value);
        remove => RemoveHandler(PGraspChangedEvent, value);
    }

    public event RoutedEventHandler PGraspHovered
    {
        add => AddHandler(PGraspHoveredEvent, value);
        remove => RemoveHandler(PGraspHoveredEvent, value);
    }

    public int? PGraspHover => _pGraspHover;

    public int PGraspStep
    {
        get => (int)GetValue(PGraspStepProperty);
        set => SetValue(PGraspStepProperty, value);
    }

    public Brush PGraspFill
    {
        get => (Brush)GetValue(PGraspFillProperty);
        set => SetValue(PGraspFillProperty, value);
    }

    public Brush PGraspEmpty
    {
        get => (Brush)GetValue(PGraspEmptyProperty);
        set => SetValue(PGraspEmptyProperty, value);
    }

    public Brush PGraspUnrated
    {
        get => (Brush)GetValue(PGraspUnratedProperty);
        set => SetValue(PGraspUnratedProperty, value);
    }

    public Brush PGraspPreview
    {
        get => (Brush)GetValue(PGraspPreviewProperty);
        set => SetValue(PGraspPreviewProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return new Size(
            PGraspStarCount * PGraspStarSize + (PGraspStarCount - 1) * PGraspStarGap + 2 * PGraspHitSlack,
            PGraspStarSize + 2 * PGraspHitSlack);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        PGraspDraw(drawingContext);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        int hovered = PGraspStepResolve(e.GetPosition(this));
        if (_pGraspHover != hovered)
        {
            PGraspHoverChange(hovered);
        }
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        if (_pGraspHover is not null)
        {
            PGraspHoverChange(null);
        }
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        Focus();
        int chosen = PGraspStepResolve(e.GetPosition(this));
        PGraspStepChange(chosen == PGraspStep ? 0 : chosen);
        e.Handled = true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        int? next = e.Key switch
        {
            Key.Left => Math.Max(0, PGraspStep - 1),
            Key.Right => Math.Min(LGrasp.LGraspStep, PGraspStep + 1),
            Key.Home => 0,
            Key.End => LGrasp.LGraspStep,
            _ => null,
        };

        if (next is int step)
        {
            PGraspStepChange(step);
            e.Handled = true;
        }
    }

    public static string PGraspLabelResolve(int step)
    {
        return "Grasp.Level" + step.ToString(CultureInfo.InvariantCulture);
    }

    private void PGraspHoverChange(int? hovered)
    {
        _pGraspHover = hovered;
        InvalidateVisual();
        RaiseEvent(new RoutedEventArgs(PGraspHoveredEvent, this));
    }

    private void PGraspStepChange(int step)
    {
        if (step == PGraspStep)
        {
            return;
        }

        PGraspStep = step;
        RaiseEvent(new RoutedEventArgs(PGraspChangedEvent, this));
    }

    private static int PGraspStepResolve(Point point)
    {
        double pitch = PGraspStarSize + PGraspStarGap;
        double x = point.X - PGraspHitSlack + PGraspStarGap / 2;
        int star = (int)Math.Floor(x / pitch);
        star = Math.Clamp(star, 0, PGraspStarCount - 1);
        double within = x - star * pitch;
        return star * 2 + (within < pitch / 2 ? 1 : 2);
    }

    private void PGraspDraw(DrawingContext context)
    {
        int shown = _pGraspHover ?? PGraspStep;
        Brush fill = _pGraspHover is null ? PGraspFill : PGraspPreview;
        Pen outline = new(shown == 0 ? PGraspUnrated : PGraspEmpty, PGraspStrokeWidth) { LineJoin = PenLineJoin.Round };

        context.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));
        context.PushTransform(new TranslateTransform(PGraspHitSlack, PGraspHitSlack));
        context.PushTransform(new ScaleTransform(PGraspStarSize / 16, PGraspStarSize / 16));
        double pitch = (PGraspStarSize + PGraspStarGap) * 16 / PGraspStarSize;

        for (int star = 0; star < PGraspStarCount; star++)
        {
            int filled = Math.Clamp(shown - star * 2, 0, 2);
            context.PushTransform(new TranslateTransform(star * pitch, 0));

            if (filled > 0)
            {
                context.PushClip(new RectangleGeometry(new Rect(0, 0, filled == 2 ? 16 : 8, 16)));
                context.DrawGeometry(fill, null, PGraspStarGeometry);
                context.Pop();
            }

            context.DrawGeometry(null, outline, PGraspStarGeometry);
            context.Pop();
        }

        context.Pop();
        context.Pop();
    }
}
