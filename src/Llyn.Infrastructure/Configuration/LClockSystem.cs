using System;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LClockSystem : LClock
{
    public DateTimeOffset LClockRead()
    {
        return DateTimeOffset.UtcNow;
    }
}
