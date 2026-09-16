using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PScriptImage : INotifyPropertyChanged, PImagePending
{
    private const double PScriptImageMeasure = 65;

    private const int PScriptImageDecode = 2;

    private byte[]? _pScriptImageData;

    private BitmapSource? _pScriptImageSource;

    private PScriptImage(byte[] data, string caption, double width)
    {
        _pScriptImageData = data;
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

    public double PScriptImageWidth { get; }

    public double PScriptImageHeight => PScriptImageMeasure;

    public event PropertyChangedEventHandler? PropertyChanged;

    internal static PScriptImage? PScriptImageCreate(LScriptImage image)
    {
        ArgumentNullException.ThrowIfNull(image);

        if (PScriptShapeRead(image.LScriptImageData) is not Size shape)
        {
            return null;
        }

        double width = shape.Height > 0
            ? Math.Round(PScriptImageMeasure * shape.Width / shape.Height)
            : PScriptImageMeasure;
        return new PScriptImage(image.LScriptImageData, image.LScriptImageCaption, width);
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
