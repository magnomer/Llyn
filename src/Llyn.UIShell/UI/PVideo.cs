using System;
using System.ComponentModel;
using System.Globalization;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PVideo : INotifyPropertyChanged
{
    private string _pVideoLocation = string.Empty;
    private bool _pVideoUnknown;
    private bool _pVideoUnwritten;
    private string _pVideoTimestamp = string.Empty;
    private Uri? _pVideoPreview;
    private TimeSpan _pVideoFrom = TimeSpan.Zero;
    private TimeSpan? _pVideoUntil;
    private bool _pVideoPlaying;
    private long _pVideoRow;

    internal PVideo(LVideoDraft written)
    {
        ArgumentNullException.ThrowIfNull(written);

        _pVideoRow = written.LVideoDraftId;

        PVideoLocation = written.LVideoDraftLocation.LStateValueShow();
        PVideoUnknown = written.LVideoDraftLocation.LStateValueState == LState.LStateUnknown;
        PVideoTimestamp = written.LVideoDraftSpan.LStateValueShow();
        _pVideoUnwritten = written.LVideoDraftSpan.LStateValueState == LState.LStateUnknown;
    }

    public bool PVideoUnknown
    {
        get => _pVideoUnknown;
        private set
        {
            if (_pVideoUnknown == value)
            {
                return;
            }

            _pVideoUnknown = value;
            PVideoRaise(nameof(PVideoUnknown));
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
            PVideoUnknown = false;
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
            _pVideoUnwritten = false;
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

    internal long PVideoId => _pVideoRow;

    internal LStateWritten PVideoLocationRead()
    {
        return new LStateWritten(_pVideoLocation, _pVideoUnknown);
    }

    internal LStateWritten PVideoSpanRead()
    {
        return new LStateWritten(_pVideoTimestamp, _pVideoUnwritten);
    }

    internal void PVideoShow(LVideoDraft written, Func<string, bool> pending)
    {
        ArgumentNullException.ThrowIfNull(written);
        ArgumentNullException.ThrowIfNull(pending);

        _pVideoRow = written.LVideoDraftId;

        if (!pending(nameof(PVideoLocation)) && !PVideoLocationRead().LStateWrittenMatch(written.LVideoDraftLocation))
        {
            _pVideoLocation = written.LVideoDraftLocation.LStateValueShow();
            PVideoRaise(nameof(PVideoLocation));
            PVideoUnknown = written.LVideoDraftLocation.LStateValueState == LState.LStateUnknown;
            PVideoPreview = PImage.PImageAddressRead(_pVideoLocation);
        }

        if (!pending(nameof(PVideoTimestamp)) && !PVideoSpanRead().LStateWrittenMatch(written.LVideoDraftSpan))
        {
            _pVideoTimestamp = written.LVideoDraftSpan.LStateValueShow();
            _pVideoUnwritten = written.LVideoDraftSpan.LStateValueState == LState.LStateUnknown;
            PVideoRaise(nameof(PVideoTimestamp));
            PVideoTimestampApply(_pVideoTimestamp);
        }
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
