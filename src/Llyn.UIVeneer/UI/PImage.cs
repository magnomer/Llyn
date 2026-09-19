using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

internal sealed class PImage : INotifyPropertyChanged, PImagePending
{
    private readonly LEngine _lEngine;

    private LStateValue _pImageLocation;
    private bool _pImageSeen;
    private ImageSource? _pImagePreview;
    private long _pImageRow;

    internal PImage(LEngine engine, LImageDraft written)
    {
        ArgumentNullException.ThrowIfNull(engine);
        ArgumentNullException.ThrowIfNull(written);

        _lEngine = engine;
        _pImageRow = written.LImageDraftId;
        _pImageLocation = written.LImageDraftLocation;
    }

    public LStateValue PImageLocation
    {
        get => _pImageLocation;
        private set
        {
            if (_pImageLocation == value)
            {
                return;
            }

            _pImageLocation = value;
            PImageRaise(nameof(PImageLocation));
            PImagePreviewUpdate();
        }
    }

    public ImageSource? PImagePreview
    {
        get => _pImagePreview;
        private set
        {
            if (ReferenceEquals(_pImagePreview, value))
            {
                return;
            }

            _pImagePreview = value;
            PImageRaise(nameof(PImagePreview));
        }
    }

    internal long PImageId => _pImageRow;

    public void PImageLoad()
    {
        if (_pImageSeen)
        {
            return;
        }

        _pImageSeen = true;
        PImagePreviewUpdate();
    }

    internal void PImageShow(LImageDraft written)
    {
        ArgumentNullException.ThrowIfNull(written);

        _pImageRow = written.LImageDraftId;
        PImageLocation = written.LImageDraftLocation;
    }

    internal static string? PImageOpen(Window owner)
    {
        Microsoft.Win32.OpenFileDialog dialog = new()
        {
            Title = "Choose an image",
            Filter = "Image files|*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.webp;*.tif;*.tiff|All files|*.*",
            CheckFileExists = true,
        };

        return dialog.ShowDialog(owner) == true ? dialog.FileName : null;
    }

    private void PImagePreviewUpdate()
    {
        if (!_pImageSeen)
        {
            return;
        }

        Uri? address = _lEngine.LEngineLocationRead(_pImageLocation.LStateValueShow());
        if (address is null)
        {
            PImagePreview = null;
            return;
        }

        try
        {
            BitmapImage loaded = new();
            loaded.BeginInit();
            loaded.UriSource = address;
            loaded.CacheOption = address.IsFile ? BitmapCacheOption.OnLoad : BitmapCacheOption.Default;
            loaded.EndInit();
            PImagePreview = loaded;
        }
        catch (NotSupportedException)
        {
            PImagePreview = null;
        }
        catch (System.IO.IOException)
        {
            PImagePreview = null;
        }
        catch (UriFormatException)
        {
            PImagePreview = null;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PImageRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
