using System;
using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Core;

public interface LClock
{
    DateTimeOffset LClockRead();

    Task LClockPause(TimeSpan span, CancellationToken cancellation);
}
