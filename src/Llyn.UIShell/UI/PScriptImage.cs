using System;
using System.IO;
using System.Windows.Media.Imaging;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PScriptImage
{
    private const double PScriptImageMeasure = 65;

    private PScriptImage(BitmapSource source, string caption, double width)
    {
        PScriptImageSource = source;
        PScriptImageCaption = caption;
        PScriptImageWidth = width;
    }

    public BitmapSource PScriptImageSource { get; }

    public string PScriptImageCaption { get; }

    public double PScriptImageWidth { get; }

    public double PScriptImageHeight => PScriptImageMeasure;

    internal static PScriptImage? PScriptImageCreate(LScriptImage image)
    {
        ArgumentNullException.ThrowIfNull(image);

        BitmapSource? source = PScriptImageRead(image.LScriptImageData);
        if (source is null)
        {
            return null;
        }

        double width = source.PixelHeight > 0
            ? Math.Round(PScriptImageMeasure * source.PixelWidth / source.PixelHeight)
            : PScriptImageMeasure;
        return new PScriptImage(source, image.LScriptImageCaption, width);
    }

    private static BitmapSource? PScriptImageRead(byte[] data)
    {
        try
        {
            using MemoryStream stream = new(data);
            BitmapImage loaded = new();
            loaded.BeginInit();
            loaded.StreamSource = stream;
            loaded.CacheOption = BitmapCacheOption.OnLoad;
            loaded.EndInit();
            loaded.Freeze();
            return loaded;
        }
        catch (Exception exception) when (exception is NotSupportedException or IOException or ArgumentException)
        {
            return null;
        }
    }
}
