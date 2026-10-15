using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Llyn.UIDeportment;

public sealed class QContour : FrameworkElement
{
    private const double QContourLabelSize = 15;

    private const double QContourAxisSize = 10;

    private const double QContourStrokeWidth = 4;

    private const double QContourDotRadius = 4.5;

    private const double QContourCornerRadius = 10;

    public static readonly DependencyProperty QContourSyllablesProperty = DependencyProperty.Register(
        nameof(QContourSyllables),
        typeof(IReadOnlyList<QContourItem>),
        typeof(QContour),
        new FrameworkPropertyMetadata(Array.Empty<QContourItem>(), QContourChangeRefine));

    public static readonly DependencyProperty QContourScaleProperty = DependencyProperty.Register(
        nameof(QContourScale),
        typeof(IReadOnlyList<int>),
        typeof(QContour),
        new FrameworkPropertyMetadata(Array.Empty<int>(), QContourChangeRefine));

    public static readonly DependencyProperty QContourGuideProperty = QContourBrushCreate(nameof(QContourGuide));

    public static readonly DependencyProperty QContourAxisProperty = QContourBrushCreate(nameof(QContourAxis));

    public static readonly DependencyProperty QContourInkProperty = QContourBrushCreate(nameof(QContourInk));

    public static readonly DependencyProperty QContourFrameProperty = QContourBrushCreate(nameof(QContourFrame));

    public static readonly DependencyProperty QContourEdgeProperty = QContourBrushCreate(nameof(QContourEdge));

    public static readonly DependencyProperty QContourFontProperty = DependencyProperty.Register(
        nameof(QContourFont),
        typeof(FontFamily),
        typeof(QContour),
        new FrameworkPropertyMetadata(new FontFamily("Segoe UI"), FrameworkPropertyMetadataOptions.AffectsRender));

    public QContour()
    {
        IsHitTestVisible = false;
        Visibility = Visibility.Collapsed;
        SetResourceReference(QContourGuideProperty, "Theme.Contour.Guide");
        SetResourceReference(QContourAxisProperty, "Theme.Contour.Axis");
        SetResourceReference(QContourInkProperty, "Theme.Contour.Ink");
        SetResourceReference(QContourFrameProperty, "Theme.Contour.Frame");
        SetResourceReference(QContourEdgeProperty, "Theme.Contour.Edge");
        SetResourceReference(QContourFontProperty, "Theme.Phonetic");
    }

    public IReadOnlyList<QContourItem> QContourSyllables
    {
        get => (IReadOnlyList<QContourItem>)GetValue(QContourSyllablesProperty);
        set => SetValue(QContourSyllablesProperty, value);
    }

    public IReadOnlyList<int> QContourScale
    {
        get => (IReadOnlyList<int>)GetValue(QContourScaleProperty);
        set => SetValue(QContourScaleProperty, value);
    }

    public Brush QContourGuide
    {
        get => (Brush)GetValue(QContourGuideProperty);
        set => SetValue(QContourGuideProperty, value);
    }

    public Brush QContourAxis
    {
        get => (Brush)GetValue(QContourAxisProperty);
        set => SetValue(QContourAxisProperty, value);
    }

    public Brush QContourInk
    {
        get => (Brush)GetValue(QContourInkProperty);
        set => SetValue(QContourInkProperty, value);
    }

    public Brush QContourFrame
    {
        get => (Brush)GetValue(QContourFrameProperty);
        set => SetValue(QContourFrameProperty, value);
    }

    public Brush QContourEdge
    {
        get => (Brush)GetValue(QContourEdgeProperty);
        set => SetValue(QContourEdgeProperty, value);
    }

    public FontFamily QContourFont
    {
        get => (FontFamily)GetValue(QContourFontProperty);
        set => SetValue(QContourFontProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return new Size(0, new QContourPlot(QContourScale, QContourSyllables.Count).QContourPlotSize.Height);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        if (QContourSyllables.Count == 0 || QContourScale.Count == 0)
        {
            return;
        }

        QContourPlot plot = new(QContourScale, QContourSyllables.Count);
        drawingContext.PushTransform(new TranslateTransform(plot.QContourOriginRead(RenderSize.Width), 0));
        QContourFrameDraw(drawingContext, plot.QContourPlotSize);
        QContourGuideDraw(drawingContext, plot);
        for (int index = 0; index < QContourSyllables.Count; index++)
        {
            QContourCellDraw(drawingContext, plot, QContourSyllables[index], index);
        }

        drawingContext.Pop();
    }

    private static DependencyProperty QContourBrushCreate(string name)
    {
        return DependencyProperty.Register(
            name,
            typeof(Brush),
            typeof(QContour),
            new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender));
    }

