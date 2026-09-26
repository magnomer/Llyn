using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TClockFake : LClock
{
    private Func<DateTimeOffset> _tClockFakeHand = static () => DateTimeOffset.UtcNow;

    public DateTimeOffset LClockRead() => _tClockFakeHand();

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
