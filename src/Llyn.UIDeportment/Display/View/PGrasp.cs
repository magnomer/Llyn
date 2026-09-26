using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
namespace Llyn.UIDeportment;

public sealed class PGrasp : FrameworkElement
{
    private static readonly ImageSource? PGraspStarImage = PIcon.PIconResolve("star", 16);

    private static readonly ImageSource? PGraspGrayImage = PIcon.PIconResolve(null, 0, PGraspStarImage, false);

    public static readonly DependencyProperty PGraspLimitProperty = DependencyProperty.Register(
        nameof(PGraspLimit),
        typeof(int),
        typeof(PGrasp),
        new FrameworkPropertyMetadata(
            0,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
            PGraspLimitHandle),
        value => (int)value >= 0);

    public static readonly DependencyProperty PGraspStepProperty = DependencyProperty.Register(
        nameof(PGraspStep),
        typeof(int),
        typeof(PGrasp),
        new FrameworkPropertyMetadata(
            0,
            FrameworkPropertyMetadataOptions.AffectsRender,
            null,
            (sender, value) => LGraspStar.LGraspStepClamp(sender, value, PGraspLimitProperty)),
        value => (int)value >= 0);

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

    private readonly LGraspStar _lGraspStar;

    public PGrasp()
    {
        Focusable = true;
        Cursor = Cursors.Hand;
        SetResourceReference(PGraspFillProperty, "Theme.Grasp.Fill");
        SetResourceReference(PGraspEmptyProperty, "Theme.Grasp.Empty");
        SetResourceReference(PGraspUnratedProperty, "Theme.Grasp.Unrated");
        SetResourceReference(PGraspPreviewProperty, "Theme.Grasp.Preview");
        _lGraspStar = new LGraspStar(
            this,
            PGraspStepProperty,
            PGraspLimitProperty,
            PGraspChangedEvent,
            PGraspHoveredEvent,
            PGraspStarImage,
            PGraspGrayImage);
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

    public int PGraspPointed => _lGraspStar.LGraspPointed;

    public int PGraspLimit
    {
        get => (int)GetValue(PGraspLimitProperty);
        set => SetValue(PGraspLimitProperty, value);
    }

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
        return _lGraspStar.LGraspSizeResolve();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        _lGraspStar.LGraspDraw(drawingContext);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        _lGraspStar.LGraspHoverHandle(e.GetPosition(this));
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        _lGraspStar.LGraspLeaveHandle();
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        _lGraspStar.LGraspPressHandle(e.GetPosition(this));
        e.Handled = true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        _lGraspStar.LGraspKeyHandle(e);
    }

    private static void PGraspLimitHandle(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        sender.CoerceValue(PGraspStepProperty);
    }
}
