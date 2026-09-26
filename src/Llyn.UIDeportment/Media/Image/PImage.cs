using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PImage : INotifyPropertyChanged, PImagePending
{
    private readonly LWindow _lWindow;

    private LStateValue _pImageLocation;
    private bool _pImageSeen;
    private ImageSource? _pImagePreview;
    private long _pImageRow;

    internal PImage(LWindow window, LImageDraft written)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(written);

        _lWindow = window;
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

    internal static void PImageRowApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PImage row)
        {
            return;
        }

        if (PLook.PLookPartFind<PImageLazy>(container, "PImageHold") is PImageLazy hold)
        {
            hold.PImageLazyRow = row;
        }

        if (PLook.PLookPartFind<Border>(container, "PImageFrame") is Border frame)
        {
            frame.Visibility = PLook.PLookVisibleRead(row.PImagePreview is not null);
        }

        if (PLook.PLookPartFind<Image>(container, "PImagePreview") is Image preview)
        {
            preview.Source = row.PImagePreview;
        }

        if (PLook.PLookPartFind<TextBox>(container, "PImageLocation") is TextBox location)
        {
            PStateConverter state = new();
            CultureInfo culture = CultureInfo.CurrentCulture;
            location.Text = (string)state.Convert(row.PImageLocation, typeof(string), string.Empty, culture);
            location.Tag = state.Convert(
                [
                    row.PImageLocation,
                    PLocalizationCatalog.PLocalizationTextRead("Display.Unknown"),
                    PLocalizationCatalog.PLocalizationTextRead("Card.LocationHint"),
                ],
                typeof(string),
                string.Empty,
                culture);
        }

        if (PLook.PLookPartFind<PIconImage>(container, "PImageRemoveIcon") is PIconImage icon)
        {
            icon.PIconSource = PIcon.PIconResolve("remove", 12);
        }

        if (ItemsControl.ItemsControlFromItemContainer(container) is ItemsControl list)
        {
            PMedia.PMediaRevealAttach(list);
        }
    }

    internal static void PImageLineApply(FrameworkElement container, object item, string? _)
    {
        if (PLook.PLookPartFind<PImageLazy>(container, "PImageHold") is not PImageLazy { DataContext: PImage row } hold
            || PLook.PLookPartFind<Image>(container, "PImagePreview") is not Image preview)
        {
            return;
        }

        hold.PImageLazyRow = row;
        preview.Source = row.PImagePreview;
        row.PropertyChanged += (_, _) => preview.Source = row.PImagePreview;
    }

    private void PImagePreviewUpdate()
    {
        if (!_pImageSeen)
        {
            return;
        }

        Uri? address = _lWindow.LWindowLocationRead(_pImageLocation.LStateValueShow());
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