    private static void QContourChangeRefine(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is QContour contour)
        {
            contour.QContourRefine();
        }
    }

    private void QContourRefine()
    {
        Visibility = QContourSyllables.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        InvalidateMeasure();
        InvalidateVisual();
    }

    private void QContourFrameDraw(DrawingContext drawingContext, Size size)
    {
        Rect frame = new(0.5, 0.5, Math.Max(0, size.Width - 1), Math.Max(0, size.Height - 1));
        drawingContext.DrawRoundedRectangle(
            QContourFrame, new Pen(QContourEdge, 1), frame, QContourCornerRadius, QContourCornerRadius);
    }

    private void QContourGuideDraw(DrawingContext drawingContext, QContourPlot plot)
    {
        double left = plot.QContourLeftRead(0);
        double right = plot.QContourRightRead(plot.QContourPlotSize.Width);
        Pen guide = new(QContourGuide, 1);
        foreach (int level in QContourScale)
        {
            double y = Math.Round(plot.QContourLevelResolve(level)) + 0.5;
            drawingContext.DrawLine(guide, new Point(left, y), new Point(right, y));

            FormattedText digit = QContourTextBuild(
                level.ToString(CultureInfo.InvariantCulture), QContourAxisSize, QContourAxis);
            drawingContext.DrawText(digit, new Point(left - digit.Width - 6, y - digit.Height / 2));
        }
    }

    private void QContourCellDraw(DrawingContext drawingContext, QContourPlot plot, QContourItem syllable, int index)
    {
        double left = plot.QContourLeftRead(index);
        double top = plot.QContourLabelTop;

        FormattedText label = QContourTextBuild(syllable.QContourItemText, QContourLabelSize, QContourInk);
        label.MaxTextWidth = QContourPlot.QContourCellWidth;
        label.MaxLineCount = 1;
        label.Trimming = TextTrimming.CharacterEllipsis;
        label.TextAlignment = TextAlignment.Center;
        drawingContext.DrawText(label, new Point(left, top));

        if (!syllable.QContourItemToned)
        {
            return;
        }

        IReadOnlyList<Point> points = plot.QContourPointResolve(syllable.QContourItemLevels, index);
        Pen line = QContourLineBuild(syllable.QContourItemBrushes, points);
        for (int step = 1; step < points.Count; step++)
        {
            drawingContext.DrawLine(line, points[step - 1], points[step]);
        }

        Pen rim = new(QContourFrame, 1.5);
        for (int step = 0; step < points.Count; step++)
        {
            Brush ink = syllable.QContourItemBrushes[Math.Min(step, syllable.QContourItemBrushes.Count - 1)];
            drawingContext.DrawEllipse(ink, rim, points[step], QContourDotRadius, QContourDotRadius);
        }
    }

    private Pen QContourLineBuild(IReadOnlyList<Brush> inks, IReadOnlyList<Point> points)
    {
        Brush brush;
        if (inks.Count == 1)
        {
            brush = inks[0];
        }
        else
        {
            GradientStopCollection stops = [];
            double start = points[0].X;
            double span = points[^1].X - start;
            for (int step = 0; step < inks.Count; step++)
            {
                stops.Add(new GradientStop(QContourColorRead(inks[step]), (points[step].X - start) / span));
            }

            brush = new LinearGradientBrush(stops, new Point(start, 0), new Point(start + span, 0))
            {
                MappingMode = BrushMappingMode.Absolute,
            };
        }

        Pen pen = new(brush, QContourStrokeWidth)
        {
            StartLineCap = PenLineCap.Round,
            EndLineCap = PenLineCap.Round,
            LineJoin = PenLineJoin.Round,
        };
        return pen;
    }

    private FormattedText QContourTextBuild(string text, double size, Brush brush)
    {
        return new FormattedText(
            text,
            CultureInfo.CurrentUICulture,
            FlowDirection.LeftToRight,
            new Typeface(QContourFont, FontStyles.Normal, FontWeights.Medium, FontStretches.Normal),
            size,
            brush,
            VisualTreeHelper.GetDpi(this).PixelsPerDip);
    }

    private static Color QContourColorRead(Brush ink)
    {
        return ink is SolidColorBrush solid ? solid.Color : Colors.Gray;
    }
}
