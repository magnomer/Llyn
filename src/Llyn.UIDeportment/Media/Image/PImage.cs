using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PImage : INotifyPropertyChanged, PImagePending
{
    private readonly CAtelier _pImageAtelier;

    private CStateWording _pImageLocation;
    private bool _pImageSeen;
    private ImageSource? _pImagePreview;
    private long _pImageRow;

    internal PImage(CAtelier atelier, CImageDraft written)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(written);

        _pImageAtelier = atelier;
        _pImageRow = written.CImageDraftId;
        _pImageLocation = written.CImageDraftWording;
    }

    public CStateWording PImageLocation
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

    internal void PImageShow(CImageDraft written)
    {
        ArgumentNullException.ThrowIfNull(written);

        _pImageRow = written.CImageDraftId;
        PImageLocation = written.CImageDraftWording;
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

        if (QLook.QLookPartFind<PImageLazy>(container, "PImageHold") is PImageLazy hold)
        {
            hold.PImageLazyRow = row;
        }

        if (QLook.QLookPartFind<Border>(container, "PImageFrame") is Border frame)
        {
            frame.Visibility = QLook.QLookVisibleRead(row.PImagePreview is not null);
        }

        if (QLook.QLookPartFind<Image>(container, "PImagePreview") is Image preview)
        {
            preview.Source = row.PImagePreview;
        }

        if (QLook.QLookPartFind<TextBox>(container, "PImageLocation") is TextBox location)
        {
            location.Text = row.PImageLocation.CStateWordingText;
            QStateConverter.QStateHintRefine(location, QField.QFieldHintProperty, row.PImageLocation);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PImageRemoveIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("remove", 12);
        }

        if (ItemsControl.ItemsControlFromItemContainer(container) is ItemsControl list)
        {
            PMedia.PMediaRevealAttach(list);
        }
    }

    internal static void PImageLineApply(FrameworkElement container, object item, string? _)
    {
        if (QLook.QLookPartFind<PImageLazy>(container, "PImageHold") is not PImageLazy { DataContext: PImage row } hold
            || QLook.QLookPartFind<Image>(container, "PImagePreview") is not Image preview)
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

        Uri? address = _pImageAtelier.CAtelierLocationRead(_pImageLocation.CStateWordingText);
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
