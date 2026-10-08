using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Llyn.UIDeportment;

public sealed class PContour : FrameworkElement
{
    private const double PContourLabelSize = 15;

    private const double PContourAxisSize = 10;

    private const double PContourStrokeWidth = 4;

    private const double PContourDotRadius = 4.5;

    private const double PContourCornerRadius = 10;

    public static readonly DependencyProperty PContourSyllablesProperty = DependencyProperty.Register(
        nameof(PContourSyllables),
        typeof(IReadOnlyList<QContourItem>),
        typeof(PContour),
        new FrameworkPropertyMetadata(Array.Empty<QContourItem>(), PContourChangeRefine));

    public static readonly DependencyProperty PContourScaleProperty = DependencyProperty.Register(
        nameof(PContourScale),
        typeof(IReadOnlyList<int>),
        typeof(PContour),
        new FrameworkPropertyMetadata(Array.Empty<int>(), PContourChangeRefine));

    public static readonly DependencyProperty PContourGuideProperty = PContourBrushCreate(nameof(PContourGuide));

    public static readonly DependencyProperty PContourAxisProperty = PContourBrushCreate(nameof(PContourAxis));

    public static readonly DependencyProperty PContourInkProperty = PContourBrushCreate(nameof(PContourInk));

    public static readonly DependencyProperty PContourFrameProperty = PContourBrushCreate(nameof(PContourFrame));

    public static readonly DependencyProperty PContourEdgeProperty = PContourBrushCreate(nameof(PContourEdge));

    public static readonly DependencyProperty PContourFontProperty = DependencyProperty.Register(
        nameof(PContourFont),
        typeof(FontFamily),
        typeof(PContour),
        new FrameworkPropertyMetadata(new FontFamily("Segoe UI"), FrameworkPropertyMetadataOptions.AffectsRender));

    public PContour()
    {
        IsHitTestVisible = false;
        Visibility = Visibility.Collapsed;
        SetResourceReference(PContourGuideProperty, "Theme.Contour.Guide");
        SetResourceReference(PContourAxisProperty, "Theme.Contour.Axis");
        SetResourceReference(PContourInkProperty, "Theme.Contour.Ink");
        SetResourceReference(PContourFrameProperty, "Theme.Contour.Frame");
        SetResourceReference(PContourEdgeProperty, "Theme.Contour.Edge");
        SetResourceReference(PContourFontProperty, "Theme.Phonetic");
    }

    public IReadOnlyList<QContourItem> PContourSyllables
    {
        get => (IReadOnlyList<QContourItem>)GetValue(PContourSyllablesProperty);
        set => SetValue(PContourSyllablesProperty, value);
    }

    public IReadOnlyList<int> PContourScale
    {
        get => (IReadOnlyList<int>)GetValue(PContourScaleProperty);
        set => SetValue(PContourScaleProperty, value);
    }

    public Brush PContourGuide
    {
        get => (Brush)GetValue(PContourGuideProperty);
        set => SetValue(PContourGuideProperty, value);
    }

    public Brush PContourAxis
    {
        get => (Brush)GetValue(PContourAxisProperty);
        set => SetValue(PContourAxisProperty, value);
    }

    public Brush PContourInk
    {
        get => (Brush)GetValue(PContourInkProperty);
        set => SetValue(PContourInkProperty, value);
    }

    public Brush PContourFrame
    {
        get => (Brush)GetValue(PContourFrameProperty);
        set => SetValue(PContourFrameProperty, value);
    }

    public Brush PContourEdge
    {
        get => (Brush)GetValue(PContourEdgeProperty);
        set => SetValue(PContourEdgeProperty, value);
    }

