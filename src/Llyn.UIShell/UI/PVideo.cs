using System;
using System.ComponentModel;
using System.Globalization;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PVideo : INotifyPropertyChanged
{
    private string _pVideoLocation = string.Empty;
    private bool _pVideoUnreadable;
    private string _pVideoTimestamp = string.Empty;
    private Uri? _pVideoPreview;
    private TimeSpan _pVideoFrom = TimeSpan.Zero;
    private TimeSpan? _pVideoUntil;
    private bool _pVideoPlaying = true;

    internal PVideo()
    {
    }

    internal PVideo(LStateValue location)
    {
        ArgumentNullException.ThrowIfNull(location);

        PVideoLocation = location.LStateValueShow();
        PVideoUnreadable = location.LStateValueState == LState.LStateUnknown;
    }

    public bool PVideoUnreadable
    {
        get => _pVideoUnreadable;
        private set
        {
            if (_pVideoUnreadable == value)
            {
                return;
            }

            _pVideoUnreadable = value;
            PVideoRaise(nameof(PVideoUnreadable));
        }
    }

    public string PVideoLocation
    {
        get => _pVideoLocation;
        set
        {
            string chosen = value ?? string.Empty;
            if (string.Equals(_pVideoLocation, chosen, StringComparison.Ordinal))
            {
                return;
            }

            _pVideoLocation = chosen;
            PVideoUnreadable = false;
            PVideoRaise(nameof(PVideoLocation));
            PVideoPreview = PImage.PImageAddressRead(chosen);
        }
    }

    public string PVideoTimestamp
    {
        get => _pVideoTimestamp;
        set
        {
            string chosen = value ?? string.Empty;
            if (string.Equals(_pVideoTimestamp, chosen, StringComparison.Ordinal))
            {
                return;
            }

            _pVideoTimestamp = chosen;
            PVideoRaise(nameof(PVideoTimestamp));
            PVideoTimestampApply(chosen);
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

    internal LStateValue PVideoLocationRead()
    {
        return _pVideoUnreadable
            ? LStateValue.LStateValueUnknown
            : LStateValue.LStateValueRead(_pVideoLocation);
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
