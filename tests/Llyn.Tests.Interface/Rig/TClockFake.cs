using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TClockFake : LClock
{
    private readonly bool _tClockFakeInstant;

    private readonly List<TimeSpan> _tClockFakeSpans = [];

    private Func<DateTimeOffset> _tClockFakeHand = static () => DateTimeOffset.UtcNow;

    internal TClockFake(bool instant = false)
    {
        _tClockFakeInstant = instant;
    }

    internal IReadOnlyList<TimeSpan> TClockFakeSpans
    {
        get
        {
            lock (_tClockFakeSpans)
            {
                return [.. _tClockFakeSpans];
            }
        }
    }

    public DateTimeOffset LClockRead() => _tClockFakeHand();

    public Task LClockPause(TimeSpan span, CancellationToken cancellation)
    {
        lock (_tClockFakeSpans)
        {
            _tClockFakeSpans.Add(span);
        }

        if (!_tClockFakeInstant)
        {
            return Task.Delay(span, cancellation);
        }

        return cancellation.IsCancellationRequested ? Task.FromCanceled(cancellation) : Task.CompletedTask;
    }

    internal void TClockSet(Func<DateTimeOffset> read)
    {
        ArgumentNullException.ThrowIfNull(read);
        _tClockFakeHand = read;
    }

    internal void TClockSet(DateTimeOffset moment)
    {
        _tClockFakeHand = () => moment;
    }
}
