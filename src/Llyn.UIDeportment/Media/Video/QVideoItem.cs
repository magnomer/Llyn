using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QVideoItem : INotifyPropertyChanged
{
    private CVideoDraft _qVideoItemDraft;
    private bool _qVideoItemPlaying;

    internal QVideoItem(CVideoDraft written)
    {
        ArgumentNullException.ThrowIfNull(written);

        _qVideoItemDraft = written;
    }

    public CStateValue QVideoItemLocation => _qVideoItemDraft.CVideoDraftLocation;

    public CStateValue QVideoItemTimestamp => _qVideoItemDraft.CVideoDraftSpan;

    public CScreen? QVideoItemPreview => _qVideoItemDraft.CVideoDraftScreen;

    public TimeSpan QVideoItemFrom => _qVideoItemDraft.CVideoDraftFrom;

    public TimeSpan? QVideoItemUntil => _qVideoItemDraft.CVideoDraftUntil;

    public bool QVideoItemPlaying
    {
        get => _qVideoItemPlaying;
        set
        {
            if (_qVideoItemPlaying == value)
            {
                return;
            }

            _qVideoItemPlaying = value;
            QVideoItemRaise(nameof(QVideoItemPlaying));
        }
    }

    internal long QVideoItemId => _qVideoItemDraft.CVideoDraftId;

    internal void QVideoItemShow(CVideoDraft written)
    {
        ArgumentNullException.ThrowIfNull(written);

        CVideoDraft held = _qVideoItemDraft;
        _qVideoItemDraft = written;

        if (held.CVideoDraftLocation != written.CVideoDraftLocation)
        {
            QVideoItemRaise(nameof(QVideoItemLocation));
        }

        if (held.CVideoDraftSpan != written.CVideoDraftSpan)
        {
            QVideoItemRaise(nameof(QVideoItemTimestamp));
        }

        if (!Equals(held.CVideoDraftScreen, written.CVideoDraftScreen))
        {
            QVideoItemRaise(nameof(QVideoItemPreview));
        }

        if (held.CVideoDraftFrom != written.CVideoDraftFrom)
        {
            QVideoItemRaise(nameof(QVideoItemFrom));
        }

        if (held.CVideoDraftUntil != written.CVideoDraftUntil)
        {
            QVideoItemRaise(nameof(QVideoItemUntil));
        }
    }

    internal static void QVideoItemRefine(FrameworkElement container, object item, string? changed)
    {
        if (item is not QVideoItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<Border>(container, "PVideoSurface") is Border surface)
        {
            surface.Visibility = QLook.QLookVisibleRead(row.QVideoItemPreview is not null);
        }

        if (QLook.QLookPartFind<FrameworkElement>(container, "PVideoScreen") is FrameworkElement screen)
        {
            QVideoItemAttach(screen, row);
        }

        if (changed is null or nameof(QVideoItemLocation)
            && QLook.QLookPartFind<TextBox>(container, "PVideoLocation") is TextBox location)
        {
            location.Text = row.QVideoItemLocation.CStateValueText;
            location.SetResourceReference(QField.QFieldHintProperty, "Card.LocationHint");
        }

        if (changed is null or nameof(QVideoItemTimestamp)
            && QLook.QLookPartFind<TextBox>(container, "PVideoTimestamp") is TextBox timestamp)
        {
            timestamp.Text = row.QVideoItemTimestamp.CStateValueText;
            timestamp.SetResourceReference(QField.QFieldHintProperty, "Card.TimestampHint");
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PVideoRemoveIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("remove", 12);
        }

        if (ItemsControl.ItemsControlFromItemContainer(container) is ItemsControl list)
        {
            PMedia.PMediaRevealAttach(list);
        }
    }

    internal static void QVideoItemApply(FrameworkElement container, object item, string? _)
    {
        if (QLook.QLookPartFind<FrameworkElement>(container, "PVideoScreen")
            is FrameworkElement { DataContext: QVideoItem row } screen)
        {
            QVideoItemAttach(screen, row);
        }
    }

    private static void QVideoItemAttach(FrameworkElement screen, QVideoItem row)
    {
        QScreen.QScreenIntroduce(screen);
        screen.SetValue(QScreen.QScreenFromProperty, row.QVideoItemFrom);
        screen.SetValue(QScreen.QScreenUntilProperty, row.QVideoItemUntil);
        screen.SetValue(QScreen.QScreenFilmProperty, row.QVideoItemPreview?.CScreenFilm);
        screen.SetValue(QScreen.QScreenAddressProperty, row.QVideoItemPreview?.CScreenAddress);
        screen.SetBinding(
            QScreen.QScreenPlayingProperty,
            new Binding(nameof(QVideoItemPlaying)) { Source = row, Mode = BindingMode.TwoWay });
        screen.SetBinding(
            QScreen.QScreenVolumeProperty,
            new Binding(nameof(PVolumeCatalog.PVolumeCatalogLevel)) { Source = PVolumeCatalog.PVolumeCatalogCurrent });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void QVideoItemRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
