using System;

namespace Llyn.Core;

public interface LClock
{
    DateTimeOffset LClockRead();
}
