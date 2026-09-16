using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

internal sealed class PImage : INotifyPropertyChanged, PImagePending
{
    private static LEngine? _pImageEngine;

    private string _pImageLocation;
    private bool _pImageUnknown;
    private bool _pImageSeen;
    private ImageSource? _pImagePreview;
    private long _pImageRow;

    internal PImage(LImageDraft written)
    {
        ArgumentNullException.ThrowIfNull(written);

        _pImageRow = written.LImageDraftId;
        _pImageLocation = written.LImageDraftLocation.LStateValueShow();
        _pImageUnknown = written.LImageDraftLocation.LStateValueState == LState.LStateUnknown;
    }

    public string PImageLocation
    {
        get => _pImageLocation;
        set
        {
            string chosen = value ?? string.Empty;
            if (string.Equals(_pImageLocation, chosen, StringComparison.Ordinal))
            {
                return;
            }

            _pImageLocation = chosen;
            PImageUnknown = false;
            PImageRaise(nameof(PImageLocation));
            PImagePreviewUpdate();
        }
    }

    public bool PImageUnknown
    {
        get => _pImageUnknown;
        private set
        {
            if (_pImageUnknown == value)
            {
                return;
            }

            _pImageUnknown = value;
            PImageRaise(nameof(PImageUnknown));
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

    internal LStateWritten PImageLocationRead()
    {
        return new LStateWritten(_pImageLocation, _pImageUnknown);
    }

    internal void PImageShow(LImageDraft written, bool pending)
    {
        ArgumentNullException.ThrowIfNull(written);

        _pImageRow = written.LImageDraftId;
        if (pending || PImageLocationRead().LStateWrittenMatch(written.LImageDraftLocation))
        {
            return;
        }

        _pImageLocation = written.LImageDraftLocation.LStateValueShow();
        PImageRaise(nameof(PImageLocation));
        PImageUnknown = written.LImageDraftLocation.LStateValueState == LState.LStateUnknown;
        PImagePreviewUpdate();
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

    internal static void PImageAttach(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);

        _pImageEngine = engine;
    }

    internal static Uri? PImageAddressRead(string location)
    {
        Uri? address = _pImageEngine?.LEngineLocationResolve(location);
        return address is null || (address.IsFile && !File.Exists(address.LocalPath)) ? null : address;
    }

    private void PImagePreviewUpdate()
    {
        if (!_pImageSeen)
        {
            return;
        }

        Uri? address = PImageAddressRead(_pImageLocation);
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
        catch (IOException)
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
