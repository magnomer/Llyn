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
    private long _pImageRow;

    internal PImage()
        : this(new LImageDraft(LStateValue.LStateValueUnspecified))
    {
    }

    internal PImage(LImageDraft written)
    {
        ArgumentNullException.ThrowIfNull(written);

        _pImageRow = written.LImageDraftId;
        _pImageLocation = written.LImageDraftLocation.LStateValueShow();
        _pImageUnreadable = written.LImageDraftLocation.LStateValueState == LState.LStateUnknown;
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

    internal void PImageIdentityApply(LImageDraft stored)
    {
        ArgumentNullException.ThrowIfNull(stored);
        _pImageRow = stored.LImageDraftId;
    }

    internal LImageDraft PImageDraftRead()
    {
        return new LImageDraft(
            LStateValue.LStateValueResolve(_pImageLocation, _pImageUnreadable), _pImageRow);
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
