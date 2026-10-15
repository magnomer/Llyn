using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
namespace Llyn.UIDeportment;

public sealed class QGrasp : FrameworkElement
{
    private static readonly ImageSource? QGraspStarImage = QIcon.QIconResolve("star", 16);

    public static readonly DependencyProperty QGraspLimitProperty = DependencyProperty.Register(
        nameof(QGraspLimit),
        typeof(int),
        typeof(QGrasp),
        new FrameworkPropertyMetadata(
            0,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
        value => (int)value >= 0);

    public static readonly DependencyProperty QGraspStepProperty = DependencyProperty.Register(
        nameof(QGraspStep),
        typeof(int),
        typeof(QGrasp),
        new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender),
        value => (int)value >= 0);

    public static readonly DependencyProperty QGraspFillProperty = DependencyProperty.Register(
        nameof(QGraspFill),
        typeof(Brush),
        typeof(QGrasp),
        new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty QGraspEmptyProperty = DependencyProperty.Register(
        nameof(QGraspEmpty),
        typeof(Brush),
        typeof(QGrasp),
        new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty QGraspUnratedProperty = DependencyProperty.Register(
        nameof(QGraspUnrated),
        typeof(Brush),
        typeof(QGrasp),
        new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty QGraspPreviewProperty = DependencyProperty.Register(
        nameof(QGraspPreview),
        typeof(Brush),
        typeof(QGrasp),
        new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender));

    private readonly QGraspStar _qGraspStar;

    public QGrasp()
    {
        Focusable = true;
        Cursor = Cursors.Hand;
        SetResourceReference(QGraspFillProperty, "Theme.Grasp.Fill");
        SetResourceReference(QGraspEmptyProperty, "Theme.Grasp.Empty");
        SetResourceReference(QGraspUnratedProperty, "Theme.Grasp.Unrated");
        SetResourceReference(QGraspPreviewProperty, "Theme.Grasp.Preview");
        _qGraspStar = new QGraspStar(
            this,
            QGraspStepProperty,
            QGraspLimitProperty,
            hovered => QGraspHovered?.Invoke(hovered),
            pressed => QGraspPressed?.Invoke(pressed),
            moved => QGraspMoved?.Invoke(moved),
            QGraspStarImage);
    }

    public event Action<int?>? QGraspHovered;

    public event Action<int>? QGraspPressed;

    public event Action<int>? QGraspMoved;

    public QGraspStar QGraspStar => _qGraspStar;

    public int QGraspLimit
    {
        get => (int)GetValue(QGraspLimitProperty);
        set => SetValue(QGraspLimitProperty, value);
    }

    public int QGraspStep
    {
        get => (int)GetValue(QGraspStepProperty);
        set => SetValue(QGraspStepProperty, value);
    }

    public Brush QGraspFill
    {
        get => (Brush)GetValue(QGraspFillProperty);
        set => SetValue(QGraspFillProperty, value);
    }

    public Brush QGraspEmpty
    {
        get => (Brush)GetValue(QGraspEmptyProperty);
        set => SetValue(QGraspEmptyProperty, value);
    }

    public Brush QGraspUnrated
    {
        get => (Brush)GetValue(QGraspUnratedProperty);
        set => SetValue(QGraspUnratedProperty, value);
    }

    public Brush QGraspPreview
    {
        get => (Brush)GetValue(QGraspPreviewProperty);
        set => SetValue(QGraspPreviewProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return _qGraspStar.QGraspSizeDraw();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        _qGraspStar.QGraspDraw(drawingContext, QGraspFill, QGraspEmpty, QGraspUnrated, QGraspPreview);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        _qGraspStar.QGraspHoverRefine(e.GetPosition(this));
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        _qGraspStar.QGraspLeaveRefine();
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        _qGraspStar.QGraspPressRefine(e.GetPosition(this));
        e.Handled = true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        _qGraspStar.QGraspKeyRefine(e);
    }
}
