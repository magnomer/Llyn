using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PScriptImage : INotifyPropertyChanged, PImagePending
{
    private const double PScriptImageMeasure = 65;

    private const int PScriptImageDecode = 2;

    private const string PScriptImageArea = "Epoch.";

    private readonly string _pScriptImageEpoch;

    private byte[]? _pScriptImageData;

    private BitmapSource? _pScriptImageSource;

    private PScriptImage(byte[] data, string caption, string epoch, double width)
    {
        _pScriptImageData = data;
        _pScriptImageEpoch = epoch;
        PScriptImageCaption = caption;
        PScriptImageWidth = width;
    }

    public BitmapSource? PScriptImageSource
    {
        get => _pScriptImageSource;
        private set
        {
            if (ReferenceEquals(_pScriptImageSource, value))
            {
                return;
            }

            _pScriptImageSource = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PScriptImageSource)));
        }
    }

    public string PScriptImageCaption { get; }

    public string PScriptImageEpoch => _pScriptImageEpoch.Length == 0
        ? string.Empty
        : QLocalizationCatalog.QLocalizationTextFind(PScriptImageArea + _pScriptImageEpoch) ?? string.Empty;

    public double PScriptImageWidth { get; }

    public double PScriptImageHeight => PScriptImageMeasure;

    public event PropertyChangedEventHandler? PropertyChanged;

    internal static void PScriptImageRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not PScriptImage image)
        {
            return;
        }

        if (QLook.QLookPartFind<PImageLazy>(container, "PScriptLazy") is PImageLazy lazy)
        {
            lazy.PImageLazyRow = image;
        }

        if (QLook.QLookPartFind<Rectangle>(container, "PScriptShape") is Rectangle shape)
        {
            shape.Width = image.PScriptImageWidth;
            shape.Height = image.PScriptImageHeight;
            shape.OpacityMask = new ImageBrush(image.PScriptImageSource) { Stretch = Stretch.Uniform };
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PScriptEpoch") is TextBlock epoch)
        {
            epoch.Text = image.PScriptImageEpoch;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PScriptCaption") is TextBlock caption)
        {
            caption.Text = image.PScriptImageCaption;
        }
    }

    internal static PScriptImage? PScriptImageCreate(CScriptImage image)
    {
        ArgumentNullException.ThrowIfNull(image);

        if (PScriptShapeRead(image.CScriptImageData) is not Size shape)
        {
            return null;
        }

        double width = shape.Height > 0
            ? Math.Round(PScriptImageMeasure * shape.Width / shape.Height)
            : PScriptImageMeasure;
        return new PScriptImage(
            image.CScriptImageData, image.CScriptImageCaption, image.CScriptImageEpoch, width);
    }

    internal void PScriptImageRaise()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PScriptImageEpoch)));
    }

    public void PImageLoad()
    {
        if (_pScriptImageData is not byte[] data)
        {
            return;
        }

        _pScriptImageData = null;
        PScriptImageSource = PScriptImageRead(data);
    }

    private static Size? PScriptShapeRead(byte[] data)
    {
        try
        {
            using MemoryStream stream = new(data);
            BitmapDecoder decoder = BitmapDecoder.Create(
                stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
            BitmapFrame frame = decoder.Frames[0];
            return new Size(frame.PixelWidth, frame.PixelHeight);
        }
        catch (Exception exception) when (exception is NotSupportedException or IOException or ArgumentException)
        {
            return null;
        }
    }

    private static BitmapSource? PScriptImageRead(byte[] data)
    {
        try
        {
            using MemoryStream stream = new(data);
            BitmapImage loaded = new();
            loaded.BeginInit();
            loaded.StreamSource = stream;
            loaded.DecodePixelHeight = (int)PScriptImageMeasure * PScriptImageDecode;
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
