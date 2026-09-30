using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PVideo : INotifyPropertyChanged
{
    private CStateValue _pVideoLocation = CStateValue.CStateValueEmpty;
    private CStateValue _pVideoTimestamp = CStateValue.CStateValueEmpty;
    private CScreen? _pVideoPreview;
    private TimeSpan _pVideoFrom = TimeSpan.Zero;
    private TimeSpan? _pVideoUntil;
    private bool _pVideoPlaying;
    private long _pVideoRow;

    internal PVideo(CVideoDraft written)
    {
        ArgumentNullException.ThrowIfNull(written);

        _pVideoRow = written.CVideoDraftId;

        PVideoLocation = written.CVideoDraftLocation;
        PVideoTimestamp = written.CVideoDraftSpan;
        PVideoPreview = written.CVideoDraftScreen;
        PVideoFrom = written.CVideoDraftFrom;
        PVideoUntil = written.CVideoDraftUntil;
    }

    public CStateValue PVideoLocation
    {
        get => _pVideoLocation;
        private set
        {
            if (_pVideoLocation == value)
            {
                return;
            }

            _pVideoLocation = value;
            PVideoRaise(nameof(PVideoLocation));
        }
    }

    public CStateValue PVideoTimestamp
    {
        get => _pVideoTimestamp;
        private set
        {
            if (_pVideoTimestamp == value)
            {
                return;
            }

            _pVideoTimestamp = value;
            PVideoRaise(nameof(PVideoTimestamp));
        }
    }

    public CScreen? PVideoPreview
    {
        get => _pVideoPreview;
        private set
        {
            if (Equals(_pVideoPreview, value))
            {
                return;
            }

            _pVideoPreview = value;
            PVideoRaise(nameof(PVideoPreview));
        }
    }

    public TimeSpan PVideoFrom
    {
        get => _pVideoFrom;
        private set
        {
            if (_pVideoFrom == value)
            {
                return;
            }

            _pVideoFrom = value;
            PVideoRaise(nameof(PVideoFrom));
        }
    }

    public TimeSpan? PVideoUntil
    {
        get => _pVideoUntil;
        private set
        {
            if (_pVideoUntil == value)
            {
                return;
            }

            _pVideoUntil = value;
            PVideoRaise(nameof(PVideoUntil));
        }
    }

    public bool PVideoPlaying
    {
        get => _pVideoPlaying;
        set
        {
            if (_pVideoPlaying == value)
            {
                return;
            }

            _pVideoPlaying = value;
            PVideoRaise(nameof(PVideoPlaying));
        }
    }

    internal long PVideoId => _pVideoRow;

    internal static string? PVideoOpen(Window owner)
    {
        Microsoft.Win32.OpenFileDialog dialog = new()
        {
            Title = "Choose a video",
            Filter = "Video files|*.mp4;*.m4v;*.mov;*.avi;*.wmv;*.mkv;*.webm|All files|*.*",
            CheckFileExists = true,
        };

        return dialog.ShowDialog(owner) == true ? dialog.FileName : null;
    }

    internal void PVideoShow(CVideoDraft written)
    {
        ArgumentNullException.ThrowIfNull(written);

        _pVideoRow = written.CVideoDraftId;
        PVideoLocation = written.CVideoDraftLocation;
        PVideoTimestamp = written.CVideoDraftSpan;
        PVideoPreview = written.CVideoDraftScreen;
        PVideoFrom = written.CVideoDraftFrom;
        PVideoUntil = written.CVideoDraftUntil;
    }

    internal static void PVideoItemRefine(FrameworkElement container, object item, string? changed)
    {
        if (item is not PVideo row)
        {
            return;
        }

        if (QLook.QLookPartFind<Border>(container, "PVideoSurface") is Border surface)
        {
            surface.Visibility = QLook.QLookVisibleRead(row.PVideoPreview is not null);
        }

        if (QLook.QLookPartFind<PScreen>(container, "PVideoScreen") is PScreen screen)
        {
            PVideoScreenApply(screen, row);
        }

        if (changed is null or nameof(PVideoLocation)
            && QLook.QLookPartFind<TextBox>(container, "PVideoLocation") is TextBox location)
        {
            location.Text = row.PVideoLocation.CStateValueText;
            location.SetResourceReference(QField.QFieldHintProperty, "Card.LocationHint");
        }

        if (changed is null or nameof(PVideoTimestamp)
            && QLook.QLookPartFind<TextBox>(container, "PVideoTimestamp") is TextBox timestamp)
        {
            timestamp.Text = row.PVideoTimestamp.CStateValueText;
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

    internal static void PVideoLineApply(FrameworkElement container, object item, string? _)
    {
        if (QLook.QLookPartFind<PScreen>(container, "PVideoScreen") is PScreen { DataContext: PVideo row } screen)
        {
            PVideoScreenApply(screen, row);
        }
    }

    private static void PVideoScreenApply(PScreen screen, PVideo row)
    {
        screen.PScreenFrom = row.PVideoFrom;
        screen.PScreenUntil = row.PVideoUntil;
        screen.PScreenFilm = row.PVideoPreview?.CScreenFilm;
        screen.PScreenAddress = row.PVideoPreview?.CScreenAddress;
        screen.SetBinding(
            PScreen.PScreenPlayingProperty,
            new Binding(nameof(PVideoPlaying)) { Source = row, Mode = BindingMode.TwoWay });
        screen.SetBinding(
            PScreen.PScreenVolumeProperty,
            new Binding(nameof(PVolumeCatalog.PVolumeCatalogLevel)) { Source = PVolumeCatalog.PVolumeCatalogCurrent });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PVideoRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
