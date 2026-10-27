using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QImageItem : INotifyPropertyChanged, QImagePending
{
    private CStateWording _qImageItemLocation;
    private Uri? _qImageItemAddress;
    private string _qImageItemName;
    private bool _qImageItemSeen;
    private ImageSource? _qImageItemPreview;
    private readonly long _qImageItemRow;

    internal QImageItem(CImageDraft written, bool collocated = false, long? holder = null)
    {
        ArgumentNullException.ThrowIfNull(written);

        QImageItemCollocated = collocated;
        QImageItemHolder = holder;
        _qImageItemRow = written.CImageDraftId;
        _qImageItemLocation = written.CImageDraftWording;
        _qImageItemAddress = written.CImageDraftAddress;
        _qImageItemName = written.CImageDraftName;
    }

    public CStateWording QImageItemLocation
    {
        get => _qImageItemLocation;
        private set
        {
            if (_qImageItemLocation == value)
            {
                return;
            }

            _qImageItemLocation = value;
            QImageItemRaise(nameof(QImageItemLocation));
        }
    }

    public ImageSource? QImageItemPreview
    {
        get => _qImageItemPreview;
        private set
        {
            if (ReferenceEquals(_qImageItemPreview, value))
            {
                return;
            }

            _qImageItemPreview = value;
            QImageItemRaise(nameof(QImageItemPreview));
        }
    }

    internal long QImageItemId => _qImageItemRow;

    internal bool QImageItemCollocated { get; }

    internal long? QImageItemHolder { get; }

    internal string QImageItemName => _qImageItemName;

    public void QImagePendingLoad()
    {
        if (_qImageItemSeen)
        {
            return;
        }

        _qImageItemSeen = true;
        QImageItemBuild();
    }

    internal void QImageItemShow(CImageDraft written)
    {
        ArgumentNullException.ThrowIfNull(written);

        _qImageItemName = written.CImageDraftName;
        QImageItemLocation = written.CImageDraftWording;
        if (Equals(_qImageItemAddress, written.CImageDraftAddress))
        {
            return;
        }

        _qImageItemAddress = written.CImageDraftAddress;
        QImageItemBuild();
    }

    internal static void QImageItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QImageItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<QImageLazy>(container, "PImageHold") is QImageLazy hold)
        {
            hold.QImageLazyRow = row;
        }

        if (QLook.QLookPartFind<Border>(container, "PImageFrame") is Border frame)
        {
            frame.Visibility = QLook.QLookVisibleRead(row.QImageItemPreview is not null);
        }

        if (QLook.QLookPartFind<Image>(container, "PImagePreview") is Image preview)
        {
            preview.Source = row.QImageItemPreview;
            System.Windows.Automation.AutomationProperties.SetName(preview, row.QImageItemName);
        }

        if (QLook.QLookPartFind<TextBox>(container, "PImageLocation") is TextBox location)
        {
            location.Text = row.QImageItemLocation.CStateWordingText;
            QStateConverter.QStateHintRefine(location, QField.QFieldHintProperty, row.QImageItemLocation);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PImageRemoveIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("remove", 12);
        }

        if (ItemsControl.ItemsControlFromItemContainer(container) is ItemsControl list)
        {
            QMedia.QMediaRevealAttach(list);
        }
    }

    internal static void QImageItemApply(FrameworkElement container, object item, string? _)
    {
        if (QLook.QLookPartFind<QImageLazy>(container, "PImageHold")
                is not QImageLazy { DataContext: QImageItem row } hold
            || QLook.QLookPartFind<Image>(container, "PImagePreview") is not Image preview)
        {
            return;
        }

        hold.QImageLazyRow = row;
        preview.Source = row.QImageItemPreview;
        System.Windows.Automation.AutomationProperties.SetName(preview, row.QImageItemName);
        row.PropertyChanged += (_, _) =>
        {
            preview.Source = row.QImageItemPreview;
            System.Windows.Automation.AutomationProperties.SetName(preview, row.QImageItemName);
        };
    }

    private void QImageItemBuild()
    {
        if (!_qImageItemSeen)
        {
            return;
        }

        if (_qImageItemAddress is not Uri address)
        {
            QImageItemPreview = null;
            return;
        }

        try
        {
            BitmapImage loaded = new();
            loaded.BeginInit();
            loaded.UriSource = address;
            loaded.CacheOption = address.IsFile ? BitmapCacheOption.OnLoad : BitmapCacheOption.Default;
            loaded.EndInit();
            QImageItemPreview = loaded;
        }
        catch (NotSupportedException)
        {
            QImageItemPreview = null;
        }
        catch (System.IO.IOException)
        {
            QImageItemPreview = null;
        }
        catch (UriFormatException)
        {
            QImageItemPreview = null;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void QImageItemRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