    public FontFamily PContourFont
    {
        get => (FontFamily)GetValue(PContourFontProperty);
        set => SetValue(PContourFontProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return new QContourPlot(PContourScale, PContourSyllables.Count).QContourPlotSize;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        if (PContourSyllables.Count == 0 || PContourScale.Count == 0)
        {
            return;
        }

        QContourPlot plot = new(PContourScale, PContourSyllables.Count);
        PContourFrameDraw(drawingContext);
        PContourGuideDraw(drawingContext, plot);
        for (int index = 0; index < PContourSyllables.Count; index++)
        {
            PContourCellDraw(drawingContext, plot, PContourSyllables[index], index);
        }
    }

    private static DependencyProperty PContourBrushCreate(string name)
    {
        return DependencyProperty.Register(
            name,
            typeof(Brush),
            typeof(PContour),
            new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender));
    }

    private static void PContourChangeRefine(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is PContour contour)
        {
            contour.PContourRefine();
        }
    }

    private void PContourRefine()
    {
        Visibility = PContourSyllables.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        InvalidateMeasure();
        InvalidateVisual();
    }

    private void PContourFrameDraw(DrawingContext drawingContext)
    {
        Rect frame = new(0.5, 0.5, Math.Max(0, RenderSize.Width - 1), Math.Max(0, RenderSize.Height - 1));
        drawingContext.DrawRoundedRectangle(
            PContourFrame, new Pen(PContourEdge, 1), frame, PContourCornerRadius, PContourCornerRadius);
    }

    private void PContourGuideDraw(DrawingContext drawingContext, QContourPlot plot)
    {
        double left = plot.QContourLeftRead(0);
        double right = plot.QContourRightRead(RenderSize.Width);
        Pen guide = new(PContourGuide, 1);
        foreach (int level in PContourScale)
        {
            double y = Math.Round(plot.QContourLevelResolve(level)) + 0.5;
            drawingContext.DrawLine(guide, new Point(left, y), new Point(right, y));

            FormattedText digit = PContourTextBuild(
                level.ToString(CultureInfo.InvariantCulture), PContourAxisSize, PContourAxis);
            drawingContext.DrawText(digit, new Point(left - digit.Width - 6, y - digit.Height / 2));
        }
    }

    private void PContourCellDraw(DrawingContext drawingContext, QContourPlot plot, QContourItem syllable, int index)
    {
        double left = plot.QContourLeftRead(index);
        double top = plot.QContourLabelTop;

        FormattedText label = PContourTextBuild(syllable.QContourItemText, PContourLabelSize, PContourInk);
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
        Pen line = PContourLineBuild(syllable.QContourItemBrushes, points);
        for (int step = 1; step < points.Count; step++)
        {
            drawingContext.DrawLine(line, points[step - 1], points[step]);
        }

        Pen rim = new(PContourFrame, 1.5);
        for (int step = 0; step < points.Count; step++)
        {
            Brush ink = syllable.QContourItemBrushes[Math.Min(step, syllable.QContourItemBrushes.Count - 1)];
            drawingContext.DrawEllipse(ink, rim, points[step], PContourDotRadius, PContourDotRadius);
        }
    }

    private Pen PContourLineBuild(IReadOnlyList<Brush> inks, IReadOnlyList<Point> points)
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
                stops.Add(new GradientStop(PContourColorRead(inks[step]), (points[step].X - start) / span));
            }

            brush = new LinearGradientBrush(stops, new Point(start, 0), new Point(start + span, 0))
            {
                MappingMode = BrushMappingMode.Absolute,
            };
        }

        Pen pen = new(brush, PContourStrokeWidth)
        {
            StartLineCap = PenLineCap.Round,
            EndLineCap = PenLineCap.Round,
            LineJoin = PenLineJoin.Round,
        };
        return pen;
    }

    private FormattedText PContourTextBuild(string text, double size, Brush brush)
    {
        return new FormattedText(
            text,
            CultureInfo.CurrentUICulture,
            FlowDirection.LeftToRight,
            new Typeface(PContourFont, FontStyles.Normal, FontWeights.Medium, FontStretches.Normal),
            size,
            brush,
            VisualTreeHelper.GetDpi(this).PixelsPerDip);
    }

    private static Color PContourColorRead(Brush ink)
    {
        return ink is SolidColorBrush solid ? solid.Color : Colors.Gray;
    }
}
