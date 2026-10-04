using System;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LClockSystem : LClock
{
    public DateTimeOffset LClockRead()
    {
        return DateTimeOffset.UtcNow;
    }

    public Task LClockPause(TimeSpan span, CancellationToken cancellation)
    {
        return Task.Delay(span, cancellation);
    }
}
