using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TClockFake : LClock
{
    private Func<DateTimeOffset> _tClockFakeHand = static () => DateTimeOffset.UtcNow;

    public DateTimeOffset LClockRead() => _tClockFakeHand();

    public Task LClockPause(TimeSpan span, CancellationToken cancellation) => Task.Delay(span, cancellation);

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
