using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIShell;

public sealed class PContour : FrameworkElement
{
    private const double PContourPadding = 14;

    private const double PContourAxisWidth = 18;

    private const double PContourCellWidth = 76;

    private const double PContourCellGap = 10;

    private const double PContourCellInset = 12;

    private const double PContourLevelGap = 18;

    private const double PContourLabelGap = 8;

    private const double PContourLabelHeight = 24;

    private const double PContourLabelSize = 15;

    private const double PContourAxisSize = 10;

    private const double PContourStrokeWidth = 4;

    private const double PContourDotRadius = 4.5;

    private const double PContourCornerRadius = 10;

    private const double PContourPlotHeight = (LContour.LContourCeiling - LContour.LContourFloor) * PContourLevelGap;

    public static readonly DependencyProperty PContourIpaProperty = DependencyProperty.Register(
        nameof(PContourIpa),
        typeof(string),
        typeof(PContour),
        new FrameworkPropertyMetadata(string.Empty, PContourChangeHandle));

    public static readonly DependencyProperty PContourTonalProperty = DependencyProperty.Register(
        nameof(PContourTonal),
        typeof(bool),
        typeof(PContour),
        new FrameworkPropertyMetadata(false, PContourChangeHandle));

    public static readonly DependencyProperty PContourTopProperty = PContourBrushCreate(nameof(PContourTop));

    public static readonly DependencyProperty PContourHighProperty = PContourBrushCreate(nameof(PContourHigh));

    public static readonly DependencyProperty PContourMidProperty = PContourBrushCreate(nameof(PContourMid));

    public static readonly DependencyProperty PContourLowProperty = PContourBrushCreate(nameof(PContourLow));

    public static readonly DependencyProperty PContourBottomProperty = PContourBrushCreate(nameof(PContourBottom));

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

    private IReadOnlyList<LContour> _pContourSyllables = [];

    public PContour()
    {
        IsHitTestVisible = false;
        Visibility = Visibility.Collapsed;
        SetResourceReference(PContourTopProperty, "Theme.Contour.Top");
        SetResourceReference(PContourHighProperty, "Theme.Contour.High");
        SetResourceReference(PContourMidProperty, "Theme.Contour.Mid");
        SetResourceReference(PContourLowProperty, "Theme.Contour.Low");
        SetResourceReference(PContourBottomProperty, "Theme.Contour.Bottom");
        SetResourceReference(PContourGuideProperty, "Theme.Contour.Guide");
        SetResourceReference(PContourAxisProperty, "Theme.Contour.Axis");
        SetResourceReference(PContourInkProperty, "Theme.Contour.Ink");
        SetResourceReference(PContourFrameProperty, "Theme.Contour.Frame");
        SetResourceReference(PContourEdgeProperty, "Theme.Contour.Edge");
        SetResourceReference(PContourFontProperty, "Theme.Phonetic");
    }

    public string PContourIpa
    {
        get => (string)GetValue(PContourIpaProperty);
        set => SetValue(PContourIpaProperty, value);
    }

    public bool PContourTonal
    {
        get => (bool)GetValue(PContourTonalProperty);
        set => SetValue(PContourTonalProperty, value);
    }

    public Brush PContourTop
    {
        get => (Brush)GetValue(PContourTopProperty);
        set => SetValue(PContourTopProperty, value);
    }

    public Brush PContourHigh
    {
        get => (Brush)GetValue(PContourHighProperty);
        set => SetValue(PContourHighProperty, value);
    }

    public Brush PContourMid
    {
        get => (Brush)GetValue(PContourMidProperty);
        set => SetValue(PContourMidProperty, value);
    }

    public Brush PContourLow
    {
        get => (Brush)GetValue(PContourLowProperty);
        set => SetValue(PContourLowProperty, value);
    }

