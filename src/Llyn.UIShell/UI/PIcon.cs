using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Resources;
using System.Xml.Linq;

namespace Llyn.UIShell;

[MarkupExtensionReturnType(typeof(Geometry))]
public sealed class PIcon : MarkupExtension
{
    private static readonly Dictionary<string, Geometry> PIconStore = [];

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

    internal static Geometry PIconResolve(string name, double size)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        string key = string.Concat(name, "/", size.ToString(CultureInfo.InvariantCulture));
        lock (PIconStore)
        {
            if (PIconStore.TryGetValue(key, out Geometry? cached))
            {
                return cached;
            }

            (Geometry symbol, Rect frame) = PIconLoad(name);
            double scale = size / Math.Max(frame.Width, frame.Height);
            Geometry scaled = symbol.Clone();
            scaled.Transform = new MatrixTransform(scale, 0, 0, scale, -frame.X * scale, -frame.Y * scale);
            scaled.Freeze();
            PIconStore[key] = scaled;
            return scaled;
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
