using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Xml.Linq;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace Llyn.UIVeneer;

[MarkupExtensionReturnType(typeof(ImageSource))]
public sealed class PIcon : MarkupExtension
{
    private static readonly Dictionary<string, ImageSource> PIconStore = [];

    private static readonly HashSet<string> PIconVector = new(StringComparer.Ordinal)
    {
        "add", "check", "close", "remove",
    };

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

                ImageSource grayImage;
                if (source is DrawingImage drawingImage)
                {
                    Drawing drawing = drawingImage.Drawing.Clone();
                    PImageApply(drawing);
                    grayImage = new DrawingImage(drawing);
                    grayImage.Freeze();
                }
                else if (source is BitmapSource bitmapSource)
                {
                    grayImage = PImageGrayCreate(bitmapSource);
                }
                else
                {
                    return source;
                }

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

            ImageSource image = PIconAssetLoad(name);
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

        static ImageSource PImageGrayCreate(BitmapSource source)
        {
            FormatConvertedBitmap bitmap = new(source, PixelFormats.Bgra32, null, 0);
            int stride = bitmap.PixelWidth * 4;
            byte[] pixels = new byte[stride * bitmap.PixelHeight];
            bitmap.CopyPixels(pixels, stride, 0);
            for (int index = 0; index < pixels.Length; index += 4)
            {
                byte gray = (byte)Math.Round(
                    (0.2126 * pixels[index + 2]) + (0.7152 * pixels[index + 1]) + (0.0722 * pixels[index]));
                pixels[index] = gray;
                pixels[index + 1] = gray;
                pixels[index + 2] = gray;
            }

            BitmapSource grayImage = BitmapSource.Create(
                bitmap.PixelWidth,
                bitmap.PixelHeight,
                bitmap.DpiX,
                bitmap.DpiY,
                PixelFormats.Bgra32,
                null,
                pixels,
                stride);
            grayImage.Freeze();
            return grayImage;
        }
    }

    private static ImageSource PIconAssetLoad(string name)
    {
        StreamResourceInfo? pngResource = null;
        if (!PIconVector.Contains(name))
        {
            Uri pngUri = new(string.Concat("pack://application:,,,/Llyn;component/icons/", name, ".png"));
            try
            {
                pngResource = System.Windows.Application.GetResourceStream(pngUri);
            }
            catch (IOException)
            {
            }
        }

        if (pngResource is not null)
        {
            using Stream stream = pngResource.Stream;
            return BitmapFrame.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        }

        Uri svgUri = new(string.Concat("pack://application:,,,/Llyn;component/icons/", name, ".svg"));
        StreamResourceInfo svgResource = System.Windows.Application.GetResourceStream(svgUri)
            ?? throw new FileNotFoundException(name);
        using Stream svgStream = svgResource.Stream;
        using FileSvgReader reader = new(new WpfDrawingSettings
        {
            IncludeRuntime = false,
            TextAsGeometry = true,
        });
        return new DrawingImage(reader.Read(svgStream) ?? throw new InvalidDataException(name));
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
