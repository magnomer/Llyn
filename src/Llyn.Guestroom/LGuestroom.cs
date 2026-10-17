using System;

namespace Llyn.Guestroom;

public static class LGuestroom
{
    public static int LGuestroomRun(string? path, bool marked, Func<int> console, Func<int> window) =>
        marked || (path is not null && path.EndsWith(".com", StringComparison.OrdinalIgnoreCase))
            ? console()
            : window();
}
