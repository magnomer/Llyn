using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PVideo : INotifyPropertyChanged
{
    private readonly LWindow _lWindow;

    private CStateValue _pVideoLocation = CStateValue.CStateValueEmpty;
    private CStateValue _pVideoTimestamp = CStateValue.CStateValueEmpty;
    private Uri? _pVideoPreview;
    private TimeSpan _pVideoFrom = TimeSpan.Zero;
    private TimeSpan? _pVideoUntil;
    private bool _pVideoPlaying;
    private long _pVideoRow;

    internal PVideo(LWindow window, CVideoDraft written)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(written);

        _lWindow = window;
        _pVideoRow = written.CVideoDraftId;

        PVideoLocation = written.CVideoDraftLocation;
        PVideoTimestamp = written.CVideoDraftSpan;
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
            PVideoPreview = _lWindow.LWindowWorkspace.QWorkspaceLocationRead(value.CStateValueText);
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
            PVideoTimestampApply(value.CStateValueText);
        }
    }

    public Uri? PVideoPreview
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
    }

    internal static void PVideoRowApply(FrameworkElement container, object item, string? changed)
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

        QStateConverter state = new();
        if (changed is null or nameof(PVideoLocation)
            && QLook.QLookPartFind<TextBox>(container, "PVideoLocation") is TextBox location)
        {
            location.Text = (string)state.Convert(
                row.PVideoLocation, typeof(string), string.Empty, CultureInfo.CurrentCulture);
            location.SetResourceReference(QField.QFieldHintProperty, "Card.LocationHint");
        }

        if (changed is null or nameof(PVideoTimestamp)
            && QLook.QLookPartFind<TextBox>(container, "PVideoTimestamp") is TextBox timestamp)
        {
            timestamp.Text = (string)state.Convert(
                row.PVideoTimestamp, typeof(string), string.Empty, CultureInfo.CurrentCulture);
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
        screen.PScreenAddress = row.PVideoPreview;
        screen.SetBinding(
            PScreen.PScreenPlayingProperty,
            new Binding(nameof(PVideoPlaying)) { Source = row, Mode = BindingMode.TwoWay });
        screen.SetBinding(
            PScreen.PScreenVolumeProperty,
            new Binding(nameof(PVolumeCatalog.PVolumeCatalogLevel)) { Source = PVolumeCatalog.PVolumeCatalogCurrent });
    }

    private void PVideoTimestampApply(string timestamp)
    {
        string[] parts = timestamp.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        PVideoFrom = parts.Length > 0 && PVideoMomentParse(parts[0]) is TimeSpan from ? from : TimeSpan.Zero;
        PVideoUntil = parts.Length > 1 ? PVideoMomentParse(parts[1]) : null;
    }

    private static TimeSpan? PVideoMomentParse(string moment)
    {
        string[] fields = moment.Split(':', StringSplitOptions.TrimEntries);
        if (fields.Length is < 2 or > 3)
        {
            return null;
        }

        TimeSpan parsed = TimeSpan.Zero;
        foreach (string field in fields)
        {
            if (!int.TryParse(field, NumberStyles.None, CultureInfo.InvariantCulture, out int value))
            {
                return null;
            }

            parsed = (parsed * 60) + TimeSpan.FromSeconds(value);
        }

        return parsed;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PVideoRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
