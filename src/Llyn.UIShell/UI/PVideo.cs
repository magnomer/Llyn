using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PVideo : INotifyPropertyChanged
{
    private LStateValue _pVideoLocation = LStateValue.LStateValueUnspecified;
    private LStateValue _pVideoTimestamp = LStateValue.LStateValueUnspecified;
    private Uri? _pVideoPreview;
    private TimeSpan _pVideoFrom = TimeSpan.Zero;
    private TimeSpan? _pVideoUntil;
    private bool _pVideoPlaying;
    private long _pVideoRow;

    internal PVideo(LVideoDraft written)
    {
        ArgumentNullException.ThrowIfNull(written);

        _pVideoRow = written.LVideoDraftId;

        PVideoLocation = written.LVideoDraftLocation;
        PVideoTimestamp = written.LVideoDraftSpan;
    }

    public LStateValue PVideoLocation
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
            PVideoPreview = PImage.PImageAddressRead(value.LStateValueShow());
        }
    }

    public LStateValue PVideoTimestamp
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
            PVideoTimestampApply(value.LStateValueShow());
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

    internal void PVideoShow(LVideoDraft written)
    {
        ArgumentNullException.ThrowIfNull(written);

        _pVideoRow = written.LVideoDraftId;
        PVideoLocation = written.LVideoDraftLocation;
        PVideoTimestamp = written.LVideoDraftSpan;
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
