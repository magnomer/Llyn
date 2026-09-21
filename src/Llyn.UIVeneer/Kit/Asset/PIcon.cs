using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Resources;
using System.Xml.Linq;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace Llyn.UIVeneer;

[MarkupExtensionReturnType(typeof(ImageSource))]
public sealed class PIcon : MarkupExtension
{
    private static readonly Dictionary<string, ImageSource> PIconStore = [];

    private static readonly Dictionary<ImageSource, ImageSource> PIconGrayStore = [];

    private static readonly XNamespace PIconSpace = "http://www.w3.org/2000/svg";

    public PIcon(string name, double size)
    {
        PIconName = name;
        PIconSize = size;
    }

    public string PIconName { get; }

    public double PIconSize { get; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return PIconResolve(PIconName, PIconSize);
    }

    [return: NotNullIfNotNull(nameof(name))]
    internal static ImageSource? PIconResolve(string? name, double size, ImageSource? source = null, bool active = true)
    {
        if (source is not null)
        {
            if (active)
            {
                return source;
            }

            lock (PIconGrayStore)
            {
                if (PIconGrayStore.TryGetValue(source, out ImageSource? graySource))
                {
                    return graySource;
                }

                if (source is not DrawingImage drawingImage)
                {
                    return source;
                }

                Drawing drawing = drawingImage.Drawing.Clone();
                PImageApply(drawing);
                DrawingImage grayImage = new(drawing);
                grayImage.Freeze();
                PIconGrayStore[source] = grayImage;
                return grayImage;
            }
        }

        if (name is null)
        {
            return null;
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        lock (PIconStore)
        {
            if (PIconStore.TryGetValue(name, out ImageSource? cached))
            {
                return cached;
            }

            Uri uri = new(string.Concat("pack://application:,,,/Llyn;component/icons/", name, ".svg"));
            StreamResourceInfo resource = System.Windows.Application.GetResourceStream(uri)
                ?? throw new FileNotFoundException(name);
            using Stream stream = resource.Stream;
            using FileSvgReader reader = new(new WpfDrawingSettings
            {
                IncludeRuntime = false,
                TextAsGeometry = true,
            });
            DrawingImage image = new(reader.Read(stream) ?? throw new InvalidDataException(name));
            image.Freeze();
            PIconStore[name] = image;
            return image;
        }

        void PImageApply(Drawing drawing)
        {
            switch (drawing)
            {
                case DrawingGroup group:
                    group.OpacityMask = PImageResolve(group.OpacityMask);
                    foreach (Drawing child in group.Children)
                    {
                        PImageApply(child);
                    }

                    break;
                case GeometryDrawing geometry:
                    geometry.Brush = PImageResolve(geometry.Brush);
                    if (geometry.Pen is Pen pen)
                    {
                        geometry.Pen = pen.Clone();
                        geometry.Pen.Brush = PImageResolve(geometry.Pen.Brush);
                    }

                    break;
                case GlyphRunDrawing glyph:
                    glyph.ForegroundBrush = PImageResolve(glyph.ForegroundBrush);
                    break;
            }
        }

        Brush? PImageResolve(Brush? brush)
        {
            if (brush is SolidColorBrush solid)
            {
                SolidColorBrush gray = solid.Clone();
                gray.Color = PGlyphResolve(gray.Color);
                return gray;
            }

            if (brush is GradientBrush gradient)
            {
                GradientBrush gray = gradient.Clone();
                foreach (GradientStop stop in gray.GradientStops)
                {
                    stop.Color = PGlyphResolve(stop.Color);
                }

                return gray;
            }

            if (brush is DrawingBrush drawingBrush)
            {
                DrawingBrush gray = drawingBrush.Clone();
                PImageApply(gray.Drawing);
                return gray;
            }

            return brush;
        }

        static Color PGlyphResolve(Color color)
        {
            byte gray = (byte)Math.Round((0.2126 * color.R) + (0.7152 * color.G) + (0.0722 * color.B));
            return Color.FromArgb(color.A, gray, gray, gray);
        }
    }

    internal static (Geometry, Rect) PIconLoad(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Uri uri = new(string.Concat("pack://application:,,,/Llyn;component/icons/", name, ".svg"));
        StreamResourceInfo resource = System.Windows.Application.GetResourceStream(uri)
            ?? throw new FileNotFoundException(name);
        using Stream stream = resource.Stream;
        XElement root = XDocument.Load(stream).Root ?? throw new InvalidDataException(name);

        string[] box = ((string?)root.Attribute("viewBox") ?? string.Empty)
            .Split([' ', ','], StringSplitOptions.RemoveEmptyEntries);
        Rect frame = box.Length == 4
            ? new Rect(
                double.Parse(box[0], CultureInfo.InvariantCulture),
                double.Parse(box[1], CultureInfo.InvariantCulture),
                double.Parse(box[2], CultureInfo.InvariantCulture),
                double.Parse(box[3], CultureInfo.InvariantCulture))
            : new Rect(
                0,
                0,
                double.Parse((string?)root.Attribute("width") ?? "24", CultureInfo.InvariantCulture),
                double.Parse((string?)root.Attribute("height") ?? "24", CultureInfo.InvariantCulture));

        GeometryGroup symbol = new() { FillRule = FillRule.Nonzero };
        foreach (XElement path in root.Descendants(PIconSpace + "path"))
        {
            string? data = (string?)path.Attribute("d");
            if (!string.IsNullOrWhiteSpace(data))
            {
                symbol.Children.Add(Geometry.Parse(string.Concat("F1 ", data)));
            }
        }

        symbol.Freeze();
        return (symbol, frame);
    }
}
