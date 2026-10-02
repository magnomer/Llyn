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

internal sealed class QScriptImage : INotifyPropertyChanged, QImagePending
{
    private const double QScriptImageMeasure = 65;

    private const int QScriptImageDecode = 2;

    private const string QScriptImageArea = "Epoch.";

    private readonly string _qScriptImageEpoch;

    private readonly Action<Exception> _qScriptImageFailure;

    private byte[]? _qScriptImageData;

    private BitmapSource? _qScriptImageSource;

    private QScriptImage(byte[] data, string caption, string epoch, double width, Action<Exception> failure)
    {
        _qScriptImageData = data;
        _qScriptImageFailure = failure;
        _qScriptImageEpoch = epoch;
        QScriptImageCaption = caption;
        QScriptImageWidth = width;
    }

    public BitmapSource? QScriptImageSource
    {
        get => _qScriptImageSource;
        private set
        {
            if (ReferenceEquals(_qScriptImageSource, value))
            {
                return;
            }

            _qScriptImageSource = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QScriptImageSource)));
        }
    }

    public string QScriptImageCaption { get; }

    public string QScriptImageEpoch => _qScriptImageEpoch.Length == 0
        ? string.Empty
        : QLocalizationCatalog.QLocalizationTextFind(QScriptImageArea + _qScriptImageEpoch) ?? string.Empty;

    public double QScriptImageWidth { get; }

    public double QScriptImageHeight => QScriptImageMeasure;

    public event PropertyChangedEventHandler? PropertyChanged;

    internal static void QScriptImageRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QScriptImage image)
        {
            return;
        }

        if (QLook.QLookPartFind<QImageLazy>(container, "PScriptLazy") is QImageLazy lazy)
        {
            lazy.QImageLazyRow = image;
        }

        if (QLook.QLookPartFind<Rectangle>(container, "PScriptShape") is Rectangle shape)
        {
            shape.Width = image.QScriptImageWidth;
            shape.Height = image.QScriptImageHeight;
            shape.OpacityMask = new ImageBrush(image.QScriptImageSource) { Stretch = Stretch.Uniform };
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PScriptEpoch") is TextBlock epoch)
        {
            epoch.Text = image.QScriptImageEpoch;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PScriptCaption") is TextBlock caption)
        {
            caption.Text = image.QScriptImageCaption;
        }
    }

    internal static QScriptImage? QScriptImageCreate(CScriptImage image, Action<Exception> failure)
    {
        ArgumentNullException.ThrowIfNull(image);
        ArgumentNullException.ThrowIfNull(failure);

        if (QScriptShapeRead(image.CScriptImageData, failure) is not Size shape)
        {
            return null;
        }

        double width = shape.Height > 0
            ? Math.Round(QScriptImageMeasure * shape.Width / shape.Height)
            : QScriptImageMeasure;
        return new QScriptImage(
            image.CScriptImageData, image.CScriptImageCaption, image.CScriptImageEpoch, width, failure);
    }

    internal void QScriptImageRaise()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QScriptImageEpoch)));
    }

    public void QImagePendingLoad()
    {
        if (_qScriptImageData is not byte[] data)
        {
            return;
        }

        _qScriptImageData = null;
        QScriptImageSource = QScriptImageRead(data, _qScriptImageFailure);
    }

    private static Size? QScriptShapeRead(byte[] data, Action<Exception> failure)
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
            failure(exception);
            return null;
        }
    }

    private static BitmapSource? QScriptImageRead(byte[] data, Action<Exception> failure)
    {
        try
        {
            using MemoryStream stream = new(data);
            BitmapImage loaded = new();
            loaded.BeginInit();
            loaded.StreamSource = stream;
            loaded.DecodePixelHeight = (int)QScriptImageMeasure * QScriptImageDecode;
            loaded.CacheOption = BitmapCacheOption.OnLoad;
            loaded.EndInit();
            loaded.Freeze();
            return loaded;
        }
        catch (Exception exception) when (exception is NotSupportedException or IOException or ArgumentException)
        {
            failure(exception);
            return null;
        }
    }
}
