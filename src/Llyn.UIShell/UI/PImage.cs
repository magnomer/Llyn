using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PImage : INotifyPropertyChanged
{
    private string _pImageLocation;
    private bool _pImageUnreadable;
    private ImageSource? _pImagePreview;

    internal PImage()
        : this(LStateValue.LStateValueUnspecified)
    {
    }

    internal PImage(LStateValue location)
    {
        ArgumentNullException.ThrowIfNull(location);

        _pImageLocation = location.LStateValueShow();
        _pImageUnreadable = location.LStateValueState == LState.LStateUnknown;
        PImagePreviewUpdate();
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
            PImageUnreadable = false;
            PImageRaise(nameof(PImageLocation));
            PImagePreviewUpdate();
        }
    }

    public bool PImageUnreadable
    {
        get => _pImageUnreadable;
        private set
        {
            if (_pImageUnreadable == value)
            {
                return;
            }

            _pImageUnreadable = value;
            PImageRaise(nameof(PImageUnreadable));
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

    internal LStateValue PImageLocationRead()
    {
        return _pImageUnreadable
            ? LStateValue.LStateValueUnknown
            : LStateValue.LStateValueRead(_pImageLocation);
    }

    internal static Uri? PImageAddressRead(string location)
    {
        string trimmed = (location ?? string.Empty).Trim();
        if (trimmed.Length == 0)
        {
            return null;
        }

        if (Uri.TryCreate(trimmed, UriKind.Absolute, out Uri? absolute))
        {
            return absolute;
        }

        try
        {
            return File.Exists(trimmed) ? new Uri(Path.GetFullPath(trimmed)) : null;
        }
        catch (ArgumentException)
        {
            return null;
        }
        catch (IOException)
        {
            return null;
        }
    }

    private void PImagePreviewUpdate()
    {
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
