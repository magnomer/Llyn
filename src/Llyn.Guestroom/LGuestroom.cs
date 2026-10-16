using System;

namespace Llyn.Guestroom;

public static class LGuestroom
{
    public static int LGuestroomRun(string? path, Func<int> console, Func<int> window) =>
        path is not null && path.EndsWith(".com", StringComparison.OrdinalIgnoreCase) ? console() : window();
}