    public Brush PContourBottom
    {
        get => (Brush)GetValue(PContourBottomProperty);
        set => SetValue(PContourBottomProperty, value);
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
        int count = Math.Max(1, _pContourSyllables.Count);
        double width = 2 * PContourPadding + PContourAxisWidth
            + count * PContourCellWidth + (count - 1) * PContourCellGap;
        double height = 2 * PContourPadding + PContourPlotHeight + PContourLabelGap + PContourLabelHeight;
        return new Size(width, height);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        if (_pContourSyllables.Count == 0)
        {
            return;
        }

        PContourFrameDraw(drawingContext);
        PContourGuideDraw(drawingContext);
        for (int index = 0; index < _pContourSyllables.Count; index++)
        {
            PContourCellDraw(drawingContext, _pContourSyllables[index], index);
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

    private static void PContourChangeHandle(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is PContour contour)
        {
            contour.PContourUpdate();
        }
    }

    private void PContourUpdate()
    {
        _pContourSyllables = PContourTonal ? LContour.LContourParse(PContourIpa ?? string.Empty) : [];
        bool shown = LContour.LContourToneCheck(_pContourSyllables);
        Visibility = shown ? Visibility.Visible : Visibility.Collapsed;
        InvalidateMeasure();
        InvalidateVisual();
    }

    private void PContourFrameDraw(DrawingContext drawingContext)
    {
        Rect frame = new(0.5, 0.5, Math.Max(0, RenderSize.Width - 1), Math.Max(0, RenderSize.Height - 1));
        drawingContext.DrawRoundedRectangle(
            PContourFrame, new Pen(PContourEdge, 1), frame, PContourCornerRadius, PContourCornerRadius);
    }

    private void PContourGuideDraw(DrawingContext drawingContext)
    {
        double left = PContourPadding + PContourAxisWidth;
        double right = RenderSize.Width - PContourPadding;
        Pen guide = new(PContourGuide, 1);
        for (int level = LContour.LContourFloor; level <= LContour.LContourCeiling; level++)
        {
            double y = Math.Round(PContourLevelResolve(level)) + 0.5;
            drawingContext.DrawLine(guide, new Point(left, y), new Point(right, y));

            FormattedText digit = PContourTextBuild(
                level.ToString(CultureInfo.InvariantCulture), PContourAxisSize, PContourAxis);
            drawingContext.DrawText(digit, new Point(left - digit.Width - 6, y - digit.Height / 2));
        }
    }

    private void PContourCellDraw(DrawingContext drawingContext, LContour syllable, int index)
    {
        double left = PContourPadding + PContourAxisWidth + index * (PContourCellWidth + PContourCellGap);
        double top = PContourPadding + PContourPlotHeight + PContourLabelGap;

        FormattedText label = PContourTextBuild(syllable.LContourText, PContourLabelSize, PContourInk);
        label.MaxTextWidth = PContourCellWidth;
        label.MaxLineCount = 1;
        label.Trimming = TextTrimming.CharacterEllipsis;
        label.TextAlignment = TextAlignment.Center;
        drawingContext.DrawText(label, new Point(left, top));

        if (syllable.LContourLevels.Count == 0)
        {
            return;
        }

        IReadOnlyList<Point> points = PContourPointResolve(syllable.LContourLevels, left);
        Pen line = PContourLineBuild(syllable.LContourLevels, points);
        for (int step = 1; step < points.Count; step++)
        {
            drawingContext.DrawLine(line, points[step - 1], points[step]);
        }

        Pen rim = new(PContourFrame, 1.5);
        for (int step = 0; step < points.Count; step++)
        {
            int level = syllable.LContourLevels[Math.Min(step, syllable.LContourLevels.Count - 1)];
            drawingContext.DrawEllipse(
                PContourBrushRead(level), rim, points[step], PContourDotRadius, PContourDotRadius);
        }
    }

    private IReadOnlyList<Point> PContourPointResolve(IReadOnlyList<int> levels, double left)
    {
        double start = left + PContourCellInset;
        double span = PContourCellWidth - 2 * PContourCellInset;
        List<Point> points = [];
        if (levels.Count == 1)
        {
            double y = PContourLevelResolve(levels[0]);
            points.Add(new Point(start, y));
            points.Add(new Point(start + span, y));
            return points;
        }

        for (int step = 0; step < levels.Count; step++)
        {
            double x = start + span * step / (levels.Count - 1);
            points.Add(new Point(x, PContourLevelResolve(levels[step])));
        }

        return points;
    }

    private Pen PContourLineBuild(IReadOnlyList<int> levels, IReadOnlyList<Point> points)
    {
        Brush brush;
        if (levels.Count == 1)
        {
            brush = PContourBrushRead(levels[0]);
        }
        else
        {
            GradientStopCollection stops = [];
            double start = points[0].X;
            double span = points[^1].X - start;
            for (int step = 0; step < levels.Count; step++)
            {
                stops.Add(new GradientStop(PContourColorRead(levels[step]), (points[step].X - start) / span));
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

    private static double PContourLevelResolve(int level)
    {
        return PContourPadding + (LContour.LContourCeiling - level) * PContourLevelGap;
    }

    private Brush PContourBrushRead(int level)
    {
        return level switch
        {
            5 => PContourTop,
            4 => PContourHigh,
            3 => PContourMid,
            2 => PContourLow,
            _ => PContourBottom,
        };
    }

    private Color PContourColorRead(int level)
    {
        return PContourBrushRead(level) is SolidColorBrush solid ? solid.Color : Colors.Gray;
    }
}
