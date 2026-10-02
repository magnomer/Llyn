using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QVideoItem : INotifyPropertyChanged
{
    private CStateValue _qVideoItemLocation = CStateValue.CStateValueEmpty;
    private CStateValue _qVideoItemTimestamp = CStateValue.CStateValueEmpty;
    private CScreen? _qVideoItemPreview;
    private TimeSpan _qVideoItemFrom = TimeSpan.Zero;
    private TimeSpan? _qVideoItemUntil;
    private bool _qVideoItemPlaying;
    private long _qVideoItemRow;

    internal QVideoItem(CVideoDraft written)
    {
        ArgumentNullException.ThrowIfNull(written);

        _qVideoItemRow = written.CVideoDraftId;

        QVideoItemLocation = written.CVideoDraftLocation;
        QVideoItemTimestamp = written.CVideoDraftSpan;
        QVideoItemPreview = written.CVideoDraftScreen;
        QVideoItemFrom = written.CVideoDraftFrom;
        QVideoItemUntil = written.CVideoDraftUntil;
    }

    public CStateValue QVideoItemLocation
    {
        get => _qVideoItemLocation;
        private set
        {
            if (_qVideoItemLocation == value)
            {
                return;
            }

            _qVideoItemLocation = value;
            QVideoItemRaise(nameof(QVideoItemLocation));
        }
    }

    public CStateValue QVideoItemTimestamp
    {
        get => _qVideoItemTimestamp;
        private set
        {
            if (_qVideoItemTimestamp == value)
            {
                return;
            }

            _qVideoItemTimestamp = value;
            QVideoItemRaise(nameof(QVideoItemTimestamp));
        }
    }

    public CScreen? QVideoItemPreview
    {
        get => _qVideoItemPreview;
        private set
        {
            if (Equals(_qVideoItemPreview, value))
            {
                return;
            }

            _qVideoItemPreview = value;
            QVideoItemRaise(nameof(QVideoItemPreview));
        }
    }

    public TimeSpan QVideoItemFrom
    {
        get => _qVideoItemFrom;
        private set
        {
            if (_qVideoItemFrom == value)
            {
                return;
            }

            _qVideoItemFrom = value;
            QVideoItemRaise(nameof(QVideoItemFrom));
        }
    }

    public TimeSpan? QVideoItemUntil
    {
        get => _qVideoItemUntil;
        private set
        {
            if (_qVideoItemUntil == value)
            {
                return;
            }

            _qVideoItemUntil = value;
            QVideoItemRaise(nameof(QVideoItemUntil));
        }
    }

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

    internal long QVideoItemId => _qVideoItemRow;

    internal static string? QVideoItemOpen(Window owner)
    {
        Microsoft.Win32.OpenFileDialog dialog = new()
        {
            Title = "Choose a video",
            Filter = "Video files|*.mp4;*.m4v;*.mov;*.avi;*.wmv;*.mkv;*.webm|All files|*.*",
            CheckFileExists = true,
        };

        return dialog.ShowDialog(owner) == true ? dialog.FileName : null;
    }

    internal void QVideoItemShow(CVideoDraft written)
    {
        ArgumentNullException.ThrowIfNull(written);

        _qVideoItemRow = written.CVideoDraftId;
        QVideoItemLocation = written.CVideoDraftLocation;
        QVideoItemTimestamp = written.CVideoDraftSpan;
        QVideoItemPreview = written.CVideoDraftScreen;
        QVideoItemFrom = written.CVideoDraftFrom;
        QVideoItemUntil = written.CVideoDraftUntil;
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
